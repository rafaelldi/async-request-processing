using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using static async_request_processing.StateMachine.Contracts;

namespace async_request_processing
{
    [ApiController]
    [Route("requests")]
    public class RequestsController : ControllerBase
    {
        private readonly IRequestClient<ProcessRequestCommand> _processRequestClient;
        private readonly IRequestClient<RequestStatusQuery> _getRequestStatusClient;

        public RequestsController(
            IRequestClient<ProcessRequestCommand> processRequestClient,
            IRequestClient<RequestStatusQuery> getRequestStatusClient)
        {
            _processRequestClient = processRequestClient;
            _getRequestStatusClient = getRequestStatusClient;
        }

        [HttpPost]
        public async Task<IActionResult> Process(Request request)
        {
            var response =
                await _processRequestClient.GetResponse<RequestAccepted>(new ProcessRequestCommand(request.Id, request.Items));
            return Ok(response.Message.RequestId);
        }

        [HttpGet("{id:guid}/status")]
        public async Task<IActionResult> Status(Guid id)
        {
            var response =
                await _getRequestStatusClient.GetResponse<RequestStatus>(new RequestStatusQuery(id));
            return Ok(new
            {
                response.Message.Status,
                toProcess = string.Join(",", response.Message.ToProcess),
                processed = string.Join(",", response.Message.Processed)
            });
        }
    }

    public record Request(Guid Id, List<Guid> Items);
}