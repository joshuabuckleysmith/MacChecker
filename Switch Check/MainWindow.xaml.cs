using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Switch_Check
{

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }
        string portstateinput;
        //"([0:9a:fA:F]{2}:[0:9a:fA:F]{2}:[0:9a:fA:F]{2}:[0:9a:fA:F]{2}:[0:9a:fA:F]{2}:[0:9a:fA:F]{2}|ge:0/0/\d+)";
        char[] delimiters = new char[] { '\r', '\n', ' ' };
        
        public List<string> macaddresses = new List<string>();
        public List<string> portsformacaddresses = new List<string>();
        public List<string> portsfromadminupdownstate = new List<string>();
        public List<string> adminstatesofports = new List<string>();
        public List<string> linkstatesofports = new List<string>();
        public List<Label> MacAddressBoxes = new List<Label>();
        public List<Label> IsValidBoxes = new List<Label>();
        public List<Rectangle> HighlightList = new List<Rectangle>();
        public List<Label> PortBoxes = new List<Label>();
        public List<Label> AdminStateBoxes = new List<Label>();
        public List<Label> LinkStateBoxes = new List<Label>();
        public List<Label> DeviceTypeBoxes = new List<Label>();
        public List<Label> IpAddressList = new List<Label>();
        public List<Label> ExpectedDeviceBoxes = new List<Label>();
        public List<Ping> Pings = new List<Ping>();
        public List<Task> Pingtasks = new List<Task>();
        public PingReply reply = null;
        public string octet = "low";

        List<int> rtts = new List<int>();
        Funks F = new Funks();
        private delegate void ThreadDelegate();
        string a;
        string b;
        string networkaddress = "";
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task taskA = Task.Factory.StartNew(() => TheMagic());
            taskA.Wait();
            ThreadDelegate updater = new ThreadDelegate(UpdateInterface);
            updater.BeginInvoke(null, null);
        }

        Match macs;
        Match ports;
        Match arps;
        string macpattern = "([0-9a-fA-F]{2}:[0-9a-fA-F]{2}:[0-9a-fA-F]{2}:[0-9a-fA-F]{2}:[0-9a-fA-F]{2}:[0-9a-fA-F]{2}|ge-0/0/\\d+(?=\\.))";
        string portpattern = "((?<!.)ge-0/0/\\d+|(?<=ge-0/0/\\d+\\s+)up|(?<=ge-0/0/\\d+\\s+)down|(?<=ge-0/0/\\d+\\s+up\\s+)up|(?<=ge-0/0/\\d+\\s+down\\s+)down|(?<=ge-0/0/\\d+\\s+up\\s+)down)";
        string ipaddresspattern = "([0-9a-fA-F]{2}:[0-9a-fA-F]{2}:[0-9a-fA-F]{2}:[0-9a-fA-F]{2}:[0-9a-fA-F]{2}:[0-9a-fA-F]{2}(?= 10\\.)|(?<=[0-9a-fA-F] )10.[0-9]+.[0-9]+.[0-9]+)";
        string portnumberpattern = "((?<=ge-0/0/)[0-9]+)";
        string iplastoctetpattern = "((?<=[0-9]+\\.[0-9]+\\.[0-9]+\\.)[0-9]+)";

        private void TheMagic()
        {
            foreach (PortState p in PortState.PortsList)
            {
                p.Clear();
            }
            
            ports = Regex.Match(portstateinput, portpattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(5));
            Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(10))));
            while (ports.Success)
            {
                foreach (PortState port in PortState.PortsList)
                {
                    port.Port = ports.Groups[0].Value;
                    ports = ports.NextMatch();
                    port.AdminState = ports.Groups[0].Value;
                    ports = ports.NextMatch();
                    port.LinkState = ports.Groups[0].Value;
                    ports = ports.NextMatch();
                    Match portno = Regex.Match(port.Port, portnumberpattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(5));
                    try
                    {
                        port.PortNumber = Convert.ToInt32(portno.Groups[0].Value);
                    }
                    catch (FormatException)
                    {
                        port.PortNumber = 0;
                    }
                }
            }

            Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(20))));
            macs = Regex.Match(portstateinput, macpattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(5));
            while (macs.Success)
            {
                a = macs.Groups[0].Value;
                macs = macs.NextMatch();
                b = macs.Groups[0].Value;
                macs = macs.NextMatch();
                foreach (PortState port in PortState.PortsList)
                {
                if (port.Port == b)
                    if (port.Port.Length != 0)
                        {
                            {
                                port.Mac = a; // set a mac address
                                port.DeviceType = ConvertOuiToName(port.Mac);
                                break;
                            }
                        }
                }
                foreach (PortState port in PortState.PortsList)
                {
                    if (port.DeviceType == "")
                    {
                        port.DeviceType = "?";
                    }
                }
            }

            Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(30))));
            arps = Regex.Match(arpentry, ipaddresspattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(5));
            while (arps.Success)
            {
                a = arps.Groups[0].Value;
                arps = arps.NextMatch();
                b = arps.Groups[0].Value;
                arps = arps.NextMatch();
                foreach (PortState port in PortState.PortsList)
                {
                    if (port.Mac == a)
                    {
                        port.IpAddress = b;
                        port.IpLastOctet = (Regex.Match(b, iplastoctetpattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(5))).Groups[0].Value;
                        break;
                    }
                }
                Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(40))));
            }
        }

        bool highsubnet;
        IPAddress addr;
        string storenumber = "0";
        IPHostEntry hostEntry = new IPHostEntry();

        private void Button_Click_1(object sender, RoutedEventArgs e) //===============port scanner here===============
        {
            ThreadDelegate updater = new ThreadDelegate(PortScanner);
            updater.BeginInvoke(null, null);
        }

        async public void PortScanner()
        {
            await PortScanProgress.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => PortScanProgress.Fill = Brushes.LightGray)));
            await ScanOk.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => ScanOk.Text = "Scan in progress...")));
            addr = ParseInput(storenumber);
            string ipfirst3pattern = "([0-9]+\\.[0-9]+\\.[0-9]+\\.)";
            Match first3match = Regex.Match(Convert.ToString(addr), ipfirst3pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(5));
            Match last3match = Regex.Match(Convert.ToString(addr), iplastoctetpattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(5));
            string first3octets = first3match.Groups[0].Value;
            string lastoctet = last3match.Groups[0].Value;
            networkaddress = first3octets;
            if (lastoctet == "1")
            {
                highsubnet = false;
            }
            else
            {
                highsubnet = true;
            }
            byte[] icmpdata = new byte[Convert.ToInt32("64")];
            int i = 1;
            while (i < 128)
            {
                Pings.Add(new Ping());
                i++;
            }
            i = 1;
            if (highsubnet == true)
            {
                i = 129;
            }
            foreach (Ping pingobj in Pings)
            {
                Thread.Sleep(10);
                pingobj.SendAsync(IPAddress.Parse(first3octets + Convert.ToString(i)), 1, icmpdata);
                i++;
            }
            await PortScanProgress.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => PortScanProgress.Fill = Brushes.LightGreen)));
            await ScanOk.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => ScanOk.Text = "Scan Complete")));
            Thread.Sleep(1000);
            await PortScanProgress.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => PortScanProgress.Fill = Brushes.White)));
            await ScanOk.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => ScanOk.Text = "")));
        }

        private IPAddress ParseInput(string addressinputvar)
        {
                storenumber = addressinputvar.PadLeft(5, '0');
                char trim = 'S';
                storenumber = storenumber.TrimStart(trim);
                try
                {
                    hostEntry = Dns.GetHostEntry("dg"+storenumber);
                }
                catch (System.Net.Sockets.SocketException)
                {
                    return addr;
                }
                addr = hostEntry.AddressList[0];
                return addr;
        }
        
        //#==========================================================PING!
        
        private void Sendicmp(IPAddress addresstoping)
        {
            Ping pingSender = new Ping();
            //byte[] icmpdata = new byte[1];
            byte[] icmpdata = new byte[Convert.ToInt32("64")];
            for (int i = 0; i < icmpdata.Length; i++)
            {
                icmpdata[i] = 0x61;
            }
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            pingSender.Send(addresstoping, 2000, icmpdata);
        }



        async private void UpdateInterface()
        {
            foreach (Rectangle rect in HighlightList)
            {
                await rect.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => rect.Fill = Brushes.White)));
            }
            int searchindex = 0;
            foreach (Label ipbox in IpAddressList)
            {
                await ipbox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => ipbox.Content = PortState.PortsList[searchindex].IpAddress)));
                
                searchindex++;
            }
            await Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(60))));
            searchindex = 0;
            foreach (Label portbox in PortBoxes)
            {
                await portbox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => portbox.Content = PortState.PortsList[searchindex].Port)));
                searchindex++;
            }
            await Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(40))));
            searchindex = 0;
            foreach (Label macbox in MacAddressBoxes)
            {
                await macbox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => macbox.Content = PortState.PortsList[searchindex].Mac)));
                searchindex++;
            }
            await Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(50))));
            searchindex = 0;
            
            foreach (Label adminbox in AdminStateBoxes)
            {
                await adminbox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => adminbox.Content = PortState.PortsList[searchindex].AdminState)));
                searchindex++;
            }
            await Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(70))));
            searchindex = 0;
            foreach (Label linkbox in LinkStateBoxes)
            {
                await linkbox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => linkbox.Content = PortState.PortsList[searchindex].LinkState)));
                searchindex++;
            }
            await Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(80))));
            searchindex = 0;
            foreach (Label validbox in IsValidBoxes)
            {
                await validbox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => validbox.Content = Funks.portstates[PortState.PortsList[searchindex].CheckIsValid])));
                searchindex++;
            }
            await Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(80))));
            searchindex = 0;
            foreach (Label devicelabel in DeviceTypeBoxes)
            {
                await devicelabel.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => devicelabel.Content = PortState.PortsList[searchindex].DeviceType)));
                searchindex++;
            }
            await Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(80))));
            searchindex = 0;
            foreach (Rectangle rect in HighlightList)
            {
                await rect.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => rect.Fill = Funks.portcolors[(PortState.PortsList[searchindex].CheckIsValid)])));
                    searchindex++;
            }
            await Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(0))));
        }


        private void UpdateProgressbar(int val)
        {
            Progbar.Value = val;
        }

        private void PortInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            portstateinput = PortInput.Text;
        }

        private void OutputBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PopulateOutputBox(string t)
        {

        }

        private void PopulateLogBox(string t)
        {

        }

        private void OutputBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }


        private string ConvertOuiToName(string oui)
        {
            try
            {
                oui = oui.Substring(0, 8);
            }
            catch (Exception e)
            {
                MessageBox.Show(Convert.ToString(e));
            }
            try
            {
                oui = Funks.ouiDictionary[oui];
            }
            catch (KeyNotFoundException)
            {
                oui = "?";
            }
            return oui;
        }

        bool firstrun = true;
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (firstrun == true)
            {
                firstrun = false;
                ToMacList();
                ToAdminList();
                ToLinkList();
                ToDeviceList();
                ToHighlightList();
                ToIpList();
                ToIsValidList();
                ToExpectedDeviceList();
                int i = 0;
                foreach (Label box in ExpectedDeviceBoxes)
                {
                    box.Content = Funks.ExpectedDeviceDictionaryUS[i];
                    i++;
                }
            }
        }
 


        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }



        public void ToExpectedDeviceList()
        {
            ExpectedDeviceBoxes.Add(ExpectedDevice1);
            ExpectedDeviceBoxes.Add(ExpectedDevice2);
            ExpectedDeviceBoxes.Add(ExpectedDevice3);
            ExpectedDeviceBoxes.Add(ExpectedDevice4);
            ExpectedDeviceBoxes.Add(ExpectedDevice5);
            ExpectedDeviceBoxes.Add(ExpectedDevice6);
            ExpectedDeviceBoxes.Add(ExpectedDevice7);
            ExpectedDeviceBoxes.Add(ExpectedDevice8);
            ExpectedDeviceBoxes.Add(ExpectedDevice9);
            ExpectedDeviceBoxes.Add(ExpectedDevice10);
            ExpectedDeviceBoxes.Add(ExpectedDevice11);
            ExpectedDeviceBoxes.Add(ExpectedDevice12);
            ExpectedDeviceBoxes.Add(ExpectedDevice13);
            ExpectedDeviceBoxes.Add(ExpectedDevice14);
            ExpectedDeviceBoxes.Add(ExpectedDevice15);
            ExpectedDeviceBoxes.Add(ExpectedDevice16);
            ExpectedDeviceBoxes.Add(ExpectedDevice17);
            ExpectedDeviceBoxes.Add(ExpectedDevice18);
            ExpectedDeviceBoxes.Add(ExpectedDevice19);
            ExpectedDeviceBoxes.Add(ExpectedDevice20);
            ExpectedDeviceBoxes.Add(ExpectedDevice21);
            ExpectedDeviceBoxes.Add(ExpectedDevice22);
            ExpectedDeviceBoxes.Add(ExpectedDevice23);
            ExpectedDeviceBoxes.Add(ExpectedDevice24);
            ExpectedDeviceBoxes.Add(ExpectedDevice25);
            ExpectedDeviceBoxes.Add(ExpectedDevice26);
            ExpectedDeviceBoxes.Add(ExpectedDevice27);
            ExpectedDeviceBoxes.Add(ExpectedDevice28);
            ExpectedDeviceBoxes.Add(ExpectedDevice29);
            ExpectedDeviceBoxes.Add(ExpectedDevice30);
            ExpectedDeviceBoxes.Add(ExpectedDevice31);
            ExpectedDeviceBoxes.Add(ExpectedDevice32);
            ExpectedDeviceBoxes.Add(ExpectedDevice33);
            ExpectedDeviceBoxes.Add(ExpectedDevice34);
            ExpectedDeviceBoxes.Add(ExpectedDevice35);
            ExpectedDeviceBoxes.Add(ExpectedDevice36);
            ExpectedDeviceBoxes.Add(ExpectedDevice37);
            ExpectedDeviceBoxes.Add(ExpectedDevice38);
            ExpectedDeviceBoxes.Add(ExpectedDevice39);
            ExpectedDeviceBoxes.Add(ExpectedDevice40);
            ExpectedDeviceBoxes.Add(ExpectedDevice41);
            ExpectedDeviceBoxes.Add(ExpectedDevice42);
            ExpectedDeviceBoxes.Add(ExpectedDevice43);
            ExpectedDeviceBoxes.Add(ExpectedDevice44);
            ExpectedDeviceBoxes.Add(ExpectedDevice45);
            ExpectedDeviceBoxes.Add(ExpectedDevice46);
            ExpectedDeviceBoxes.Add(ExpectedDevice47);
            ExpectedDeviceBoxes.Add(ExpectedDevice48);
        }


        public void ToMacList()
        {
            MacAddressBoxes.Add(MacLabel1);
            MacAddressBoxes.Add(MacLabel2);
            MacAddressBoxes.Add(MacLabel3);
            MacAddressBoxes.Add(MacLabel4);
            MacAddressBoxes.Add(MacLabel5);
            MacAddressBoxes.Add(MacLabel6);
            MacAddressBoxes.Add(MacLabel7);
            MacAddressBoxes.Add(MacLabel8);
            MacAddressBoxes.Add(MacLabel9);
            MacAddressBoxes.Add(MacLabel10);
            MacAddressBoxes.Add(MacLabel11);
            MacAddressBoxes.Add(MacLabel12);
            MacAddressBoxes.Add(MacLabel13);
            MacAddressBoxes.Add(MacLabel14);
            MacAddressBoxes.Add(MacLabel15);
            MacAddressBoxes.Add(MacLabel16);
            MacAddressBoxes.Add(MacLabel17);
            MacAddressBoxes.Add(MacLabel18);
            MacAddressBoxes.Add(MacLabel19);
            MacAddressBoxes.Add(MacLabel20);
            MacAddressBoxes.Add(MacLabel21);
            MacAddressBoxes.Add(MacLabel22);
            MacAddressBoxes.Add(MacLabel23);
            MacAddressBoxes.Add(MacLabel24);
            MacAddressBoxes.Add(MacLabel25);
            MacAddressBoxes.Add(MacLabel26);
            MacAddressBoxes.Add(MacLabel27);
            MacAddressBoxes.Add(MacLabel28);
            MacAddressBoxes.Add(MacLabel29);
            MacAddressBoxes.Add(MacLabel30);
            MacAddressBoxes.Add(MacLabel31);
            MacAddressBoxes.Add(MacLabel32);
            MacAddressBoxes.Add(MacLabel33);
            MacAddressBoxes.Add(MacLabel34);
            MacAddressBoxes.Add(MacLabel35);
            MacAddressBoxes.Add(MacLabel36);
            MacAddressBoxes.Add(MacLabel37);
            MacAddressBoxes.Add(MacLabel38);
            MacAddressBoxes.Add(MacLabel39);
            MacAddressBoxes.Add(MacLabel40);
            MacAddressBoxes.Add(MacLabel41);
            MacAddressBoxes.Add(MacLabel42);
            MacAddressBoxes.Add(MacLabel43);
            MacAddressBoxes.Add(MacLabel44);
            MacAddressBoxes.Add(MacLabel45);
            MacAddressBoxes.Add(MacLabel46);
            MacAddressBoxes.Add(MacLabel47);
            MacAddressBoxes.Add(MacLabel48);
        }

        public void ToAdminList()
        {
            AdminStateBoxes.Add(AdminStateLabel1);
            AdminStateBoxes.Add(AdminStateLabel2);
            AdminStateBoxes.Add(AdminStateLabel3);
            AdminStateBoxes.Add(AdminStateLabel4);
            AdminStateBoxes.Add(AdminStateLabel5);
            AdminStateBoxes.Add(AdminStateLabel6);
            AdminStateBoxes.Add(AdminStateLabel7);
            AdminStateBoxes.Add(AdminStateLabel8);
            AdminStateBoxes.Add(AdminStateLabel9);
            AdminStateBoxes.Add(AdminStateLabel10);
            AdminStateBoxes.Add(AdminStateLabel11);
            AdminStateBoxes.Add(AdminStateLabel12);
            AdminStateBoxes.Add(AdminStateLabel13);
            AdminStateBoxes.Add(AdminStateLabel14);
            AdminStateBoxes.Add(AdminStateLabel15);
            AdminStateBoxes.Add(AdminStateLabel16);
            AdminStateBoxes.Add(AdminStateLabel17);
            AdminStateBoxes.Add(AdminStateLabel18);
            AdminStateBoxes.Add(AdminStateLabel19);
            AdminStateBoxes.Add(AdminStateLabel20);
            AdminStateBoxes.Add(AdminStateLabel21);
            AdminStateBoxes.Add(AdminStateLabel22);
            AdminStateBoxes.Add(AdminStateLabel23);
            AdminStateBoxes.Add(AdminStateLabel24);
            AdminStateBoxes.Add(AdminStateLabel25);
            AdminStateBoxes.Add(AdminStateLabel26);
            AdminStateBoxes.Add(AdminStateLabel27);
            AdminStateBoxes.Add(AdminStateLabel28);
            AdminStateBoxes.Add(AdminStateLabel29);
            AdminStateBoxes.Add(AdminStateLabel30);
            AdminStateBoxes.Add(AdminStateLabel31);
            AdminStateBoxes.Add(AdminStateLabel32);
            AdminStateBoxes.Add(AdminStateLabel33);
            AdminStateBoxes.Add(AdminStateLabel34);
            AdminStateBoxes.Add(AdminStateLabel35);
            AdminStateBoxes.Add(AdminStateLabel36);
            AdminStateBoxes.Add(AdminStateLabel37);
            AdminStateBoxes.Add(AdminStateLabel38);
            AdminStateBoxes.Add(AdminStateLabel39);
            AdminStateBoxes.Add(AdminStateLabel40);
            AdminStateBoxes.Add(AdminStateLabel41);
            AdminStateBoxes.Add(AdminStateLabel42);
            AdminStateBoxes.Add(AdminStateLabel43);
            AdminStateBoxes.Add(AdminStateLabel44);
            AdminStateBoxes.Add(AdminStateLabel45);
            AdminStateBoxes.Add(AdminStateLabel46);
            AdminStateBoxes.Add(AdminStateLabel47);
            AdminStateBoxes.Add(AdminStateLabel48);
        }

        public void ToLinkList()
        {
            LinkStateBoxes.Add(LinkStateLabel1);
            LinkStateBoxes.Add(LinkStateLabel2);
            LinkStateBoxes.Add(LinkStateLabel3);
            LinkStateBoxes.Add(LinkStateLabel4);
            LinkStateBoxes.Add(LinkStateLabel5);
            LinkStateBoxes.Add(LinkStateLabel6);
            LinkStateBoxes.Add(LinkStateLabel7);
            LinkStateBoxes.Add(LinkStateLabel8);
            LinkStateBoxes.Add(LinkStateLabel9);
            LinkStateBoxes.Add(LinkStateLabel10);
            LinkStateBoxes.Add(LinkStateLabel11);
            LinkStateBoxes.Add(LinkStateLabel12);
            LinkStateBoxes.Add(LinkStateLabel13);
            LinkStateBoxes.Add(LinkStateLabel14);
            LinkStateBoxes.Add(LinkStateLabel15);
            LinkStateBoxes.Add(LinkStateLabel16);
            LinkStateBoxes.Add(LinkStateLabel17);
            LinkStateBoxes.Add(LinkStateLabel18);
            LinkStateBoxes.Add(LinkStateLabel19);
            LinkStateBoxes.Add(LinkStateLabel20);
            LinkStateBoxes.Add(LinkStateLabel21);
            LinkStateBoxes.Add(LinkStateLabel22);
            LinkStateBoxes.Add(LinkStateLabel23);
            LinkStateBoxes.Add(LinkStateLabel24);
            LinkStateBoxes.Add(LinkStateLabel25);
            LinkStateBoxes.Add(LinkStateLabel26);
            LinkStateBoxes.Add(LinkStateLabel27);
            LinkStateBoxes.Add(LinkStateLabel28);
            LinkStateBoxes.Add(LinkStateLabel29);
            LinkStateBoxes.Add(LinkStateLabel30);
            LinkStateBoxes.Add(LinkStateLabel31);
            LinkStateBoxes.Add(LinkStateLabel32);
            LinkStateBoxes.Add(LinkStateLabel33);
            LinkStateBoxes.Add(LinkStateLabel34);
            LinkStateBoxes.Add(LinkStateLabel35);
            LinkStateBoxes.Add(LinkStateLabel36);
            LinkStateBoxes.Add(LinkStateLabel37);
            LinkStateBoxes.Add(LinkStateLabel38);
            LinkStateBoxes.Add(LinkStateLabel39);
            LinkStateBoxes.Add(LinkStateLabel40);
            LinkStateBoxes.Add(LinkStateLabel41);
            LinkStateBoxes.Add(LinkStateLabel42);
            LinkStateBoxes.Add(LinkStateLabel43);
            LinkStateBoxes.Add(LinkStateLabel44);
            LinkStateBoxes.Add(LinkStateLabel45);
            LinkStateBoxes.Add(LinkStateLabel46);
            LinkStateBoxes.Add(LinkStateLabel47);
            LinkStateBoxes.Add(LinkStateLabel48);
        }
        public void ToDeviceList()
        {
            DeviceTypeBoxes.Add(DeviceTypeLabel1);
            DeviceTypeBoxes.Add(DeviceTypeLabel2);
            DeviceTypeBoxes.Add(DeviceTypeLabel3);
            DeviceTypeBoxes.Add(DeviceTypeLabel4);
            DeviceTypeBoxes.Add(DeviceTypeLabel5);
            DeviceTypeBoxes.Add(DeviceTypeLabel6);
            DeviceTypeBoxes.Add(DeviceTypeLabel7);
            DeviceTypeBoxes.Add(DeviceTypeLabel8);
            DeviceTypeBoxes.Add(DeviceTypeLabel9);
            DeviceTypeBoxes.Add(DeviceTypeLabel10);
            DeviceTypeBoxes.Add(DeviceTypeLabel11);
            DeviceTypeBoxes.Add(DeviceTypeLabel12);
            DeviceTypeBoxes.Add(DeviceTypeLabel13);
            DeviceTypeBoxes.Add(DeviceTypeLabel14);
            DeviceTypeBoxes.Add(DeviceTypeLabel15);
            DeviceTypeBoxes.Add(DeviceTypeLabel16);
            DeviceTypeBoxes.Add(DeviceTypeLabel17);
            DeviceTypeBoxes.Add(DeviceTypeLabel18);
            DeviceTypeBoxes.Add(DeviceTypeLabel19);
            DeviceTypeBoxes.Add(DeviceTypeLabel20);
            DeviceTypeBoxes.Add(DeviceTypeLabel21);
            DeviceTypeBoxes.Add(DeviceTypeLabel22);
            DeviceTypeBoxes.Add(DeviceTypeLabel23);
            DeviceTypeBoxes.Add(DeviceTypeLabel24);
            DeviceTypeBoxes.Add(DeviceTypeLabel25);
            DeviceTypeBoxes.Add(DeviceTypeLabel26);
            DeviceTypeBoxes.Add(DeviceTypeLabel27);
            DeviceTypeBoxes.Add(DeviceTypeLabel28);
            DeviceTypeBoxes.Add(DeviceTypeLabel29);
            DeviceTypeBoxes.Add(DeviceTypeLabel30);
            DeviceTypeBoxes.Add(DeviceTypeLabel31);
            DeviceTypeBoxes.Add(DeviceTypeLabel32);
            DeviceTypeBoxes.Add(DeviceTypeLabel33);
            DeviceTypeBoxes.Add(DeviceTypeLabel34);
            DeviceTypeBoxes.Add(DeviceTypeLabel35);
            DeviceTypeBoxes.Add(DeviceTypeLabel36);
            DeviceTypeBoxes.Add(DeviceTypeLabel37);
            DeviceTypeBoxes.Add(DeviceTypeLabel38);
            DeviceTypeBoxes.Add(DeviceTypeLabel39);
            DeviceTypeBoxes.Add(DeviceTypeLabel40);
            DeviceTypeBoxes.Add(DeviceTypeLabel41);
            DeviceTypeBoxes.Add(DeviceTypeLabel42);
            DeviceTypeBoxes.Add(DeviceTypeLabel43);
            DeviceTypeBoxes.Add(DeviceTypeLabel44);
            DeviceTypeBoxes.Add(DeviceTypeLabel45);
            DeviceTypeBoxes.Add(DeviceTypeLabel46);
            DeviceTypeBoxes.Add(DeviceTypeLabel47);
            DeviceTypeBoxes.Add(DeviceTypeLabel48);
        }

        public void ToHighlightList()
        {
            HighlightList.Add(Highlight1);
            HighlightList.Add(Highlight2);
            HighlightList.Add(Highlight3);
            HighlightList.Add(Highlight4);
            HighlightList.Add(Highlight5);
            HighlightList.Add(Highlight6);
            HighlightList.Add(Highlight7);
            HighlightList.Add(Highlight8);
            HighlightList.Add(Highlight9);
            HighlightList.Add(Highlight10);
            HighlightList.Add(Highlight11);
            HighlightList.Add(Highlight12);
            HighlightList.Add(Highlight13);
            HighlightList.Add(Highlight14);
            HighlightList.Add(Highlight15);
            HighlightList.Add(Highlight16);
            HighlightList.Add(Highlight17);
            HighlightList.Add(Highlight18);
            HighlightList.Add(Highlight19);
            HighlightList.Add(Highlight20);
            HighlightList.Add(Highlight21);
            HighlightList.Add(Highlight22);
            HighlightList.Add(Highlight23);
            HighlightList.Add(Highlight24);
            HighlightList.Add(Highlight25);
            HighlightList.Add(Highlight26);
            HighlightList.Add(Highlight27);
            HighlightList.Add(Highlight28);
            HighlightList.Add(Highlight29);
            HighlightList.Add(Highlight30);
            HighlightList.Add(Highlight31);
            HighlightList.Add(Highlight32);
            HighlightList.Add(Highlight33);
            HighlightList.Add(Highlight34);
            HighlightList.Add(Highlight35);
            HighlightList.Add(Highlight36);
            HighlightList.Add(Highlight37);
            HighlightList.Add(Highlight38);
            HighlightList.Add(Highlight39);
            HighlightList.Add(Highlight40);
            HighlightList.Add(Highlight41);
            HighlightList.Add(Highlight42);
            HighlightList.Add(Highlight43);
            HighlightList.Add(Highlight44);
            HighlightList.Add(Highlight45);
            HighlightList.Add(Highlight46);
            HighlightList.Add(Highlight47);
            HighlightList.Add(Highlight48);
        }


        public void ToIpList()
        {
            IpAddressList.Add(IPAddressLabel1);
            IpAddressList.Add(IPAddressLabel2);
            IpAddressList.Add(IPAddressLabel3);
            IpAddressList.Add(IPAddressLabel4);
            IpAddressList.Add(IPAddressLabel5);
            IpAddressList.Add(IPAddressLabel6);
            IpAddressList.Add(IPAddressLabel7);
            IpAddressList.Add(IPAddressLabel8);
            IpAddressList.Add(IPAddressLabel9);
            IpAddressList.Add(IPAddressLabel10);
            IpAddressList.Add(IPAddressLabel11);
            IpAddressList.Add(IPAddressLabel12);
            IpAddressList.Add(IPAddressLabel13);
            IpAddressList.Add(IPAddressLabel14);
            IpAddressList.Add(IPAddressLabel15);
            IpAddressList.Add(IPAddressLabel16);
            IpAddressList.Add(IPAddressLabel17);
            IpAddressList.Add(IPAddressLabel18);
            IpAddressList.Add(IPAddressLabel19);
            IpAddressList.Add(IPAddressLabel20);
            IpAddressList.Add(IPAddressLabel21);
            IpAddressList.Add(IPAddressLabel22);
            IpAddressList.Add(IPAddressLabel23);
            IpAddressList.Add(IPAddressLabel24);
            IpAddressList.Add(IPAddressLabel25);
            IpAddressList.Add(IPAddressLabel26);
            IpAddressList.Add(IPAddressLabel27);
            IpAddressList.Add(IPAddressLabel28);
            IpAddressList.Add(IPAddressLabel29);
            IpAddressList.Add(IPAddressLabel30);
            IpAddressList.Add(IPAddressLabel31);
            IpAddressList.Add(IPAddressLabel32);
            IpAddressList.Add(IPAddressLabel33);
            IpAddressList.Add(IPAddressLabel34);
            IpAddressList.Add(IPAddressLabel35);
            IpAddressList.Add(IPAddressLabel36);
            IpAddressList.Add(IPAddressLabel37);
            IpAddressList.Add(IPAddressLabel38);
            IpAddressList.Add(IPAddressLabel39);
            IpAddressList.Add(IPAddressLabel40);
            IpAddressList.Add(IPAddressLabel41);
            IpAddressList.Add(IPAddressLabel42);
            IpAddressList.Add(IPAddressLabel43);
            IpAddressList.Add(IPAddressLabel44);
            IpAddressList.Add(IPAddressLabel45);
            IpAddressList.Add(IPAddressLabel46);
            IpAddressList.Add(IPAddressLabel47);
            IpAddressList.Add(IPAddressLabel48);
        }


        public void ToIsValidList()
        {
            IsValidBoxes.Add(IsValidLabel1);
            IsValidBoxes.Add(IsValidLabel2);
            IsValidBoxes.Add(IsValidLabel3);
            IsValidBoxes.Add(IsValidLabel4);
            IsValidBoxes.Add(IsValidLabel5);
            IsValidBoxes.Add(IsValidLabel6);
            IsValidBoxes.Add(IsValidLabel7);
            IsValidBoxes.Add(IsValidLabel8);
            IsValidBoxes.Add(IsValidLabel9);
            IsValidBoxes.Add(IsValidLabel10);
            IsValidBoxes.Add(IsValidLabel11);
            IsValidBoxes.Add(IsValidLabel12);
            IsValidBoxes.Add(IsValidLabel13);
            IsValidBoxes.Add(IsValidLabel14);
            IsValidBoxes.Add(IsValidLabel15);
            IsValidBoxes.Add(IsValidLabel16);
            IsValidBoxes.Add(IsValidLabel17);
            IsValidBoxes.Add(IsValidLabel18);
            IsValidBoxes.Add(IsValidLabel19);
            IsValidBoxes.Add(IsValidLabel20);
            IsValidBoxes.Add(IsValidLabel21);
            IsValidBoxes.Add(IsValidLabel22);
            IsValidBoxes.Add(IsValidLabel23);
            IsValidBoxes.Add(IsValidLabel24);
            IsValidBoxes.Add(IsValidLabel25);
            IsValidBoxes.Add(IsValidLabel26);
            IsValidBoxes.Add(IsValidLabel27);
            IsValidBoxes.Add(IsValidLabel28);
            IsValidBoxes.Add(IsValidLabel29);
            IsValidBoxes.Add(IsValidLabel30);
            IsValidBoxes.Add(IsValidLabel31);
            IsValidBoxes.Add(IsValidLabel32);
            IsValidBoxes.Add(IsValidLabel33);
            IsValidBoxes.Add(IsValidLabel34);
            IsValidBoxes.Add(IsValidLabel35);
            IsValidBoxes.Add(IsValidLabel36);
            IsValidBoxes.Add(IsValidLabel37);
            IsValidBoxes.Add(IsValidLabel38);
            IsValidBoxes.Add(IsValidLabel39);
            IsValidBoxes.Add(IsValidLabel40);
            IsValidBoxes.Add(IsValidLabel41);
            IsValidBoxes.Add(IsValidLabel42);
            IsValidBoxes.Add(IsValidLabel43);
            IsValidBoxes.Add(IsValidLabel44);
            IsValidBoxes.Add(IsValidLabel45);
            IsValidBoxes.Add(IsValidLabel46);
            IsValidBoxes.Add(IsValidLabel47);
            IsValidBoxes.Add(IsValidLabel48);
        }

        string arpentry;
        private void ArpEntryBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            arpentry = ArpEntryBox.Text;
        }

        private void StoreEntryBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            storenumber = StoreEntryBox.Text;
        }

        private void PingOutput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CAButton_Checked(object sender, RoutedEventArgs e)
        {
            PortState.CanadaStore = true;
            ClearAll();
            int i = 0;
            foreach (Label box in ExpectedDeviceBoxes)
            {
                box.Content = Funks.ExpectedDeviceDictionaryCA[i];
                i++;
            }
        }

        private void USButton_Checked(object sender, RoutedEventArgs e)
        {
            PortState.CanadaStore = false;
            ClearAll();
            int i = 0;
            foreach (Label box in ExpectedDeviceBoxes)
            {
                box.Content = Funks.ExpectedDeviceDictionaryUS[i];
                i++;
            }
        }

        public void ClearAll()
        {
            foreach (Rectangle rect in HighlightList)
            {
                rect.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => rect.Fill = Brushes.White)));
            }
            try
            {
                foreach (PortState p in PortState.PortsList)
                {
                    if (p != null)
                    {
                        p.Clear();
                    }
                }
            }
            catch (NullReferenceException)
            {

            }
        }
    }
}


