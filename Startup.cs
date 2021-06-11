using async_request_processing.Activity;
using async_request_processing.StateMachine;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using static async_request_processing.StateMachine.Contracts;

namespace async_request_processing
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<RandomService>();
            services.AddControllers();
            services.AddMassTransit(x =>
                {
                    x.AddExecuteActivity<ProcessItemActivity, ProcessItemArgument>();

                    x.AddSagaStateMachine<RequestProcessingStateMachine, RequestProcessingState>()
                        .InMemoryRepository();

                    x.AddRequestClient<ProcessRequestCommand>();
                    x.AddRequestClient<RequestStatusQuery>();

                    x.UsingRabbitMq((context, cfg) => { cfg.ConfigureEndpoints(context); });
                })
                .AddMassTransitHostedService();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}