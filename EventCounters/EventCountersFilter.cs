using System.Diagnostics;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Courier;

namespace async_request_processing.EventCounters
{
    public class EventCountersFilter<T> : IFilter<ExecuteContext<T>> where T : class
    {
        private readonly Stopwatch _stopwatch = new();
        public async Task Send(ExecuteContext<T> context, IPipe<ExecuteContext<T>> next)
        {
            _stopwatch.Start();
            await next.Send(context);
            _stopwatch.Stop();

            ActivityEventCountersSource.Instance.Log(_stopwatch.ElapsedMilliseconds);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("event-counters");
        }
    }
}