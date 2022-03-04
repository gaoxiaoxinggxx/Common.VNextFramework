using Microsoft.AspNetCore.Builder;
using System;

namespace Common.VNextFramework.Extensions
{
    public static class HangFireExtensions
    {
        private static bool _isExecuted = false;
        private static readonly object LockHangFireObject = new object();

        public static void UseHangFireDispatch(this IApplicationBuilder app, Action<IApplicationBuilder> once, Action<IApplicationBuilder> action)
        {
            if (!_isExecuted)
            {
                lock (LockHangFireObject)
                {
                    if (!_isExecuted)
                    {
                        once?.Invoke(app);
                        _isExecuted = true;
                    }
                }
            }
            action?.Invoke(app);
        }
    }
}
