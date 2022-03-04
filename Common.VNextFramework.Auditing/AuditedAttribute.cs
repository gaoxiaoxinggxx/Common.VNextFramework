using System;
using System.Collections.Generic;
using System.Text;

namespace Common.VNextFramework.Auditing
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class AuditedAttribute : Attribute
    {

    }
}
