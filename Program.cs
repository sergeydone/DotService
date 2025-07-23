using System;
using System.ServiceProcess;
using PingService;

static class Program
{
    static void Main()
    {
        ServiceBase[] ServicesToRun;
        ServicesToRun = new ServiceBase[]
        {
            new Service1()
        };
        ServiceBase.Run(ServicesToRun);
    }
}
