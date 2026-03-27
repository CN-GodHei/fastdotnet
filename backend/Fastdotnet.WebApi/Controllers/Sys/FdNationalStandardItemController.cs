namespace Fastdotnet.WebApi.Controllers.Sys
{
    /// <summary>
    /// FdNationalStandardItem 控制器
    /// </summary>
    [Route("api/[controller]")]
    public class FdNationalStandardItemController : GenericDtoControllerBase<FdNationalStandardItem, string, CreateFdNationalStandardItemDto, UpdateFdNationalStandardItemDto, FdNationalStandardItemDto>
    {
        private readonly IBaseService<FdNationalStandardItem, string> _fdnationalstandarditembaseService;
        private readonly IFdNationalStandardItemService _fdnationalstandarditemService;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public FdNationalStandardItemController(
            IFdNationalStandardItemService fdnationalstandarditemService,
            IBaseService<FdNationalStandardItem, string> fdnationalstandarditembaseService,
            IMapper mapper, ICurrentUser currentUser) : base(fdnationalstandarditembaseService, mapper, currentUser)
        {
            _fdnationalstandarditembaseService = fdnationalstandarditembaseService;
            _fdnationalstandarditemService = fdnationalstandarditemService;
            _mapper = mapper;
            _currentUser = currentUser;
        }
    }
}
