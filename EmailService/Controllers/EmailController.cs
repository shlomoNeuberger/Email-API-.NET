using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using System.Text.Json;
using System.Web.Mvc;

namespace EmailService.Controllers
{
    public class EmailController : ApiController
    {
        const long MAX_SIZE_OF_FILE_BYTES = 26214400;//25 mega byte
        const string EMAIL_PARAMATER_VALUE = "email_parms";
        String Error_mail = "TODO CHANGE";
        public class Email
        {
            public string subject { get; set; }
            public string body { get; set; }
            public string[] distantions { get; set; }
            public string emailUserAdress { get; set; }
            public string emailPass { get; set; }
            public string displayName { get; set; }//optinal defualt same as emailUserAdress
            public string smtpDomain { get; set; } //optinal  defualt is Gmail

            internal bool isValid()
            {
                bool isEmailUserAdress = this.emailUserAdress != null && this.emailUserAdress != "";
                bool isEmailPass = this.emailPass != null && this.emailPass != "";
                bool isBody = this.body != null && this.body != "";
                bool isSubject = this.subject != null && this.subject != "";
                bool isDistantions = this.emailUserAdress != null && this.distantions.Length > 0;
                return isEmailUserAdress && isEmailPass && isBody && isSubject && isDistantions;
            }
            //public string fileName;
        }
        
        

        // POST: api/Email
        [ValidateInput(false)]
        public async System.Threading.Tasks.Task<string> PostAsync()
        {
            
            var httpRequest = HttpContext.Current.Request;
            
            Email emailParms = new Email();
            List<Attachment> attachments = new List<Attachment>();
            string jsonEmail = string.Empty;
            string ErrorFiles = string.Empty;
            jsonEmail = httpRequest.Unvalidated.Form[EMAIL_PARAMATER_VALUE];
            if (httpRequest.Files.Count > 0 || jsonEmail != null)
            { 
                foreach (string fileName in httpRequest.Files)
                {
                    HttpPostedFile postedFile =  httpRequest.Files[fileName];
                    if (postedFile.ContentLength < MAX_SIZE_OF_FILE_BYTES
                        &&
                        postedFile.ContentLength > 0)
                    {
                        attachments.Add(new Attachment(postedFile.InputStream, postedFile.FileName));
                    }
                    else
                    {
                        ErrorFiles += $"{postedFile.FileName} too big";
                    }
                }
                jsonEmail = httpRequest.Unvalidated.Form["EMAIL_PARAMATER_VALUE"];
            }
            else
            {
                jsonEmail = await Request.Content.ReadAsStringAsync();
            }
            
            Email e = JsonSerializer.Deserialize<Email>(jsonEmail);
            if (e.isValid())
            {
                HttpContext.Current.Response.StatusCode = 200;
                string res = SendAlertEmail(e, attachments);
                return res;
            }
            else
            {
                HttpContext.Current.Response.StatusCode = 400;
                return "{\"Error\":\"Invalid email parms\"";
            }
        }

        private string SendAlertEmail(Email email,List<Attachment> attachments)
        {
            List<string> respone = new List<string>();
            try
            {
                MailMessage mail = new MailMessage();
                using (SmtpClient SmtpServer = new SmtpClient(email.smtpDomain ?? "smtp.gmail.com"))
                {
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

                    foreach(Attachment attachment in attachments)
                    {
                        mail.Attachments.Add(attachment);
                    }



                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(email.emailUserAdress, email.emailPass);
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                }
                Console.WriteLine("mail sent");
            }
            catch (Exception ex)
            {
                respone.Add($"\"Exception Msg\":\"{ex.Message }");
            }
            return respone.Count >=1? "{\"OK\":false," + String.Join(",\n",respone)+"}":"{\"OK\":\"OK\"}";
        }

    }
}
