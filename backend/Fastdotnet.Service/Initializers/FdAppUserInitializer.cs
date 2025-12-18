using Fastdotnet.Core.Entities.App;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.IService;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Service.Initializers
{
    public class FdAppUserInitializer : IApplicationInitializer
    {
        private readonly IRepository<FdAppUser> _Repository;

        public FdAppUserInitializer(IRepository<FdAppUser> Repository)
        {
            _Repository = Repository;
        }

        public async Task InitializeAsync()
        {
            if (await _Repository.ExistsAsync(a => a.Id != null))
            {
                return;
            }
            var entity = new FdAppUser {Username="admintest", Nickname="AdminTest",Email="fastdotnet@test.com",Password="123456",PhoneNumber="159****7417"};

            await _Repository.InsertAsync(entity);

            //_logger.LogInformation("Finish: Default Email Config initialization complete.");
        }
    }
}
