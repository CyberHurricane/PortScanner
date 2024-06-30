using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using System.Text.RegularExpressions;
using System.Threading;

namespace TCPPortScanner
{

    internal class InputValidation
    {
        internal static bool RegExValid(string input)
        {
            Regex regex = new Regex(@"\b(?:(?:2(?:[0-4][0-9]|5[0-5])|[0-1]?[0-9]?[0-9])\.){3}(?:(?:2([0-4][0-9]|5[0-5])|[0-1]?[0-9]?[0-9]))\b"); // Detects valid IPV4 Layout
            return regex.IsMatch(input);
        }
        internal static bool RegExNotAllowed(string inputout)
        {
            Regex regex = new Regex(@"\b255\b|\b255\.\d{1,3}\.\d{1,3}\.\d{1,3}\b|\b\d{1,3}\.255\.\d{1,3}\.\d{1,3}\b|\b\d{1,3}\.\d{1,3}\.255\.\d{1,3}\b|\b\d{1,3}\.\d{1,3}\.\d{1,3}\.255\b"); // Detects the digit 255 in an IP
            return regex.IsMatch(inputout);
        }
        internal static bool RegExInvalid(string inputout)
        {
            Regex regex = new Regex(@"^0|\.0$"); // Check to see if an IP starts with 0 or ends with 0
            return regex.IsMatch(inputout);
        }
        internal static void StartPortValidation(int startPort, string IP)
        {

            if (startPort <= 0)
            {
                Console.WriteLine("Error: Enter a number greater than 0");
                TCPPortScanner.PortRange(IP); // Restart function if input is invalid
            }
            return;
        }
        internal static void EndPortValidation(int startPort, int endPort, string IP)
        {
            if (endPort <= startPort)
            {
                Console.WriteLine("Error: End Port must be greater than Start Port");
                TCPPortScanner.PortRange(IP); // Restart function if input is invalid
            }
            bool V = true;
            TCPPortScanner.CreatePortRangeArray(startPort, endPort, IP, V);
            //
        }
    } //Dedicated purely for input validation
    internal class CommonPorts
    {
        internal int[] DPorts = {
    21, 22, 25, 80, 110, 135, 143, 261, 271, 324,
    443, 448, 465, 563, 614, 631, 636, 664, 684, 695,
    832, 853, 854, 990, 993, 989, 992, 994, 995, 1129,
    1131, 1184, 2083, 2087, 2089, 2096, 2221, 2252, 2376, 2381,
    2478, 2479, 2482, 2484, 2679, 2762, 3077, 3078, 3183, 3191,
    3220, 3269, 3306, 3410, 3424, 3471, 3496, 3509, 3529, 3539,
    3535, 3660, 36611, 3713, 3747, 3766, 3864, 3885, 3995, 3896,
    4031, 4036, 4062, 4064, 4081, 4083, 4116, 4335, 4336, 4536,
    4590, 4740, 4843, 4849, 5443, 5007, 5061, 5321, 5349, 5671,
    5783, 5868, 5986, 5989, 5990, 6209, 6251, 6443, 6513, 6514,
    6619, 6697, 6771, 7202, 7443, 7673, 7674, 7677, 7775, 8243,
    8443, 8991, 8989, 9089, 9295, 9318, 9443, 9444, 9614, 9802,
    10161, 10162, 11751, 12013, 12109, 14143, 15002, 16995, 41230,
    16993, 20003};
        public int[] UPorts { get; private set; }
        internal void UserPortArrayScan(int[] ports)
        {
            UPorts = ports;
        }
    }
    internal class TCPPortScanner
    {
           private static List<int> openPorts = new List<int>();  // List to store open ports
           private static List<int> closedPorts = new List<int>(); // List to store closed ports

