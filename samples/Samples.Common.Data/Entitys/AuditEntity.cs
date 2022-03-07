using Common.VNextFramework.Auditing;
using Common.VNextFramework.EntityFramework;
using Common.VNextFramework.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Common.Data.Entitys
{
    [Audited]
    public class AuditEntity : GuidEntity
    {
        public override Guid Id { get; set; } = GuidTool.GenerateSequentialGuid();
    }
}
