using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Common.VNextFramework.Auditing
{
    public interface IAuditSerializer
    {
        string Serialize(object obj);
    }

    public class JsonAuditSerializer : IAuditSerializer
    {
        protected AuditingOptions Options;

        public JsonAuditSerializer(IOptions<AuditingOptions> options)
        {
            Options = options.Value;
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, GetSharedJsonSerializerSettings());
        }

        private static readonly object SyncObj = new object();
        private static JsonSerializerSettings _sharedJsonSerializerSettings;

        private JsonSerializerSettings GetSharedJsonSerializerSettings()
        {
            if (_sharedJsonSerializerSettings == null)
            {
                lock (SyncObj)
                {
                    if (_sharedJsonSerializerSettings == null)
                    {
                        _sharedJsonSerializerSettings = new JsonSerializerSettings
                        {
                            ContractResolver = new AuditingContractResolver(Options.IgnoredTypes)
                        };
                    }
                }
            }

            return _sharedJsonSerializerSettings;
        }
    }

    public class AuditingContractResolver : CamelCasePropertyNamesContractResolver
    {
        private readonly List<Type> _ignoredTypes;

        public AuditingContractResolver(List<Type> ignoredTypes)
        {
            _ignoredTypes = ignoredTypes;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (_ignoredTypes.Any(ignoredType => ignoredType.GetTypeInfo().IsAssignableFrom(property.PropertyType)))
            {
                property.ShouldSerialize = instance => false;
                return property;
            }

            if (member.DeclaringType != null && (member.DeclaringType.IsDefined(typeof(DisableAuditingAttribute)) || member.DeclaringType.IsDefined(typeof(JsonIgnoreAttribute))))
            {
                property.ShouldSerialize = instance => false;
                return property;
            }

            if (member.IsDefined(typeof(DisableAuditingAttribute)) || member.IsDefined(typeof(JsonIgnoreAttribute)))
            {
                property.ShouldSerialize = instance => false;
            }

            return property;
        }
    }
}
