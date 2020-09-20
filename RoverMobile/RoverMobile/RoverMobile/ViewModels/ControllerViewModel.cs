using System;
using System.Collections.Generic;
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

            ConnectionStatus = Status.Connected;
        }

        public async Task PowerOffDevice()
        {
            await Task.Run(() =>
            { 
                if (ConnectionStatus.Equals(Status.Connected))
                {
                    //await client.PowerOffAsync(new PowerOffRequest());
                    System.Diagnostics.Debug.WriteLine("Powered Off!");
                    ConnectionStatus = Status.Disconnected;
                }
            });
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
                //await client.MoveAsync(new MoveRequest
                //{
                //    Speed = Speed,
                //    //Convert the UI enum to the grpc enum
                //    Direction = (GrpcController.Direction)((int)direction)
                //});
                System.Diagnostics.Debug.WriteLine("Moving " + direction.ToString());
            }
        }

        private async Task CheckConnection()
        {
            //if ((DateTime.Now - lastHeartbeat).TotalSeconds > connectionDelay
            //    || ConnectionStatus.Equals(Status.Disconnected))
            //{
            //    try
            //    {
            //        //Anything longer than 5 seconds is unreliable
            //        await client.HeartbeatAsync(new HeartbeatEcho(), deadline: DateTime.UtcNow.AddSeconds(5));
            //        ConnectionStatus = Status.Connected;
            //    }
            //    catch (Exception ex)
            //    {
            //        ConnectionStatus = Status.Disconnected;
            //        MessageBox.Show($"Connection error to host {_configuration.GetSection("TargetURL").Value}! Is the server online?");
            //    }
            //    lastHeartbeat = DateTime.Now;
            //}
        }

        public enum Direction { FORWARD, BACKWARD, LEFT, RIGHT, STOP }

        public enum Status { Disconnected, Connected }
    }
}
