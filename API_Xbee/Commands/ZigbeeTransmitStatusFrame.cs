using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace API_Xbee.Commands
{
    public class ZigbeeTransmitStatusFrame : XbeeFrame
    {
        public byte[] NA16bit = new byte[2];
        public byte TransmitRetryCount = 0x00;
        public TypeDeliveryStatus DeliveryStatus = TypeDeliveryStatus.Success;
        public TypeDiscoveryStatus DiscoveryStatus = TypeDiscoveryStatus.Address_And_Route_Discovery;

        public ZigbeeTransmitStatusFrame(byte[] _Na16Bit,
            byte _TransmitRetryCount,
            byte _TypeDeliveryStatus,
            byte _TypeDiscoveryStatus)
        {
            base.API_Identifier = TypeApiFramesNames.Zigbee_Transmit_Status;
            this.NA16bit = _Na16Bit;
            this.TransmitRetryCount = _TransmitRetryCount;

            this.DeliveryStatus = ByteTypeDeliveryStatus(_TypeDeliveryStatus);
            this.DiscoveryStatus = ByteTypeDiscoveryStatus(_TypeDiscoveryStatus);

            ArmandoElPaquete();
        }

        private void ArmandoElPaquete()
        {
            int TamañoEspecifico = 0x05;
            this.API_ID_Specific_Data = new byte[TamañoEspecifico];
            //this.API_ID_Specific_Data[0] = FrameID;
            this.API_ID_Specific_Data[0] = NA16bit[0];
            this.API_ID_Specific_Data[1] = NA16bit[1];

            this.API_ID_Specific_Data[2] = TransmitRetryCount;
            this.API_ID_Specific_Data[3] = (byte)DeliveryStatus;
            this.API_ID_Specific_Data[4] = (byte)DiscoveryStatus;
 
        }

        public enum TypeDeliveryStatus
        {
            Success = 0x00,
            CCA_Failure = 0x02,
            Invalid_Destination_Endpoint = 0x15,
            Network_ACK_Failure = 0x21,
            Not_Joined_To_Network = 0x22,
            Self_Addressed = 0x23,
            Address_Not_Found = 0x24,
            Route_Not_Found = 0x25,
            Data_Payload_Too_Large = 0x74
        }

        private TypeDeliveryStatus ByteTypeDeliveryStatus(byte _typedeliverystatus)
        {
            TypeDeliveryStatus otypedelivery;

            switch (_typedeliverystatus)
            {
                case (byte)TypeDeliveryStatus.Success:
                    otypedelivery = TypeDeliveryStatus.Success;
                    break;
                case (byte)TypeDeliveryStatus.CCA_Failure:
                    otypedelivery = TypeDeliveryStatus.CCA_Failure;
                    break;
                case (byte)TypeDeliveryStatus.Invalid_Destination_Endpoint:
                    otypedelivery = TypeDeliveryStatus.Invalid_Destination_Endpoint;
                    break;
                case (byte)TypeDeliveryStatus.Network_ACK_Failure:
                    otypedelivery = TypeDeliveryStatus.Network_ACK_Failure;
                    break;
                case (byte)TypeDeliveryStatus.Not_Joined_To_Network:
                    otypedelivery = TypeDeliveryStatus.Not_Joined_To_Network;
                    break;
                case (byte)TypeDeliveryStatus.Self_Addressed:
                    otypedelivery = TypeDeliveryStatus.Self_Addressed;
                    break;
                case (byte)TypeDeliveryStatus.Address_Not_Found:
                    otypedelivery = TypeDeliveryStatus.Address_Not_Found;
                    break;
                case (byte)TypeDeliveryStatus.Route_Not_Found:
                    otypedelivery = TypeDeliveryStatus.Route_Not_Found;
                    break;
                case (byte)TypeDeliveryStatus.Data_Payload_Too_Large:
                    otypedelivery = TypeDeliveryStatus.Data_Payload_Too_Large;
                    break;
                default:
                    otypedelivery = TypeDeliveryStatus.Success;
                    break;
            }
            return otypedelivery;
        }

        public enum TypeDiscoveryStatus
        {
            No_Discovery_Overhead = 0x00,
            Address_Discovery = 0x01,
            Route_Discovery = 0x02,
            Address_And_Route_Discovery = 0x03
        }

        private TypeDiscoveryStatus ByteTypeDiscoveryStatus(byte _typediscstatus)
        {
            TypeDiscoveryStatus otypedisc;

            switch (_typediscstatus)
            {
                case (byte)TypeDiscoveryStatus.No_Discovery_Overhead:
                    otypedisc = TypeDiscoveryStatus.No_Discovery_Overhead;
                    break;
                case (byte)TypeDiscoveryStatus.Address_Discovery:
                    otypedisc = TypeDiscoveryStatus.Address_Discovery;
                    break;
                case (byte)TypeDiscoveryStatus.Route_Discovery:
                    otypedisc = TypeDiscoveryStatus.Route_Discovery;
                    break;
                case (byte)TypeDiscoveryStatus.Address_And_Route_Discovery:
                    otypedisc = TypeDiscoveryStatus.Address_And_Route_Discovery;
                    break;
                default:
                    otypedisc = TypeDiscoveryStatus.No_Discovery_Overhead;
                    break;
            }

            return otypedisc;
        }

    }
}
