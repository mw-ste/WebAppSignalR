using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Shared;

namespace Backend
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IHubContext<CommunicationSignalRHub, ISignalRClient> _hubContext;

        public MessageController(IHubContext<CommunicationSignalRHub, ISignalRClient> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("MessageAll")]
        public async Task<ActionResult> MessageAll([Required] string message)
        {
            return await Execute(
                _hubContext
                    .Clients
                    .All
                    .ReceiveMessage("TheBackend", message));
        }

        [HttpPost("MessageUser")]
        public async Task<ActionResult> MessageUser([Required] string connectionId, [Required] string message)
        {
            return await Execute(
                _hubContext
                    .Clients
                    .Client(connectionId)
                    .ReceiveMessage("TheBackend", message));
        }

        private async Task<ActionResult> Execute(Task action)
        {
            try
            {
                await action;

                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }
    }
}