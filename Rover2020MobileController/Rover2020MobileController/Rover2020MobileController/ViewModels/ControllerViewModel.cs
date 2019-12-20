using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        public async Task PowerOffDevice()
        {           
            await Task.Run(() => 
            { System.Diagnostics.Debug.WriteLine("Powered Off!"); /*power down device*/ }); 
        }
    }
}
