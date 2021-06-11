using System;
using System.Collections.Generic;
using Automatonymous;

namespace async_request_processing.StateMachine
{
    public class RequestProcessingState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public Guid RequestId { get; set; }
        public Guid TrackingNumber { get; set; }

        public List<Guid> ToProcess { get; } = new();
        public List<Guid> Processed { get; } = new();
    }
}