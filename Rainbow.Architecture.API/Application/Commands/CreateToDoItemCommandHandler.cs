using MediatR;
using Microsoft.Extensions.Logging;
using Rainbow.Architecture.Domain.AggregatesModel.ToDoAggregate.Entities;
using Rainbow.Architecture.Domain.AggregatesModel.ToDoAggregate.IRepositories;
using Rainbow.Architecture.Infrastructure.Idempotency;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rainbow.Architecture.API.Application.Commands
{
    public class CreateToDoItemCommandHandler : IRequestHandler<CreateToDoItemCommand, bool>
    {
        private readonly IToDoItemRepository _todoRepository;
        private readonly ILoggerFactory _logger;

        public CreateToDoItemCommandHandler(ILoggerFactory logger, IToDoItemRepository todoRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _todoRepository = todoRepository ?? throw new ArgumentNullException(nameof(todoRepository));
        }

        /// <summary>
        /// Handler which processes the command when create ToDoItem
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<bool> Handle(CreateToDoItemCommand command, CancellationToken cancellationToken)
        {
            _logger.CreateLogger<CreateToDoItemCommand>().LogTrace("Create todo item start ......");

            var item = new ToDoItem(command.Title, command.Description);
            await _todoRepository.AddAsync(item);

            return await Task.FromResult(true);
            //return await _todoRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }


    // Use for Idempotency in Command process
    public class CreateToDoItemIdentifiedCommandHandler : IdentifiedCommandHandler<CreateToDoItemCommand, bool>
    {
        public CreateToDoItemIdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<CreateToDoItemCommand, bool>> logger)
            : base(mediator, requestManager, logger)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;                // Ignore duplicate requests for processing order.
        }
    }
}
