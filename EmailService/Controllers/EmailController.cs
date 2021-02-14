using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Web.Http;

namespace EmailService.Controllers
{
    public class EmailController : ApiController
    {
        String Error_mail = "TODO CHANGE";
        public struct Email
        {
            public string subject;
            public string body;
            public string[] distantions;
            public string emailUserAdress;
            public string emailPass;
            public string displayName; //optinal defualt same as emailUserAdress
            public string smtpDomain; //optinal  defualt is Gmail
            //public string fileName;
        }
        // GET: api/Email
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Email/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Email
        public string Post([FromBody]Email emailParms)
        {
            string res = SendAlertEmail(emailParms);
            return res;
        }

        // PUT: api/Email/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Email/5
        public void Delete(int id)
        {
        }

        private string SendAlertEmail(Email email)
        {
            List<string> respone = new List<string>();
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(email.smtpDomain ?? "smtp.gmail.com");

                mail.From = new MailAddress(email.emailUserAdress, email.displayName ?? email.emailUserAdress);

                if (email.distantions.Length < 1)
                {
                    respone.Add("\"Error Msg\":\"No distantion\"");
                }
                else
                {
                    string errorInMailAddres = string.Empty;
                    foreach (string dis in email.distantions)
                    {
                        if (dis != string.Empty)
                        {
                            mail.To.Add(dis);
                        }
                    }
                }
                mail.Subject = email.subject;
                mail.IsBodyHtml = true;
                mail.Body = email.body;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(email.emailUserAdress, email.emailPass);
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
                Console.WriteLine("mail sent");
            }
            catch (Exception ex)
            {
                respone.Add($"\"Exception Msg\":\"{ex.Message}\"");
            }
            return respone.Count >=1? "{\"OK\":false," +     String.Join(",\n",respone)   +"}":"{\"OK\":\"OK\"}";
        }

    }
}
