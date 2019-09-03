using Acr.UserDialogs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace eDroplet.ViewModels
{
    public class CgmPageViewModel : ViewModelBase
    {
        public static IUserDialogs dialogScr;
        public static INavigationService _navigationService;
        public DelegateCommand NfcScanCmd { get; set; }
        public DelegateCommand BleScanCmd { get; set; }

        public CgmPageViewModel(INavigationService navigationService, IUserDialogs dialogs) : base(navigationService)
        {
            _navigationService = navigationService;
            dialogScr = dialogs;
            NfcScanCmd = new DelegateCommand(ScanTag);
            BleScanCmd = new DelegateCommand(ScanBle);
        }

        private void ScanBle()
        {
            
        }

        private void ScanTag()
        {
            if (Device.RuntimePlatform == Device.iOS) dialogScr.Alert("NFC not available on iOS devices."); 
        }
    }
}
