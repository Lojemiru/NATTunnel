using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace NATTunnel
{
    public class NodeOptions
    {
        public bool isServer = false;
        public string endpoint = "serverhost.address.example.com:26702";
        public IPEndPoint mediationIP = new IPEndPoint(IPAddress.Parse("150.136.166.80"), 6510);
        public string remoteIP = "127.0.0.1";
        public List<IPEndPoint> endpoints = new List<IPEndPoint>();
        public int localPort = 0;
        public int mediationClientPort = 5000;
        public int uploadSpeed = 512;
        public int downloadSpeed = 512;
        public int minRetransmitTime = 100;
        public int masterServerID = 0;
        public int masterServerSecret = 0;

        public NodeOptions()
        {
            //Make sure masterServerID is random
            Random r = new Random();
            masterServerID = r.Next();
            masterServerSecret = r.Next();
        }

        public bool Load(StreamReader sr)
        {
            string currentLine;
            while ((currentLine = sr.ReadLine()) != null)
            {
                int splitIndex = currentLine.IndexOf("=");
                if (splitIndex <= 0) continue;

                string lhs = currentLine.Substring(0, splitIndex);
                string rhs = currentLine.Substring(splitIndex + 1);
                switch (lhs)
                {
                    case "mode":
                        isServer = rhs == "server";
                        break;
                    case "endpoint":
                        endpoint = rhs;
                        ResolveAddress();
                        break;
                    case "mediationIP":
                        mediationIP = IPEndPoint.Parse(rhs);
                        break;
                    case "remoteIP":
                        remoteIP = rhs;
                        break;
                    case "localPort":
                        localPort = Int32.Parse(rhs);
                        break;
                    case "mediationClientPort":
                        mediationClientPort = Int32.Parse(rhs);
                        break;
                    case "uploadSpeed":
                        uploadSpeed = Int32.Parse(rhs);
                        break;
                    case "downloadSpeed":
                        downloadSpeed = Int32.Parse(rhs);
                        break;
                    case "minRetransmitTime":
                        minRetransmitTime = Int32.Parse(rhs);
                        break;
                    case "masterServerID":
                        masterServerID = Int32.Parse(rhs);
                        break;
                    case "masterServerSecret":
                        masterServerSecret = Int32.Parse(rhs);
                        break;
                }
            }
            return true;
        }

        public void Save(StreamWriter sw)
        {
            sw.WriteLine("#mode: Set to server if you want to host a local server over UDP, client if you want to connect to a server over UDP");
            sw.WriteLine(isServer ? "mode=server" : "mode=client");
            sw.WriteLine();
            sw.WriteLine("#endpoint, servers: The TCP server to connect to for forwarding over UDP. Client: The UDP server to connect to (not used when masterServerID is set)");
            sw.WriteLine($"endpoint={endpoint}");
            sw.WriteLine();
            sw.WriteLine("#mediationIP: The public IP and port of the mediation server you want to connect to.");
            sw.WriteLine($"mediationIP={mediationIP.ToString()}");
            sw.WriteLine();
            sw.WriteLine("#remoteIP, clients: The public IP of the peer you want to connect to.");
            sw.WriteLine($"remoteIP={remoteIP}");
            sw.WriteLine();
            sw.WriteLine("#localPort: servers: The UDP server port. client: The TCP port to host the forwarded server on.");
            sw.WriteLine($"localPort={localPort}");
            sw.WriteLine();
            sw.WriteLine("#mediationClientPort: The UDP mediation client port. This is the port that will have a hole punched through the NAT by the mediation server, and all traffic will pass through it.");
            sw.WriteLine($"mediationClientPort={mediationClientPort}");
            sw.WriteLine();
            sw.WriteLine("#uploadSpeed/downloadSpeed: Specify your connection limit (kB/s), this program sends at a fixed rate.");
            sw.WriteLine($"uploadSpeed={uploadSpeed}");
            sw.WriteLine($"downloadSpeed={downloadSpeed}");
            sw.WriteLine();
            sw.WriteLine("#minRetransmitTime: How many milliseconds delay to send unacknowledged packets");
            sw.WriteLine($"minRetransmitTime={minRetransmitTime}");
            sw.WriteLine();
            sw.WriteLine("#masterServerID: Automatically register (server mode) or connect (client mode)");
            sw.WriteLine("#masterServerSecret: Do not change this or you will have to change your server ID (server mode only)");
            sw.WriteLine($"masterServerID={masterServerID}");
            sw.WriteLine($"masterServerSecret={masterServerSecret}");
        }

        private void ResolveAddress()
        {
            endpoints.Clear();
            int splitIndex = endpoint.LastIndexOf(":");
            string lhs = endpoint.Substring(0, splitIndex);
            string rhs = endpoint.Substring(splitIndex + 1);
            int port = Int32.Parse(rhs);
            if (IPAddress.TryParse(lhs, out IPAddress addr))
            {
                endpoints.Add(new IPEndPoint(addr, port));
            }
            else
            {
                foreach (IPAddress addr2 in Dns.GetHostAddresses(lhs))
                {
                    endpoints.Add(new IPEndPoint(addr2, port));
                }
            }
        }
    }
}