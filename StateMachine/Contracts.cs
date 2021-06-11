using System;
using System.Collections.Generic;

namespace async_request_processing.StateMachine
{
    public static class Contracts
    {
        public record ProcessRequestCommand(Guid RequestId, List<Guid> ItemsIds);
        public record RequestAccepted(Guid RequestId);
        public record ItemProcessed(Guid Id, Guid TrackingNumber);
        public record RequestStatusQuery(Guid RequestId);
        public record RequestStatus(string Status, List<Guid> ToProcess, List<Guid> Processed);
    }
}