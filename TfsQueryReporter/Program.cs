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
            DataTable queryResult = tfsUtil.Query(ApplicationSettings.QueryGuid);
            Console.WriteLine("Query returned with " + queryResult.Rows.Count + " records");
            MailSender sender = new MailSender();
            MailAddress mailFrom = new MailAddress(ApplicationSettings.MailFrom);
            List<MailAddress> recipients = new List<MailAddress>();
            foreach (string recipient in ApplicationSettings.Recipients)
            {
                recipients.Add(new MailAddress(recipient));
            }
            sender.SendMail(mailFrom, recipients, ApplicationSettings.SmtpHost, "Daily mail" , queryResult);

        }
    }
}
