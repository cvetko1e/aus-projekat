using Common;

namespace ProcessingModule
{
    /// <summary>
    /// Class containing logic for alarm processing.
    /// </summary>
    public class AlarmProcessor
	{
        /// <summary>
        /// Processes the alarm for analog point.
        /// </summary>
        /// <param name="eguValue">The EGU value of the point.</param>
        /// <param name="configItem">The configuration item.</param>
        /// <returns>The alarm indication.</returns>
		public AlarmType GetAlarmForAnalogPoint(double eguValue, IConfigItem configItem)
		{
            if (configItem == null)
            {
                return AlarmType.NO_ALARM;
            }

            if (configItem.AbnormalValue != 0 && (ushort)eguValue == configItem.AbnormalValue)
            {
                return AlarmType.ABNORMAL_VALUE;
            }

            if (eguValue > configItem.HighLimit && configItem.HighLimit != 0)
            {
                return AlarmType.HIGH_ALARM;
            }

            if (eguValue < configItem.LowLimit && configItem.LowLimit != 0)
            {
                return AlarmType.LOW_ALARM;
            }

            if (configItem.EGU_Min != 0 || configItem.EGU_Max != 0)
            {
                if (eguValue < configItem.EGU_Min || eguValue > configItem.EGU_Max)
                {
                    return AlarmType.REASONABILITY_FAILURE;
                }
            }

            return AlarmType.NO_ALARM;
		}

        /// <summary>
        /// Processes the alarm for digital point.
        /// </summary>
        /// <param name="state">The digital point state</param>
        /// <param name="configItem">The configuration item.</param>
        /// <returns>The alarm indication.</returns>
		public AlarmType GetAlarmForDigitalPoint(ushort state, IConfigItem configItem)
		{
            if (state > 1)
            {
                return AlarmType.REASONABILITY_FAILURE;
            }

            return AlarmType.NO_ALARM;
        }
	}
}
