using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using TfsQueryReporter.Mail;
using TfsQueryReporter.Tfs;

namespace TfsQueryReporter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            TfsUtils tfsUtil = new TfsUtils(ApplicationSettings.BaseUrl, ApplicationSettings.ProjectGuid);
            DataTable queryResult = tfsUtil.Query("Oh Query my query",  new Guid("01234567-d96b-4f03-9ceb-1b49de27ef41"), ApplicationSettings.ImportantFields);
            Console.WriteLine($"Query returned with {queryResult.Rows.Count} records");
            MailSender sender = new MailSender();
            MailAddress mailFrom = new MailAddress(ApplicationSettings.MailFrom);
            List<MailAddress> recipients = new List<MailAddress>();
            foreach (string recipient in ApplicationSettings.Recipients)
            {
                recipients.Add(new MailAddress(recipient));
            }
            sender.SendMail(mailFrom, recipients,  "Daily mail", ApplicationSettings.SmtpHost, queryResult);

        }
    }
}
