using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Threading;

namespace Switch_Check
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        string macaddressinput;
        string portstateinput;
        //"([0:9a:fA:F]{2}:[0:9a:fA:F]{2}:[0:9a:fA:F]{2}:[0:9a:fA:F]{2}:[0:9a:fA:F]{2}:[0:9a:fA:F]{2}|ge:0/0/\d+)";
        string[] lines;
        string[] ports;
        char[] delimiters = new char[] { '\r', '\n', ' ' };
        public List<PortState> PortsList = new List<PortState>() {
        PortState.g0,         PortState.g1,         PortState.g2,        PortState.g3,         PortState.g4,         PortState.g5,         PortState.g6,         PortState.g7,         PortState.g8,         PortState.g9,         PortState.g10,        PortState.g11,        PortState.g12,        PortState.g13,        PortState.g14,        PortState.g15,        PortState.g16,
        PortState.g17,        PortState.g18,        PortState.g19,       PortState.g20,        PortState.g21,        PortState.g22,        PortState.g23,        PortState.g24,        PortState.g25,        PortState.g26,        PortState.g27,        PortState.g28,        PortState.g29,        PortState.g30,        PortState.g31,
        PortState.g32,        PortState.g33,        PortState.g34,       PortState.g35,        PortState.g36,        PortState.g37,        PortState.g38,        PortState.g39,        PortState.g40,        PortState.g41,        PortState.g42,        PortState.g43,        PortState.g44,        PortState.g45,        PortState.g46,        PortState.g47,        PortState.g48,        PortState.g49
        };
        public List<string> macaddresses = new List<string>();
        public List<string> portsformacaddresses = new List<string>();
        public List<string> portsfromadminupdownstate = new List<string>();
        public List<string> adminstatesofports = new List<string>();
        public List<string> linkstatesofports = new List<string>();
        public List<Label> MacAddressBoxes = new List<Label>();
        public List<Rectangle> HighlightList = new List<Rectangle>();
        public List<Label> PortBoxes = new List<Label>();
        public List<Label> AdminStateBoxes = new List<Label>();
        public List<Label> LinkStateBoxes = new List<Label>();
        public List<Label> DeviceTypeBoxes = new List<Label>();
        Funks F = new Funks();
        private delegate void ThreadDelegate();
        string a;
        string b;
        string c;
        bool firstrun = true;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(1))));
            Task taskA = Task.Factory.StartNew(() => TheMagic());
            taskA.Wait();
            ThreadDelegate updater = new ThreadDelegate(UpdateInterface);
            updater.BeginInvoke(null, null);

        }

        private void TheMagic()
        {
            if (firstrun == true)
            {
                ToMacList();
                ToAdminList();
                ToLinkList();
                ToDeviceList();
                ToHighlightList();
            }
            firstrun = false;
            foreach (PortState p in PortsList)
            {
                p.Clear();
            }
            Match macs;
            Match ports;
            string macpattern = "([0-9a-fA-F]{2}:[0-9a-fA-F]{2}:[0-9a-fA-F]{2}:[0-9a-fA-F]{2}:[0-9a-fA-F]{2}:[0-9a-fA-F]{2}|ge-0/0/\\d+)";
            string portpattern = "(ge-0/0/\\d+|(?<=ge-0/0/\\d+\\s+)up|(?<=ge-0/0/\\d+\\s+)down|(?<=ge-0/0/\\d+\\s+up\\s+)up|(?<=ge-0/0/\\d+\\s+down\\s+)down|(?<=ge-0/0/\\d+\\s+up\\s+)down)";
            ports = Regex.Match(portstateinput, portpattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(5));
            Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(10))));
            while (ports.Success)
            {
                foreach (PortState port in PortsList)
                {
                    port.Port = ports.Groups[0].Value;
                    ports = ports.NextMatch();
                    port.AdminState = ports.Groups[0].Value;
                    ports = ports.NextMatch();
                    port.LinkState = ports.Groups[0].Value;
                    ports = ports.NextMatch();
                }
            }
            Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(20))));
            macs = Regex.Match(macaddressinput, macpattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(5));
            while (macs.Success)
            {
                a = macs.Groups[0].Value;
                macs = macs.NextMatch();
                b = macs.Groups[0].Value;
                macs = macs.NextMatch();
                foreach (PortState port in PortsList)
                {
                    if (port.Port == b)
                    {
                        port.Mac = a; // set a mac address
                        port.DeviceType = ConvertOuiToName(port.Mac);
                        break;
                    }
                }
            }
            Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(30))));
            
        }

        

        async private void UpdateInterface()
        {
            int searchindex = 0;
            foreach (Label portbox in PortBoxes)
            {
                await portbox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => portbox.Content = PortsList[searchindex].Port)));
                //portbox.Content = PortsList[searchindex].Port;
                searchindex++;
            }
            Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(40))));
            searchindex = 0;
            foreach (Label macbox in MacAddressBoxes)
            {
                await macbox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => macbox.Content = PortsList[searchindex].Mac)));
                //macbox.Content = PortsList[searchindex].Mac;
                searchindex++;
            }
            Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(50))));
            searchindex = 0;
            foreach (Label adminbox in AdminStateBoxes)
            {
                await adminbox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => adminbox.Content = PortsList[searchindex].AdminState)));
                //adminbox.Content = PortsList[searchindex].AdminState;
                searchindex++;
            }
            Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(60))));
            searchindex = 0;
            foreach (Label linkbox in LinkStateBoxes)
            {
                await linkbox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => linkbox.Content = PortsList[searchindex].LinkState)));
                //linkbox.Content = PortsList[searchindex].LinkState;
                searchindex++;
            }
            Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(70))));
            searchindex = 0;
            foreach (Label devicelabel in DeviceTypeBoxes)
            {
                if (searchindex == 0)
                {
                    //device.Content = "";
                    await devicelabel.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => devicelabel.Content = "")));
                }
                try
                {
                    PortsList[searchindex].DeviceType = Funks.conversiondictionary[PortsList[searchindex].DeviceType];
                    string a = Funks.conversiondictionary[PortsList[searchindex].DeviceType];
                    await devicelabel.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => devicelabel.Content=a)));
                    //device.Content = Funks.conversiondictionary[PortsList[searchindex].DeviceType];
                }
                catch (KeyNotFoundException)
                {
                    await devicelabel.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => devicelabel.Content = PortsList[searchindex].DeviceType)));
                    //device.Content = PortsList[searchindex].DeviceType;
                }
                searchindex++;
            }
            Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(80))));
            searchindex = 0;
            foreach (Rectangle rect in HighlightList)
            {
                if (searchindex == 2)
                {
                    if (PortsList[searchindex].DeviceType != "?")
                        if (PortsList[searchindex].DeviceType != "")
                        {
                            PortsList[searchindex].DeviceType = "Aruba AP";
                        }
                }

                if (searchindex == 8)
                {
                    if (PortsList[searchindex].DeviceType != "?")
                        if (PortsList[searchindex].DeviceType != "")
                        {
                            {
                                PortsList[searchindex].DeviceType = "Back Office PC";
                                await DeviceTypeBoxes[8].Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => DeviceTypeBoxes[8].Content = "Back Office PC")));
                            }
                        }
                }
                if (searchindex == 28)
                {
                    if (PortsList[searchindex].DeviceType != "?")
                        if (PortsList[searchindex].DeviceType != "")
                        {
                            {
                                PortsList[searchindex].DeviceType = "Alarm Panel";
                                await DeviceTypeBoxes[28].Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => DeviceTypeBoxes[28].Content = "Alarm Panel")));
                            }
                        }
                }
                if (searchindex == 29)
                {
                    if (PortsList[searchindex].DeviceType != "?")
                        if (PortsList[searchindex].DeviceType != "")
                        {
                            {
                                PortsList[searchindex].DeviceType = "DVR";
                                await DeviceTypeBoxes[29].Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => DeviceTypeBoxes[29].Content = "DVR")));
                            }
                        }
                }

                if (PortsList[searchindex].DeviceType == Funks.validationdictionary[searchindex])
                {
                    if (PortsList[searchindex].LinkState == "up")
                    {
                        await rect.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => rect.Fill = Brushes.LightGreen)));
                        //rect.Fill = Brushes.LightGreen;
                        searchindex++;
                        continue;
                    }
                }
                if (PortsList[searchindex].AdminState == "up") //Implies device was invalid
                {
                    await rect.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => rect.Fill = Brushes.Salmon)));
                    //rect.Fill = Brushes.Salmon;
                    searchindex++;
                    continue;
                }

                if (PortsList[searchindex].AdminState == "down")
                {
                    if (Funks.validationdictionary[searchindex] == "")
                    {
                        await rect.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => rect.Fill = Brushes.Yellow)));
                        //rect.Fill = Brushes.Yellow;
                        searchindex++;
                        continue;
                    }
                    if (searchindex == 48)
                    {
                        await rect.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => rect.Fill = Brushes.LightGreen)));
                        //rect.Fill = Brushes.LightGreen;
                        searchindex++;
                        continue;
                    }
                    searchindex++;
                }
            }
            Progbar.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadDelegate(new Action(() => UpdateProgressbar(0))));
        }


        async private void UpdateProgressbar(int val)
        {
            Progbar.Value = val;
        }

        private void PortInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            portstateinput = PortInput.Text;

        }

        private void MacAddressInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            macaddressinput = MacAddressInput.Text;
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
                //oui = ouiDictionary.TryGetValue("f8:62:14", out oui);
                oui = Funks.ouiDictionary[oui];
            }
            catch (KeyNotFoundException e)
            {
                //MessageBox.Show("Could not find OUI for " + Convert.ToString(oui));
                oui = "?";
            }
            return oui;
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
            MacAddressBoxes.Add(MacLabel49);
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
            AdminStateBoxes.Add(AdminStateLabel49);
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
            LinkStateBoxes.Add(LinkStateLabel49);
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
            DeviceTypeBoxes.Add(DeviceTypeLabel49);
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
            HighlightList.Add(Highlight49);
        }


    }
}


