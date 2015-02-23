using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace API_Xbee.Commands
{
    public class ATCommandResponse : XbeeFrame
    {

        public string ATCommand;

        public bool ATCommandStatus = false;

        public string CadenaRespuesta;


        /// <summary>
        /// Respuesta a un comando AT
        /// </summary>
        /// <param name="arraydebytes">Arreglo que llega desde el puerto serial</param>
        public ATCommandResponse(byte[] arraydebytes)
        {
            //El arreglo llega con varios elementos de mas
            // arraydebytes[0] --> es el caracter delimitador
            // arraydebytes[1] --> es el caracter MSB
            // arraydebytes[2] --> es el caracter LSB
            // arraydebytes[3] --> es el caracter Frame Type --> en este caso es igual a 0x88 para respuestas a Comandos AT
            // arraydebytes[4] --> es el caracter Frame ID
            // arraydebytes[arraydebytes.Length -1] --> es el ultimo caracter: es el Checksum.

            //this.API_ID_Specific_Data --> se debe llenar con los datos específicos del paquete en cuestion, excluyendo lo de arriba.
            setFrameSpecificData(arraydebytes);

            // arraydebytes[4] --> es el caracter Frame ID
            this.FrameID = arraydebytes[4];

            // arraydebytes[3] --> es el caracter Frame Type --> en este caso es igual a 0x88 para respuestas a Comandos AT
            if (arraydebytes[3] == (byte)API_Xbee.Commands.TypeApiFramesNames.AT_Command_Response)
            {
                this.API_Identifier = API_Xbee.Commands.TypeApiFramesNames.AT_Command_Response;

                System.Text.ASCIIEncoding codificador = new System.Text.ASCIIEncoding();


                byte[] comandoatrespuesta = new byte[2];
                comandoatrespuesta[0] = this.API_ID_Specific_Data[0];
                comandoatrespuesta[1] = this.API_ID_Specific_Data[1];

                ATCommand = codificador.GetString(comandoatrespuesta);

                ATCommandStatus = this.API_ID_Specific_Data[2] == 0x00 ? true : false;


                byte[] nuevacadenasalida = new byte[this.API_ID_Specific_Data.Length - 3];

                for (int i = 0; i < nuevacadenasalida.Length; i++)
                {
                    nuevacadenasalida[i] = this.API_ID_Specific_Data[i + 3];
                }

                CadenaRespuesta = codificador.GetString(nuevacadenasalida);

                if (arraydebytes[1] != this.LengthMSB) throw new Exception("MSB Diferente");
                if (arraydebytes[2] != this.LengthLSB) throw new Exception("LSB Diferente");
                if (arraydebytes[arraydebytes.Length -1 ] != this.CheckSum) throw new Exception("Checksum Diferente");
            }
            else
            {
                throw new Exception("No es Respuesta a Comando AT");
            }

        }


        //this.API_ID_Specific_Data --> se debe llenar con los datos específicos del paquete en cuestion, excluyendo lo de arriba.
        private void setFrameSpecificData(byte[] arraydebytes)
        {
            this.API_ID_Specific_Data = new byte[arraydebytes.Length - 6];

            for (int i = 5; i < arraydebytes.Length - 1; i++)
            {
                this.API_ID_Specific_Data[i - 5] = arraydebytes[i];
            }
        }
    }
}
