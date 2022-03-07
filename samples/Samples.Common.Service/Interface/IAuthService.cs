using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Common.Service.Interface
{
    public interface IAuthService
    {
        Task<bool> Auth();
    }
}
