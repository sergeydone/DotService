using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

namespace PingService
{
    public class PingTarget
    {
        public string PingAddress { get; set; }
        public int PingTimerSec { get; set; }
        public bool IsActive { get; set; }
    }

    public class TcpTarget
    {
        public string TcpHost { get; set; }
        public int TcpPort { get; set; }
        public int TcpTimerSec { get; set; }
        public bool IsActive { get; set; }
    }

    public class AliveTarget
    {
        public string AliveText { get; set; }
        public int AliveTimerSec { get; set; }
        public bool IsActive { get; set; }
    }

    public class ServiceConfig
    {
        public List<PingTarget> PingTargets { get; set; }
        public List<TcpTarget> TcpTargets { get; set; }
        public List<AliveTarget> AliveTargets { get; set; }

        public ServiceConfig()
        {
            PingTargets = new List<PingTarget>();
            TcpTargets = new List<TcpTarget>();
            AliveTargets = new List<AliveTarget>();
        }

        public static ServiceConfig Load(string path)
        {
            var config = new ServiceConfig();

            try
            {
                if (!File.Exists(path))
                {
                    // Return default configuration
                    config.PingTargets.Add(new PingTarget
                    {
                        PingAddress = "8.8.8.8",
                        PingTimerSec = 60,
                        IsActive = true
                    });

                    config.TcpTargets.Add(new TcpTarget
                    {
                        TcpHost = "google.com",
                        TcpPort = 80,
                        TcpTimerSec = 60,
                        IsActive = true
                    });

                    config.AliveTargets.Add(new AliveTarget
                    {
                        AliveText = "ALIVE",
                        AliveTimerSec = 60,
                        IsActive = true
                    });

                    return config;
                }

                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                // Load Ping targets
                var pingNodes = doc.SelectNodes("//Config/Ping/Target");
                if (pingNodes != null)
                {
                    foreach (XmlNode node in pingNodes)
                    {
                        var target = new PingTarget();
                        
                        var addressNode = node.SelectSingleNode("PingAddress");
                        if (addressNode != null && !string.IsNullOrEmpty(addressNode.InnerText.Trim()))
                            target.PingAddress = addressNode.InnerText;

                        var timerNode = node.SelectSingleNode("PingTimerSec");
                        if (timerNode != null)
                        {
                            int pingTimer;
                            if (int.TryParse(timerNode.InnerText, out pingTimer))
                                target.PingTimerSec = pingTimer;
                            else
                                target.PingTimerSec = 60; // default
                        }
                        else
                            target.PingTimerSec = 60; // default

                        var activeNode = node.SelectSingleNode("IsActive");
                        if (activeNode != null)
                        {
                            bool pingActive;
                            if (bool.TryParse(activeNode.InnerText, out pingActive))
                                target.IsActive = pingActive;
                            else
                                target.IsActive = true; // default
                        }
                        else
                            target.IsActive = true; // default

                        if (!string.IsNullOrEmpty(target.PingAddress))
                            config.PingTargets.Add(target);
                    }
                }

                // Load TCP targets
                var tcpNodes = doc.SelectNodes("//Config/TCP/Target");
                if (tcpNodes != null)
                {
                    foreach (XmlNode node in tcpNodes)
                    {
                        var target = new TcpTarget();

                        var hostNode = node.SelectSingleNode("TcpHost");
                        if (hostNode != null && !string.IsNullOrEmpty(hostNode.InnerText.Trim()))
                            target.TcpHost = hostNode.InnerText;

                        var portNode = node.SelectSingleNode("TcpPort");
                        if (portNode != null)
                        {
                            int tcpPort;
                            if (int.TryParse(portNode.InnerText, out tcpPort))
                                target.TcpPort = tcpPort;
                            else
                                target.TcpPort = 80; // default
                        }
                        else
                            target.TcpPort = 80; // default

                        var timerNode = node.SelectSingleNode("TcpTimerSec");
                        if (timerNode != null)
                        {
                            int tcpTimer;
                            if (int.TryParse(timerNode.InnerText, out tcpTimer))
                                target.TcpTimerSec = tcpTimer;
                            else
                                target.TcpTimerSec = 60; // default
                        }
                        else
                            target.TcpTimerSec = 60; // default

                        var activeNode = node.SelectSingleNode("IsActive");
                        if (activeNode != null)
                        {
                            bool tcpActive;
                            if (bool.TryParse(activeNode.InnerText, out tcpActive))
                                target.IsActive = tcpActive;
                            else
                                target.IsActive = true; // default
                        }
                        else
                            target.IsActive = true; // default

                        if (!string.IsNullOrEmpty(target.TcpHost))
                            config.TcpTargets.Add(target);
                    }
                }

                // Load Alive targets
                var aliveNodes = doc.SelectNodes("//Config/Alive/Target");
                if (aliveNodes != null)
                {
                    foreach (XmlNode node in aliveNodes)
                    {
                        var target = new AliveTarget();

                        var textNode = node.SelectSingleNode("AliveText");
                        if (textNode != null && !string.IsNullOrEmpty(textNode.InnerText.Trim()))
                            target.AliveText = textNode.InnerText;
                        else
                            target.AliveText = "ALIVE"; // default

                        var timerNode = node.SelectSingleNode("AliveTimerSec");
                        if (timerNode != null)
                        {
                            int aliveTimer;
                            if (int.TryParse(timerNode.InnerText, out aliveTimer))
                                target.AliveTimerSec = aliveTimer;
                            else
                                target.AliveTimerSec = 60; // default
                        }
                        else
                            target.AliveTimerSec = 60; // default

                        var activeNode = node.SelectSingleNode("IsActive");
                        if (activeNode != null)
                        {
                            bool aliveActive;
                            if (bool.TryParse(activeNode.InnerText, out aliveActive))
                                target.IsActive = aliveActive;
                            else
                                target.IsActive = true; // default
                        }
                        else
                            target.IsActive = true; // default

                        config.AliveTargets.Add(target);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
                // File.AppendAllText(@"C:\Logs\ConfigError.txt", string.Format("{0}: {1}\r\n", DateTime.Now, ex.Message));
            }

            return config;
        }
    }

    public class Service1 : ServiceBase
    {
        private ServiceConfig config;
        private List<System.Threading.Timer> pingTimers;
        private List<Thread> tcpThreads;
        private List<Thread> aliveThreads;
        private bool stopRequested = false;
        private string logDirectory = @"C:\Logs";

        public Service1()
        {
            this.ServiceName = "PingService";
            pingTimers = new List<System.Threading.Timer>();
            tcpThreads = new List<Thread>();
            aliveThreads = new List<Thread>();
        }

        protected override void OnStart(string[] args)
        {
            stopRequested = false;

            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string configPath = Path.Combine(exePath, "PingService.config.xml");
            config = ServiceConfig.Load(configPath);

            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            // Start ping timers for active targets
            foreach (var target in config.PingTargets)
            {
                if (target.IsActive)
                {
                    var timer = new System.Threading.Timer(
                        new TimerCallback(DoPing),
                        target,
                        0,
                        target.PingTimerSec * 1000
                    );
                    pingTimers.Add(timer);
                }
            }

            // Start TCP threads for active targets
            foreach (var target in config.TcpTargets)
            {
                if (target.IsActive)
                {
                    var thread = new Thread(new ParameterizedThreadStart(CheckTcpLoop));
                    thread.IsBackground = true;
                    thread.Start(target);
                    tcpThreads.Add(thread);
                }
            }

            // Start Alive threads for active targets
            foreach (var target in config.AliveTargets)
            {
                if (target.IsActive)
                {
                    var thread = new Thread(new ParameterizedThreadStart(LogAliveStatus));
                    thread.IsBackground = true;
                    thread.Start(target);
                    aliveThreads.Add(thread);
                }
            }
        }

        protected override void OnStop()
        {
            stopRequested = true;

            // Dispose ping timers
            foreach (var timer in pingTimers)
            {
                if (timer != null)
                    timer.Dispose();
            }
            pingTimers.Clear();

            // Stop TCP threads
            foreach (var thread in tcpThreads)
            {
                if (thread != null && thread.IsAlive)
                {
                    if (!thread.Join(5000))
                    {
                        thread.Abort(); // .NET 4.0
                    }
                }
            }
            tcpThreads.Clear();

            // Stop Alive threads
            foreach (var thread in aliveThreads)
            {
                if (thread != null && thread.IsAlive)
                {
                    if (!thread.Join(5000))
                    {
                        thread.Abort(); // .NET 4.0
                    }
                }
            }
            aliveThreads.Clear();
        }

        private void DoPing(object state)
        {
            var target = (PingTarget)state;
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send(target.PingAddress, 4000);

                    string result = string.Format("{0}: PING {1} : {2}",
                        DateTime.Now, target.PingAddress, reply.Status);

                    Log(result);
                }
            }
            catch (Exception ex)
            {
                Log(string.Format("{0}: Ping error for {1} - {2}",
                    DateTime.Now, target.PingAddress, ex.Message));
            }
        }

