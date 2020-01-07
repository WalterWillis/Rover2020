using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcGreeter
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            var parsedEnum = Enum.GetName(typeof(Direction), request.Direction);
            Console.WriteLine($"Performing move to location {parsedEnum}");
            return Task.FromResult(new HelloReply
            {
                Message = $"Hello {request.Name}. Moving {parsedEnum} as requested."
            });
        }
    }
}
