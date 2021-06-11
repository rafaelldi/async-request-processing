using Automatonymous;
using MassTransit.Courier.Contracts;
using Microsoft.Extensions.Logging;
using static async_request_processing.StateMachine.Contracts;

namespace async_request_processing.StateMachine
{
    public class RequestProcessingStateMachine : MassTransitStateMachine<RequestProcessingState>
    {
        public RequestProcessingStateMachine(ILogger<RequestProcessingStateMachine> logger)
        {
            InstanceState(x => x.CurrentState, Processing, Completed);

            Event(() => ProcessRequestCommand, x => x.CorrelateById(m => m.Message.RequestId));
            Event(() => RequestStatusQueried, x => x.CorrelateById(m => m.Message.RequestId));
            Event(() => ItemProcessed,
                x => x.CorrelateById(instance => instance.TrackingNumber, context => context.Message.TrackingNumber));
            Event(() => RequestProcessingCompleted,
                x => x.CorrelateById(instance => instance.TrackingNumber, context => context.Message.TrackingNumber));

            Initially(
                When(ProcessRequestCommand)
                    .Then(context =>
                    {
                        logger.LogInformation("Start processing request with id={RequestId}", context.Data.RequestId);
                        context.Instance.RequestId = context.Data.RequestId;
                        context.Instance.ToProcess.AddRange(context.Data.ItemsIds);
                    })
                    .CreateRoutingSlip()
                    .TransitionTo(Processing)
                    .Respond(context => new RequestAccepted(context.Instance.RequestId)));

            During(Processing,
                When(ItemProcessed)
                    .Then(context =>
                    {
                        logger.LogInformation("Item with id={ItemId} is processed", context.Data.Id);
                        context.Instance.ToProcess.Remove(context.Data.Id);
                        context.Instance.Processed.Add(context.Data.Id);
                    }));

            During(Processing,
                When(ProcessRequestCommand)
                    .Respond(context => new RequestAccepted(context.Instance.RequestId)));

            During(Processing,
                When(RequestStatusQueried)
                    .Respond(context => new RequestStatus(
                        $"{nameof(Processing)}",
                        context.Instance.ToProcess,
                        context.Instance.Processed
                    )));

            During(Processing,
                When(RequestProcessingCompleted)
                    .Then(context =>
                    {
                        logger.LogInformation("Process completed for request with id={RequestId}",
                            context.Instance.RequestId);
                    })
                    .TransitionTo(Completed));

            During(Completed,
                When(RequestStatusQueried)
                    .Respond(context => new RequestStatus(
                        $"{nameof(Completed)}",
                        context.Instance.ToProcess,
                        context.Instance.Processed
                    )));
        }

        public Event<ProcessRequestCommand> ProcessRequestCommand { get; }
        public Event<RequestStatusQuery> RequestStatusQueried { get; }
        public Event<ItemProcessed> ItemProcessed { get; }
        public Event<RoutingSlipCompleted> RequestProcessingCompleted { get; }

        public State Processing { get; }
        public State Completed { get; }
    }
}