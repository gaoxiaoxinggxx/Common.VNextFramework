using Common.VNextFramework.EntityFramework;
using Samples.Common.Data.Entitys;
using Samples.Common.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Common.Service.Implement
{
    public class AuthService : IAuthService
    {
        private readonly IEFAsyncRepository<User> _userRepository;

        public AuthService(IEFAsyncRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Auth()
        {
            var userSpec = Specification<User>.GetSpecification();
            userSpec.AddInclude(x => x.Email);
            userSpec.AddPredicate(x => x.UserName == "xxx");
            var user = await _userRepository.GetSingleAsync(userSpec);
            return true;
        }
    }
}
