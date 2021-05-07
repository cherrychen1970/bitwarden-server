using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bit.Core;
using Bit.Core.Enums;
using Bit.Core.Models;
using Bit.Core.Repositories;
using Bit.Core.Services;
using Bit.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bit.Events.Controllers
{
    [Route("collect")]
    [Authorize("Application")]
    abstract public class CollectController : Controller
    {
        private readonly ISessionContext _currentContext;
        private readonly IEventService _eventService;
        private readonly ICipherRepository _cipherRepository;

        public CollectController(
            ISessionContext currentContext,
            IEventService eventService,
            ICipherRepository cipherRepository)
        {
            _currentContext = currentContext;
            _eventService = eventService;
            _cipherRepository = cipherRepository;
        }

        [HttpGet("~/alive")]
        [HttpGet("~/now")]
        [AllowAnonymous]
        public DateTime GetAlive()
        {
            return DateTime.UtcNow;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]IEnumerable<EventModel> model)
        {
            if (model == null || !model.Any())
            {
                return new BadRequestResult();
            }
            var cipherEvents = new List<Tuple<Cipher, EventType, DateTime?>>();
            var ciphersCache = new Dictionary<Guid, Cipher>();
            foreach (var eventModel in model)
            {
                switch (eventModel.Type)
                {
                    // User events
                    case EventType.User_ClientExportedVault:
                        await _eventService.LogUserEventAsync(_currentContext.UserId, eventModel.Type, eventModel.Date);
                        break;
                    // Cipher events
                    case EventType.Cipher_ClientAutofilled:
                    case EventType.Cipher_ClientCopiedHiddenField:
                    case EventType.Cipher_ClientCopiedPassword:
                    case EventType.Cipher_ClientCopiedCardCode:
                    case EventType.Cipher_ClientToggledCardCodeVisible:
                    case EventType.Cipher_ClientToggledHiddenFieldVisible:
                    case EventType.Cipher_ClientToggledPasswordVisible:
                    case EventType.Cipher_ClientViewed:
                        if (!eventModel.CipherId.HasValue)
                        {
                            continue;
                        }
                        Cipher cipher = null;
                        if (ciphersCache.ContainsKey(eventModel.CipherId.Value))
                        {
                            cipher = ciphersCache[eventModel.CipherId.Value];
                        }
                        else
                        {
                            cipher = await _cipherRepository.GetByIdAsync(eventModel.CipherId.Value,
                               _currentContext.UserId);
                        }
                        if (cipher == null)
                        {
                            continue;
                        }
                        if (!ciphersCache.ContainsKey(eventModel.CipherId.Value))
                        {
                            ciphersCache.Add(eventModel.CipherId.Value, cipher);
                        }
                        cipherEvents.Add(new Tuple<Cipher, EventType, DateTime?>(cipher, eventModel.Type, eventModel.Date));
                        break;
                    default:
                        continue;
                }
            }
            if (cipherEvents.Any())
            {
                foreach (var eventsBatch in cipherEvents.Batch(50))
                {
                    await _eventService.LogCipherEventsAsync(eventsBatch);
                }
            }
            return new OkResult();
        }
    }
    public class EventModel
    {
        public EventType Type { get; set; }
        public Guid? CipherId { get; set; }
        public DateTime Date { get; set; }
    }    
}
