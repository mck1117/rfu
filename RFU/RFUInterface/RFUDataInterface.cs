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

    interface IRFUDataInterface
    {
        event EventHandler<RFUDataReceivedEventArgs> RFUDataReceived;
        int Send(byte[] buffer, int count);
    }

    class RFUUDPDataInterface : IRFUDataInterface
    {
        public event EventHandler<RFUDataReceivedEventArgs> RFUDataReceived;
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

        public int Send(byte[] buffer, int count)
        {
            return udp.Send(buffer, count);
        }
    }

    class RFUSerialDataInterface : IRFUDataInterface
    {
        public event EventHandler<RFUDataReceivedEventArgs> RFUDataReceived;
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

        public int Send(byte[] buffer, int count)
        {
            sp.Write(buffer, 0, count);
            return count;
        }
    }
}
