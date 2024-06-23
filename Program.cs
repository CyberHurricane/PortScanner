using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace TCPPortScanner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Thread mainThread = Thread.CurrentThread;
            Stager();           
        }
        static void Stager()
        {
            string IP;
            Console.WriteLine("Enter IP");
            IP = Console.ReadLine();
            PortRange(IP);
        }
        static void PortRange(string IP)
        {
            string StartPort;
            string EndPort;

            Console.WriteLine("Enter Start Port");
            StartPort = Console.ReadLine();
            if (StartPort == "0")
            {
                Console.WriteLine("Error, Enter a number greater than 1");
                Stager();
            }
            int SPort = (int)Convert.ToInt64(StartPort);
            Console.WriteLine("Enter End Port");
            EndPort = Console.ReadLine();
            int EPort = (int)Convert.ToInt64(EndPort);
            if (SPort >= EPort)
            {
                Console.WriteLine("Error, Enter a number greater than Start Port");
                Stager();
            }
            if (EPort == 0)
            {
                Console.WriteLine("Error, Enter a number greater than 1");
                Stager();
            }
            ThreadManager(SPort, EPort, IP);

        }
        static void ThreadManager(int SPort, int EPort, string IP)
        {
            Thread thread0 = new Thread(() => LoopSend0(SPort, EPort, IP));
            Thread thread1 = new Thread(() => LoopSend1(SPort, EPort, IP));
            Thread thread2 = new Thread(() => LoopSend2(SPort, EPort, IP));
            Thread thread3 = new Thread(() => LoopSend3(SPort, EPort, IP));
            Thread thread4 = new Thread(() => LoopSend4(SPort, EPort, IP));
            thread0.Start();
            thread0.IsBackground = true;
            thread1.Start();
            thread1.IsBackground = true;
            thread2.Start();
            thread2.IsBackground = true;
            thread3.Start();
            thread3.IsBackground = true;
            thread4.Start();
            thread4.IsBackground = true;
        }
        static void LoopSend0(int portstart, int portend, string IP)
        {
            for (int testport = portstart; testport <= portend; testport = testport + 5)
            {
                SendPacket(IP, testport);
            }

        }
        static void LoopSend1(int portstart, int portend, string IP)
        {
            for (int testport = portstart + 1; testport <= portend; testport = testport + 5)
            {
                SendPacket(IP, testport);
            }

        }
        static void LoopSend2(int portstart, int portend, string IP)
        {
            Thread.Sleep(600);
            for (int testport = portstart + 2; testport <= portend; testport = testport + 5)
            {
                SendPacket(IP, testport);
            }
        }
        static void LoopSend3(int portstart, int portend, string IP)
        {
            Thread.Sleep(900);
            for (int testport = portstart + 3; testport <= portend; testport = testport + 5)
            {
                SendPacket(IP, testport);
            }
        }
        static void LoopSend4(int portstart, int portend, string IP)
        {
            Thread.Sleep(1200);
            for (int testport = portstart + 4; testport <= portend; testport = testport + 5)
            {
                SendPacket(IP, testport);
            }
        }
        static void SendPacket(string IP, int port)
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                bool connection;

                try
                {
                    tcpClient.Connect(IP, port);
                    Console.WriteLine($"Port {port} open");
                    connection = true;
                    List(connection, port);

                    tcpClient.Close();

                }
                catch (Exception)
                {
                    Console.WriteLine($"Port {port} closed");
                    connection = false;
                    List(connection, port);
                }             
            }
        }
        static void List(bool connection, int port)
        {

            int i = 0;
            if (0 == i)
            {
                i++;
            }
            if (connection == true)
            {
               
            }
            else
            {
                
            }


        }

    }
}