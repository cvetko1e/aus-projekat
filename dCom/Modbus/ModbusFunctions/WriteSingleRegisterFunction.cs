using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write single register functions/requests.
    /// </summary>
    public class WriteSingleRegisterFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleRegisterFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleRegisterFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            ModbusWriteCommandParameters p = (ModbusWriteCommandParameters)CommandParameters;
            byte[] retVal = new byte[12];

            ushort transactionId = (ushort)IPAddress.HostToNetworkOrder((short)CommandParameters.TransactionId);
            ushort protocolId = (ushort)IPAddress.HostToNetworkOrder((short)CommandParameters.ProtocolId);
            ushort length = (ushort)IPAddress.HostToNetworkOrder((short)CommandParameters.Length);
            ushort outputAddress = (ushort)IPAddress.HostToNetworkOrder((short)p.OutputAddress);
            ushort value = (ushort)IPAddress.HostToNetworkOrder((short)p.Value);

            Buffer.BlockCopy(BitConverter.GetBytes(transactionId), 0, retVal, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(protocolId), 0, retVal, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(length), 0, retVal, 4, 2);
            retVal[6] = CommandParameters.UnitId;
            retVal[7] = CommandParameters.FunctionCode;
            Buffer.BlockCopy(BitConverter.GetBytes(outputAddress), 0, retVal, 8, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, retVal, 10, 2);

            return retVal;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            Dictionary<Tuple<PointType, ushort>, ushort> r = new Dictionary<Tuple<PointType, ushort>, ushort>();

            if (response[7] != CommandParameters.FunctionCode + 0x80)
            {
                short address = BitConverter.ToInt16(response, 8);
                short value = BitConverter.ToInt16(response, 10);

                address = IPAddress.NetworkToHostOrder(address);
                value = IPAddress.NetworkToHostOrder(value);

                r.Add(new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, (ushort)address), (ushort)value);
            }
            else
            {
                HandeException(response[8]);
            }

            return r;
        }
    }
}
