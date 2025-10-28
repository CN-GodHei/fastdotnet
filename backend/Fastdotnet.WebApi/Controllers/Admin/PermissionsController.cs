using AutoMapper;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/permissions")]
    public class PermissionsController : ControllerBase
    {
        private readonly IRepository<FdPermission> _permissionRepository;
        private readonly IMapper _mapper;

        public PermissionsController(IRepository<FdPermission> permissionRepository, IMapper mapper)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取所有权限，按模块分组，方便前端展示
        /// </summary>
        [HttpGet]
        public async Task<Dictionary<string,List<PermissionDto>>> GetAll()
        {
            var permissions = await _permissionRepository.GetAllAsync();
            var permissionDtos = _mapper.Map<List<PermissionDto>>(permissions);

            var groupedPermissions = permissionDtos
                .GroupBy(p => p.Module ?? "System") // 将没有模块的权限归类到"System"
                .ToDictionary(g => g.Key, g => g.ToList());

            return groupedPermissions;
        }
    }
}
