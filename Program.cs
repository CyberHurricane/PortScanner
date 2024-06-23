using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace TCPPortScanner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Thread mainThread = Thread.CurrentThread;
            Stager();
            Console.ReadLine();
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
            Thread thread1 = new Thread(() => LoopSend(SPort, EPort, IP));
            Thread thread2 = new Thread(() => LoopSend2(SPort, EPort, IP));
            Thread thread3 = new Thread(() => LoopSend3(SPort, EPort, IP));
            Thread thread4 = new Thread(() => LoopSend4(SPort, EPort, IP));
            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();
        }
        static void LoopSend(int portstart, int portend, string IP)
        {
            for (int testport = portstart + 1; testport <= portend; testport = testport + 4)
            {
                SendPacket(IP, testport);
            }

        }
        static void LoopSend2(int portstart, int portend, string IP)
        {
            Thread.Sleep(300);
            for (int testport = portstart + 2; testport <= portend; testport = testport + 4)
            {
                SendPacket(IP, testport);
            }
        }
        static void LoopSend3(int portstart, int portend, string IP)
        {
            Thread.Sleep(600);
            for (int testport = portstart + 3; testport <= portend; testport = testport + 4)
            {
                SendPacket(IP, testport);
            }
        }
        static void LoopSend4(int portstart, int portend, string IP)
        {
            Thread.Sleep(900);
            for (int testport = portstart + 4; testport <= portend; testport = testport + 4)
            {
                SendPacket(IP, testport);
            }
        }
        static void SendPacket(string IP, int port)
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.Connect(IP, port);                   
                    Console.WriteLine($"Port {port} open");
                }
                catch (Exception)
                {
                    Console.WriteLine($"Port {port} closed");
                }
            }
        }


    }
}