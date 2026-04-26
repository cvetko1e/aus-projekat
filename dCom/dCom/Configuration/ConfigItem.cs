using Common;
using System;
using System.Collections.Generic;

namespace dCom.Configuration
{
    internal class ConfigItem : IConfigItem
	{
		#region Fields

		private PointType registryType;
		private ushort numberOfRegisters;
		private ushort startAddress;
		private ushort decimalSeparatorPlace;
		private ushort minValue;
		private ushort maxValue;
		private ushort defaultValue;
		private string processingType;
		private string description;
		private int acquisitionInterval;
		private double scalingFactor;
		private double deviation;
		private double egu_max;
		private double egu_min;
		private ushort abnormalValue;
		private double highLimit;
		private double lowLimit;
        private int secondsPassedSinceLastPoll;

		#endregion Fields

		#region Properties

		public PointType RegistryType
		{
			get
			{
				return registryType;
			}

			set
			{
				registryType = value;
			}
		}

		public ushort NumberOfRegisters
		{
			get
			{
				return numberOfRegisters;
			}

			set
			{
				numberOfRegisters = value;
			}
		}

		public ushort StartAddress
		{
			get
			{
				return startAddress;
			}

			set
			{
				startAddress = value;
			}
		}

		public ushort DecimalSeparatorPlace
		{
			get
			{
				return decimalSeparatorPlace;
			}

			set
			{
				decimalSeparatorPlace = value;
			}
		}

		public ushort MinValue
		{
			get
			{
				return minValue;
			}

			set
			{
				minValue = value;
			}
		}

		public ushort MaxValue
		{
			get
			{
				return maxValue;
			}

			set
			{
				maxValue = value;
			}
		}

		public ushort DefaultValue
		{
			get
			{
				return defaultValue;
			}

			set
			{
				defaultValue = value;
			}
		}

		public string ProcessingType
		{
			get
			{
				return processingType;
			}

			set
			{
				processingType = value;
			}
		}

		public string Description
		{
			get
			{
				return description;
			}

			set
			{
				description = value;
			}
		}

		public int AcquisitionInterval
		{
			get
			{
				return acquisitionInterval;
			}

			set
			{
				acquisitionInterval = value;
			}
		}

		public double ScaleFactor
		{
			get
			{
				return scalingFactor;
			}
			set
			{
				scalingFactor = value;
			} 
		}

		public double Deviation
		{
			get
			{
				return deviation;
			}

			set
			{
				deviation = value;
			}
		}

		public double EGU_Max
		{
			get
			{
				return egu_max;
			}

			set
			{
				egu_max = value;
			}
		}

		public double EGU_Min
		{
			get
			{
				return egu_min;
			}

			set
			{
				egu_min = value;
			}
		}

		public ushort AbnormalValue
		{
			get
			{
				return abnormalValue;
			}

			set
			{
				abnormalValue = value;
			}
		}

		public double HighLimit
		{
			get
			{
				return highLimit;
			}

			set
			{
				highLimit = value;
			}
		}

		public double LowLimit
		{
			get
			{
				return lowLimit;
			}

			set
			{
				lowLimit = value;
			}
		}

        public int SecondsPassedSinceLastPoll
        {
            get
            {
                return secondsPassedSinceLastPoll;
            }

            set
            {
                secondsPassedSinceLastPoll = value;
            }
        }

        #endregion Properties

        public ConfigItem(List<string> configurationParameters)
		{
			RegistryType = GetRegistryType(configurationParameters[0]);
			int temp;
			Int32.TryParse(configurationParameters[1], out temp);
			NumberOfRegisters = (ushort)temp;
			Int32.TryParse(configurationParameters[2], out temp);
			StartAddress = (ushort)temp;
			Int32.TryParse(configurationParameters[3], out temp);
			DecimalSeparatorPlace = (ushort)temp;
			Int32.TryParse(configurationParameters[4], out temp);
			MinValue = (ushort)temp;
			Int32.TryParse(configurationParameters[5], out temp);
			MaxValue = (ushort)temp;
			Int32.TryParse(configurationParameters[6], out temp);
			DefaultValue = (ushort)temp;
			ProcessingType = configurationParameters[7];
			Description = configurationParameters[8].TrimStart('@');
            if (configurationParameters[9].Equals("#"))
            {
                AcquisitionInterval = 1;
            }
            else
            {
                Int32.TryParse(configurationParameters[9], out temp);
                AcquisitionInterval = temp;
            }

            if (configurationParameters.Count >= 17)
            {
                ScaleFactor = ParseDoubleParameter(configurationParameters[10], 1);
                Deviation = ParseDoubleParameter(configurationParameters[11], 0);
                EGU_Min = ParseDoubleParameter(configurationParameters[12], 0);
                EGU_Max = ParseDoubleParameter(configurationParameters[13], 0);
                AbnormalValue = ParseUShortParameter(configurationParameters[14], 0);
                HighLimit = ParseDoubleParameter(configurationParameters[15], 0);
                LowLimit = ParseDoubleParameter(configurationParameters[16], 0);
            }
            else
            {
                ScaleFactor = 1;
                Deviation = 0;
                EGU_Min = 0;
                EGU_Max = 0;
                AbnormalValue = 0;
                HighLimit = 0;
                LowLimit = 0;
            }
        }

        private double ParseDoubleParameter(string parameter, double defaultValue)
        {
            if (parameter == "#")
            {
                return defaultValue;
            }

            double value;
            if (Double.TryParse(parameter, out value))
            {
                return value;
            }

            return defaultValue;
        }

        private ushort ParseUShortParameter(string parameter, ushort defaultValue)
        {
            if (parameter == "#")
            {
                return defaultValue;
            }

            int value;
            if (Int32.TryParse(parameter, out value))
            {
                return (ushort)value;
            }

            return defaultValue;
        }

		private PointType GetRegistryType(string registryTypeName)
		{
			PointType registryType;
			switch (registryTypeName)
			{
				case "DO_REG":
					registryType = PointType.DIGITAL_OUTPUT;
					break;

				case "DI_REG":
					registryType = PointType.DIGITAL_INPUT;
					break;

				case "IN_REG":
					registryType = PointType.ANALOG_INPUT;
					break;

				case "HR_INT":
					registryType = PointType.ANALOG_OUTPUT;
					break;

				default:
					registryType = PointType.HR_LONG;
					break;
			}
			return registryType;
		}
	}
}
