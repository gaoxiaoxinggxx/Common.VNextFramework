using System;

namespace Samples.Common.Data.Entitys
{
    public class User : AuditEntity
    {
        public enum TdoUserRoleEnum
        {
            Supervisor = 1
        }
        public string Email { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public TdoUserRoleEnum Role { get; set; }

        public bool IsNeedChangePassword { get; set; }
        public DateTimeOffset? UnlockTime { get; set; }
        public int LoginFailedCount { get; set; }
    }
}
