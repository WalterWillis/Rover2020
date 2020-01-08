using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcController
{
    public class ControllerService : Controller.ControllerBase
    {
        private readonly ILogger<ControllerService> _logger;
        public ControllerService(ILogger<ControllerService> logger)
        {
            _logger = logger;
        }

        public override Task<MoveReply> Move(MoveRequest request, ServerCallContext context)
        {
            var parsedEnum = Enum.GetName(typeof(Direction), request.Direction);
            Console.WriteLine($"Performing move to location {parsedEnum}");
            return Task.FromResult(new MoveReply
            {
                Message = $"Hello {request.Name}. Moving {parsedEnum} as requested."
            });
        }
    }
}
