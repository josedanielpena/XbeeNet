using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace API_Xbee.Commands
{
    public abstract class XbeeFrame
    {
        //public static byte AT_Command = 0x08;
        //public static byte AT_Command_Queue_Param_Value = 0x09;
        //public static byte Zigbee_Transmit_Request = 0x10;
        //public static byte Explicit_Addressing_Zigbee_Command_Frame = 0x11;
        //public static byte Remote_Command_Request = 0x17;
        //public static byte Create_Source_Route = 0x21;
        //public static byte AT_Command_Response = 0x88;
        //public static byte Modem_Status = 0x8A;
        //public static byte Zigbee_Transmit_Status = 0x8B;
        //public static byte Zigbee_Receive_Packter_AO_0 = 0x90;
        //public static byte Zigbee_Receive_Packter_AO_1 = 0x91;
        //public static byte Zigbee_IO_Data_Sample_RX_Indicator = 0x92;
        //public static byte XBee_Sensor_Read_Indicator_AO_0 = 0x94;
        //public static byte Node_Identificator_AO_0 = 0x95;
        //public static byte Remote_Command_Response = 0x97;
        //public static byte Over_The_Air_Firmware_Update_Status = 0xA0;
        //public static byte Route_Record_Indicator = 0xA1;
        //public static byte Many_To_One_Route_Request_Indicator = 0xA3;


        private byte startdelimiter = 0x7E;

        public byte StartDelimiter  
        {
            get { return startdelimiter; }
        }

        private byte lengthMSB = 0x00;

        public byte LengthMSB 
        {
            get {return getMSB(); 
                //return lengthMSB; 
            }
        }

        private byte lengthLSB = 0x00;

        public byte LengthLSB 
        {
            get 
            { 
                return getLSB(); 
            } 
        }

        protected TypeApiFramesNames API_Identifier { get; set; }

        public byte[] API_ID_Specific_Data { get; set; }

        public byte FrameID { get; set; }

        public byte CheckSum
        {
            get { return getChecksum(); }
        }

        private byte getChecksum()
        {
            byte checksum = 0x00;
            checksum += (byte)API_Identifier;
            checksum += FrameID;

            foreach (byte item in API_ID_Specific_Data) 
            {
                checksum += item;
            }

            byte ff_arestar = 0xFF;
            ff_arestar -= checksum;
            return ff_arestar;
        }

        private byte getLSB()
        {
            byte lsb = 0x00;

            lsb += (byte)(API_ID_Specific_Data.Length);

            lsb += 1; //frame id

            lsb += 1; //command id

            return lsb;
        }

        /// <summary>
        /// no esta listo
        /// </summary>
        /// <returns></returns>
        private byte getMSB()
        {
            int msb = 0x00;

            msb += (API_ID_Specific_Data.Length);

            msb += 1; //frame id

            msb += 1; //command id

            int salida = msb >> 8; //tengo mis dudas

            return Convert.ToByte(salida);
        }


        public static byte[] StringToBytes(string cadena)
        {
            System.Text.ASCIIEncoding codificador = new System.Text.ASCIIEncoding();
            return codificador.GetBytes(cadena);
        }





        public byte[] PaqueteToBytes()
        {
            int conteo = 0x00;

            conteo += 3; //START DELIMITER, LENGTH MSB Y LSB
            conteo += 2; //API FRAME NAME, API FRAME ID
            conteo += (API_ID_Specific_Data.Length);
            conteo += 1; //checksum

            byte[] paquete = new byte[conteo];
            paquete[0] = StartDelimiter;
            paquete[1] = this.LengthMSB;
            paquete[2] = this.LengthLSB;
            paquete[3] = (byte)this.API_Identifier;
            paquete[4] = this.FrameID;

            for (int i = 0; i < API_ID_Specific_Data.Length; i++)
            {
                paquete[i+5] = API_ID_Specific_Data[i];
            }
            paquete[conteo - 1] = this.CheckSum;
            return paquete;          

        }
    }

    /// <summary>
    /// API Frame: Nombre y Valores:
    /// Identifican que mensajes de la API tendrá el cmdData del paquete.
    /// </summary>
    public enum TypeApiFramesNames
    {
        AT_Command = 0x08,
        AT_Command_Queue_Param_Value = 0x09,
        Zigbee_Transmit_Request = 0x10,
        Explicit_Addressing_Zigbee_Command_Frame = 0x11,
        Remote_Command_Request = 0x17,
        Create_Source_Route = 0x21,
        AT_Command_Response = 0x88,
        Modem_Status = 0x8A,
        Zigbee_Transmit_Status = 0x8B,
        Zigbee_Receive_Packter_AO_0 = 0x90,
        Zigbee_Receive_Packter_AO_1 = 0x91,
        Zigbee_IO_Data_Sample_RX_Indicator = 0x92,
        XBee_Sensor_Read_Indicator_AO_0 = 0x94,
        Node_Identificator_AO_0 = 0x95,
        Remote_Command_Response = 0x97,
        Over_The_Air_Firmware_Update_Status = 0xA0,
        Route_Record_Indicator = 0xA1,
        Many_To_One_Route_Request_Indicator = 0xA3
    }


}
