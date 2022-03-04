using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Common.VNextFramework.Auditing;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Common.VNextFramework.EntityFramework.EntityHistory
{
    public interface IEntityHistoryHelper
    {
        List<EntityChangeInfo> CreateChangeInfos(ICollection<EntityEntry> entityEntries);

        void UpdateChangeList(List<EntityChangeInfo> entityChangeInfos );
    }
}
