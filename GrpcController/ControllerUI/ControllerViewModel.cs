using Grpc.Net.Client;
using GrpcController;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace ControllerUI.ViewModels
{
    public class ControllerViewModel : INotifyPropertyChanged
    {
       
        private int min;
        private int max;
        private int speedIncrement = 10;
        private Controller.ControllerClient client;
        private GrpcChannel channel;
        private DateTime lastHeartbeat;
        private int connectionDelay = 60; // how long between heartbeat calls

        private List<string> displayVariants;
        private List<string> devices;
        private int speed;
        private Status connectionStatus;
        private IConfiguration _configuration;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Speed { get => speed; set => SetProperty(ref speed, value); }
        public List<string> DisplayVariants
        { get => displayVariants; set => SetProperty(ref displayVariants, value); }

        public List<string> Devices
        { get => devices; set => SetProperty(ref devices, value); }
        public Status ConnectionStatus
        { get => connectionStatus; set => SetProperty(ref connectionStatus, value); }

        public ControllerViewModel(IConfiguration configuration, int speed = 100, int minimum = 0, int maximum = 255,
            int increments = 10)
        {
            Speed = speed;
            min = minimum;
            max = maximum;
            speedIncrement = increments;
            _configuration = configuration;

            List<string> displays = new List<string>();
            displays.Add("BRCTC SpaceRover UI Phase I");
            DisplayVariants = displays;
            try
            {
                var handler = new HttpClientHandler();
                handler.ClientCertificates.Add(Helpers.GetClientCertificate(_configuration.GetSection("ClientCert").Value, _configuration.GetSection("ClientCertPass").Value));
                
                bool.TryParse(_configuration.GetSection("ValidateCerts").Value, out bool verifyCerts);

                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    bool isValid = false;

                    if (verifyCerts)
                        isValid = cert.Equals(Helpers.GetServerCertificate(_configuration.GetSection("ServerCert").Value, 
                            _configuration.GetSection("ServerCertPass").Value));
                    else
                        isValid = true;

                    return isValid;
                };
                var httpClient = new HttpClient(handler);

                //Create the channel given our parameters
                channel = GrpcChannel.ForAddress(_configuration.GetSection("TargetURL").Value, new GrpcChannelOptions { HttpClient = httpClient });
                client = new Controller.ControllerClient(channel);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error configuring connection to server. Cannot find certificate. " +
                    $"{Environment.NewLine} Program requires a server.pfx and client.pfx file. ");
            }
            //initialize with a time that is greater than the delay variable
            lastHeartbeat = DateTime.Now.AddSeconds(-connectionDelay + 1);
            //call willy nilly, message box will not show until timeout ocurrs.
            CheckConnection();

        }

        public async Task PowerOffDevice()
        {
            await CheckConnection();
            if (ConnectionStatus.Equals(Status.Connected))
            {
                await client.PowerOffAsync(new PowerOffRequest());
                System.Diagnostics.Debug.WriteLine("Powered Off!");
                ConnectionStatus = Status.Disconnected;
            }
        }

        public void IncreaseSpeed()
        {
            if (speed != max)
            {
                int newSpeed = speed + speedIncrement;

                if (newSpeed >= max)
                    Speed = max;
                else
                    Speed = newSpeed;
            }
        }

        public void DecreaseSpeed()
        {
            if (speed != min)
            {
                int newSpeed = speed - speedIncrement;

                if (newSpeed <= min)
                    Speed = min;
                else
                    Speed = newSpeed;
            }
        }

        public async void Move(Direction direction)
        {
            await CheckConnection();

            if (ConnectionStatus.Equals(Status.Connected))
            {
                MoveReply reply = await client.MoveAsync(new MoveRequest { Speed = Speed, 
                        //Convert the UI enum to the grpc enum
                        Direction = (GrpcController.Direction)((int)direction) });
                System.Diagnostics.Debug.WriteLine("Moving " + direction.ToString());
            }
        }

        public enum Direction { FORWARD, BACKWARD, LEFT, RIGHT, STOP }
        public enum Status { Disconnected, Connected }

        private async Task CheckConnection()
        {
            if ((DateTime.Now - lastHeartbeat).TotalSeconds > connectionDelay 
                || ConnectionStatus.Equals(Status.Disconnected))
            {
                try
                {
                    //Anything longer than 5 seconds is unreliable
                    await client.HeartbeatAsync(new HeartbeatEcho(), deadline: DateTime.UtcNow.AddSeconds(5));
                    ConnectionStatus = Status.Connected;
                }
                catch(Exception ex)
                {
                    ConnectionStatus = Status.Disconnected;
                    MessageBox.Show($"Connection error to host {_configuration.GetSection("TargetURL").Value}! Is the server online?");
                }
                lastHeartbeat = DateTime.Now;
            }
        }

        //Found online - super useful
        protected bool SetProperty<T>(ref T backingStore, T value,
                [CallerMemberName]string propertyName = "",
                Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
