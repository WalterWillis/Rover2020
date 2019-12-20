using System;
using System.Collections.Generic;
using System.Text;

namespace Rover2020MobileController.ViewModels
{
    public class ControllerViewModel : BaseViewModel
    {
        private int speed;
        public int Speed { get => speed; set => SetProperty(ref speed, value); }
        public ControllerViewModel(int speed = 100)
        {
            Speed = speed;
        }
    }
}
