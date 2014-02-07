using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RFUInterface
{
    public class UDPBroadcast
    {
        private UdpClient send;
        private IPEndPoint sendEp;

        Socket recv;

        public UDPBroadcast(int sendPort, int receivePort) : this(AddressFamily.InterNetwork, sendPort, receivePort) { }
        public UDPBroadcast(AddressFamily addressFamily, int sendPort, int receivePort)
        {
            send = new UdpClient(addressFamily);
            sendEp = new IPEndPoint(IPAddress.Broadcast, sendPort);

            recv = new Socket(addressFamily, SocketType.Dgram, ProtocolType.Udp);
            recv.Bind(new IPEndPoint(IPAddress.Any, receivePort));
        }

        public void StartListening()
        {
            isListening = true;
            Task.Run(() => RecvLoop());
        }

        public int Send(byte[] dgram, int bytes)
        {
            return send.Send(dgram, bytes, sendEp);
        }

        public class UDPBroadcastReceivedEventArgs : EventArgs
        {
            public byte[] Buffer { get; set; }
        }

        public event EventHandler<UDPBroadcastReceivedEventArgs> BroadcastReceived;

        private bool isListening = true;
        private void RecvLoop()
        {
            while (isListening)
            {
                if (recv.Available != 0)
                {
                    byte[] buf = new byte[recv.Available];
                    recv.Receive(buf, buf.Length, SocketFlags.None);
                    var temp = this.BroadcastReceived;
                    if (temp != null)
                    {
                        temp(this, new UDPBroadcastReceivedEventArgs() { Buffer = buf });
                    }
                }
                else
                {

                }
            }
        }
    }
}
