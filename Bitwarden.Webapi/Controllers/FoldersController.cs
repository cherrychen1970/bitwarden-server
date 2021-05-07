using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bit.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Bit.Core.Models.Api;
using Bit.Core.Exceptions;
using Bit.Core.Services;
using Bit.Core;

namespace Bit.Api.Controllers
{
    [Route("api/folders")]
    [Authorize("Application")]
    public class FoldersController : Controller
    {
        private readonly IFolderRepository _folderRepository;
        private readonly ICipherService _cipherService;
        private readonly IUserService _userService;
        private readonly ISessionContext _currentContext;
        private Guid userId => _currentContext.UserId;

        public FoldersController(
            IFolderRepository folderRepository,
            ICipherService cipherService,
            IUserService userService,
            ISessionContext currentContext
            )
        {
            _folderRepository = folderRepository;
            _cipherService = cipherService;
            _userService = userService;
            _currentContext = currentContext;
        }

        [HttpGet("{id}")]
        public async Task<FolderResponseModel> Get(string id)
        {
            
            var folder = await _folderRepository.GetByIdAsync(new Guid(id), userId);
            if (folder == null)
            {
                throw new NotFoundException();
            }

            return new FolderResponseModel(folder);
        }

        [HttpGet("")]
        public async Task<ListResponseModel<FolderResponseModel>> Get()
        {
            
            var folders = await _folderRepository.GetManyByUserIdAsync(userId);
            var responses = folders.Select(f => new FolderResponseModel(f));
            return new ListResponseModel<FolderResponseModel>(responses);
        }

        [HttpPost("")]
        public async Task<FolderResponseModel> Post([FromBody]FolderRequestModel model)
        {            
            var folder = model.ToFolder(userId);
            await _cipherService.SaveFolderAsync(folder);
            return new FolderResponseModel(folder);
        }

        [HttpPut("{id}")]
        [HttpPost("{id}")]
        public async Task<FolderResponseModel> Put(string id, [FromBody]FolderRequestModel model)
        {
            
            var folder = await _folderRepository.GetByIdAsync(new Guid(id), userId);
            if (folder == null)
            {
                throw new NotFoundException();
            }

            await _cipherService.SaveFolderAsync(model.ToFolder(folder));
            return new FolderResponseModel(folder);
        }

        [HttpDelete("{id}")]
        [HttpPost("{id}/delete")]
        public async Task Delete(string id)
        {
            
            var folder = await _folderRepository.GetByIdAsync(new Guid(id), userId);
            if (folder == null)
            {
                throw new NotFoundException();
            }

            await _cipherService.DeleteFolderAsync(folder);
        }
    }
}
