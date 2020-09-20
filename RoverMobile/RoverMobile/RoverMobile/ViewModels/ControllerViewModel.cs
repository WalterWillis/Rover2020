using RoverMobileGrpcClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace RoverMobile.ViewModels
{
    class ControllerViewModel : BaseViewModel
    {
        private int speed;
        private int min;
        private int max;
        private int speedIncrement = 10;
        private List<string> displayVariants;
        private Status connectionStatus;
        private Client client;
        private DateTime lastHeartbeat;
        private int connectionDelay = 60; // how long between heartbeat calls

        private Task startTask;

        public int Speed { get => speed; set => SetProperty(ref speed, value); }
        public List<string> DisplayVariants
        { get => displayVariants; set => SetProperty(ref displayVariants, value); }

        public Status ConnectionStatus
        { get => connectionStatus; set => SetProperty(ref connectionStatus, value); }

        public ControllerViewModel(int speed = 100, int minimum = 0, int maximum = 255,
            int increments = 10)
        {
            Speed = speed;
            min = minimum;
            max = maximum;
            speedIncrement = increments;

            List<string> displays = new List<string>();
            displays.Add("Phase I");
            DisplayVariants = displays;

            client = new Client();

            //initialize with a time that is greater than the delay variable
            lastHeartbeat = DateTime.Now.AddSeconds(-connectionDelay + 1);

            startTask = Task.Run(async () =>
            {
                await client.CreateNewConnection(@"10.0.2.2", 5443);
                if (await client.HeartBeat())
                {
                    ConnectionStatus = Status.Connected;
                }
                else
                {
                    ConnectionStatus = Status.Disconnected;
                }
            });
        }

        public void Enable()
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                //do something here
            }
        }

        public void Disable()
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                //do something here
            }
        }

        public async Task PowerOffDevice()
        {
            await CheckConnection();

            if (ConnectionStatus.Equals(Status.Connected))
            {
                if (await client.PowerOff())
                {
                    System.Diagnostics.Debug.WriteLine("Powered Off!");
                    ConnectionStatus = Status.Disconnected;
                }
                else
                {
                    Debug.WriteLine("Failed to power off!");
                }
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
                await client.Move((int)direction, Speed);

                Debug.WriteLine("Moving " + direction.ToString());
            }
        }

        private async Task CheckConnection()
        {
            if ((DateTime.Now - lastHeartbeat).TotalSeconds > connectionDelay
                || ConnectionStatus.Equals(Status.Disconnected))
            {
                //Anything longer than 5 seconds is unreliable
                if (await client.HeartBeat())
                    ConnectionStatus = Status.Connected;
                else
                    ConnectionStatus = Status.Disconnected;

                lastHeartbeat = DateTime.Now;
            }
        }

        public enum Direction { FORWARD, BACKWARD, LEFT, RIGHT, STOP }

        public enum Status { Disconnected, Connected }
    }
}
