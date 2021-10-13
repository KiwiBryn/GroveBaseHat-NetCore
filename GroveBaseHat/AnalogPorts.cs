//---------------------------------------------------------------------------------
// Copyright (c) September 2021, devMobile Software
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Must have one of following options defined in the project\build definitions
//		GROVE_BASE_HAT_RPI or GROVE_BASE_HAT_RPI_ZERO
//
//---------------------------------------------------------------------------------
#if (!GROVE_BASE_HAT_RPI && !GROVE_BASE_HAT_RPI_ZERO)
#error Library must have at least one of GROVE_BASE_HAT_RPI or GROVE_BASE_HAT_RPI_ZERO defined
#endif

#if (GROVE_BASE_HAT_RPI && GROVE_BASE_HAT_RPI_ZERO)
#error Library must have at most one of GROVE_BASE_HAT_RPI or GROVE_BASE_HAT_RPI_ZERO defined
#endif

namespace devMobile.IoT.NetCore.GroveBaseHat
{
	using System;
#if DEBUG
	using System.Diagnostics;
#endif
	using System.Device.I2c;

	public class AnalogPorts : IDisposable
	{
		public const int DefaultI2cAddress = 0x04;
		private const byte RegisterDeviceId = 0x0;
		private const byte RegisterVersion = 0x02;
		private const byte RegisterPowerSupplyVoltage = 0x29;
		private const byte RegisterRawBase = 0x10;
		private const byte RegisterVoltageBase = 0x20;
		private const byte RegisterValueBase = 0x30;
#if GROVE_BASE_HAT_RPI
		private const byte DeviceId = 0x0004;
#endif
#if GROVE_BASE_HAT_RPI_ZERO
		private const byte DeviceId = 0x0005;
#endif
		private I2cDevice _i2cDevice = null;

		public enum AnalogPort
		{
			A0 = 0,
			A1 = 1,
			A2 = 2,
			A3 = 3,
			A4 = 4,
			A5 = 5,
#if GROVE_BASE_HAT_RPI
			A6 = 6,
			A7 = 7,
#endif
		};

		public AnalogPorts(I2cDevice i2cDevice)
		{
			_i2cDevice = i2cDevice ?? throw new ArgumentNullException(nameof(i2cDevice));

			byte[] writeBuffer = new byte[1] { RegisterDeviceId };
			byte[] readBuffer = new byte[1] { 0 };

			_i2cDevice.WriteRead(writeBuffer, readBuffer);
			byte deviceId = readBuffer[0];
			Debug.WriteLine($"GroveBaseHatRPI DeviceId 0x{deviceId:X}");
			if (deviceId != DeviceId)
			{
				throw new ApplicationException("GroveBaseHatRPI not found");
			}
		}

		public byte Version()
		{
			byte[] writeBuffer = new byte[1] { RegisterVersion };
			byte[] readBuffer = new byte[1] { 0 };

			_i2cDevice.WriteRead(writeBuffer, readBuffer);
			byte version = readBuffer[0];

			Debug.WriteLine($"GroveBaseHatRPI version 0x{version:X}");

			return version;
		}

		public double PowerSupplyVoltage()
		{
			byte[] writeBuffer = new byte[1] { RegisterPowerSupplyVoltage };
			byte[] readBuffer = new byte[2] { 0, 0 };

			_i2cDevice.WriteRead(writeBuffer, readBuffer);
			ushort value = BitConverter.ToUInt16(readBuffer, 0);

			Debug.WriteLine($"GroveBaseHatRPI PowerSupplyVoltage MSB 0x{readBuffer[1]:X} LSB 0x{readBuffer[0]:X} Value {value}");

			return value / 1000.0;
		}

		public ushort ReadRaw(AnalogPort analogPort)
		{
			byte register = (byte)analogPort;
			register += RegisterRawBase;
			byte[] writeBuffer = new byte[1] { register };
			byte[] readBuffer = new byte[2] { 0, 0 };

			_i2cDevice.WriteRead(writeBuffer, readBuffer);
			ushort value = BitConverter.ToUInt16(readBuffer, 0);

			Debug.WriteLine($"GroveBaseHatRPI ReadRaw {analogPort} MSB 0x{readBuffer[1]:X} LSB 0x{readBuffer[0]:X} Value {value}");

			return value;
		}

		public double ReadVoltage(AnalogPort analogPort)
		{
			byte register = (byte)analogPort;
			register += RegisterVoltageBase;
			byte[] writeBuffer = new byte[1] { register };
			byte[] readBuffer = new byte[2] { 0, 0 };

			_i2cDevice.WriteRead(writeBuffer, readBuffer);
			ushort value = BitConverter.ToUInt16(readBuffer, 0);

			Debug.WriteLine($"GroveBaseHatRPI ReadVoltage {analogPort} MSB 0x{readBuffer[1]:X} LSB 0x{readBuffer[0]:X} Value {value}");

			return value / 1000.0;
		}

		public double Read(AnalogPort analogPort)
		{
			byte register = (byte)analogPort;
			register += RegisterValueBase;
			byte[] writeBuffer = new byte[1] { register };
			byte[] readBuffer = new byte[2] { 0, 0 };

			_i2cDevice.WriteRead(writeBuffer, readBuffer);
			ushort value = BitConverter.ToUInt16(readBuffer, 0);

			Debug.WriteLine($"GroveBaseHatRPI Read {analogPort} MSB 0x{readBuffer[1]:X} LSB 0x{readBuffer[0]:X} Value {value}");

			return value / 10.0;
		}

		public void Dispose()
		{
			_i2cDevice?.Dispose();
			_i2cDevice = null!;
		}
	}
}
