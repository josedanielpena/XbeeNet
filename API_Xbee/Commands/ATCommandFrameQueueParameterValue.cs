using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace API_Xbee.Commands
{
    public class ATCommandFrameQueueParameterValue : XbeeFrame
    {
        public string ATCommand = "ND";

        public ATCommandFrameQueueParameterValue(string atCommand, byte _frameID)
        {
            this.ATCommand = atCommand;

            base.API_Identifier = TypeApiFramesNames.AT_Command_Queue_Param_Value;

            byte[] cmddata = new byte[ATCommand.Length];            

            for (int i = 0; i < ATCommand.Length; i++)
			{
                cmddata[i] = Convert.ToByte(ATCommand[i]);
			}

            base.FrameID = _frameID;

            base.API_ID_Specific_Data = cmddata;
        }
    }
}
