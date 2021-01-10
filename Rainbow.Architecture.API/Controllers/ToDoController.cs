using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rainbow.Architecture.API.Application.Commands;
using Rainbow.Architecture.API.Application.Queries;
using Rainbow.Architecture.API.Infrastructure.Services;
using Rainbow.Architecture.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Rainbow.Architecture.API.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IToDoItemQueries _todoQueries;
        private readonly IIdentityService _identityService;
        private readonly ILogger<ToDoController> _logger;

        public ToDoController(
            IMediator mediator,
            IToDoItemQueries todoQueries,
            IIdentityService identityService,
            ILogger<ToDoController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _todoQueries = todoQueries ?? throw new ArgumentNullException(nameof(todoQueries));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 查询TODO项
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("{itemId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetToDoItemAsync(Guid itemId)
        {
            try
            {
                //Todo: It's good idea to take advantage of GetToDoItemByIdQuery and handle by GetToDoItemByIdQueryHandler
                //var order todoItem = await _mediator.Send(new GetToDoItemByIdQuery(todoId));
                var order = await _todoQueries.GetOrderAsync(itemId);

                return Ok(order);
            }
            catch
            {
                return NotFound();
            }
        }

        /// <summary>
        /// 创建TODO项
        /// </summary>
        /// <param name="command"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("create")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateToDoItemAsync([FromBody] CreateToDoItemCommand command, [FromHeader(Name = "x-requestid")] string requestId)
        {
            bool commandResult = false;

            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var identifiedCommand = new IdentifiedCommand<CreateToDoItemCommand, bool>(command, guid);

                _logger.LogInformation("Log Information ...");

                commandResult = await _mediator.Send(identifiedCommand);
            }

            if (!commandResult)
            {
                return BadRequest();
            }

            return Ok();
        }


    }
}
