using Common.VNextFramework.Tools;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Common.VNextFramework.EntityFramework
{
    public class BaseAuditEntity
    {
        public virtual DateTimeOffset? CreatedOn { get; set; }

        public virtual string CreatedBy { get; set; }

        public virtual DateTimeOffset? LastModifiedOn { get; set; }

        public virtual string LastModifiedBy { get; set; }

        [DefaultValue("true")]
        public virtual bool IsDeleted { get; set; } = false;

        [Timestamp]
        public virtual byte[] Timestamp { get; set; }
    }

    public interface IEntity
    {
        object[] GetKeys();
    }

    public interface IBaseEntity<T>: IEntity where T : new()
    {
        T Id { get; set; }
    }

    public abstract class IntEntity : BaseAuditEntity, IBaseEntity<int>
    {
        public virtual int Id { get; set; }
        public virtual object[] GetKeys()
        {
            return new object[] { Id };
        }
    }

    public abstract class GuidEntity : BaseAuditEntity, IBaseEntity<Guid>
    {
        public virtual Guid Id { get; set; } = Guid.NewGuid();
        public virtual object[] GetKeys()
        {
            return new object[] { Id };
        }
    }

    public abstract class SequentialGuidEntity : BaseAuditEntity, IBaseEntity<Guid>
    {
        public virtual Guid Id { get; set; } = GuidTool.GenerateSequentialGuid();
        public virtual object[] GetKeys()
        {
            return new object[] { Id };
        }
    }
}