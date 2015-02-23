using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace API_Xbee.Commands
{
    public class ZigbeeTransmitRequestFrame : XbeeFrame
    {

        public byte BroadcastRadius = 0x00;
        public Options Opcion = Options.None;
        public byte[] DA64Bits = new byte[8];
        public byte[] DA16Bits = new byte[2] { 0xFF, 0xFE };

        public byte[] RFData;


        /// <summary>
        /// Envia datos desde el módulo como un paquete RF a un destino específico.
        /// 
        /// Para un broadcast, se debe poner la dirección de 64bits en 0x000000000000FFFE.
        /// El coordinador puede direccionarse como 0x00  y la direccion de 16bits como 0xFFFE.
        /// </summary>
        /// <param name="_opcion">Opciones soportadas para la transmisión</param>
        /// <param name="_64BitsDestinationAddress">Direccion de 64Bits para dispositivo destino. Si es broadcast, 0x000000000000FFFE </param>
        /// <param name="_16BitsDestinationAddress">Direccion de 16bits para dispositivo destino. Si es broadcast, 0xFFFE</param>
        /// <param name="_RFData">Datos que son enviados al dispositivo destino</param>
        /// <param name="_BroadcastRadius">Numero máximo de saltos una transmision de broadcast puede saltar. 
        /// Si es cero, es el maximo numero de saltos.</param>
        /// <param name="_frameID">Identificador del Frame</param>
        public ZigbeeTransmitRequestFrame(Options _opcion, 
            byte[] _64BitsDestinationAddress,
            byte[] _16BitsDestinationAddress,
            byte[] _RFData,
            byte _BroadcastRadius = 0x00, 
            byte _frameID = 0x01)
        {
            base.API_Identifier = TypeApiFramesNames.Zigbee_Transmit_Request;
            this.FrameID = _frameID;


            this.BroadcastRadius = _BroadcastRadius;
            this.Opcion = _opcion;
            this.DA64Bits = _64BitsDestinationAddress;
            this.DA16Bits = _16BitsDestinationAddress;

            this.RFData = _RFData;


            this.ArmandoElPaquete();

        }

        private void ArmandoElPaquete()
        {
            int TamañoEspecifico = 0x00;

            TamañoEspecifico += DA64Bits.Length;

            TamañoEspecifico += DA16Bits.Length;

            TamañoEspecifico += 1; //broadcast radius

            TamañoEspecifico += 1; //options

            TamañoEspecifico += RFData.Length;

            this.API_ID_Specific_Data = new byte[TamañoEspecifico];

            //llenando el direccionamiento 64 bits
            for (int i = 0; i < DA64Bits.Length; i++)
            {
                this.API_ID_Specific_Data[i] = DA64Bits[i];
            }
            //llenando direccionamiento 16 bits
            this.API_ID_Specific_Data[8] = DA16Bits[0];
            this.API_ID_Specific_Data[9] = DA16Bits[1];

            //broadcast radius
            this.API_ID_Specific_Data[10] = this.BroadcastRadius;
            //opcion
            this.API_ID_Specific_Data[11] = (byte)this.Opcion;

            //los datos
            for (int i = 0; i < RFData.Length; i++)
            {
                this.API_ID_Specific_Data[i + 12] = RFData[i];
            }
        }

        /// <summary>
        /// Opciones soportadas para la transmision.
        /// 
        /// </summary>
        public enum Options
        {
            None = 0x00,
            Enable_APS_Encription = 0x20,
            Use_Extended_Transmition_Timeout_For_Destination = 0x40
        }
    }
}
