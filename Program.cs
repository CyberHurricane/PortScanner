using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace TCPPortScanner
{
    internal class TCPPortScanner
    {
        static List<int> openPorts = new List<int>();  // List to store open ports
        static List<int> closedPorts = new List<int>(); // List to store closed ports

        static void Main(string[] args)
        {
            string IP;
            Console.WriteLine("Enter IP:");
            IP = Console.ReadLine();
            PortRange(IP);
            Console.ReadLine();
        }
        static void PortRange(string IP)
        {
            Console.WriteLine("Enter Start Port:");
            int startPort = Convert.ToInt32(Console.ReadLine());

            if (startPort <= 0)
            {
                Console.WriteLine("Error: Enter a number greater than 0");
                PortRange(IP); // Restart function if input is invalid
                return;
            }

            Console.WriteLine("Enter End Port:");
            int endPort = Convert.ToInt32(Console.ReadLine());

            if (endPort <= startPort)
            {
                Console.WriteLine("Error: End Port must be greater than Start Port");
                PortRange(IP); // Restart function if input is invalid
                return;
            }
            ThreadManager(startPort, endPort, IP);
        }
        static void ThreadManager(int startPort, int endPort, string IP)
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
        static void LoopSend(int startPort, int endPort, string IP)
        {
            for (int testPort = startPort; testPort <= endPort; testPort += 10)
            {
                SendPacket(IP, testPort);
            }
        }
        static void SendPacket(string IP, int port)
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
        static void AddToOpenPorts(int port)
        {
            lock (openPorts)  
            {
                openPorts.Add(port);
            }
        }
        static void AddToClosedPorts(int port)
        {
            lock (closedPorts)
            {
                closedPorts.Add(port);

            }
        }
    }
}
