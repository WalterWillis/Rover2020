using System;
using System.Net.Http;
using System.Threading.Tasks;
using GrpcGreeter;
using Grpc.Net.Client;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace GrpcGreeterClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            bool endProgram = false;

            //We don't care about mutual authentication here, so accept any cert
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => 
            { 
                return true; 
            };
            var httpClient = new HttpClient(handler);
            
            //Create the channel given our parameters
            var channel = GrpcChannel.ForAddress("https://MobileDestroyer:5443", new GrpcChannelOptions { HttpClient = httpClient });
            var client = new Greeter.GreeterClient(channel);
            while (!endProgram)
            {
                try
                {
                    var reply = await client.SayHelloAsync(
                                      new HelloRequest { Name = "GreeterClient", Direction = Direction.Forward });
                    Console.WriteLine("Greeting: " + reply.Message);
                } 
                //errors are likely due to a mishap in configuration, so ignore for debug
                finally
                {
                    var input = Console.ReadKey();

                    if (input.Key == ConsoleKey.Y)
                        endProgram = true;
                }
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}