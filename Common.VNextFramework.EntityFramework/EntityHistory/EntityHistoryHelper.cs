using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.VNextFramework.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace Common.VNextFramework.EntityFramework.EntityHistory
{
    public class EntityHistoryHelper : IEntityHistoryHelper
    {
        private readonly IAuditingHelper _auditingHelper;
        public List<EntityChangeInfo> CreateChangeInfos(ICollection<EntityEntry> entityEntries)
        {

            var list = new List<EntityChangeInfo>();

            foreach (var entityEntry in entityEntries)
            {
                if (!ShouldSaveEntityHistory(entityEntry))
                {
                    continue;
                }

                var entityChange = CreateEntityChangeOrNull(entityEntry);
                if (entityChange == null)
                {
                    continue;
                }

                list.Add(entityChange);
            }

            return list;
        }

        public void UpdateChangeList(List<EntityChangeInfo> entityChangeInfos)
        {
            foreach (var entityChangeInfo in entityChangeInfos)
            {
                entityChangeInfo.ChangeTime = DateTime.Now;
                var entityEntry = entityChangeInfo.EntityEntry as EntityEntry;
                entityChangeInfo.EntityId = GetEntityId(entityEntry.Entity);
                var foreignKeys = entityEntry.Metadata.GetForeignKeys();

                foreach (var foreignKey in foreignKeys)
                {
                    foreach (var property in foreignKey.Properties)
                    {
                        var propertyEntry = entityEntry.Property(property.Name);
                        var propertyChange = entityChangeInfo.PropertyChanges.FirstOrDefault(pc => pc.PropertyName == property.Name);

                        if (propertyChange == null)
                        {
                            if (!(propertyEntry.OriginalValue?.Equals(propertyEntry.CurrentValue) ?? propertyEntry.CurrentValue == null))
                            {
                                // Add foreign key
                                entityChangeInfo.PropertyChanges.Add(new EntityPropertyChangeInfo
                                {
                                    NewValue = Serialize(propertyEntry.CurrentValue),
                                    OriginalValue = Serialize(propertyEntry.OriginalValue),
                                    PropertyName = property.Name,
                                    PropertyTypeFullName = GetFirstGenericArgumentIfNullable( property.ClrType).FullName,
                                });
                            }

                            continue;
                        }

                        if (propertyChange.OriginalValue == propertyChange.NewValue)
                        {
                            var newValue = Serialize(propertyEntry.CurrentValue);
                            if (newValue == propertyChange.NewValue)
                            {
                                // No change
                                entityChangeInfo.PropertyChanges.Remove(propertyChange);
                            }
                            else
                            {
                                // Update foreign key
                                propertyChange.NewValue = newValue;
                            }
                        }
                    }
                }
            }
        }

        protected virtual EntityChangeInfo CreateEntityChangeOrNull(EntityEntry entityEntry)
        {
            var entity = entityEntry.Entity;
            EntityChangeType changeType;
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    changeType = EntityChangeType.Created;
                    break;
                case EntityState.Modified:
                    changeType = EntityChangeType.Updated;
                    break;
                case EntityState.Deleted:
                    changeType = EntityChangeType.Deleted;
                    break;
                default: return null;
                   
            }

            var entityId = GetEntityId(entity);
            if (entityId == null && changeType != EntityChangeType.Created)
            {
                return null;
            }

            var entityChangeInfo = new EntityChangeInfo()
            {
                EntityId = entityId,
                EntityTypeFullName = entity.GetType().FullName,
                ChangeType = changeType,
                EntityEntry = entityEntry,
                PropertyChanges =GetPropertyChanges(entityEntry,changeType),
            };

            return entityChangeInfo;
        }

        protected virtual string GetEntityId(object entityAsObj)
        {
            if (!(entityAsObj is IEntity entity))
            {
                throw new Exception($"Entities should implement the {typeof(IEntity).AssemblyQualifiedName} interface! Given entity does not implement it: {entityAsObj.GetType().AssemblyQualifiedName}");
            }

            var keys = entity.GetKeys();
            if (keys.All(k => k == null))
            {
                return null;
            }

            return string.Join(",", keys);
        }

        protected virtual List<EntityPropertyChangeInfo> GetPropertyChanges(EntityEntry entityEntry, EntityChangeType changeType)
        {
            var propertyChangeInfos = new List<EntityPropertyChangeInfo>();
            var properies = entityEntry.Metadata.GetProperties();

            foreach (var property in properies)
            {
                var propertyEntity = entityEntry.Property(property.Name);
                if (ShouldSavePropertyHistory(propertyEntity,
                    changeType == EntityChangeType.Created || changeType == EntityChangeType.Deleted))
                {
                    propertyChangeInfos.Add(new EntityPropertyChangeInfo()
                    {
                        NewValue = changeType== EntityChangeType.Deleted?null: Serialize(propertyEntity.CurrentValue),
                        OriginalValue = changeType == EntityChangeType.Created?null:Serialize(propertyEntity.OriginalValue),
                        PropertyName = property.Name,
                        PropertyTypeFullName = GetFirstGenericArgumentIfNullable(property.ClrType).FullName
                    });
                }
            }

            return propertyChangeInfos;
        }

        protected virtual bool ShouldSavePropertyHistory(PropertyEntry propertyEntry, bool defaultValue)
        {
            if (propertyEntry.Metadata.Name == "Id")
            {
                return false;
            }

            var propertyInfo = propertyEntry.Metadata.PropertyInfo;
            if (propertyInfo != null && propertyInfo.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return false;
            }

            var entityType = propertyEntry.EntityEntry.Entity.GetType();
            if (entityType.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                if (propertyInfo == null || !propertyInfo.IsDefined(typeof(AuditedAttribute), true))
                {
                    return false;
                }
            }

            if (propertyInfo != null && IsBaseAuditProperty(propertyInfo, entityType))
            {
                return false;
            }

            
            var isModified = !(propertyEntry.OriginalValue?.Equals(propertyEntry.CurrentValue) ?? propertyEntry.CurrentValue == null);
            if (isModified)
            {
                return true;
            }

            return defaultValue;
        }

        protected virtual bool ShouldSaveEntityHistory(EntityEntry entityEntry, bool defaultValue = false)
        {
            if (entityEntry.State == EntityState.Detached ||
                entityEntry.State == EntityState.Unchanged)
            {
                return false;
            }

            var entityType = entityEntry.Metadata.ClrType;

            if (!typeof(IEntity).IsAssignableFrom(entityType))
            {
                return false;
            }

            if (_auditingHelper.IsEntityHistoryEnabled(entityType))
            {
                return true;
            }

            return defaultValue;
        }


        private static readonly HashSet<string> BaseAuditEntityFieldHashSet = new HashSet<string>(new List<string>()
            {"CreatedOn", "CreatedBy", "LastModifiedOn", "LastModifiedBy", "IsDeleted", "Timestamp"});

        public EntityHistoryHelper(IAuditingHelper auditingHelper)
        {
            _auditingHelper = auditingHelper;
        }

        protected virtual bool IsBaseAuditProperty(PropertyInfo propertyInfo, Type entityType)
        {

            if (typeof(BaseAuditEntity).IsAssignableFrom(entityType)
                && BaseAuditEntityFieldHashSet.Contains(propertyInfo.Name))
            {
                return true;
            }

            return false;
        }

        protected  string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        private  Type GetFirstGenericArgumentIfNullable( Type t)
        {
            if (t.GetGenericArguments().Length > 0 && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return t.GetGenericArguments().FirstOrDefault();
            }

            return t;
        }
    }
}