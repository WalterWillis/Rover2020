using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rover2020MobileController.ViewModels
{
    public class ControllerViewModel : BaseViewModel
    {
        private int speed;
        private int min;
        private int max;
        private int speedIncrement = 10;
        private List<string> displayVariants;
        public int Speed { get => speed; set => SetProperty(ref speed, value); }
        public List<string> DisplayVariants 
            { get => displayVariants; set => SetProperty(ref displayVariants, value); }

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
        }

        public async Task PowerOffDevice()
        {
            await Task.Run(() =>
            { System.Diagnostics.Debug.WriteLine("Powered Off!"); /*power down device*/ });
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

        public void Move(Direction direction)
        {
            System.Diagnostics.Debug.WriteLine("Moving " + direction.ToString());
        }

        public enum Direction { FORWARD, BACKWARD, LEFT, RIGHT}
    }
}
