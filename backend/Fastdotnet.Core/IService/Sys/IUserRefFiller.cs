using Fastdotnet.Core.Dtos.Common;
using Fastdotnet.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Core.IService.Sys
{
    public interface IUserRefFiller
    {
        Task FillNamesAsync<T>(
            IList<T> dtos,
            SystemCategory systemCategory,
            params Expression<Func<T, UserRefDto>>[] userRefSelectors);
    }
}
