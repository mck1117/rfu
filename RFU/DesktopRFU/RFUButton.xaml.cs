using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UDPRfu
{
    /// <summary>
    /// Interaction logic for RFUButton.xaml
    /// </summary>
    public partial class RFUButton : Button
    {
        public int KeyDownKeyCode
        {
            get { return (int)GetValue(KeyDownKeyCodeProperty); }
            set { SetValue(KeyDownKeyCodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeyDownKeyCode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyDownKeyCodeProperty =
            DependencyProperty.Register("KeyDownKeyCode", typeof(int), typeof(RFUButton), new PropertyMetadata(0));



        public RFUInterface.RFUInterface RFU
        {
            get { return (RFUInterface.RFUInterface)GetValue(RFUProperty); }
            set { SetValue(RFUProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RFU.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RFUProperty =
            DependencyProperty.Register("RFU", typeof(RFUInterface.RFUInterface), typeof(RFUButton), new PropertyMetadata(null));




        public RFUButton()
        {
            InitializeComponent();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (RFU != null)
            {
                RFU.SendKeyDown((RFUInterface.RFUKey)this.KeyDownKeyCode);
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (RFU != null)
            {
                RFU.SendKeyUp((RFUInterface.RFUKey)this.KeyDownKeyCode);
            }

            base.OnMouseUp(e);
        }
    }
}
