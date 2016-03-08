﻿using System;
using System.Collections.ObjectModel;
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

using System.Threading;
using System.Net;
using System.Data;
using System.Xml;

using TH_Configuration;
using TH_Global;
using TH_Global.Functions;
using TH_MTConnect.Components;
using TH_UserManagement.Management;

namespace TH_DeviceManager.AddDevice.Pages
{
    /// <summary>
    /// Interaction logic for AutoDetect.xaml
    /// </summary>
    public partial class AutoDetect : UserControl, IPage
    {
        public AutoDetect()
        {
            InitializeComponent();
            DataContext = this;

            LoadCatalog();
        }

        #region "Properties"

        public string PageName { get { return "Auto Detect"; } }

        public ImageSource Image { get { return new BitmapImage(new Uri("pack://application:,,,/TH_DeviceManager;component/Resources/options_gear_30px.png")); } }

        public AddDevice.Page ParentPage { get; set; }

        public bool DevicesLoading
        {
            get { return (bool)GetValue(DevicesLoadingProperty); }
            set { SetValue(DevicesLoadingProperty, value); }
        }

        public static readonly DependencyProperty DevicesLoadingProperty =
            DependencyProperty.Register("DevicesLoading", typeof(bool), typeof(AutoDetect), new PropertyMetadata(true));

        #endregion

        const System.Windows.Threading.DispatcherPriority PRIORITY_BACKGROUND = System.Windows.Threading.DispatcherPriority.Background;

        # region "Add Device"

        class AddShared_Return
        {
            public DeviceInfo DeviceInfo { get; set; }
            public Configuration Configuration { get; set; }
            public bool Success { get; set; }
        }

        void AddSharedItem(DeviceInfo info)
        {
            if (ParentPage != null && ParentPage.ParentManager != null)
            {
                info.Loading = true;
                Devices_DG.Items.Refresh();

                ThreadPool.QueueUserWorkItem(new WaitCallback(AddSharedItem_Worker), info);
            }
        }

