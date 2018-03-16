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

namespace Switch_Check
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }
        public class PortState
        {
            public PortState(){}
            public PortState(string port, string mac, string adminstate, string linkstate)
            {
                Port = port;
                Mac = mac;
                AdminState = adminstate;
                LinkState = linkstate;
            }
            public string Port { get; set; }
            public string Mac { get; set; }
            public string AdminState { get; set; }
            public string LinkState { get; set; }
        }
        
        string macaddressinput;
        string portstateinput;
        //"([0-9a-fA-F]{2}:[0-9a-fA-F]{2}:[0-9a-fA-F]{2}:[0-9a-fA-F]{2}:[0-9a-fA-F]{2}:[0-9a-fA-F]{2}|ge-0/0/\d+)";
        string[] lines;
        string[] ports;
        char[] delimiters = new char[] { '\r', '\n' , ' '};
        PortState g0 = new PortState();
        PortState g1 = new PortState();
        PortState g2 = new PortState();
        PortState g3 = new PortState();
        PortState g4 = new PortState();
        PortState g5 = new PortState();
        PortState g6 = new PortState();
        PortState g7 = new PortState();
        PortState g8 = new PortState();
        PortState g9 = new PortState();
        PortState g10 = new PortState();
        PortState g11 = new PortState();
        PortState g12 = new PortState();
        PortState g13 = new PortState();
        PortState g14 = new PortState();
        PortState g15 = new PortState();
        PortState g16 = new PortState();
        PortState g17 = new PortState();
        PortState g18 = new PortState();
        PortState g19 = new PortState();
        PortState g20 = new PortState();
        PortState g21 = new PortState();
        PortState g22 = new PortState();
        PortState g23 = new PortState();
        PortState g24 = new PortState();
        PortState g25 = new PortState();
        PortState g26 = new PortState();
        PortState g27 = new PortState();
        PortState g28 = new PortState();
        PortState g29 = new PortState();
        PortState g30 = new PortState();
        PortState g31 = new PortState();
        PortState g32 = new PortState();
        PortState g33 = new PortState();
        PortState g34 = new PortState();
        PortState g35 = new PortState();
        PortState g36 = new PortState();
        PortState g37 = new PortState();
        PortState g38 = new PortState();
        PortState g39 = new PortState();
        PortState g40 = new PortState();
        PortState g41 = new PortState();
        PortState g42 = new PortState();
        PortState g43 = new PortState();
        PortState g44 = new PortState();
        PortState g45 = new PortState();
        PortState g46 = new PortState();
        PortState g47 = new PortState();
        PortState g48 = new PortState();
        PortState g49 = new PortState();
        public void ToPortList()
        {
            PortsList.Add(g0);
            PortsList.Add(g1);
            PortsList.Add(g2);
            PortsList.Add(g3);
            PortsList.Add(g4);
            PortsList.Add(g5);
            PortsList.Add(g6);
            PortsList.Add(g7);
            PortsList.Add(g8);
            PortsList.Add(g9);
            PortsList.Add(g10);
            PortsList.Add(g11);
            PortsList.Add(g12);
            PortsList.Add(g13);
            PortsList.Add(g14);
            PortsList.Add(g15);
            PortsList.Add(g16);
            PortsList.Add(g17);
            PortsList.Add(g18);
            PortsList.Add(g19);
            PortsList.Add(g20);
            PortsList.Add(g21);
            PortsList.Add(g22);
            PortsList.Add(g23);
            PortsList.Add(g24);
            PortsList.Add(g25);
            PortsList.Add(g26);
            PortsList.Add(g27);
            PortsList.Add(g28);
            PortsList.Add(g29);
            PortsList.Add(g30);
            PortsList.Add(g31);
            PortsList.Add(g32);
            PortsList.Add(g33);
            PortsList.Add(g34);
            PortsList.Add(g35);
            PortsList.Add(g36);
            PortsList.Add(g37);
            PortsList.Add(g38);
            PortsList.Add(g39);
            PortsList.Add(g40);
            PortsList.Add(g41);
            PortsList.Add(g42);
            PortsList.Add(g43);
            PortsList.Add(g44);
            PortsList.Add(g45);
            PortsList.Add(g46);
            PortsList.Add(g47);
            PortsList.Add(g48);
            PortsList.Add(g49);
        }

        List<string> macaddresses = new List<string>();
        List<string> portsformacaddresses = new List<string>();
        List<string> portsfromadminupdownstate = new List<string>();
        List<string> adminstatesofports = new List<string>();
        List<string> linkstatesofports = new List<string>();
        List<PortState> PortsList = new List<PortState>();

        private delegate void ThreadDelegate();
        string a;
        string b;
        string c;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ToPortList();
            Match macs;
            Match ports;
            string macpattern = "([0-9a-fA-F]{2}:[0-9a-fA-F]{2}:[0-9a-fA-F]{2}:[0-9a-fA-F]{2}:[0-9a-fA-F]{2}:[0-9a-fA-F]{2}|ge-0/0/\\d+)";
            string portpattern = "(ge-0/0/\\d+|(?<=ge-0/0/\\d+\\s+)up|(?<=ge-0/0/\\d+\\s+)down|(?<=ge-0/0/\\d+\\s+up\\s+)up|(?<=ge-0/0/\\d+\\s+down\\s+)down|(?<=ge-0/0/\\d+\\s+up\\s+)down)";
            ports = Regex.Match(portstateinput, portpattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(5));
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
                        port.Mac = a; // get a mac address
                        break;
                    }
                }
            }
            foreach (PortState port in PortsList)
            {
                MacBox.AppendText(port.Mac+"\n");
                PortBox.AppendText(port.Port + "\n");
                AdminBox.AppendText(port.AdminState + "\n");
                LinkStateBox.AppendText(port.LinkState + "\n");
            }


            //foreach (PortState port in PortsList)
            //    {
            //        ports = Regex.Match(portstateinput, portpattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(5));
            //        while (ports.Success)
            //        {
            //        if (port.Port == ports.Groups[0].Value)
            //            {
            //                ports = ports.NextMatch();
            //                port.AdminState=ports.Groups[0].Value;
            //                ports = ports.NextMatch();
            //                port.LinkState = ports.Groups[0].Value;
            //                ports = ports.NextMatch();
            //                continue;
            //            }
            //        }  
            //    }
            //foreach (PortState port in PortsList)
            //    {
            //        MacBox.AppendText(port.Mac);
            //        PortBox.AppendText(port.Port);
            //        AdminBox.AppendText(port.AdminState);
            //        LinkStateBox.AppendText(port.LinkState);
            //    }

            //macaddresses.Add(macs.Groups[0].Value);
            //macs = macs.NextMatch();
            //portsformacaddresses.Add(macs.Groups[0].Value);
            //macs = macs.NextMatch();


            //foreach (String i in macaddresses)
            //{
            //    MacBox.AppendText(i+"\n");
            //}
            //foreach (String i in portsformacaddresses)
            //{
            //    PortBox.AppendText(i + "\n");
            //}
            //foreach (String i in portsfromadminupdownstate)
            //{
            //    PortFromAdminStateBox.AppendText(i + "\n");
            //}
            //foreach (String i in linkstatesofports)
            //{
            //    LinkStateBox.AppendText(i + "\n");
            //}
            //foreach (String i in adminstatesofports)
            //{
            //    AdminBox.AppendText(i + "\n");
            //}

        }



                //ports = portstateinput.Split(delimiters);
                //OutputBox.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new ThreadDelegate(new Action(() => ))

                //
                //            m = m.NextMatch();
                //        OutputBox.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new ThreadDelegate(new Action(() => PopulateOutputBox(m.Groups[0].Value + "\n"))));
                //        m = m.NextMatch();
                //        OutputBox.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new ThreadDelegate(new Action(() => PopulateOutputBox(m.Groups[1].Value + "\n"))));
                //        m = m.NextMatch();
                //        OutputBox.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new ThreadDelegate(new Action(() => PopulateOutputBox(m.Groups[0].Value + "\n"))));
                //        //OutputBox.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new ThreadDelegate(new Action(() => PopulateOutputBox(m.Groups[0].Value))));



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
            MacBox.AppendText(t);
        }

        private void PopulateLogBox(string t)
        {
            LogBox.AppendText(t);
        }

        private void OutputBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
   
        }
    }
}





//ge-0/0/0
//ge-0/0/1
//ge-0/0/2
//ge-0/0/3
//ge-0/0/4
//ge-0/0/5
//ge-0/0/6
//ge-0/0/7
//ge-0/0/8
//ge-0/0/9
//ge-0/0/10
//ge-0/0/11
//ge-0/0/12
//ge-0/0/13
//ge-0/0/14
//ge-0/0/15
//ge-0/0/16
//ge-0/0/17
//ge-0/0/18
//ge-0/0/19
//ge-0/0/20
//ge-0/0/21
//ge-0/0/22
//ge-0/0/23
//ge-0/0/24
//ge-0/0/25
//ge-0/0/26
//ge-0/0/27
//ge-0/0/28
//ge-0/0/29
//ge-0/0/30
//ge-0/0/31
//ge-0/0/32
//ge-0/0/33
//ge-0/0/34
//ge-0/0/35
//ge-0/0/36
//ge-0/0/37
//ge-0/0/38
//ge-0/0/39
//ge-0/0/40
//ge-0/0/41
//ge-0/0/42
//ge-0/0/43
//ge-0/0/44
//ge-0/0/45
//ge-0/0/46
//ge-0/0/47
                                                                                