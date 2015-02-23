using System;
using System.IO;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommUART
{
    public class SerialComm
    {

        // Declare the delegate (if using non-generic pattern).
        public delegate void SerialEventHandler(object sender, UARTEventArgs e);

        // Declare the event.
        public event SerialEventHandler SerialEvent;

        public int Velocidad { get; set; }
        public List<string> Puertos { get; set; }
        public SerialPort PuertoSerie { get; set; }
        public string NombrePuerto { get; set; }
        public StopBits BitsDeParada { get; set; }
        public int DataBits { get; set; }

        public Parity Paridad { get; set; }


        private static volatile SerialComm instancia;
        private static object syncRoot = new Object();

        protected virtual void RaiseSampleEvent(byte[] entrada)
        {
            byte[] salida = entrada;
            SerialEvent(this, new UARTEventArgs(salida));
        }


        private SerialComm()
        {
            this.NombrePuerto = "COM6";
            this.Paridad = Parity.None;
            this.Velocidad = 9600;
            this.DataBits = 8;
            this.BitsDeParada = StopBits.One;
            
            PuertoSerie = new SerialPort(NombrePuerto, Velocidad, Paridad, DataBits, BitsDeParada);

            PuertoSerie.DataReceived += new SerialDataReceivedEventHandler(recibo_datosxPuerto);

        }


        public static SerialComm Instancia
        {
            get
            {
                if (instancia == null)
                {
                    lock (syncRoot)
                    {
                        if (instancia == null)
                            instancia = new SerialComm();
                    }
                }

                return instancia;
            }
        }

        public void EnviarDatoxPuerto(byte[] datos)
        {
            if (!PuertoSerie.IsOpen)
            {
                PuertoSerie.Open();
            }
            PuertoSerie.Write(datos, 0, datos.Length);
        }

        /// <summary>
        /// el Recibo Datos x puerto es asi porque el buffer es variable. La peticion de bytes debe ser sincronica
        /// para saber la longitud del paquete.
        /// Se utiliza ReadByte() del puerto serial para que los entregue de forma sincronica y ordenada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recibo_datosxPuerto(object sender, SerialDataReceivedEventArgs e)
        {
            
            byte[] cabecerarecibido = new byte[3];

            for (int i = 0; i < cabecerarecibido.Length; i++)
                cabecerarecibido[i] = (byte)(this.PuertoSerie.ReadByte());

            int tamañomensaje = cabecerarecibido[2] + cabecerarecibido[1]*256 + 1;

            byte[] mensaje = new byte[tamañomensaje];

            for (int j = 0; j < tamañomensaje; j++)
                mensaje[j] = (byte)this.PuertoSerie.ReadByte();

            byte[] mensajecompleto = new byte[cabecerarecibido.Length + tamañomensaje];

            for (int i = 0; i < cabecerarecibido.Length; i++)
                mensajecompleto[i] = cabecerarecibido[i];

            for (int i = 0; i < tamañomensaje; i++)
                mensajecompleto[i + cabecerarecibido.Length] = mensaje[i];

            RaiseSampleEvent(mensajecompleto);
        }

        public static byte[] StrToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

            byte[] salida = encoding.GetBytes(str);
            return salida;
        }

        public static string ByteArrayToString(byte[] data)
        {
            string str;
            //System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            str = Encoding.ASCII.GetString(data);
            return str;
        }


    }



    public class UARTEventArgs : EventArgs
    {
        public UARTEventArgs(byte[] s)
        {
            msg = s;
        }
        private byte[] msg;
        public byte[] Mensaje
        {
            get { return msg; }
        }
    }
}
