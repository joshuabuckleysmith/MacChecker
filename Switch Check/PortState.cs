using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Switch_Check
{
    public class PortState : MainWindow
    {
        public static bool CanadaStore;
        public PortState(
            ) { }
        public PortState(int portnumber, string port, string mac, string adminstate, string linkstate, string device, string ipaddress, string iplastoctet, int isvalid)
        {
            PortNumber = portnumber;
            Port = port;
            Mac = mac;
            AdminState = adminstate;
            LinkState = linkstate;
            DeviceType = device;
            IpAddress = ipaddress;
            IpLastOctet = iplastoctet;
            IsValid = isvalid;
        }
        public int PortNumber { get; set; }
        public string Port { get; set; }
        public string Mac { get; set; }
        public string AdminState { get; set; }
        public string LinkState { get; set; }
        private string devicetype;
        public string DeviceType {
            get
            {
                if (AdminState == "down")
                {
                    return "";
                }
                return devicetype;
            }
            set
            {
                devicetype = value;
            }
            }

        public string IpAddress { get; set; }
        public string IpLastOctet { get; set; }
        private int IsValid;
        public int CheckIsValid
        {
            get
            {
                //{ 1, "Port is OK" },
                //{ 2, "IP address is wrong" },
                //{ 3, "Device type is unknown" },
                //{ 4, "Link is up down" },
                //{ 5, "Port is down down" },
                //{ 0, "Port is not OK" }
                //==============================================================
                if (AdminState == "down")
                {
                    IsValid = 5; // port admin down
                    return IsValid;
                }
                //==============================================================
                if (AdminState == "up" && LinkState == "down")
                {
                    IsValid = 4;// link is up down
                    return IsValid;
                }
                //==============================================================USA
                if (CanadaStore == false){
                    
                    if (DeviceType != Funks.validationdictionaryusa0[PortNumber] && DeviceType != Funks.validationdictionaryusa1[PortNumber])
                    
                        {
                            IsValid = 3; // possible wrong device
                            return IsValid;
                        }
                }
                //==============================================================USA
                if (CanadaStore == false)
                    if (DeviceType == Funks.validationdictionaryusa0[PortNumber] || DeviceType == Funks.validationdictionaryusa1[PortNumber])
                    {
                        {
                            IsValid = 1; //correct device and ip
                            return IsValid;
                        }
                    }
                //==============================================================CA
                if (CanadaStore == true)
                    
                if (DeviceType != Funks.validationdictionaryca0[PortNumber] && DeviceType != Funks.validationdictionaryca1[PortNumber])
                    {
                        {
                            IsValid = 3; // possible wrong device
                            return IsValid;
                        }
                    }
                //==============================================================CA
                if (CanadaStore == true)
                    if (DeviceType == Funks.validationdictionaryca0[PortNumber] || DeviceType == Funks.validationdictionaryca1[PortNumber])
                    {
                        {
                            IsValid = 1; //correct device and ip
                            return IsValid;
                        }
                    }



                    //==============================================================

                        IsValid = 0;
                        return IsValid;
            }
            set { IsValid = value; }
        }

        public void Clear()
        {
            PortNumber = 0;
            Port = "";
            Mac = "";
            AdminState = "";
            LinkState = "";
            DeviceType = "";
            IpAddress = "";
            IsValid = 0;
        }

        public static PortState g0 = new PortState();
        public static PortState g1 = new PortState();
        public static PortState g2 = new PortState();
        public static PortState g3 = new PortState();
        public static PortState g4 = new PortState();
        public static PortState g5 = new PortState();
        public static PortState g6 = new PortState();
        public static PortState g7 = new PortState();
        public static PortState g8 = new PortState();
        public static PortState g9 = new PortState();
        public static PortState g10 = new PortState();
        public static PortState g11 = new PortState();
        public static PortState g12 = new PortState();
        public static PortState g13 = new PortState();
        public static PortState g14 = new PortState();
        public static PortState g15 = new PortState();
        public static PortState g16 = new PortState();
        public static PortState g17 = new PortState();
        public static PortState g18 = new PortState();
        public static PortState g19 = new PortState();
        public static PortState g20 = new PortState();
        public static PortState g21 = new PortState();
        public static PortState g22 = new PortState();
        public static PortState g23 = new PortState();
        public static PortState g24 = new PortState();
        public static PortState g25 = new PortState();
        public static PortState g26 = new PortState();
        public static PortState g27 = new PortState();
        public static PortState g28 = new PortState();
        public static PortState g29 = new PortState();
        public static PortState g30 = new PortState();
        public static PortState g31 = new PortState();
        public static PortState g32 = new PortState();
        public static PortState g33 = new PortState();
        public static PortState g34 = new PortState();
        public static PortState g35 = new PortState();
        public static PortState g36 = new PortState();
        public static PortState g37 = new PortState();
        public static PortState g38 = new PortState();
        public static PortState g39 = new PortState();
        public static PortState g40 = new PortState();
        public static PortState g41 = new PortState();
        public static PortState g42 = new PortState();
        public static PortState g43 = new PortState();
        public static PortState g44 = new PortState();
        public static PortState g45 = new PortState();
        public static PortState g46 = new PortState();
        public static PortState g47 = new PortState();
        public static PortState g48 = new PortState();
        public static PortState g49 = new PortState();
    }
}
