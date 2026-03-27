namespace Fastdotnet.WebApi.Controllers.Sys
{
    /// <summary>
    /// FdNationalStandard 控制器
    /// </summary>
    [Route("api/[controller]")]
    public class FdNationalStandardController : GenericDtoControllerBase<FdNationalStandard, string, CreateFdNationalStandardDto, UpdateFdNationalStandardDto, FdNationalStandardDto>
    {
        private readonly IBaseService<FdNationalStandard, string> _fdnationalstandardbaseService;
        private readonly IFdNationalStandardService _fdnationalstandardService;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        
        public FdNationalStandardController(
            IFdNationalStandardService fdnationalstandardService,
            IBaseService<FdNationalStandard, string> fdnationalstandardbaseService,
            IMapper mapper, ICurrentUser currentUser) : base(fdnationalstandardbaseService, mapper, currentUser)
        {
            _fdnationalstandardbaseService = fdnationalstandardbaseService;
            _fdnationalstandardService = fdnationalstandardService;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        /// <summary>
        /// 导入标准及其条目（批量操作）
        /// </summary>
        [HttpPost("import")]
        public async Task<ActionResult<string>> ImportStandard([FromBody] ImportStandardRequest request)
        {
            var standardId = await _fdnationalstandardService.ImportStandardAsync(request.Standard, request.Items);
            return Ok(new { id = standardId, message = "导入成功" });
        }

        /// <summary>
        /// 获取标准的完整树形结构
        /// </summary>
        [HttpGet("{standardCode}/tree")]
        public async Task<ActionResult<TreeModel<FdNationalStandardItemDto>>> GetStandardTree(string standardCode)
        {
            var tree = await _fdnationalstandardService.GetStandardTreeAsync(standardCode);
            return Ok(tree);
        }

        /// <summary>
        /// 根据标准编码获取标准详情（包含条目统计）
        /// </summary>
        [HttpGet("{standardCode}/detail")]
        public async Task<ActionResult<FdNationalStandardDetailDto>> GetStandardDetail(string standardCode)
        {
            var detail = await _fdnationalstandardService.GetStandardDetailAsync(standardCode);
            return Ok(detail);
        }

        /// <summary>
        /// 更新标准版本（版本升级）
        /// </summary>
        [HttpPost("{standardCode}/version")]
        public async Task<ActionResult> UpdateVersion(
            string standardCode, 
            [FromBody] UpdateVersionRequest request)
        {
            var result = await _fdnationalstandardService.UpdateVersionAsync(
                standardCode, 
                request.NewVersion, 
                request.NewItems);
            
            return Ok(new { 
                oldVersionId = result.oldVersionId,
                newVersionId = result.newVersionId,
                message = "版本升级成功"
            });
        }
    }

    /// <summary>
    /// 导入标准请求 DTO
    /// </summary>
    public class ImportStandardRequest
    {
        /// <summary>
        /// 标准主表数据
        /// </summary>
        public FdNationalStandard Standard { get; set; } = new();

        /// <summary>
        /// 标准条目列表
        /// </summary>
        public List<FdNationalStandardItem> Items { get; set; } = new();
    }

    /// <summary>
    /// 更新版本请求 DTO
    /// </summary>
    public class UpdateVersionRequest
    {
        /// <summary>
        /// 新版本号
        /// </summary>
        public string NewVersion { get; set; } = string.Empty;

        /// <summary>
        /// 新版本的条目数据
        /// </summary>
        public List<FdNationalStandardItem> NewItems { get; set; } = new();
    }
}
