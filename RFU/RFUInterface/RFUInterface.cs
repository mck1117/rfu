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
        M1 = 129,
        AB_Clear = 130,
        AB_Back = 131,
        AB_Hold = 132,
        AB_Go = 133,
        Stage = 134,
        MStar = 135,
        Sub = 136,
        Time = 137,
        Track = 138,
        Blind = 139,
        SStar = 140,
        Group = 141,
        Cue = 142,
        Record = 143,
        Num1 = 146,
        Num2 = 151,
        Num3 = 156,
        Num4 = 145,
        Num5 = 150,
        Num6 = 155,
        Num7 = 144,
        Num8 = 149,
        Num9 = 154,
        Num0 = 152,
        Decimal = 153,
        Plus = 157,
        Minus = 147,
        Clear = 148,
        Enter = 158,
        Chan = 159,
        Thru = 160,
        And = 161,
        Except=162,
        Relative = 163,
        Dim = 164,
        At = 165,
        Full = 166,
        Level = 167,
        FocalPoint = 168
    }

    public class RFUInterface
    {
        private static byte[] ScreenUpdateBuffer = new byte[] { (byte)'*' };
        private static RFUKey[] NumToKey = new RFUKey[] { RFUKey.Num0, RFUKey.Num1, RFUKey.Num2, RFUKey.Num3, RFUKey.Num4, RFUKey.Num5, RFUKey.Num6, RFUKey.Num7, RFUKey.Num8, RFUKey.Num9 };

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
            while (true)
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

        public void PressKey(RFUKey key)
        {
            SendKeyDown(key);
            SendKeyUp(key);
        }

        private void TypeNumber(int number)
        {
            List<int> nums = new List<int>();

            while(number > 0)
            {
                int n = number % 10;
                number = number / 10;
                nums.Add(n);
            }

            foreach(var n in nums.Reverse<int>())
            {
                PressKey(NumToKey[n]);
            }
        }

        public void SetChannel(int channel, int value)
        {
            PressKey(RFUKey.Chan);

            TypeNumber(channel);

            PressKey(RFUKey.At);

            TypeNumber(value);

            PressKey(RFUKey.Enter);
        }

        public void SetDimmer(int dimmer, int value)
        {
            PressKey(RFUKey.Dim);

            TypeNumber(dimmer);

            PressKey(RFUKey.At);

            TypeNumber(value);

            PressKey(RFUKey.Enter);
        }
    }
}
