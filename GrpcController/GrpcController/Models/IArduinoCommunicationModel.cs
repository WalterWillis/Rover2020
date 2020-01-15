using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcController
{
    /// <summary>
    /// Interface for singleton dependency injection so we only need to initialize the I2C bus once.
    /// </summary>
    public interface IArduinoCommunicationModel
    {
        public void Move(Direction direction, int speed);

        public void Initialize(int bus = 1, int addr = 0x08);
    }
}
