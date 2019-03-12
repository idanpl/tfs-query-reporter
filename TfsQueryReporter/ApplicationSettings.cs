using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsQueryReporter
{
    public static class ApplicationSettings
    {
        public static string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        public static Guid ProjectGuid = new Guid(ConfigurationManager.AppSettings["ProjectGuid"]);
        public static List<string> ImportantFields = new List<string>(ConfigurationManager.AppSettings["ImportantFields"].Split(';'));
        public static List<string> Recipients = new List<string>(ConfigurationManager.AppSettings["Recipients"].Split(';'));
        public static string SmtpHost = ConfigurationManager.AppSettings["smtpHost"];
        public static string MailFrom = ConfigurationManager.AppSettings["MailFrom"];
        public static Guid QueryGuid = new Guid(ConfigurationManager.AppSettings["queryGuid"]);
        public static string QueryTitle = ConfigurationManager.AppSettings["queryTitle"];
    }
}
