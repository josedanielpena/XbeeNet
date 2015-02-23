using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinPruebaXbee
{
    public partial class Form1 : Form
    {
        string loquellevo = "";

        delegate void FuncionDelegado();

        public Form1()
        {
            InitializeComponent();

            //serialPort1.BaudRate = 9600;
            //serialPort1.PortName = "COM6";
            ////serialPort1.PortName = "COM4";
            //serialPort1.DataBits = 8;
            //serialPort1.Parity = System.IO.Ports.Parity.None;
            //serialPort1.StopBits = System.IO.Ports.StopBits.One;

            //CommUART.SerialComm oser = CommUART.SerialComm.Instancia;
        }

        private byte[] EstablecerDEstino64Bits(string entrada)
        {
            byte[] DA64Bits = new byte[8];

            string[] salida = entrada.Trim().Split(' ');

            for (int i = 0; i < DA64Bits.Length; i++)
            {
                int iNumber = int.Parse(salida[i], System.Globalization.NumberStyles.HexNumber);
                DA64Bits[i] = (byte)iNumber;
            }

            return DA64Bits;
        }

        private byte[] EstablecerDestino16Bits(string entrada)
        {
            byte[] DA16Bits = new byte[2];
            DA16Bits[0] = (byte)(int.Parse(entrada.Split(' ')[0], System.Globalization.NumberStyles.HexNumber));
            DA16Bits[1] = (byte)(int.Parse(entrada.Split(' ')[1], System.Globalization.NumberStyles.HexNumber));

            return DA16Bits;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            API_Xbee.Commands.XbeeFrame oframe = null;

            switch (comboBoxTipo.SelectedItem.ToString())
            {
                //COMANDO AT
                case "AT":
                    oframe = new API_Xbee.Commands.ATCommandFrame(textBoxComandoAT.Text.Trim());
                    break;
                //ENVIAR DATO A DESTINO
                case "TRANSMIT":
                    oframe = new API_Xbee.Commands.ZigbeeTransmitRequestFrame(API_Xbee.Commands.ZigbeeTransmitRequestFrame.Options.None,
                                EstablecerDEstino64Bits(textBoxDA64Bits.Text), //new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFE }, //new byte[] { 0x00, 0x13, 0xA2, 0x00, 0x40, 0x33, 0x7D, 0x60 },
                                EstablecerDestino16Bits(textBoxDA16Bits.Text), //new byte[] { 0xFF, 0xFE }, //new byte[] { 0x74, 0x1E},
                                API_Xbee.Commands.XbeeFrame.StringToBytes(textBoxComandoAT.Text.Trim()));
                    break;
                default:
                    oframe = new API_Xbee.Commands.ATCommandFrame(textBoxComandoAT.Text.Trim());
                    break;
            }

            

            //COMANDO AT

            //API_Xbee.Commands.ATCommandFrame oframe = new API_Xbee.Commands.ATCommandFrame(textBoxComandoAT.Text.Trim());
            
            //ENVIANDO PAQUETE
            //API_Xbee.Commands.ZigbeeTransmitRequestFrame oframe = new API_Xbee.Commands.ZigbeeTransmitRequestFrame(API_Xbee.Commands.ZigbeeTransmitRequestFrame.Options.None,
            //    new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
            //    new byte[] { 0xFF, 0xFE},
            //    API_Xbee.Commands.XbeeFrame.StringToBytes("Hola"));


            //API_Xbee.Commands.ZigbeeTransmitRequestFrame oframe = new API_Xbee.Commands.ZigbeeTransmitRequestFrame(API_Xbee.Commands.ZigbeeTransmitRequestFrame.Options.None,
            //    new byte[] { 0x00, 0x13, 0xA2, 0x00, 0x40, 0x33, 0x7D, 0xA0 },
            //    new byte[] { 0x04, 0x1E },
            //    0x00,
            //    //new byte[] { 0x54, 0x78, 0x44, 0x61, 0x74, 0x61, 0x30, 0x41 },
            //   CommUART.SerialComm.StrToByteArray(textBoxComandoAT.Text),
            //    0x01);

            //hola = oframe.StartDelimiter;
            //oframe.FrameID = 0x01;
            //oframe.LengthMSB = 0x00;

            
            //oframe.LengthLSB = 0x04;
            //oframe.API_Identifier = API_Xbee.ApiFramesNames.AT_Command;
            //oframe.API_ID_Specific_Data = new byte[] { 0x4E, 0x44 };

            //hola.ToString("X2")
            byte elchecksum = oframe.CheckSum;

            byte[] elpaquete = oframe.PaqueteToBytes();
            
            //serialPort1.Write(elpaquete, 0, elpaquete.Length);
            //byte[] respuesta;
            //string salidita = serialPort1.ReadExisting();


            CommUART.SerialComm oser = CommUART.SerialComm.Instancia;

            //oser.PuertoSerie.PortName = textBoxPuertoSerie.Text;

            oser.SerialEvent += new CommUART.SerialComm.SerialEventHandler(oser_SerialEvent);



            oser.EnviarDatoxPuerto(elpaquete);


            string elpaquetico = elchecksum.ToString("X2"); //"X2" here is the hexadecimal format specifier.
            elpaquetico += "-";

            label1.Text = elpaquetico;

           //string respuesta = EnviarComando(textBoxComandoAT.Text);
            //label1.Text = (salidita);

            
        }

        void oser_SerialEvent(object sender, CommUART.UARTEventArgs e)
        {
            this.Invoke(new FuncionDelegado(delegate()
            {

                byte[] mensajito = e.Mensaje;

                if (e.Mensaje[3] == (byte)API_Xbee.Commands.TypeApiFramesNames.Zigbee_Transmit_Status)
                {
                    byte[] na16 = { e.Mensaje[5], e.Mensaje[6] };
                    byte transmitretricount = e.Mensaje[7];
                    byte deliverystatus = e.Mensaje[8];
                    byte discoverystatus = e.Mensaje[9];
                    byte frameid = e.Mensaje[4];

                    API_Xbee.Commands.ZigbeeTransmitStatusFrame oframe = new API_Xbee.Commands.ZigbeeTransmitStatusFrame(na16,
                        transmitretricount,
                        deliverystatus,
                        discoverystatus);

                    oframe.FrameID = frameid;
                      

                    byte[] jojo = oframe.PaqueteToBytes();

                    if (jojo == mensajito)
                    {
                        Console.WriteLine("conversion buena");
                    }
                    else
                    {
                        Console.WriteLine("conversion mala");
                    }

                    string cadena = CommUART.SerialComm.ByteArrayToString(mensajito);
                    textBox1.AppendText("recibi dato! " + CommUART.SerialComm.ByteArrayToString(mensajito));

                    textBox1.AppendText(mensajito[1].ToString("X2"));

                    textBox1.AppendText(cadena);
                }

                //AT COMMAND RESPONSE.........................:
                if (e.Mensaje[3] == (byte)API_Xbee.Commands.TypeApiFramesNames.AT_Command_Response )
                {

                    API_Xbee.Commands.ATCommandResponse orespuesta = new API_Xbee.Commands.ATCommandResponse(e.Mensaje);

                    textBox1.AppendText("Recibi Dato");
                    textBox1.AppendText("Comando AT: " + orespuesta.ATCommand);

                    textBox1.AppendText("Estado Comando " + orespuesta.ATCommandStatus.ToString());

                    textBox1.AppendText("Respuesta: " + orespuesta.CadenaRespuesta + "\r\n");
                }
                //ZIGBEE RECEIVE PACKAGE.....
                if (e.Mensaje[3] == (byte)API_Xbee.Commands.TypeApiFramesNames.Zigbee_Receive_Packter_AO_0)
                {
                    API_Xbee.Commands.ZigbeeReceivePacket orespuesta = new API_Xbee.Commands.ZigbeeReceivePacket(e.Mensaje);

                    textBox1.AppendText("Recibi Dato");
                    textBox1.AppendText("Paquete Recibido:\r\n");
                    textBox1.AppendText("Respuesta: " + orespuesta.CadenaRecibida + "\r\n");
                }
                



            }));

            //throw new NotImplementedException();
        }

        //private string EnviarComando(string cadenita)
        //{
        //    serialPort1.Open();

        //    serialPort1.Write("+++");

        //    string respuesta = serialPort1.ReadExisting();

        //    while (respuesta.Trim() == "")
        //    {
        //        respuesta = serialPort1.ReadExisting();
        //    }

        //    string salida = "";

        //    if (respuesta == "OK\r")
        //    {
        //       salida =  EnviarComandoAT(cadenita);
        //    }

        //    EnviarComandoAT("ATCN");


        //    serialPort1.Close();

        //    return salida;
            
        //}

        //private string EnviarComandoAT(string cadena)
        //{
        //    serialPort1.Write(cadena + "\r");

        //    string respuesta = serialPort1.ReadExisting();

        //    while (respuesta.Trim() == "")
        //    {
        //        respuesta = serialPort1.ReadExisting();
        //    }

            

        //    return respuesta;
        //}

        //private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        //{
        //    loquellevo = serialPort1.ReadExisting();

        //    this.Invoke(new FuncionDelegado(delegate()
        //    {

        //        textBox1.AppendText(loquellevo);
        //    }));
             
        //}

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }
    }
}
