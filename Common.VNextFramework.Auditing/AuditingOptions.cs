using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;

namespace Common.VNextFramework.Auditing
{
    public class AuditingOptions
    {
        public bool IsEnabled { get; set; }

        public bool HideErrors { get; set; }
        public string ApplicationName { get; set; }

        public List<Type> IgnoredTypes { get; }


        public List<AuditLogContributor> Contributors { get; }

        public bool IsEnabledForAnonymousUsers { get; set; }

        public bool IsEnabledForGetRequests { get; set; } = false;

        public bool AlwaysLogOnException { get; set; } = true;

        public List<IAuditingOptionsExtension> Extensions { get; set; } = new List<IAuditingOptionsExtension>();


        public AuditingOptions()
        {
            IsEnabled = true;

            Contributors = new List<AuditLogContributor>();

            IgnoredTypes = new List<Type>
            {
                typeof(Stream),
                typeof(Expression)
            };
        }
    }
}