        private void CheckTcpLoop(object state)
        {
            var target = (TcpTarget)state;
            while (!stopRequested)
            {
                try
                {
                    using (var client = new TcpClient())
                    {
                        var result = client.BeginConnect(target.TcpHost, target.TcpPort, null, null);
                        bool connected = false;

                        try
                        {
                            if (result.AsyncWaitHandle.WaitOne(1000, false))
                            {
                                client.EndConnect(result);
                                connected = true;
                            }
                        }
                        catch (Exception)
                        {
                            connected = false;
                        }

                        string log = string.Format("{0}: TCP {1}:{2} : {3}",
                            DateTime.Now, target.TcpHost, target.TcpPort, connected ? "SUCCESS" : "FAILED");
                        Log(log);
                    }
                }
                catch (Exception ex)
                {
                    Log(string.Format("{0}: TCP error for {1}:{2} - {3}",
                        DateTime.Now, target.TcpHost, target.TcpPort, ex.Message));
                }

                Thread.Sleep(target.TcpTimerSec * 1000);
            }
        }

        private void LogAliveStatus(object state)
        {
            var target = (AliveTarget)state;
            while (!stopRequested)
            {
                try
                {
                    string result = string.Format("{0}: ALIVE - {1}",
                        DateTime.Now, target.AliveText);

                    Log(result);

                    Thread.Sleep(target.AliveTimerSec * 1000);
                }
                catch (Exception ex)
                {
                    Log(string.Format("{0}: Alive error - {1}",
                        DateTime.Now, ex.Message));
                }
            }
        }

        private void Log(string message)
        {
            try
            {
                string fileName = string.Format("{0}_{1}_{2}.txt",
                    Environment.MachineName,
                    DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                    Guid.NewGuid().ToString().Substring(0, 4));

                string filePath = Path.Combine(logDirectory, fileName);

                File.WriteAllText(filePath, message, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                // Optionally log to event log or another location
            }
        }
    }
}