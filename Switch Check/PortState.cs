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
        public PortState() { }
        public PortState(List<string> Expecteddevicesus, List<string> Expecteddevicesca, int portnumber = 0, string port = "", string mac = "", string adminstate = "", string linkstate = "", string device = "", string ipaddress = "", string iplastoctet = "", int isvalid = 0)
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
            US = Expecteddevicesus;
            CA = Expecteddevicesca;
        }
        public int PortNumber { get; set; }
        public string Port { get; set; }
        public string Mac { get; set; }
        public string AdminState { get; set; }
        public string LinkState { get; set; }
        private List<string> Expecteddevicesus { get; set; }
        private List<string> Expecteddevicesca { get; set; }
        public List<string> US
        {
            get
            {
                return Expecteddevicesus;
            }
            set
            {
                Expecteddevicesus = value;
            }
        }
        public List<string> CA
        {
            get
            {
                return Expecteddevicesca;
            }
            set
            {
                Expecteddevicesca = value;
            }
        }

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
                    IsValid = 4; // port admin down
                    return IsValid;
                }
                //==============================================================
                
                if (AdminState == "up" && LinkState == "down")
                {
                    IsValid = 0;// link is up down
                    return IsValid;
                }
                //==============================================================USA
                if (CanadaStore == false)
                        foreach (string device in US)
                        if (DeviceType == device)
                        if (Funks.ipvalidationdictionary1[PortNumber] == IpLastOctet || Funks.ipvalidationdictionary2[PortNumber] == IpLastOctet)
                        {
                            IsValid = 1; //correct device and ip
                            return IsValid;
                        }
                if (CanadaStore == true)
                    foreach (string device in CA)
                    if (DeviceType == device)
                    if (Funks.ipvalidationdictionary1[PortNumber] == IpLastOctet || Funks.ipvalidationdictionary2[PortNumber] == IpLastOctet)
                        {
                            IsValid = 1; //correct device and ip
                            return IsValid;
                        }
                if (CanadaStore == false)
                    foreach (string device in US)
                        if (DeviceType == device)
                        if (Funks.ipvalidationdictionary1[PortNumber] != IpLastOctet && Funks.ipvalidationdictionary2[PortNumber] != IpLastOctet)
                        {
                            IsValid = 3; // wrong ip address for expected device
                            return IsValid;
                        }
                if (CanadaStore == true)
                    foreach (string device in CA)
                        if (DeviceType == device)
                        if (Funks.ipvalidationdictionary1[PortNumber] != IpLastOctet && Funks.ipvalidationdictionary2[PortNumber] != IpLastOctet)
                        {
                            IsValid = 3; // wrong ip address for expected device
                            return IsValid;
                        }
                IsValid = 2;
                return IsValid; //wrong device or ip
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
        /// ===CANADA===
       
        private static List<string> p0c  = new List<string>() { "Juniper Networks" };
        private static List<string> p1c  = new List<string>() { "Juniper Networks" };
        private static List<string> p2c  = new List<string>() { "Aruba Networks", "Apple, Inc." };
        private static List<string> p3c  = new List<string>() { "ARRIS Group, Inc." , "Hitron Technologies. Inc" };
        private static List<string> p4c  = new List<string>() { "Apple, Inc." };
        private static List<string> p5c  = new List<string>() { "none" };
        private static List<string> p6c  = new List<string>() { "none" };
        private static List<string> p7c  = new List<string>() { "none" };
        private static List<string> p8c  = new List<string>() { "Hewlett Packard", "IBM Corporation" };
        private static List<string> p9c  = new List<string>() { "none" };
        private static List<string> p10c = new List<string>() { "Hewlett Packard", "IBM Corporation" };
        private static List<string> p11c = new List<string>() { "Hewlett Packard", "IBM Corporation" };
        private static List<string> p12c = new List<string>() { "Hewlett Packard", "IBM Corporation" };
        private static List<string> p13c = new List<string>() { "Hewlett Packard", "IBM Corporation" };
        private static List<string> p14c = new List<string>() { "Seiko Epson Corporation" };
        private static List<string> p15c = new List<string>() { "Seiko Epson Corporation" };
        private static List<string> p16c = new List<string>() { "Seiko Epson Corporation" };
        private static List<string> p17c = new List<string>() { "Seiko Epson Corporation" };
        private static List<string> p18c = new List<string>() { "DMP Electronics INC." };
        private static List<string> p19c = new List<string>() { "None" };
        private static List<string> p20c = new List<string>() { "None" };
        private static List<string> p21c = new List<string>() { "None" };
        private static List<string> p22c = new List<string>() { "None" };
        private static List<string> p23c = new List<string>() { "None" };
        private static List<string> p24c = new List<string>() { "Cup Labeler" };
        private static List<string> p25c = new List<string>() { "Cup Labeler" };
        private static List<string> p26c = new List<string>() { "None" };
        private static List<string> p27c = new List<string>() { "None" };
        private static List<string> p28c = new List<string>() { "Alarm Panel" };
        private static List<string> p29c = new List<string>() { "CompuLab, Ltd." };
        private static List<string> p30c = new List<string>() { "AXIS COMMUNICATIONS AB", "Axis Communications AB" };
        private static List<string> p31c = new List<string>() { "AXIS COMMUNICATIONS AB", "Axis Communications AB" };
        private static List<string> p32c = new List<string>() { "AXIS COMMUNICATIONS AB", "Axis Communications AB" };
        private static List<string> p33c = new List<string>() { "AXIS COMMUNICATIONS AB", "Axis Communications AB" };
        private static List<string> p34c = new List<string>() { "AXIS COMMUNICATIONS AB", "Axis Communications AB" };
        private static List<string> p35c = new List<string>() { "Drive Thru Timer" };
        private static List<string> p36c = new List<string>() { "Drive Thru Timer" };
        private static List<string> p37c = new List<string>() { "EMS, Surveyor" };
        private static List<string> p38c = new List<string>() { "None" };
        private static List<string> p39c = new List<string>() { "None" };
        private static List<string> p40c = new List<string>() { "None" };
        private static List<string> p41c = new List<string>() { "DigiBoard" };
        private static List<string> p42c = new List<string>() { "None" };
        private static List<string> p43c = new List<string>() { "None" };
        private static List<string> p44c = new List<string>() { "None" };
        private static List<string> p45c = new List<string>() { "None" };
        private static List<string> p46c = new List<string>() { "MINIX Technology Limited" };
        private static List<string> p47c = new List<string>() { "Hewlett Packard" };
        /// ===CANADA===
        /// ===USA===
        private static List<string> p0a = new List<string>() { "Juniper Networks", "CradlePoint" };
        private static List<string> p1a = new List<string>() { "Juniper Networks"};
        private static List<string> p2a = new List<string>() { "Aruba Networks", "Apple, Inc." };
        private static List<string> p3a = new List<string>() { "ARRIS Group, Inc." , "Hitron Technologies. Inc" };
        private static List<string> p4a = new List<string>() { "Apple, Inc."};
        private static List<string> p5a = new List<string>() { "none" };
        private static List<string> p6a = new List<string>() { "none" };
        private static List<string> p7a = new List<string>() { "none" };
        private static List<string> p8a = new List<string>() { "Hewlett Packard", "IBM Corporation" };
        private static List<string> p9a = new List<string>() { "none" };
        private static List<string> p10a = new List<string>(){ "Hewlett Packard", "IBM Corporation" };
        private static List<string> p11a = new List<string>(){ "Hewlett Packard", "IBM Corporation" };
        private static List<string> p12a = new List<string>(){ "Hewlett Packard", "IBM Corporation" };
        private static List<string> p13a = new List<string>(){ "Hewlett Packard", "IBM Corporation" };
        private static List<string> p14a = new List<string>(){ "Seiko Epson Corporation" };
        private static List<string> p15a = new List<string>(){ "Seiko Epson Corporation" };
        private static List<string> p16a = new List<string>(){ "Seiko Epson Corporation" };
        private static List<string> p17a = new List<string>(){ "Seiko Epson Corporation" };
        private static List<string> p18a = new List<string>(){ "DMP Electronics INC." };
        private static List<string> p19a = new List<string>(){ "None" };
        private static List<string> p20a = new List<string>(){ "None" };
        private static List<string> p21a = new List<string>(){ "None" };
        private static List<string> p22a = new List<string>(){ "None" };
        private static List<string> p23a = new List<string>(){ "None" };
        private static List<string> p24a = new List<string>(){ "Cup Labeler" };
        private static List<string> p25a = new List<string>(){ "Cup Labeler" };
        private static List<string> p26a = new List<string>(){ "None" };
        private static List<string> p27a = new List<string>(){ "None" };
        private static List<string> p28a = new List<string>(){ "Alarm Panel" };
        private static List<string> p29a = new List<string>(){ "CompuLab, Ltd." };
        private static List<string> p30a = new List<string>(){ "AXIS COMMUNICATIONS AB", "Axis Communications AB" };
        private static List<string> p31a = new List<string>(){ "AXIS COMMUNICATIONS AB", "Axis Communications AB" };
        private static List<string> p32a = new List<string>(){ "AXIS COMMUNICATIONS AB", "Axis Communications AB" };
        private static List<string> p33a = new List<string>(){ "AXIS COMMUNICATIONS AB", "Axis Communications AB" };
        private static List<string> p34a = new List<string>(){ "AXIS COMMUNICATIONS AB", "Axis Communications AB" };
        private static List<string> p35a = new List<string>(){ "Drive Thru Timer", "HM Electronics, Inc." };
        private static List<string> p36a = new List<string>(){ "Drive Thru Timer", "Jetway Information Co., Ltd." };
        private static List<string> p37a = new List<string>(){ "EMS, Surveyor", "Venstar Inc." };
        private static List<string> p38a = new List<string>(){ "None" };
        private static List<string> p39a = new List<string>(){ "None" };
        private static List<string> p40a = new List<string>(){ "None" };
        private static List<string> p41a = new List<string>(){ "DigiBoard" };
        private static List<string> p42a = new List<string>(){ "None" };
        private static List<string> p43a = new List<string>(){ "None" };
        private static List<string> p44a = new List<string>(){ "None" };
        private static List<string> p45a = new List<string>(){ "None" };
        private static List<string> p46a = new List<string>(){ "MINIX Technology Limited", "Hewlett Packard" };
        private static List<string> p47a = new List<string>(){ "MINIX Technology Limited", "Hewlett Packard" };
        /// ===USA===


        public static PortState g0 = new PortState(  p0a,  p0c);
        public static PortState g1 = new PortState(  p1a,  p1c);
        public static PortState g2 = new PortState(  p2a,  p2c);
        public static PortState g3 = new PortState(  p3a,  p3c);
        public static PortState g4 = new PortState(  p4a,  p4c);
        public static PortState g5 = new PortState(  p5a,  p5c);
        public static PortState g6 = new PortState(  p6a,  p6c);
        public static PortState g7 = new PortState(  p7a,  p7c);
        public static PortState g8 = new PortState(  p8a,  p8c);
        public static PortState g9 = new PortState(  p9a,  p9c);
        public static PortState g10 = new PortState(p10a, p10c);
        public static PortState g11 = new PortState(p11a, p11c);
        public static PortState g12 = new PortState(p12a, p12c);
        public static PortState g13 = new PortState(p13a, p13c);
        public static PortState g14 = new PortState(p14a, p14c);
        public static PortState g15 = new PortState(p15a, p15c);
        public static PortState g16 = new PortState(p16a, p16c);
        public static PortState g17 = new PortState(p17a, p17c);
        public static PortState g18 = new PortState(p18a, p18c);
        public static PortState g19 = new PortState(p19a, p19c);
        public static PortState g20 = new PortState(p20a, p20c);
        public static PortState g21 = new PortState(p21a, p21c);
        public static PortState g22 = new PortState(p22a, p22c);
        public static PortState g23 = new PortState(p23a, p23c);
        public static PortState g24 = new PortState(p24a, p24c);
        public static PortState g25 = new PortState(p25a, p25c);
        public static PortState g26 = new PortState(p26a, p26c);
        public static PortState g27 = new PortState(p27a, p27c);
        public static PortState g28 = new PortState(p28a, p28c);
        public static PortState g29 = new PortState(p29a, p29c);
        public static PortState g30 = new PortState(p30a, p30c);
        public static PortState g31 = new PortState(p31a, p31c);
        public static PortState g32 = new PortState(p32a, p32c);
        public static PortState g33 = new PortState(p33a, p33c);
        public static PortState g34 = new PortState(p34a, p34c);
        public static PortState g35 = new PortState(p35a, p35c);
        public static PortState g36 = new PortState(p36a, p36c);
        public static PortState g37 = new PortState(p37a, p37c);
        public static PortState g38 = new PortState(p38a, p38c);
        public static PortState g39 = new PortState(p39a, p39c);
        public static PortState g40 = new PortState(p40a, p40c);
        public static PortState g41 = new PortState(p41a, p41c);
        public static PortState g42 = new PortState(p42a, p42c);
        public static PortState g43 = new PortState(p43a, p43c);
        public static PortState g44 = new PortState(p44a, p44c);
        public static PortState g45 = new PortState(p45a, p45c);
        public static PortState g46 = new PortState(p46a, p46c);
        public static PortState g47 = new PortState(p47a, p47c);



        //0       "Juniper Networks"},
        //1       "Juniper Networks"},
        //2       "Apple, Inc."},
        //3       "Hitron Technologies. Inc"},
        //4       "None"},
        //5       "None"},
        //6       "None"},
        //7       "None"},
        //8       "test"},
        //9       "None"},
        //10      "IBM Corporation"},
        //11      "IBM Corporation"},
        //12      "IBM Corporation"},
        //13      "IBM Corporation"},
        //14      "Seiko Epson Corporation"},
        //15      "Seiko Epson Corporation"},
        //16      "Seiko Epson Corporation"},
        //17      "Seiko Epson Corporation"},
        //18      "DMP Electronics INC."},
        //19      "None"},
        //20      "None"},
        //21      "None"},
        //22      "None"},
        //23      "None"},
        //24      "Cup Labeler"},
        //25      "Cup Labeler"},
        //26      "None"},
        //27      "None"},
        //28      "Alarm Panel"},
        //29      "CompuLab, Ltd."},
        //30      "AXIS COMMUNICATIONS AB"},
        //31      "AXIS COMMUNICATIONS AB"},
        //32      "AXIS COMMUNICATIONS AB"},
        //33      "AXIS COMMUNICATIONS AB"},
        //34      "AXIS COMMUNICATIONS AB"},
        //35      "Drive Thru Timer"},
        //36      "Drive Thru Timer"},
        //37      "EMS, Surveyor"},
        //38      "None"},
        //39      "None"},
        //40      "None"},
        //41      "DigiBoard"},
        //42      "None"},
        //43      "None"},
        //44      "None"},
        //45      "None"},
        //46      "MINIX Technology Limited"},
        //47      "Hewlett Packard"},

        public static List<PortState> PortsList = new List<PortState>() {
        PortState.g0,         PortState.g1,         PortState.g2,        PortState.g3,         PortState.g4,         PortState.g5,         PortState.g6,         PortState.g7,         PortState.g8,         PortState.g9,         PortState.g10,        PortState.g11,        PortState.g12,        PortState.g13,        PortState.g14,        PortState.g15,        PortState.g16,
        PortState.g17,        PortState.g18,        PortState.g19,       PortState.g20,        PortState.g21,        PortState.g22,        PortState.g23,        PortState.g24,        PortState.g25,        PortState.g26,        PortState.g27,        PortState.g28,        PortState.g29,        PortState.g30,        PortState.g31,
        PortState.g32,        PortState.g33,        PortState.g34,       PortState.g35,        PortState.g36,        PortState.g37,        PortState.g38,        PortState.g39,        PortState.g40,        PortState.g41,        PortState.g42,        PortState.g43,        PortState.g44,        PortState.g45,        PortState.g46,        PortState.g47
        };
    }
}
