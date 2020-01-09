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

        /// <summary>
        /// Instructs the Pi, which instructs the Arduino, which instructs the motors
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<MoveReply> Move(MoveRequest request, ServerCallContext context)
        {
            var parsedEnum = Enum.GetName(typeof(Direction), request.Direction);
            Console.WriteLine($"Move request from host: {context.Peer}. Moving {parsedEnum} at speed {request.Speed} as requested.");
            return Task.FromResult(new MoveReply
            {
                Message = $"Performing move to location {parsedEnum} at speed {request.Speed}."
            });
        }

        /// <summary>
        /// Used to verify the connection state between client and server
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<HeartbeatReply> Heartbeat(HeartbeatEcho request, ServerCallContext context)
        {
            Console.WriteLine($"Received heartbeat from host: {context.Peer}");
            return Task.FromResult(new HeartbeatReply());
        }

        /// <summary>
        /// Used to remotely power down the Pi
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<PowerOffReply> PowerOff(PowerOffRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Received PowerOff request from host: {context.Peer}");
            //Ensure client is notified of receipt
            return Task.FromResult(new PowerOffReply());
        }
    }
}
