using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DDAS.Services.LiveScan
{
    public class LiveScanLauncher
    {

        public bool LaunchLiveScanner(string exeFolder, int QueueNumber)
        {

            var fileName = exeFolder + @"\DDAS.LiveSiteExtractor.exe";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = fileName;
           
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            
            //startInfo.Arguments = "-q  " + QueueNumber;
            startInfo.Arguments = "" + QueueNumber;

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit(1000);
                }
                
                return true;
            }
            catch (Exception)
            {
                // Log error.

                return false;
            }
        }

    }
}
