using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Common.VNextFramework.Auditing
{
    public class EntityChangeInfo
    {
        public string EntityId { get; set; }

        public DateTime ChangeTime { get; set; }
        public EntityChangeType ChangeType { get; set; }

        [JsonIgnore]
        public virtual object EntityEntry { get; set; }

        public string EntityTypeFullName { get; set; }

        public List<EntityPropertyChangeInfo> PropertyChanges { get; set; }

        public EntityChangeInfo()
        {
        }

        public virtual void Merge(EntityChangeInfo changeInfo)
        {
            foreach (var propertyChange in changeInfo.PropertyChanges)
            {
                var existingChange = PropertyChanges.FirstOrDefault(p => p.PropertyName == propertyChange.PropertyName);
                if (existingChange == null)
                {
                    PropertyChanges.Add(propertyChange);
                }
                else
                {
                    existingChange.NewValue = propertyChange.NewValue;
                }
            }

            //foreach (var extraProperty in changeInfo.ExtraProperties)
            //{
            //    var key = extraProperty.Key;
            //    if (ExtraProperties.ContainsKey(key))
            //    {
            //        key = InternalUtils.AddCounter(key);
            //    }

            //    ExtraProperties[key] = extraProperty.Value;
            //}
        }
    }
}
