using Acr.UserDialogs;
using eDroplet.Services;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace eDroplet.ViewModels
{
    public class CgmPageViewModel : ViewModelBase
    {
        public struct _bleDevice
        {
            public string Name { get; set; }
            public bool IsConnected { get; set; }
            public Guid Uuid { get; set; }
        }
        public _bleDevice _selectedItem;
        public _bleDevice SelectedItem
        {
            get { return _selectedItem; }
            set {
                _selectedItem = value;
                if (_selectedItem.Name == "") return;
                SelectDevice.Execute();
            }
        }

        public static IUserDialogs dialogScr;
        public static INavigationService _navigationService;


        public DelegateCommand NfcScanCmd { get; set; }
        public DelegateCommand BleScanCmd { get; set; }
        public DelegateCommand BleConnectCmd { get; set; }
        public DelegateCommand SelectDevice { get; set; }
        public DelegateCommand BlePingCmd { get; set; }
        public DelegateCommand BleReadCmd { get; set; }


        private bool _IsBleScanning;
        public bool IsBleScanning
        {
            get { return _IsBleScanning; }
            set { SetProperty(ref _IsBleScanning, value); }
        }
        private bool _IsBleConnected;
        public bool IsBleConnected
        {
            get { return _IsBleConnected; }
            set { SetProperty(ref _IsBleConnected, value); }
        }

        private string _btnNfcScanTxt;
        public string BtnNfcScanTxt
        {
            get { return _btnNfcScanTxt; }
            set { SetProperty(ref _btnNfcScanTxt, value); }
        }
        private string _btnBleScanTxt;
        public string BtnBleScanTxt
        {
            get { return _btnBleScanTxt; }
            set { SetProperty(ref _btnBleScanTxt, value); }
        }
        public ObservableCollection<_bleDevice> bleDevices { get; set; }
        public static _bleDevice currentDevice = new _bleDevice();

        private bool _IsNfcAvailable;
        public bool IsNfcAvailable
        {
            get { return _IsNfcAvailable; }
            set { SetProperty(ref _IsNfcAvailable, value); }
        }

        private bool _IsDataToStoreAvailable;
        public bool IsDataToStoreAvailable
        {
            get { return _IsDataToStoreAvailable; }
            set { SetProperty(ref _IsDataToStoreAvailable, value); }
        }

        private bool _IsBleDeviceSelected;
        public bool IsBleDeviceSelected
        {
            get { return _IsBleDeviceSelected; }
            set { SetProperty(ref _IsBleDeviceSelected, value); }
        }
        private string _btnBleConnectTxt;
        public string BtnBleConnectTxt
        {
            get { return _btnBleConnectTxt; }
            set { SetProperty(ref _btnBleConnectTxt, value); }
        }

        private bool _gridDataVisible;
        public bool gridDataVisible
        {
            get { return _gridDataVisible; }
            set { SetProperty(ref _gridDataVisible, value); }
        }

        private bool _gridBleScanVisible;
        public bool gridBleScanVisible
        {
            get { return _gridBleScanVisible; }
            set { SetProperty(ref _gridBleScanVisible, value); }
        }
        private string _lblDataSource;
        public string lblDataSource
        {
            get { return _lblDataSource; }
            set { SetProperty(ref _lblDataSource, value); }
        }
        public string currentDeviceName;
        public string currentDeviceUuid;

        public IBluetoothLE ble;
        public IAdapter bleAdapter;
        public ConnectParameters cancellationToken;
        public IDevice connectedDevice;
        public IService deviceInfoService;
        public IService batteryInfoService;
        public IService dataService;
        public ICharacteristic batteryVoltageCharacteristic;
        public ICharacteristic manufacturerCharacteristic;
        public ICharacteristic deviceModelCharacteristic;
        public ICharacteristic deviceSerialCharacteristic;
        public ICharacteristic deviceHWrevCharacteristic;
        public ICharacteristic deviceFWrevCharacteristic;
        public ICharacteristic deviceSWrevCharacteristic;
        public ICharacteristic dataReadCharacteristic;
        public ICharacteristic dataWriteCharacteristic;

        public CgmPageViewModel(INavigationService navigationService, IUserDialogs dialogs) : base(navigationService)
        {
            _navigationService = navigationService;
            dialogScr = dialogs;

            if (Device.RuntimePlatform == Device.Android) IsNfcAvailable = true;
            else IsNfcAvailable = false;

            NfcScanCmd = new DelegateCommand(ScanTag);
            BtnNfcScanTxt = "Scan NFC";

            BleScanCmd = new DelegateCommand(ScanBle);
            SelectDevice = new DelegateCommand(selectCurrentDevice);
            BleConnectCmd = new DelegateCommand(connectBleDevice);
            BlePingCmd = new DelegateCommand(blePing);
            BleReadCmd = new DelegateCommand(bleRead);

            bleDevices = new ObservableCollection<_bleDevice>();
            BtnBleScanTxt = "Scan BLE";
            BtnBleConnectTxt = "CONNECT";

            IsBleScanning = false;
            IsBleConnected = false;
            IsBleDeviceSelected = false;

            ble = CrossBluetoothLE.Current;
            bleAdapter = CrossBluetoothLE.Current.Adapter;
            ble.StateChanged += Ble_StateChanged;

        }

        private async void blePing()
        {
            if (IsBleConnected)
            {
                byte[] ping = new byte[] { 0x31, 0x32, 0x33 };
                await dataWriteCharacteristic.WriteAsync(ping);
            }
        }

        private async void bleRead()
        {
            if (IsBleConnected)
            {
                byte[] ping = new byte[] { 0x34, 0x35, 0x36 };
                await dataWriteCharacteristic.WriteAsync(ping);
            }
        }
        private void Ble_StateChanged(object sender, Plugin.BLE.Abstractions.EventArgs.BluetoothStateChangedArgs e)
        {
            dialogScr.Toast("BLE state changed: " + e.OldState.ToString() + " -> " + e.NewState.ToString());
        }

        private void ScanTag()
        {

        }
        private async void connectBleDevice()
        {

            if (IsBleConnected)
            {
                BtnBleConnectTxt = "CONNECT";                            
                IsBleConnected = false;
                lblDataSource = "";
                bleDevices.Clear();
                await bleAdapter.DisconnectDeviceAsync(connectedDevice);
            }
            else
            {
                BtnBleConnectTxt = "DISCONNECT";               
                try
                {
                    IsBleConnected = false;
                    connectedDevice = null;
                    gridDataVisible = false;
                    gridBleScanVisible = true;
                    connectedDevice = await bleAdapter.ConnectToKnownDeviceAsync(new Guid(currentDeviceUuid));
                    
                    dialogScr.Toast("Droplet state: " + connectedDevice.State.ToString());
                    try
                    {
                        batteryInfoService = await connectedDevice.GetServiceAsync(new Guid("0000180f-0000-1000-8000-00805f9b34fb"));
                        batteryVoltageCharacteristic = await batteryInfoService.GetCharacteristicAsync(new Guid("00002a19-0000-1000-8000-00805f9b34fb"));
                        batteryVoltageCharacteristic.ValueUpdated += BatteryVoltageCharacteristic_ValueUpdated;
                        await batteryVoltageCharacteristic.StartUpdatesAsync();

                    }
                    catch (Exception e)
                    {
                        dialogScr.Toast("Battery read error: " + e.Message);
                    }
                 
                    deviceInfoService = await connectedDevice.GetServiceAsync(Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"));
                    manufacturerCharacteristic = await deviceInfoService.GetCharacteristicAsync(Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb"));
                    deviceModelCharacteristic =  await deviceInfoService.GetCharacteristicAsync(Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"));
                    deviceSerialCharacteristic = await deviceInfoService.GetCharacteristicAsync(Guid.Parse("00002a25-0000-1000-8000-00805f9b34fb"));
                    deviceHWrevCharacteristic =  await deviceInfoService.GetCharacteristicAsync(Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb"));
                    deviceFWrevCharacteristic =  await deviceInfoService.GetCharacteristicAsync(Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"));
                    deviceSWrevCharacteristic =  await deviceInfoService.GetCharacteristicAsync(Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"));

                    dataService = await connectedDevice.GetServiceAsync(               Guid.Parse("c97433f0-be8f-4dc8-b6f0-5343e6100eb4"));
                    dataReadCharacteristic = await dataService.GetCharacteristicAsync( Guid.Parse("c97433f1-be8f-4dc8-b6f0-5343e6100eb4"));
                    dataWriteCharacteristic = await dataService.GetCharacteristicAsync(Guid.Parse("c97433f2-be8f-4dc8-b6f0-5343e6100eb4"));
                    dataReadCharacteristic.ValueUpdated += DataReadCharacteristic_ValueUpdated;
                    await dataReadCharacteristic.StartUpdatesAsync();

                    byte[] batteryVoltageBytes = await batteryVoltageCharacteristic.ReadAsync();
                    byte[] manufacturerBytes = await manufacturerCharacteristic.ReadAsync();
                    byte[] deviceModelBytes = await deviceModelCharacteristic.ReadAsync();
                    byte[] deviceSerialBytes = await deviceSerialCharacteristic.ReadAsync();
                    byte[] deviceHWrevBytes = await deviceHWrevCharacteristic.ReadAsync();
                    byte[] deviceFWrevBytes = await deviceFWrevCharacteristic.ReadAsync();
                    byte[] deviceSWrevBytes = await deviceSWrevCharacteristic.ReadAsync();

                    Debug.WriteLine("Battery : " + batteryVoltageBytes[0].ToString() + " %");
                    Debug.WriteLine("Manufacturer: " + System.Text.Encoding.Default.GetString(manufacturerBytes));
                    Debug.WriteLine("Device model: " + System.Text.Encoding.Default.GetString(deviceModelBytes));

                }
                catch (Exception e)
                {
                    dialogScr.Toast("BLE connection error: " + e.Message);
                    Debug.WriteLine("BLE connection error: " + e.Message);
                }
                IsBleConnected = true;
                gridBleScanVisible = false;
                gridDataVisible = true;
                lblDataSource = "BLE - Droplet";
            }
        }

        private void DataReadCharacteristic_ValueUpdated(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs e)
        {
            Debug.WriteLine("Data: " + General.ByteArrayToString(e.Characteristic.Value));
        }

        private void BatteryVoltageCharacteristic_ValueUpdated(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs e)
        {
            dialogScr.Toast("Battery voltage: " + e.Characteristic.Value[0].ToString() + " %");
        }

        private void selectCurrentDevice()
        {
            currentDeviceName = _selectedItem.Name;
            currentDeviceUuid = _selectedItem.Uuid.ToString();
            IsBleDeviceSelected = true;            
            BtnBleConnectTxt = "CONNECT TO: " + currentDeviceName;
        }
        private async void ScanBle()
        {
            gridDataVisible = false;
            gridBleScanVisible = true;
            if (IsBleScanning)
            {
                IsBleScanning = false;
                BtnBleScanTxt = "Scan BLE";
                await bleAdapter.StopScanningForDevicesAsync();
                bleAdapter.DeviceDiscovered += null; 

            }
            else if (!IsBleConnected)
            {
                bleDevices.Clear();
                IsBleScanning = true;
                BtnBleScanTxt = "STOP";
                bleAdapter.DeviceDiscovered += BleAdapter_DeviceDiscovered;
                await bleAdapter.StartScanningForDevicesAsync();              
            }
        }

        private void BleAdapter_DeviceDiscovered(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            if (e.Device.Name != null )
            {
                if (e.Device.Name.ToLower().Contains("droplet") && IsBleScanning)
                {
                    var devBle = new _bleDevice();
                    devBle.Name = e.Device.Name;
                    devBle.IsConnected = false;
                    devBle.Uuid = e.Device.Id;
                    bleDevices.Add(devBle);
                }
            }
            
        }
    }
}
