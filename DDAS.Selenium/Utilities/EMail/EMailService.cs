using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.EMail
{
    public class EMailService: IEMailService
    {
        private EMailServerCredentialsModel _emailServerCredentials;
        private SmtpClient _smtpClient;
        private string _fromEMailId;

        public EMailService(EMailServerCredentialsModel cred)
        {
            _emailServerCredentials = cred;
             _smtpClient = new SmtpClient(cred.EMailHost,  cred.EMailPort);
            _smtpClient.UseDefaultCredentials = false;
            _smtpClient.Credentials = new System.Net.NetworkCredential(cred.FromEMailId, cred.FromEMailPassword);
            
            _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            _smtpClient.EnableSsl = cred.SSLEnabled;
            _fromEMailId = cred.FromEMailId;
        }

     
        public bool SendMail( EMailModel email)
        {
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true; 
            mail.From = new MailAddress(_fromEMailId);
            foreach (string to in email.To)
            {
                mail.To.Add(new MailAddress(to));
            }
            foreach (string cc in email.CC)
            {
                mail.CC.Add(new MailAddress(cc));
            }
            mail.Subject = email.Subject;
            mail.Body = email.Body;

            _smtpClient.Send(mail);
            return true;
        }
    }

    public class EMailServerCredentialsModel
    {
        public string EMailHost { get; set; }
        public int EMailPort { get; set; }
        public string FromEMailId { get; set; }
        public string FromEMailPassword { get; set; }
        public bool SSLEnabled { get; set; }
    }

    public class EMailModel
    {
        public List<string> To { get; set; } = new List<string>();
        public List<string>  CC { get; set; } = new List<string>();
        public string Subject { get; set; }
        public string Body { get; set; }

    }

    public interface IEMailService
    {
        bool SendMail(EMailModel email);
        
    }
}
