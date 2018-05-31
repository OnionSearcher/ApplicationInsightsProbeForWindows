using System;
using System.Diagnostics;
using System.ServiceProcess;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace ApplicationInsightsProbeForWindows
{
    public partial class AIProbeService : ServiceBase
    {

        internal static string NtName
        {
            get
            {
                return typeof(AIProbeService).Name;
            }
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                Trace.TraceError("CurrentDomain_UnhandledException : " + ex.GetBaseException().ToString());
                TelemetryClient ai = new TelemetryClient();
                ai.TrackException(ex);
            }
            else
            {
                Trace.TraceError("CurrentDomain_UnhandledException : NULL");
            }
        }

        public AIProbeService()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            InitializeComponent();
            this.ServiceName = NtName; // overide but in fact the same value
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
        }


        protected override void OnPause()
        {
            TelemetryConfiguration.Active.DisableTelemetry = true;

            base.OnPause();
        }

        protected override void OnContinue()
        {
            TelemetryConfiguration.Active.DisableTelemetry = false;

            base.OnContinue();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

    }
}
