using GrpcController;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using System.Threading;

namespace RoverMobileGrpcClient
{
    public class Client : IDisposable
    {
        private GrpcChannel channel;
        private Controller.ControllerClient client;

        /// <summary>
        /// Sets up the client to call from a new connection;
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="metadata"></param>
        /// <param name="metaDataValue"></param>
        public async Task CreateNewConnection(string host)
        {
            try
            {
                await CloseExistingConnection();

                Uri uri = new Uri($"https://{host}");

                channel = GrpcChannel.ForAddress(uri.AbsoluteUri, new GrpcChannelOptions
                {
                    //Let's use a client in case there's more we want to add later
                    HttpClient = new HttpClient(
                        new GrpcWebHandler(
                            new HttpClientHandler()
                            {
                                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                                {
                                    return true;
                                }
                            }
                        )
                    )
                });

                client = new Controller.ControllerClient(channel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to create a new connection!");
            }
        }

        /// <summary>
        /// Used to test connections to hosts
        /// </summary>
        /// <returns></returns>
        public async static Task<bool> TryConnection(string host)
        {
            bool canConnect = false;
            try
            {
                Uri uri = new Uri($"https://{host}");

                int baseTimeout = 2000; // give some time in ms. Shouldn't need long.

                GrpcChannel tempChannel = GrpcChannel.ForAddress(uri.AbsoluteUri, new GrpcChannelOptions
                {
                    //Let's use a client in case there's more we want to add later
                    HttpClient = new HttpClient(
                        new GrpcWebHandler(
                            new HttpClientHandler()
                            {
                                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                                {
                                    return true;
                                }
                            }
                        )
                    )
                });

                Controller.ControllerClient client = new Controller.ControllerClient(tempChannel);

                Task<HeartbeatReply> connectTask = Task.Run(() => 
                {
                    return client.Heartbeat(
                        new HeartbeatEcho(), deadline: DateTime.UtcNow.AddSeconds(2));
                });


                var completedTask = await Task.WhenAny(connectTask, Task.Delay(baseTimeout));


                if (completedTask == connectTask && connectTask.Result != null)
                    canConnect = true;
            }
            catch (Grpc.Core.RpcException ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.Status);
            }
            catch (Exception ex)
            {
                // ignore
            }
            if (canConnect)
                Debug.WriteLine($"Connected to {host}");

            return canConnect;
        }

        public async Task<bool> Move(int direction, int speed)
        {
            bool success = false;
            try
            {
                MoveRequest request = new MoveRequest()
                {
                    Direction = (GrpcController.Direction)direction,
                    Speed = speed
                };

                await client.MoveAsync(request, deadline: DateTime.UtcNow.AddSeconds(5));

                success = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to Move!");
            }

            return success;
        }

        public async Task<bool> PowerOff()
        {
            bool poweredOff = false;

            try
            {
                PowerOffRequest request = new PowerOffRequest();

                await client.PowerOffAsync(request, deadline: DateTime.UtcNow.AddSeconds(5));

                poweredOff = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to power off device!");
            }

            return poweredOff;
        }

        /// <summary>
        /// Try to verify the connection.
        /// </summary>
        /// <returns>True if the connection succeeded.</returns>
        public async Task<bool> HeartBeat()
        {
            bool connected = false;
            try
            {
                await client.HeartbeatAsync(new HeartbeatEcho(), deadline: DateTime.UtcNow.AddSeconds(5));
                connected = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed Heartbeat: {ex.Message}\n{ex.StackTrace}");
                //maybe log
            }

            return connected;
        }

        public async Task CloseExistingConnection()
        {
            try
            {
                if (client != null)
                {
                    await channel.ShutdownAsync();
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to cleanly close connection!");
            }
        }

        public async void Dispose()
        {
            await CloseExistingConnection();
        }
    }
}
