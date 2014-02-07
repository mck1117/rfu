using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFUInterface
{
    public enum RFUKey : int
    {

    }

    public class RFUInterface
    {
        private static byte[] ScreenUpdateBuffer = new byte[] { (byte)'*' };

        private IRFUDataInterface dataIface;

        private void RFUInterfaceInit()
        {
            dataIface.RFUDataReceived += dataIface_RFUDataReceived;
            Task.Run(() => ScreenUpdate());
        }

        public RFUInterface(string comPort)
        {
            dataIface = new RFUSerialDataInterface(comPort);  
            RFUInterfaceInit();
        }

        public RFUInterface(int udpSendPort, int udpRecvPort)
        {
            dataIface = new RFUUDPDataInterface(udpSendPort, udpRecvPort);
            RFUInterfaceInit();
        }

        void dataIface_RFUDataReceived(object sender, RFUDataReceivedEventArgs e)
        {

        }

        private void ScreenUpdate()
        {
            while(true)
            {
                dataIface.Send(ScreenUpdateBuffer, ScreenUpdateBuffer.Length);
                System.Threading.Thread.Sleep(500);
            }
        }

        public void SendKeyDown(RFUKey key)
        {
            dataIface.Send(new byte[] { (byte)(key) }, 1);
        }

        public void SendKeyUp(RFUKey key)
        {
            dataIface.Send(new byte[] { (byte)(key - 128) }, 1);        // Same as key down, except every key's up keycode is (keyDown - 128)
        }
    }
}