        void AddSharedItem_Worker(object o)
        {
            AddShared_Return result = new AddShared_Return();
            result.Success = false;

            if (o != null)
            {
                var info = (DeviceInfo)o;

                result.DeviceInfo = info;

                if (info.SharedListItem != null)
                {
                    string tablename = info.SharedListItem.tablename;

                    if (tablename != null)
                    {
                        Shared.UpdateDownloads(info.SharedListItem);

                        DataTable dt = TH_UserManagement.Management.Remote.Configurations.GetTable(tablename);
                        if (dt != null)
                        {
                            XmlDocument xml = Converter.TableToXML(dt);
                            if (xml != null)
                            {
                                Configuration config = Configuration.Read(xml);
                                if (config != null)
                                {
                                    UpdateEnabled(config);
                                    UpdateAgentConfiguration(info, config);
                                    UpdateDatabaseConfiguration(config);

                                    result.Configuration = config;

                                    if (UserManagementSettings.Database != null)
                                    {
                                        SaveLocalImage(config.FileLocations.Manufacturer_Logo_Path);
                                        SaveLocalImage(config.FileLocations.Image_Path);
                                    }

                                    if (ParentPage.ParentManager.CurrentUser != null)
                                    {
                                        result.Success = Configurations.AddConfigurationToUser(ParentPage.ParentManager.CurrentUser, config);

                                        result.Configuration.TableName = config.TableName;
                                    }
                                    else
                                    {
                                        result.Success = SaveLocalConfigurationToUser(config);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            this.Dispatcher.BeginInvoke(new Action<AddShared_Return>(AddSharedItem_GUI), PRIORITY_BACKGROUND, new object[] { result });
        }

        void SaveLocalImage(string filename)
        {
            if (filename != null)
            {
                var image = TH_UserManagement.Management.Remote.Images.GetImage(filename);
                if (image != null)
                {
                    string savePath = FileLocations.TrakHoundTemp + "\\" + filename;

                    image.Save(savePath);
                }
            }
        }

        private void UpdateEnabled(Configuration config)
        {
            config.ClientEnabled = true;
            XML_Functions.SetInnerText(config.ConfigurationXML, "/ClientEnabled", "True");

            config.ServerEnabled = true;
            XML_Functions.SetInnerText(config.ConfigurationXML, "/ServerEnabled", "True");
        }

        private void UpdateAgentConfiguration(DeviceInfo info, Configuration config)
        {
            // Save IP Address
            config.Agent.Address = info.IPAddress;
            XML_Functions.SetInnerText(config.ConfigurationXML, "/Agent/Address", info.IPAddress);

            // Save Port
            config.Agent.Port = info.Port;
            XML_Functions.SetInnerText(config.ConfigurationXML, "/Agent/Port", info.Port.ToString());

            // Save DeviceName
            config.Agent.DeviceName = info.Device.name;
            XML_Functions.SetInnerText(config.ConfigurationXML, "/Agent/DeviceName", info.Device.name);
        }

        private void UpdateDatabaseConfiguration(Configuration config)
        {
            // If NO databases are configured then add a SQLite database for both client and server
            if (config.Databases_Client.Databases.Count == 0 && config.Databases_Server.Databases.Count == 0)
            {
                AddDatabaseConfiguration("/Databases_Client", config);
                AddDatabaseConfiguration("/Databases_Server", config);
            }
        }

        private void AddDatabaseConfiguration(string prefix, Configuration config)
        {
            XML_Functions.AddNode(config.ConfigurationXML, prefix + "/SQLite");
            XML_Functions.SetAttribute(config.ConfigurationXML, prefix + "/SQLite", "id", "00");

            string path = FileLocations.Databases + "\\" + config.UniqueId + ".xml";

            XML_Functions.SetInnerText(config.ConfigurationXML, prefix + "/SQLite/DatabasePath", path);
        }

        private bool SaveLocalConfigurationToUser(Configuration config)
        {
            bool result = false;

            // Set new Unique Id
            string uniqueId = String_Functions.RandomString(20);
            config.UniqueId = uniqueId;
            XML_Functions.SetInnerText(config.ConfigurationXML, "UniqueId", uniqueId);

            // Set Enabled to False
            //config.ClientEnabled = false;
            //config.ServerEnabled = false;
            //XML_Functions.SetInnerText(config.ConfigurationXML, "ClientEnabled", "false");
            //XML_Functions.SetInnerText(config.ConfigurationXML, "ServerEnabled", "false");

            try
            {
                string localPath = FileLocations.Devices + "\\" + uniqueId + ".xml";

                config.ConfigurationXML.Save(localPath);

                result = true;
            }
            catch (Exception ex)
            {
                Logger.Log("SaveLocalConfiguartionToUser() :: Exception :: " + ex.Message);
            }

            return result;
        }

        void AddSharedItem_GUI(AddShared_Return result)
        {
            if (result.Success && result.Configuration != null)
            {
                ParentPage.ParentManager.AddDevice(result.Configuration);
                //if (ParentManager.DeviceAdded != null) ParentManager.DeviceAdded(result.Configuration);
                int index = DeviceInfos.ToList().FindIndex(x => x.Id == result.DeviceInfo.Id);
                if (index >= 0) DeviceInfos.RemoveAt(index);

                DevicesNotAdded = DeviceInfos.Count;
                DevicesAlreadyAdded += 1;
            }
            else
            {
                if (result.DeviceInfo != null) result.DeviceInfo.Loading = false;
                TH_WPF.MessageBox.Show("Add device failed. Try Again.", "Add device failed", TH_WPF.MessageBoxButtons.Ok);
            }
        }

        #endregion

        #region "Network Nodes"

        private void FindDevices()
        {
            this.Dispatcher.BeginInvoke(new Action(DeviceInfos.Clear));

            var pingNodes = new Network_Functions.PingNodes();
            pingNodes.PingSuccessful += PingNodes_PingSuccessful;
            pingNodes.Finished += PingNodes_Finished;
            pingNodes.Start();
        }

        private void PingNodes_PingSuccessful(System.Net.NetworkInformation.PingReply reply)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(RunProbe), reply.Address);
        }

        private void RunProbe(object o)
        {
            var ip = (IPAddress)o;

            var ports = new int[5] { 5000, 5001, 5002, 5003, 5004 };

            foreach (var port in ports)
            {
                if (RunProbe(ip, port)) break;
            }
        }

        private bool RunProbe(IPAddress ip, int port)
        {
            bool result = false;

            string url = TH_MTConnect.HTTP.GetUrl(ip.ToString(), port, null);

            var probe = Requests.Get(url, null, 2000, 1);
            if (probe != null)
            {
                result = true;

                foreach (var device in probe.devices)
                {
                    this.Dispatcher.BeginInvoke(new Action<IPAddress, int, Device>(AddDeviceInfo), PRIORITY_BACKGROUND, new object[] { ip, port, device });
                }
            }

            return result;
        }

        private void PingNodes_Finished()
        {
            Dispatcher.BeginInvoke(new Action(() => { DevicesLoading = false; }));
        }

        #endregion

        private ObservableCollection<DeviceInfo> _deviceInfos;
        public ObservableCollection<DeviceInfo> DeviceInfos
        {
            get
            {
                if (_deviceInfos == null) _deviceInfos = new ObservableCollection<DeviceInfo>();
                return _deviceInfos;
            }
            set
            {
                _deviceInfos = value;
            }
        }

        private void AddDeviceInfo(IPAddress address, int port, Device device)
        {
            // Check if already in DeviceManagerList.Devices
            bool alreadyAdded = ParentPage.ParentManager.Devices.ToList().Exists(x =>
            x.Configuration.Agent.Address == address.ToString() &&
            x.Configuration.Agent.Port == port &&
            x.Configuration.Agent.DeviceName == device.name
            );
            
            if (!alreadyAdded)
            {
                // Look for matching shared configuration
                var catalogInfo = catalogInfos.Find(x => GetLinkTagMatchValue(device, x));
                //var catalogInfo = catalogInfos.Find(x =>
                //            GetMatchValue(x.Item.link_tag) == GetMatchValue(device.name) &&
                //            GetMatchValue(x.Item.manufacturer) == GetMatchValue(device.description.manufacturer)
                //            );
                if (catalogInfo != null)
                {
                    var info = new DeviceInfo();
                    info.Id = String_Functions.RandomString(80);
                    info.IPAddress = address.ToString();
                    info.Port = port;
                    info.Device = device;
                    info.Image = GetSourceFromImage(catalogInfo.Image);
                    info.SharedListItem = catalogInfo.Item;

                    DeviceInfos.Add(info);

                    DevicesNotAdded = DeviceInfos.Count;
                }
            } 
            else
            {
                DevicesAlreadyAdded += 1;
            }
        }

        private ImageSource GetSourceFromImage(System.Drawing.Image img)
        {
            BitmapImage bitmap = null;

            if (img != null)
            {
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(img);

                IntPtr bmpPt = bmp.GetHbitmap();
                BitmapSource bmpSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmpPt, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                bmpSource.Freeze();

                if (bmpSource.PixelWidth > bmpSource.PixelHeight)
                {
                    bitmap = TH_WPF.Image_Functions.SetImageSize(bmpSource, 100);
                }
                else
                {
                    bitmap = TH_WPF.Image_Functions.SetImageSize(bmpSource, 0, 40);
                }
            }

            return bitmap;
        }

        private static string GetMatchValue(string s)
        {
            if (s != null) return s.ToLower();
            return s;
        }

        private static bool GetLinkTagMatchValue(Device device, CatalogInfo info)
        {
            if (info != null && info.Item != null && info.Item.link_tag != null && device != null)
            {
                string x = info.Item.link_tag.ToLower();

                if (x == ToLower(device.description.manufacturer)) return true;
                if (x == ToLower(device.description.manufacturer) + "." + ToLower(device.description.model)) return true;
                if (x == ToLower(device.description.manufacturer) + "." + ToLower(device.description.model) + "." + ToLower(device.name)) return true;
                if (x == ToLower(device.description.manufacturer) + "." + ToLower(device.name)) return true;
                if (x == ToLower(device.name)) return true;              
            }

            return false;
        }

        private static string ToLower(string s)
        {
            if (s != null) return s.ToLower();
            return s;
        }

        public int DevicesNotAdded
        {
            get { return (int)GetValue(DevicesNotAddedProperty); }
            set { SetValue(DevicesNotAddedProperty, value); }
        }

        public static readonly DependencyProperty DevicesNotAddedProperty =
            DependencyProperty.Register("DevicesNotAdded", typeof(int), typeof(AutoDetect), new PropertyMetadata(0));


        public int DevicesAlreadyAdded
        {
            get { return (int)GetValue(DevicesAlreadyAddedProperty); }
            set { SetValue(DevicesAlreadyAddedProperty, value); }
        }

        public static readonly DependencyProperty DevicesAlreadyAddedProperty =
            DependencyProperty.Register("DevicesAlreadyAdded", typeof(int), typeof(AutoDetect), new PropertyMetadata(0));


        #region "Catalog"

        class CatalogInfo
        {
            public Shared.SharedListItem Item { get; set; }
            public System.Drawing.Image Image { get; set; }
        }

        List<CatalogInfo> catalogInfos = new List<CatalogInfo>();

        Thread LoadCatalog_THREAD;

        public void LoadCatalog()
        {
            Dispatcher.BeginInvoke(new Action(() => {
                DevicesLoading = true;
                DeviceInfos.Clear();
                catalogInfos.Clear();
                DevicesAlreadyAdded = 0;
                DevicesNotAdded = 0;
            }));

            if (LoadCatalog_THREAD != null) LoadCatalog_THREAD.Abort();

            LoadCatalog_THREAD = new Thread(new ThreadStart(LoadCatalog_Worker));
            LoadCatalog_THREAD.Start();
        }

        void LoadCatalog_Worker()
        {
            var sharedItems = Shared.GetSharedList();

            foreach (var item in sharedItems)
            {
                var info = new CatalogInfo();
                info.Item = item;
                info.Image = GetCatalogImage(item);

                catalogInfos.Add(info);
            }

            FindDevices();
        }

        private System.Drawing.Image GetCatalogImage(Shared.SharedListItem item)
        {
            System.Drawing.Image img = null;

            if (item.image_url != null)
            {
                // Just use Remote.Images (don't look for local)
                img = TH_UserManagement.Management.Remote.Images.GetImage(item.image_url);
            }

            return img;
        }

        #endregion


        private void Add_Clicked(TH_WPF.Button bt)
        {
            if (bt.DataObject != null)
            {
                var info = (DeviceInfo)bt.DataObject;

                AddSharedItem(info);
            }
        }

        private void Refresh_Clicked(TH_WPF.Button bt)
        {
            LoadCatalog();
        }

        private void AddAll_Clicked(TH_WPF.Button bt)
        {
            foreach (var info in DeviceInfos)
            {
                AddSharedItem(info);
            }
        }
    }
}
