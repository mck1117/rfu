using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFUInterface
{
    public class RFUScreenUpdateEventArgs : EventArgs
    {

    }


    public struct RFUScreen
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
    }

    public struct RFULeds
    {
        private static bool BitValue(byte b, int bit)
        {
            return (b & (0x1 << bit)) != 0;
        }
        public static RFULeds DecodeLEDs(byte[] buf, int offset)
        {
            byte b1 = buf[offset + 0];
            byte b2 = buf[offset + 1];
            byte b3 = buf[offset + 2];
            byte b4 = buf[offset + 3];

            RFULeds l = new RFULeds();
            l.M1 = BitValue(b1, 0);
            l.ABClear = BitValue(b1, 1);
            l.ABBack = BitValue(b1, 2);
            l.ABHold = BitValue(b1, 3);
            l.ABGo = BitValue(b1, 4);

            l.Stage = BitValue(b1, 5);
            l.MStar = BitValue(b1, 6);
            l.Sub = BitValue(b1, 7);
            l.Time = BitValue(b2, 0);
            l.Track = BitValue(b2, 1);

            l.Blind = BitValue(b2, 2);
            l.SStar = BitValue(b2, 3);
            l.Group = BitValue(b2, 4);
            l.Cue = BitValue(b2, 5);
            l.Rec = BitValue(b2, 6);

            l.Chan = BitValue(b2, 0);
            l.Thru = BitValue(b2, 0);
            l.And = BitValue(b2, 0);
            l.Except = BitValue(b2, 0);
            l.Rel = BitValue(b2, 0);

            l.Dim = BitValue(b2, 0);
            l.At = BitValue(b2, 0);
            l.Full = BitValue(b2, 0);
            l.Level = BitValue(b2, 0);
            l.FocusPoint = BitValue(b2, 0);

            return l;
        }

        public bool M1 { get; set; }
        public bool ABClear { get; set; }
        public bool ABBack { get; set; }
        public bool ABHold { get; set; }
        public bool ABGo { get; set; }
        public bool Stage { get; set; }
        public bool MStar { get; set; }
        public bool Sub { get; set; }
        public bool Time { get; set; }
        public bool Track { get; set; }
        public bool Blind { get; set; }
        public bool SStar { get; set; }
        public bool Group { get; set; }
        public bool Cue { get; set; }
        public bool Rec { get; set; }
        public bool Chan { get; set; }
        public bool Thru { get; set; }
        public bool And { get; set; }
        public bool Except { get; set; }
        public bool Rel { get; set; }
        public bool Dim { get; set; }
        public bool At { get; set; }
        public bool Full { get; set; }
        public bool Level { get; set; }
        public bool FocusPoint { get; set; }
    }

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

        private RFUDataInterface dataIface;

        public RFUScreen ScreenState { get; set; }
        public RFULeds LEDState { get; set; }

        public event EventHandler<RFUScreenUpdateEventArgs> ScreenUpdated;

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
            byte[] buf = e.Buffer;

            //this.LEDState = RFULeds.DecodeLEDs(buf, 0);
        }

        private void ScreenUpdate()
        {
            while (true)
            {
                dataIface.Send(ScreenUpdateBuffer, ScreenUpdateBuffer.Length);
                dataIface.Flush();
                System.Threading.Thread.Sleep(500);
            }
        }

        public void SendKeyDown(RFUKey key)
        {
            SendKeyDownNoFlush(key);
            dataIface.Flush();
        }

        private void SendKeyDownNoFlush(RFUKey key)
        {
            dataIface.Send(new byte[] { (byte)(key) }, 1);
        }
        public void SendKeyUp(RFUKey key)
        {
            SendKeyUpNoFlush(key);
            dataIface.Flush();
        }
        private void SendKeyUpNoFlush(RFUKey key)
        {
            dataIface.Send(new byte[] { (byte)(key - 128) }, 1);        // Same as key down, except every key's up keycode is (keyDown - 128)
        }
        public void PressKey(RFUKey key)
        {
            SendKeyDownNoFlush(key);
            SendKeyUpNoFlush(key);
            dataIface.Flush();
        }
        private void PressKeyNoFlush(RFUKey key)
        {
            SendKeyDownNoFlush(key);
            SendKeyUpNoFlush(key);
        }

        private void TypeNumber(int number)
        {
            TypeNumberNoFlush(number);
            dataIface.Flush();
        }
        private void TypeNumberNoFlush(int number)
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
                PressKeyNoFlush(NumToKey[n]);
            }
        }

        public void SetChannel(int channel, int value)
        {
            PressKeyNoFlush(RFUKey.Chan);

            TypeNumberNoFlush(channel);

            PressKeyNoFlush(RFUKey.At);

            TypeNumberNoFlush(value);

            PressKeyNoFlush(RFUKey.Enter);
            dataIface.Flush();
        }

        public void SetDimmer(int dimmer, int value)
        {
            PressKeyNoFlush(RFUKey.Dim);

            TypeNumberNoFlush(dimmer);

            PressKeyNoFlush(RFUKey.At);

            TypeNumberNoFlush(value);

            PressKeyNoFlush(RFUKey.Enter);
            dataIface.Flush();
        }
    }
}
