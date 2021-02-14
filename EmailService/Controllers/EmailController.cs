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
            public string displayName;
            public string[] distantions;
            public string emailUserAdress;
            public string emailPass;
            public string smtpDomain;
            public string fileName;
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
        public void Post([FromBody]Email emailParms)
        {
            SendAlertEmail(emailParms);
        }

        // PUT: api/Email/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Email/5
        public void Delete(int id)
        {
        }

        private void SendAlertEmail(Email email)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(email.smtpDomain ?? "smtp.gmail.com");

                mail.From = new MailAddress(email.emailUserAdress, email.displayName ?? email.emailUserAdress);

                if (email.distantions.Length < 1)
                {
                    mail.To.Add(Error_mail);
                }
                else
                {
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
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
