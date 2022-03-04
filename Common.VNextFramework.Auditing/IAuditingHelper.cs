using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Options;

namespace Common.VNextFramework.Auditing
{
    public interface IAuditingHelper
    {
        bool ShouldSaveAudit(MethodInfo methodInfo, bool defaultValue = false);

        bool IsEntityHistoryEnabled(Type entityType, bool defaultValue = false);
    }

    public class AuditingHelper : IAuditingHelper
    {
        protected AuditingOptions Options;

        public AuditingHelper(IOptions<AuditingOptions> options)
        {
            Options = options.Value;
        }

        public virtual bool ShouldSaveAudit(MethodInfo methodInfo, bool defaultValue = false)
        {
            if (methodInfo == null)
            {
                return false;
            }

            if (!methodInfo.IsPublic)
            {
                return false;
            }

            if (methodInfo.IsDefined(typeof(AuditedAttribute), true))
            {
                return true;
            }

            if (methodInfo.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return false;
            }

            var classType = methodInfo.DeclaringType;
            if (classType != null)
            {
                var shouldAudit = ShouldAuditTypeByDefaultOrNull(classType);
                if (shouldAudit != null)
                {
                    return shouldAudit.Value;
                }
            }

            return defaultValue;
        }


        public static bool? ShouldAuditTypeByDefaultOrNull(Type type)
        {
            if (type.IsDefined(typeof(AuditedAttribute), true))
            {
                return true;
            }

            if (type.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return false;
            }

            
            return null;
        }


        public bool IsEntityHistoryEnabled(Type entityType, bool defaultValue = false)
        {
            if (!entityType.IsPublic)
            {
                return false;
            }

            if (Options.IgnoredTypes.Any(t => t.IsAssignableFrom(entityType)))
            {
                return false;
            }

            if (entityType.IsDefined(typeof(AuditedAttribute), true))
            {
                return true;
            }

            foreach (var propertyInfo in entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (propertyInfo.IsDefined(typeof(AuditedAttribute)))
                {
                    return true;
                }
            }

            if (entityType.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return false;
            }

            //if (Options.EntityHistorySelectors.Any(selector => selector.Predicate(entityType)))
            //{
            //    return true;
            //}

            return defaultValue;
        }
    }
}
