using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.LiveSiteExtractionService
{
    public partial class Service1 : ServiceBase
    {
        private Services.Search.LiveScan _LiveScan;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _LiveScan.StartLiveScan();
        }

        protected override void OnStop()
        {
            _LiveScan.StopLiveScan();
        }
    }
}
