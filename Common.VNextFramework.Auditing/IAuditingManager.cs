using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.VNextFramework.Auditing
{
    public interface IAuditingManager
    {
        IAuditLogScope Current { get; }

        IAuditLogSaveHandle BeginScope();
    }

    public interface IAuditLogSaveHandle : IDisposable
    {
        Task SaveAsync();
    }
}
