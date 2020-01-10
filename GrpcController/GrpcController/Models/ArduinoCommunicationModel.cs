using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcController
{
    public class ArduinoCommunicationModel : IArduinoCommunicationModel
    {
        private I2cDevice _arduino;
        private bool _initialized;

        public void Initialize(int bus = 1, int addr = 0x08)
        {
            if (!_initialized)
            {
                I2cConnectionSettings i2cSettings = new I2cConnectionSettings(bus, addr);
                _arduino = I2cDevice.Create(i2cSettings);
                _initialized = true;
            }
        }

        public void Move(Direction direction)
        {
            _arduino.WriteByte(Convert.ToByte(direction));
        }
    }
}
