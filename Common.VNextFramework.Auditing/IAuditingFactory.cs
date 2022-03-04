using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Common.VNextFramework.Auditing
{
    public interface IAuditingFactory
    {
        AuditLogInfo CreateAuditLogInfo();

        AuditLogActionInfo CreateAuditLogAction(
            AuditLogInfo auditLog,
            Type type,
            MethodInfo method,
            object[] arguments
        );

        AuditLogActionInfo CreateAuditLogAction(
            AuditLogInfo auditLog,
            Type type,
            MethodInfo method,
            IDictionary<string, object> arguments
        );
    }

    public class AuditingFactory : IAuditingFactory
    {
        protected ILogger<AuditingFactory> Logger { get; }
        protected AuditingOptions Options;
        protected IAuditSerializer AuditSerializer;
        protected IServiceProvider ServiceProvider;

        public AuditingFactory(IOptions<AuditingOptions> options, IAuditSerializer auditSerializer)
        {
            AuditSerializer = auditSerializer;
            Options = options.Value;
            Logger = NullLogger<AuditingFactory>.Instance;

        }

        public AuditLogInfo CreateAuditLogInfo()
        {
            throw new NotImplementedException();
        }

        public AuditLogActionInfo CreateAuditLogAction(AuditLogInfo auditLog, Type type, MethodInfo method, object[] arguments)
        {
            return CreateAuditLogAction(auditLog, type, method, CreateArgumentsDictionary(method, arguments));
        }

        public AuditLogActionInfo CreateAuditLogAction(AuditLogInfo auditLog, Type type, MethodInfo method, IDictionary<string, object> arguments)
        {
            var actionInfo = new AuditLogActionInfo
            {
                ServiceName = type != null
                    ? type.FullName
                    : "",
                MethodName = method.Name,
                Parameters = SerializeConvertArguments(arguments),
                ExecutionTime = DateTime.Now

            };

            //TODO Execute contributors

            return actionInfo;
        }


        protected virtual string SerializeConvertArguments(IDictionary<string, object> arguments)
        {
            try
            {
                if (arguments == null || arguments.Count == 0)
                {
                    return "{}";
                }

                var dictionary = new Dictionary<string, object>();

                foreach (var argument in arguments)
                {
                    if (argument.Value != null && Options.IgnoredTypes.Any(t => t.IsInstanceOfType(argument.Value)))
                    {
                        dictionary[argument.Key] = null;
                    }
                    else
                    {
                        dictionary[argument.Key] = argument.Value;
                    }
                }

                return AuditSerializer.Serialize(dictionary);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, ex.Message);
                return "{}";
            }
        }

        protected virtual Dictionary<string, object> CreateArgumentsDictionary(MethodInfo method, object[] arguments)
        {
            var parameters = method.GetParameters();
            var dictionary = new Dictionary<string, object>();

            for (var i = 0; i < parameters.Length; i++)
            {
                dictionary[parameters[i].Name] = arguments[i];
            }

            return dictionary;
        }
    }
}
