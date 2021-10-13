//---------------------------------------------------------------------------------
// Copyright (c) October 2021, devMobile Software
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
//---------------------------------------------------------------------------------
namespace devMobile.IoT.NetCore.GroveBaseHat
{
	using System;
	using System.Device.I2c;
	using System.Threading;

	class Program
	{
		static void Main(string[] args)
		{
			// bus id on the raspberry pi 3
			const int busId = 1;

			I2cConnectionSettings i2cConnectionSettings = new(busId, AnalogPorts.DefaultI2cAddress);

			using (I2cDevice i2cDevice = I2cDevice.Create(i2cConnectionSettings))
			using (AnalogPorts AnalogPorts = new AnalogPorts(i2cDevice))
			{
				Console.WriteLine($"{DateTime.Now:HH:mm:SS} Version:{AnalogPorts.Version()}");
				Console.WriteLine();

				double powerSupplyVoltage = AnalogPorts.PowerSupplyVoltage();
				Console.WriteLine($"{DateTime.Now:HH:mm:SS} Power Supply Voltage:{powerSupplyVoltage:F2}v");

				while (true)
				{
					double value = AnalogPorts.Read(AnalogPorts.AnalogPort.A0);
					double rawValue = AnalogPorts.ReadRaw(AnalogPorts.AnalogPort.A0);
					double voltageValue = AnalogPorts.ReadVoltage(AnalogPorts.AnalogPort.A0);

					Console.WriteLine($"{DateTime.Now:HH:mm:SS} Value:{value:F2} Raw:{rawValue:F2} Voltage:{voltageValue:F2}v");
					Console.WriteLine();

					Thread.Sleep(1000);
				}
			}
		}
	}
}
