using System;
using System.Collections.Generic;
using System.Text;

namespace Common.VNextFramework.Auditing
{
    public enum EntityChangeType : byte
    {
        Created = 0,

        Updated = 1,

        Deleted = 2
    }
}
