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
        private new readonly ICurrentUser _currentUser;
        public FdNationalStandardItemController(
            IFdNationalStandardItemService fdnationalstandarditemService,
            IBaseService<FdNationalStandardItem, string> fdnationalstandarditembaseService, ICurrentUser currentUser) : base(fdnationalstandarditembaseService, currentUser)
        {
            _fdnationalstandarditembaseService = fdnationalstandarditembaseService;
            _fdnationalstandarditemService = fdnationalstandarditemService;
            _currentUser = currentUser;
        }
    }
}
