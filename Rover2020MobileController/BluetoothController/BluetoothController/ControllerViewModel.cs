using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BluetoothController.ViewModels
{
    public class ControllerViewModel : INotifyPropertyChanged
    {
        private int speed;
        private int min;
        private int max;
        private int speedIncrement = 10;
        private List<string> displayVariants;
        private List<string> devices;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Speed { get => speed; set =>  SetProperty(ref speed, value); }
        public List<string> DisplayVariants 
            { get => displayVariants; set => SetProperty(ref displayVariants, value); }

        public List<string> Devices
        { get => devices; set => SetProperty(ref devices, value); }

        public ControllerViewModel(int speed = 100, int minimum = 0, int maximum = 255,
            int increments = 10)
        {
            Speed = speed;
            min = minimum;
            max = maximum;
            speedIncrement = increments;

            List<string> displays = new List<string>();
            displays.Add("BRCTC SpaceRover UI Phase I");
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

        public enum Direction { NONE, FORWARD, BACKWARD, LEFT, RIGHT}

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
