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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> MessageAll([Required] string message)
        {
            try
            {
                await _hubContext
                    .Clients
                    .All
                    .ReceiveMessage("TheBackend", message);

                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(StatusCodes.Status400BadRequest, null);
            }
        }

        [HttpPost("MessageUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> MessageUser([Required] string connectionId, [Required] string message)
        {
            try
            {
                await _hubContext
                    .Clients
                    .Client(connectionId)
                    .ReceiveMessage("TheBackend", message);

                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(StatusCodes.Status400BadRequest, null);
            }

        }
    }
}