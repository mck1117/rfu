using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using RFUInterface;

namespace UDPSerialProxy
{
    class Program
    {
        static SerialPort sp;
        static UDPBroadcast udp;
        static void Main(string[] args)
        {
            string comPort = "COM3";
            int recvPort = 1112;
            int sendPort = 1113;

            udp = new UDPBroadcast(sendPort, recvPort);
            udp.BroadcastReceived += udp_BroadcastReceived;
            
            sp = new SerialPort(comPort, 9600, Parity.None, 8, StopBits.One);

            sp.DataReceived += sp_DataReceived;

            sp.Open();
            udp.StartListening();

            Thread.Sleep(-1);
        }

        static void udp_BroadcastReceived(object sender, UDPBroadcast.UDPBroadcastReceivedEventArgs e)
        {
            sp.Write(e.Buffer, 0, e.Buffer.Length);
        }

        static void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int avail = sp.BytesToRead;
            byte[] buf = new byte[avail];
            sp.Read(buf, 0, avail);
            udp.Send(buf, avail);
        }
    }
}
