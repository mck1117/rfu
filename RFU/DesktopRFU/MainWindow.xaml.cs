using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopRFU
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public RFUInterface.RFUInterface RFU
        {
            get { return (RFUInterface.RFUInterface)GetValue(RFUProperty); }
            set { SetValue(RFUProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RFU.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RFUProperty =
            DependencyProperty.Register("RFU", typeof(RFUInterface.RFUInterface), typeof(MainWindow), new PropertyMetadata(null));

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