        private static void Main()
        {
             UserIPInput();
             Console.ReadLine();
        }
        private static void UserIPInput()
        {
            string IP;
            Console.WriteLine("Enter IP:");
            IP = Console.ReadLine();
            bool ValidIPCheckFail = InputValidation.RegExValid(IP);
            bool InvalidIPCheck = InputValidation.RegExNotAllowed(IP);
            bool InvalidIPZeroCheck = InputValidation.RegExInvalid(IP);
            if (ValidIPCheckFail == false || InvalidIPCheck == true || InvalidIPZeroCheck == true)
            {
                Console.WriteLine("Error Please Make Sure to Enter a Valid IP");
                IP = "";
                Main();
            }
            PortRange(IP);
        }
        internal static void PortRange(string IP)
        {
            bool V = false;
            Console.WriteLine("Would You like to Scan the Commonly Used Ports? \n Y/N?");
            string LowerCommonports = Console.ReadLine();
            string Commonports = LowerCommonports.ToUpper();
            if (Commonports == "Y")
            {
                int emptyvalue1 = 0;
                int emptyvalue2 = 0;
                              
                CreatePortRangeArray(emptyvalue1, emptyvalue2, IP, V);
            }   
                else if (Commonports == "N")
            {
            }
                else
                {
                Console.WriteLine("Please Enter a valid response");
                PortRange(IP);
                }         
            Console.WriteLine("Enter Start Port:");
            int startPort = Convert.ToInt32(Console.ReadLine());
                InputValidation.StartPortValidation(startPort, IP);
            Console.WriteLine("Enter End Port:");
            int endPort = Convert.ToInt32(Console.ReadLine());
                InputValidation.EndPortValidation(startPort, endPort, IP);
        }
        internal static void CreatePortRangeArray(int startPort, int endPort, string IP, bool V)
        {         
            if (V == true)
            {
                CommonPorts CommonPortsInstance = new CommonPorts();

                int[] UserDefinedPorts = new int[endPort - startPort + 1];
                for (int i = 0; startPort <= endPort; i++, startPort++)
                {
                    UserDefinedPorts[i] = startPort;
                }

                CommonPortsInstance.UserPortArrayScan(UserDefinedPorts);
                ThreadManager(CommonPortsInstance.UPorts, IP);
            }
            CommonPorts PreDefinedPorts = new CommonPorts();         
            for (int i = 0; startPort <= endPort; i++, startPort++)
            {
                ThreadManager(PreDefinedPorts.DPorts, IP);
            }

        }
        internal static void ThreadManager(int[] ports, string IP)
        {
            //int threadcount = 10; // Change the amount of threads to use
            Console.WriteLine("How many threads do you want to use? \n CAUTION ***values over 100 may cause stability issues***");
            string P = Console.ReadLine();
            int threadcount = Convert.ToInt32(P);
            if (threadcount <= 0)
            {
                ThreadManager(ports, IP);
            }       
                Thread[] threads = new Thread[threadcount];
            int PortsPerThread = ports.Length / threadcount;
            int RemainingPorts = ports.Length % threadcount;

            for (int i = 0; i < threadcount; i++)
            {
                int start = i * PortsPerThread;
                int end = (i == threadcount - 1) ? (i + 1) * PortsPerThread + RemainingPorts : (i + 1) * PortsPerThread;

                int[] threadPorts = new int[end - start];
                Array.Copy(ports, start, threadPorts, 0, end - start);

                threads[i] = new Thread(() => LoopSend(threadPorts, IP));
                threads[i].Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join(); // Wait for all threads to complete
            }           
            Console.WriteLine("Complete");
            Thread.Sleep(new TimeSpan(0, 0, 1));
            ArraySortandPrint(IP);
        }
        internal static void LoopSend(int[] ports, string IP)
        {
            foreach (int port in ports)
            {
                SendPacket(IP, port);
            }
        }
        internal static void SendPacket(string IP, int port)
        {
            try
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    Console.WriteLine($"Sending Packets to {IP} On Port {port}");
                    tcpClient.Connect(IP, port);
                    AddToOpenPorts(port);
                }
            }
            catch (Exception)
            {
                AddToClosedPorts(port);
            }
        }
        private static void AddToOpenPorts(int port)
        {
            lock (openPorts)  
            {
                openPorts.Add(port);
            }
        }
        private static void AddToClosedPorts(int port)
        {
            lock (closedPorts)
            {
                closedPorts.Add(port);
            }
        }        
        private static void ArraySortandPrint(string IP)
        {
            Console.Clear();
            Console.WriteLine(IP);
            openPorts.Sort();
            Console.WriteLine("\nOpen Ports:");
            foreach (var port in openPorts)
            {
                Console.WriteLine(port);
            }

            closedPorts.Sort();
            Console.WriteLine("\nClosed Ports:");
            foreach (var port in closedPorts)
            {
                Console.WriteLine(port);
                
            }
            return;
        }
    }
}
