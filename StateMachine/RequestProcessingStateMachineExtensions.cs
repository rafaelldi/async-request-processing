using System;
using async_request_processing.Activity;
using Automatonymous;
using Automatonymous.Binders;
using MassTransit.Courier;
using static async_request_processing.StateMachine.Contracts;

namespace async_request_processing.StateMachine
{
    public static class RequestProcessingStateMachineExtensions
    {
        public static EventActivityBinder<RequestProcessingState, ProcessRequestCommand> CreateRoutingSlip(
            this EventActivityBinder<RequestProcessingState, ProcessRequestCommand> binder)
        {
            return binder.ThenAsync(async context =>
            {
                var trackingNumber = Guid.NewGuid();
                context.Instance.TrackingNumber = trackingNumber;
                var builder = new RoutingSlipBuilder(trackingNumber);
                var consumeContext = context.CreateConsumeContext();

                foreach (var item in context.Data.ItemsIds)
                {
                    builder.AddActivity("ProcessItem", new Uri("queue:ProcessItem_execute"), new ProcessItemArgument(item));
                }

                var routingSlip = builder.Build();
                await consumeContext.Execute(routingSlip);
            });
        }
    }
}