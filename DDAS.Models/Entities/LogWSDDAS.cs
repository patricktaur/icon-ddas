using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities
{
    [Serializable]
    public class LogWSDDAS
    {
        public LogWSDDAS()
        {
           
        }

        public Guid? RecId { get; set; }
        public DateTime CreatedOn { get; set; }

        public string RequestPayload { get; set; }
        public string Response { get; set; }
        public string Status { get; set; }

        public string LocalIPAddress { get; set; }
        public string HostIPAddress { get; set; }
        public string PortNumber { get; set; }
        public string ServerProtocol { get; set; }
        public string ServerSoftware { get; set; }
        public string HttpHost { get; set; }
        public string ServerName { get; set; }
        public string GatewayInterface { get; set; }
        public string Https { get; set; }
    }
}
