using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

[RunInstaller(true)]
public class ProjectInstaller : Installer
{
    public ProjectInstaller()
    {
        ServiceProcessInstaller processInstaller = new ServiceProcessInstaller();
        ServiceInstaller serviceInstaller = new ServiceInstaller();

        processInstaller.Account = ServiceAccount.LocalSystem;
        serviceInstaller.ServiceName = "PingService";
        serviceInstaller.DisplayName = "Ping Service";
        serviceInstaller.StartType = ServiceStartMode.Automatic;

        Installers.Add(processInstaller);
        Installers.Add(serviceInstaller);
    }
}
