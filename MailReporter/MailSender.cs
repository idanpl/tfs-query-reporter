using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using System.Text;

namespace TfsQueryReporter.Mail
{
    /// <summary>
    /// Enables sending mails in the orgnization. 
    /// </summary>
    public class MailSender
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from">from address</param>
        /// <param name="recipients">list of recipients</param>
        /// <param name="smtpHost">the SMTP host</param>
        /// <param name="subject">mail subject</param>
        /// <param name="tableData">The table to be sent</param>
        public void SendMail(MailAddress from, List<MailAddress> recipients, string smtpHost,  string subject, DataTable tableData)
        {
            string htmlData = DataToTable(tableData);

            MailMessage msg = new MailMessage();
            foreach (var recipient in recipients)
            {
                msg.To.Add(recipient);
            }
            
            msg.From = from;
            msg.Subject = subject;

            msg.Body = htmlData;
            msg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = true;
            client.Host = smtpHost;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {
                Console.WriteLine("sending mail");
                client.Send(msg);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Could not send mail. Exception was " + ex.Message +". Inner Exception is " + ex.InnerException.Message);
                return;
            }

            Console.WriteLine("Mail was successfully sent");            
        }

        private string DataToTable(DataTable dataTable)
        {
            if (dataTable.Rows.Count == 0) return ""; // enter code here

            StringBuilder builder = new StringBuilder();
            builder.Append("<html>");
            builder.Append("<head>");
            builder.Append(@"<style>");
            builder.Append("table {font-family: arial, sans-serif;border-collapse: collapse;width: 100 %;}");
            builder.Append("td, th {border: 1px solid #dddddd;text-align: left;padding: 8px;}");
            builder.Append("tr:nth-child(even) {background-color: #dddddd;}");
            builder.Append("</style>");
            builder.Append("</head>");
            builder.Append("<body>");
            builder.Append("<h2>").Append(dataTable.TableName).Append("</h2>");
            builder.Append("<table>");

            // The table header
            builder.Append("<tr>");
            foreach (DataColumn column in dataTable.Columns)
            {
                builder.Append("<th>");
                builder.Append(column.ColumnName);
                builder.Append("</th>");
            }
            builder.Append("</tr>");

            // The rest of the rows
            foreach (DataRow row in dataTable.Rows)
            {
                builder.Append("<tr>");
                foreach (DataColumn col in dataTable.Columns)
                {
                    builder.Append("<td>");
                    builder.Append(row[col.ColumnName]);
                    builder.Append("</td>");
                }
                builder.Append("</tr>");
            }
            builder.Append("</table>");
            builder.Append("</body>");
            builder.Append("</html>");
            builder = builder.Replace('\n', ' ');
            builder = builder.Replace('\t', ' ');
            return builder.ToString();
        }
    }
}

