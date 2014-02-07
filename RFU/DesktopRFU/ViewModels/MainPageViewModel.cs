using System;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;

// Toolkit namespace
using SimpleMvvmToolkit;

// Toolkit extension methods
using SimpleMvvmToolkit.ModelExtensions;

namespace DesktopRFU
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class MainPageViewModel : ViewModelBase<MainPageViewModel>
    {
        #region Initialization and Cleanup

        // Default ctor
        public MainPageViewModel()
        {
            RFU = new RFUInterface.RFUInterface(1112, 1113);
            RFU.ScreenUpdated += RFU_ScreenUpdated;
            RFU.SetChannel(58, 24);
        }

        void RFU_ScreenUpdated(object sender, RFUInterface.RFUScreenUpdateEventArgs e)
        {

        }

        #endregion

        #region Notifications

        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;

        #endregion

        #region Properties

        private RFUInterface.RFUInterface _RFU;
        public RFUInterface.RFUInterface RFU
        {
            get { return _RFU; }
            set
            {
                _RFU = value;
                NotifyPropertyChanged(m => m.RFU);
            }
        }

        private string _ledHex;
        public string LEDHex
        {
            get { return _ledHex; }
            set
            {
                _ledHex = value;
                NotifyPropertyChanged(m => m.LEDHex);
            }
        }

        #endregion

        #region Methods

        // TODO: Add methods that will be called by the view

        #endregion

        #region Completion Callbacks

        // TODO: Optionally add callback methods for async calls to the service agent

        #endregion

        #region Helpers

        // Helper method to notify View of an error
        private void NotifyError(string message, Exception error)
        {
            // Notify view of an error
            Notify(ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        #endregion
    }
}