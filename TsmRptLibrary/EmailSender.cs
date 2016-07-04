using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace TsmRptLibrary
{
    internal class EmailSender
    {
        private ReportTablePreprocessor _reportTablePreprocessor;

        public EmailSender(ReportTablePreprocessor reportTablePreprocessor)
        {
            this._reportTablePreprocessor = reportTablePreprocessor;
        }

        private void CreateAndSendErrorEmail(Exception error)
        {
            var msgBody = new StringBuilder();
            msgBody.Append("An error occurred in DatamoveService.");
            msgBody.Append(Environment.NewLine);
            msgBody.Append(Environment.NewLine);
            msgBody.Append("Error Message: " + error.Message);
            msgBody.Append(Environment.NewLine);
            msgBody.Append(Environment.NewLine);
            msgBody.Append("Stack Trace: " + error.StackTrace);
            msgBody.Append(Environment.NewLine);
            msgBody.Append(Environment.NewLine);
            msgBody.Append("Inner Exception: " + error.InnerException?.Message);

            if (!string.IsNullOrEmpty(this._reportTablePreprocessor._configHelper.SendEmailUsername))
            {
                this._reportTablePreprocessor.SmtpClient().Credentials = new NetworkCredential(this._reportTablePreprocessor._configHelper.SendEmailUsername, this._reportTablePreprocessor._configHelper.SendEmailPassword);
            }
            this._reportTablePreprocessor.SmtpClient().EnableSsl = this._reportTablePreprocessor._configHelper.SendEmailEnableSsl;
            var mailMessage = new MailMessage();
            foreach (var email in this._reportTablePreprocessor._configHelper.EmailList)
            {
                mailMessage.To.Add(email);
            }
            mailMessage.From = new MailAddress(this._reportTablePreprocessor._configHelper.SendEmailFrom);
            mailMessage.Subject = this._reportTablePreprocessor._configHelper.EmailSubject;
            mailMessage.Body = msgBody.ToString();
            this._reportTablePreprocessor.SmtpClient().Send(mailMessage);
        }
    }
}