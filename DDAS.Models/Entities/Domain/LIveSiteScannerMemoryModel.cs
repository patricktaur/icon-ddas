using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain
{
    public class LIveSiteScannerMemoryModel
    {
        public int NumberOfProcesses { get; set; }
        public long TotalCurrentPhysicalMemory { get; set; }
        public TimeSpan TotalProcessorTime { get; set; }
        public long TotalVirtualMemory { get; set; }
        public string TotalProcessorTimeValue { get { return TotalProcessorTime.ToString(); } }

    }

    public class LiveSiteScannerProcessModel
    {
        public int ProcessId { get; set; }
        public DateTime StartTime { get; set; }
        public bool Responding { get; set; }
        public int QueueNumber { get; set; }
        public long CurrentPhysicalMemory { get; set; }
        public TimeSpan ProcessorTime { get; set; }
        public long VirtualMemory { get; set; }
        public string ProcessorTimeValue { get { return ProcessorTime.ToString(); } }

    }
}
