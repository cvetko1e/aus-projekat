using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read coil functions/requests.
    /// </summary>
    public class ReadCoilsFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadCoilsFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
		public ReadCoilsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc/>
        public override byte[] PackRequest()
        {
            ModbusReadCommandParameters p = (ModbusReadCommandParameters)CommandParameters;
            byte[] retVal = new byte[12];

            ushort transactionId = (ushort)IPAddress.HostToNetworkOrder((short)CommandParameters.TransactionId);
            ushort protocolId = (ushort)IPAddress.HostToNetworkOrder((short)CommandParameters.ProtocolId);
            ushort length = (ushort)IPAddress.HostToNetworkOrder((short)CommandParameters.Length);
            ushort startAddress = (ushort)IPAddress.HostToNetworkOrder((short)p.StartAddress);
            ushort quantity = (ushort)IPAddress.HostToNetworkOrder((short)p.Quantity);

            Buffer.BlockCopy(BitConverter.GetBytes(transactionId), 0, retVal, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(protocolId), 0, retVal, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(length), 0, retVal, 4, 2);
            retVal[6] = CommandParameters.UnitId;
            retVal[7] = CommandParameters.FunctionCode;
            Buffer.BlockCopy(BitConverter.GetBytes(startAddress), 0, retVal, 8, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(quantity), 0, retVal, 10, 2);

            return retVal;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            Dictionary<Tuple<PointType, ushort>, ushort> r = new Dictionary<Tuple<PointType, ushort>, ushort>();

            if (response[7] != CommandParameters.FunctionCode + 0x80)
            {
                ushort startAddress = ((ModbusReadCommandParameters)CommandParameters).StartAddress;
                ushort quantity = ((ModbusReadCommandParameters)CommandParameters).Quantity;
                int count = 0;

                for (int i = 0; i < response[8]; i++)
                {
                    byte currentByte = response[9 + i];
                    for (int bit = 0; bit < 8; bit++)
                    {
                        ushort value = (ushort)((currentByte >> bit) & 0x01);
                        r.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, startAddress), value);
                        count++;
                        startAddress++;

                        if (count >= quantity)
                        {
                            break;
                        }
                    }

                    if (count >= quantity)
                    {
                        break;
                    }
                }
            }
            else
            {
                HandeException(response[8]);
            }

            return r;
        }
    }
}
