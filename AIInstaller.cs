using System.ComponentModel;
using System.ServiceProcess;

namespace ApplicationInsightsProbeForWindows
{
    [RunInstaller(true)]
    public class AIInstaller : System.Configuration.Install.Installer
    {

        public AIInstaller()
        {
            Installers.Add(new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem,
            });

            Installers.Add(new ServiceInstaller
            {
                ServiceName = AIProbeService.NtName,
                DisplayName = AIProbeService.NtName,
                StartType = ServiceStartMode.Automatic,
                DelayedAutoStart = true,
                Description = "ApplicationInsights Probe for Windows",
            });

            this.AfterInstall += ServiceInstaller_AfterInstall;
        }

        private void ServiceInstaller_AfterInstall(object sender, System.Configuration.Install.InstallEventArgs e)
        {
            ServiceController sc = new ServiceController(AIProbeService.NtName);
            sc.Start();
        }

    }
}
