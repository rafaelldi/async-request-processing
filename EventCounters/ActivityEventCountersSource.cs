using System.Diagnostics.Tracing;

namespace async_request_processing.EventCounters
{
    [EventSource(Name = "AsyncRequestProcessing.ActivityEventCounters")]
    public sealed class ActivityEventCountersSource : EventSource
    {
        public static readonly ActivityEventCountersSource Instance = new();
        private EventCounter _processingTimeCounter;

        private ActivityEventCountersSource()
        {
            _processingTimeCounter = new EventCounter("processing-time", this)
            {
                DisplayName = "Item Processing Time",
                DisplayUnits = "ms"
            };
        }

        public void Log(long elapsedMilliseconds)
        {
            _processingTimeCounter?.WriteMetric(elapsedMilliseconds);
        }

        protected override void Dispose(bool disposing)
        {
            _processingTimeCounter?.Dispose();
            _processingTimeCounter = null;

            base.Dispose(disposing);
        }
    }
}