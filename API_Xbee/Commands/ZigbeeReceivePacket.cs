using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace API_Xbee.Commands
{
    public class ZigbeeReceivePacket : XbeeFrame
    {
        public byte[] SA64Bits = new byte[8];
        public byte[] SA16Bits = new byte[2] { 0xFF, 0xFE };

        public byte[] ReceivedData { get; set; }

        public string CadenaRecibida = "";

        public ReceiveOptions OptionReceived;
        /// <summary>
        /// Cuando el modulo recibe un Paquete por RF, lo envía a la UART usando este tipo de mensaje.
        /// </summary>
        /// <param name="arraydebytes">Arreglo de bytes que llega por la UART</param>
        public ZigbeeReceivePacket(byte[] arraydebytes)
        {
            byte tipoframe = arraydebytes[3];
       

            if (tipoframe == (byte)API_Xbee.Commands.TypeApiFramesNames.Zigbee_Receive_Packter_AO_0)
            {
                //Aqui todo...

                //Source Address 64bits
                for (int i = 4; i < 12; i++)
                {
                    SA64Bits[11 - i] = arraydebytes[i];
                }

                //Source Address 16bits
                SA64Bits[1] = arraydebytes[12];
                SA64Bits[0] = arraydebytes[13];

                if (arraydebytes[14] == (byte)ZigbeeReceivePacket.ReceiveOptions.Packet_Acknowledged)
                    OptionReceived = ReceiveOptions.Packet_Acknowledged;

                if (arraydebytes[14] == (byte)ZigbeeReceivePacket.ReceiveOptions.Packet_Was_A_Broadcast_Packet)
                    OptionReceived = ReceiveOptions.Packet_Was_A_Broadcast_Packet;

                if (arraydebytes[14] == (byte)ZigbeeReceivePacket.ReceiveOptions.Packet_Encrypted_With_APS_Encryption)
                    OptionReceived = ReceiveOptions.Packet_Encrypted_With_APS_Encryption;

                if (arraydebytes[14] == (byte)ZigbeeReceivePacket.ReceiveOptions.Packet_Was_Sent_From_An_End_Device)
                    OptionReceived = ReceiveOptions.Packet_Was_Sent_From_An_End_Device;

                ReceivedData = new byte[arraydebytes.Length - 16];

                for (int i = 15; i < arraydebytes.Length - 1; i++)
                {
                    ReceivedData[i - 15] = arraydebytes[i];
                }

                //if (arraydebytes[1] != this.LengthMSB) throw new Exception("MSB Diferente");
                //if (arraydebytes[2] != this.LengthLSB) throw new Exception("LSB Diferente");
                System.Text.ASCIIEncoding codificador = new System.Text.ASCIIEncoding();
                CadenaRecibida = codificador.GetString(ReceivedData);
            }
            else
            {
                throw new Exception("No es Respuesta a Paquete recibo por RF");
            }

            //throw new NotImplementedException();
        }

        public enum ReceiveOptions
        {
            Packet_Acknowledged = 0x01,
            Packet_Was_A_Broadcast_Packet = 0x02,
            Packet_Encrypted_With_APS_Encryption = 0x20,
            Packet_Was_Sent_From_An_End_Device = 0x40
        }
    }
}
