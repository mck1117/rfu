using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFUInterface
{
    public class RFUDataReceivedEventArgs : EventArgs
    {
        public byte[] Buffer { get; set; }
    }

    abstract class RFUDataInterface
    {
        public EventHandler<RFUDataReceivedEventArgs> RFUDataReceived;
        protected byte[] buffer = new byte[512];
        protected int bufferLength = 0;

        public void Send(byte[] buffer, int count)
        {
            Array.Copy(buffer, 0, this.buffer, bufferLength, count);
            bufferLength += count;
        }

        public virtual int Flush()
        {
            int retVal = this.bufferLength;
            this.bufferLength = 0;
            return retVal;
        }
    }

    class RFUUDPDataInterface : RFUDataInterface
    {
        private UDPBroadcast udp;
        public RFUUDPDataInterface(int sendPort, int recvPort)
        {
            udp = new UDPBroadcast(sendPort, recvPort);
            udp.BroadcastReceived += udp_BroadcastReceived;
            udp.StartListening();
        }

        void udp_BroadcastReceived(object sender, UDPBroadcast.UDPBroadcastReceivedEventArgs e)
        {
            var temp = this.RFUDataReceived;
            if (temp != null)
            {
                temp(this, new RFUDataReceivedEventArgs() { Buffer = e.Buffer });
            }
        }

        public override int Flush()
        {
            udp.Send(this.buffer, this.bufferLength);

            return base.Flush();
        }
    }

    class RFUSerialDataInterface : RFUDataInterface
    {
        //public event EventHandler<RFUDataReceivedEventArgs> RFUDataReceived;
        private SerialPort sp;
        public RFUSerialDataInterface(string comPort)
        {
            sp = new SerialPort(comPort, 9600, Parity.None, 8, StopBits.One);
            sp.DataReceived += sp_DataReceived;
            sp.Open();
        }

        public void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int count = sp.BytesToRead;
            byte[] buf = new byte[count];
            sp.Read(buf, 0, count);
            var temp = this.RFUDataReceived;
            if (temp != null)
            {
                temp(this, new RFUDataReceivedEventArgs() { Buffer = buf });
            }
        }

        public override int Flush()
        {
            sp.Write(this.buffer, 0, this.bufferLength);

            return base.Flush();
        }
    }
}
