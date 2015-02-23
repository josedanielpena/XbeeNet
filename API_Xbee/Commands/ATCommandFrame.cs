using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace API_Xbee.Commands
{
    public class ATCommandFrame : XbeeFrame
    {

        public string ATCommand = "ND";

        /// <summary>
        /// Usado para consultar o establecer parametros a un modulo local.
        /// Este comando aplica cambios despues de ser ejecutado.
        /// </summary>
        /// <param name="atCommand">Comando AT a Enviar</param>
        /// <param name="_frameID">Identificador del Frame</param>
        public ATCommandFrame(string atCommand, byte _frameID = 0x01)
        {
            this.ATCommand = atCommand;

            base.FrameID = _frameID;

            base.API_Identifier = TypeApiFramesNames.AT_Command;

            byte[] cmddata = new byte[ATCommand.Length];            

            for (int i = 0; i < ATCommand.Length; i++)
			{
                cmddata[i] = Convert.ToByte(ATCommand[i]);
			}

            

            base.API_ID_Specific_Data = cmddata;
        }



    }
}
