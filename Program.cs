using System;
using System.Configuration.Install;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;
using Microsoft.ApplicationInsights.Extensibility;

namespace ApplicationInsightsProbeForWindows
{

    static class Program
    {

        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);    // Doens't solve or detect the UAC issue in fact :-)
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static int Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                TelemetryConfiguration.Active.DisableTelemetry = true;
                if (args.Length == 1 && !string.IsNullOrWhiteSpace(args[0]))
                    switch (args[0].ToLowerInvariant())
                    {
                        case "i":
                        case "/i":
                        case "-i":
                        case "install":
                        case "/install":
                        case "-install":

                            if (IsAdministrator())
                            {
                                if (!string.IsNullOrWhiteSpace(TelemetryConfiguration.Active.InstrumentationKey) && TelemetryConfiguration.Active.InstrumentationKey != "Your Key")
                                {
                                    bool notExist;
                                    try
                                    {
                                        ServiceController sc = new ServiceController(AIProbeService.NtName);
                                        notExist = sc.ServiceName == null; // dummy for crash if not exist
                                    }
                                    catch
                                    {
                                        notExist = true;
                                    }
                                    if (notExist)
                                    {
                                        Console.WriteLine("Installing " + AIProbeService.NtName);
                                        ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                                        return 0;
                                    }
                                    else
                                    {
                                        Console.WriteLine("ERROR : Uninstall first.");
                                        return 1;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("ERROR : Fill the <InstrumentationKey> of ApplicationInsights.config with your Azure/AI/Properties/INSTRUMENTATION KEY.");
                                    return 1;
                                }
                            }
                            else
                            {
                                Console.WriteLine("ERROR : Open console with admin right.");
                                return 1;
                            }

                        case "u":
                        case "/u":
                        case "-u":
                        case "uninstall":
                        case "/uninstall":
                        case "-uninstall":

                            if (IsAdministrator())
                            {
                                bool exist;
                                try
                                {
                                    ServiceController sc = new ServiceController(AIProbeService.NtName);
                                    exist = sc.ServiceName != null; // dummy for crash if not exist
                                }
                                catch
                                {
                                    exist = false;
                                }
                                if (exist)
                                {
                                    Console.WriteLine("Uninstalling " + AIProbeService.NtName);
                                    ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                                    return 0;
                                }
                                else
                                {
                                    Console.WriteLine("ERROR : install first.");
                                    return 1;
                                }
                            }
                            else
                            {
                                Console.WriteLine("ERROR : Open console with admin right.");
                                return 1;
                            }

                        case "s":
                        case "/s":
                        case "-s":
                        case "standalone":
                        case "/standalone":
                        case "-standalone":

                            Console.WriteLine("Running " + AIProbeService.NtName);
                            if (!string.IsNullOrWhiteSpace(TelemetryConfiguration.Active.InstrumentationKey) && TelemetryConfiguration.Active.InstrumentationKey != "Your Key")
                            {
                                TelemetryConfiguration.Active.DisableTelemetry = false;
                                Console.WriteLine("Press ENTER to exit");
                                while (Console.ReadKey(true).Key != ConsoleKey.Enter) ; // loop
                                return 0;
                            }
                            else
                            {
                                Console.WriteLine("ERROR : Fill the <InstrumentationKey> of ApplicationInsights.config with your Azure/AI/Properties/INSTRUMENTATION KEY.");
                                return 1;
                            }

                    }

                Console.WriteLine("USAGE as administrator : /install /uninstall /standalone");
                return 1;
            }
            else // mode service
            {
                ServiceBase.Run(new ServiceBase[] { new AIProbeService() });
                return 0;
            }
        }

    }
}
