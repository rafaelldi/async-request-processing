using System;
using System.Threading.Tasks;
using MassTransit.Courier;
using Microsoft.Extensions.Logging;
using static async_request_processing.StateMachine.Contracts;

namespace async_request_processing.Activity
{
    public class ProcessItemActivity : IExecuteActivity<ProcessItemArgument>
    {
        private readonly RandomService _randomService;
        private readonly ILogger<ProcessItemActivity> _logger;

        public ProcessItemActivity(RandomService randomService, ILogger<ProcessItemActivity> logger)
        {
            _randomService = randomService;
            _logger = logger;
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<ProcessItemArgument> context)
        {
            _logger.LogInformation("Processing item with id={ItemId}", context.Arguments.ItemId);

            var delay = _randomService.GetDelay();
            await Task.Delay(TimeSpan.FromSeconds(delay));

            await context.Publish(new ItemProcessed(context.Arguments.ItemId, context.TrackingNumber));
            
            return context.Completed();
        }
    }
}