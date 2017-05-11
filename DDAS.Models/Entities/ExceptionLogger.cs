using System;

namespace DDAS.Models.Entities
{
    public class ExceptionLogger
    {
        public string Address { get; set; }
        public string UserId { get; set; }
        public string Request { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime AddedOn { get; set; }
        public Guid? RecId { get; set; }
        public long Id { get; set; }
    }
}
