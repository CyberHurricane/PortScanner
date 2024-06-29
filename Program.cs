using System;
using System.Collections.Generic;
using System.Net.Sockets;
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
            Regex regex = new Regex(@"^0|\.0"); // Check to see if an IP starts with 0 or ends with 0
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
            TCPPortScanner.ThreadManager(startPort, endPort, IP); //calls the next method for further processing
        }
    } //Dedicated purely for input validation
    
    internal class TCPPortScanner
    {
            static List<int> openPorts = new List<int>();  // List to store open ports
            static List<int> closedPorts = new List<int>(); // List to store closed ports

        private static void Main()
        {
             string IP = UserIPInput();
             PortRange(IP);
             Console.ReadLine();
        }
        private static string UserIPInput()
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
            return IP;
        }
        public static void PortRange(string IP)
        {
            Console.WriteLine("Enter Start Port:");
            int startPort = Convert.ToInt32(Console.ReadLine());
            InputValidation.StartPortValidation(startPort, IP);

            Console.WriteLine("Enter End Port:");
            int endPort = Convert.ToInt32(Console.ReadLine());
            InputValidation.EndPortValidation(startPort, endPort, IP);
        }
        public static void ThreadManager(int startPort, int endPort, string IP)
        {
            Thread[] threads = new Thread[10];
            for (int i = 0; i < 10; i++)
            {
                int threadStartPort = startPort + i;
                threads[i] = new Thread(() => LoopSend(threadStartPort, endPort, IP));
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
        private static void LoopSend(int startPort, int endPort, string IP)
        {
            for (int testPort = startPort; testPort <= endPort; testPort += 10)
            {
                SendPacket(IP, testPort);
            }
        }
        private static void SendPacket(string IP, int port)
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
        }
    }
}
