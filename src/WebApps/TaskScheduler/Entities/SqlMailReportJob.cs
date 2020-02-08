using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Text;

namespace TaskScheduler.Entities
{
    public class SqlMailReportJob : Job
    {
        public SqlMailReportJob()
        {
            JobType = "SqlMailReportJob";
        }

        public string MailSubject { get; set; }

        public string MailMessage { get; set; }

        public string To { get; set; }

        public string Cc { get; set; }

        public string Cco { get; set; }

        public SqlMailReportResultFormat ResultFormat { get; set; }

        public int MinRowsToSend { get; set; }

        public string Query { get; set; }

        public Guid DatabaseInfoId { get; set; }

        public virtual DatabaseInfo DatabaseInfo { get; set; }

        public override string Execute()
        {
            using (var conn = DatabaseInfo.CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = Query;

                using (var dr = cmd.ExecuteReader())
                    EnviarEmail(dr);
            }

            return Query;
        }
        
        private void EnviarEmail(IDataReader dr)
        {
            var mail = new MailMessage
            {
                Subject = MailSubject,
                IsBodyHtml = true,
                From = new MailAddress("ti.front.nike@cnova.com")
            };

            var result = MontarBody(dr, mail);

            if (result.NotSend)
                return;

            mail.Body = result.Body;

            foreach (var endereco in To.Split(',', ';'))
                mail.To.Add(endereco);

            if (!string.IsNullOrEmpty(Cc))
            {
                foreach (var endereco in Cc.Split(',', ';'))
                    mail.CC.Add(endereco);
            }

            if (!string.IsNullOrEmpty(Cco))
            {
                foreach (var endereco in Cco.Split(',', ';'))
                    mail.Bcc.Add(endereco);
            }

            var smtp = new SmtpClient();

            smtp.Send(mail);
        }

        private BodyResult MontarBody(IDataReader dr, MailMessage mail)
        {
            var html = new StringBuilder(1000000);

            html.Append("<div>").Append(MailMessage).Append("</div>");
            
            var total = 0;

            switch (ResultFormat)
            {
                case SqlMailReportResultFormat.HtmlInBody:
                    total = BuildHtml(dr, html);
                    break;

                case SqlMailReportResultFormat.HtmlAsExcel:
                    var resultHtml = new StringBuilder(1000000);

                    total = BuildHtml(dr, resultHtml);

                    mail.Attachments.Add(new Attachment(
                        new MemoryStream(Encoding.Unicode.GetBytes(resultHtml.ToString())),
                        "Resultado.xlsx"));

                    break;

                case SqlMailReportResultFormat.Csv:
                    var csv = new StringBuilder(1000000);

                    total = BuildCsv(dr, csv);

                    mail.Attachments.Add(new Attachment(
                        new MemoryStream(Encoding.Unicode.GetBytes(csv.ToString())),
                        "Resultado.csv"));
                    break;
            }

            html.Append("<br/><br/>");

            html.Append("<div>Total: ").Append(total).Append("</div>");
            
            return new BodyResult
            {
                Body = html.ToString(),
                NotSend = total < MinRowsToSend
            };
        }

        private int BuildCsv(IDataReader dr, StringBuilder csv)
        {
            var total = 0;

            for (var i = 0; i < dr.FieldCount; ++i)
            {
                var value = dr.GetName(i);

                csv.Append(value).Append(";");
            }

            csv.AppendLine();

            while (dr.Read())
            {
                for (var i = 0; i < dr.FieldCount; ++i)
                {
                    string value;

                    var tmp = dr[i];

                    var type = dr.GetFieldType(i);

                    if (tmp == DBNull.Value)
                        value = string.Empty;

                    else if (type == typeof(DateTime))
                        value = Convert.ToDateTime(tmp).ToString("yyyy-MM-dd HH:mm:ss");

                    else if (type == typeof(decimal))
                        value = Convert.ToDecimal(tmp).ToString("N2");

                    else
                        value = Convert.ToString(tmp);
                    
                    csv.Append(value).Append(";");
                }
                
                csv.AppendLine();

                ++total;
            }

            return total;
        }

        private int BuildHtml(IDataReader dr, StringBuilder html)
        {
            var total = 0;

            html.Append(@"
<style>
th, td { 
    border: solid black 1px;
    padding: 3px;
    margin: 0;
}

div {
    font-size: 12pt;
}
</style>");

            html.Append(@"<table cellpadding='0' cellspacing='0'><tr>");

            for (var i = 0; i < dr.FieldCount; ++i)
            {
                var value = dr.GetName(i);

                html.Append("<th>").Append(value).Append("</th>");
            }
            
            html.AppendLine("</tr>");
            
            while (dr.Read())
            {
                html.Append("<tr>");

                for (var i = 0; i < dr.FieldCount; ++i)
                {
                    string value;

                    var tmp = dr[i];

                    var type = dr.GetFieldType(i);

                    if (tmp == DBNull.Value)
                        value = string.Empty;

                    else if (type == typeof(DateTime))
                        value = Convert.ToDateTime(tmp).ToString("dd/MM/yyyy HH:mm:ss");

                    else if (type == typeof(decimal))
                        value = Convert.ToDecimal(tmp).ToString("N2");

                    else
                        value = Convert.ToString(tmp);
                    
                    html.Append("<td>").Append(value).Append("</td>");
                }
                
                html.AppendLine("</tr>");

                ++total;
            }
            
            html.Append("</table>");
            return total;
        }
    }

    public enum SqlMailReportResultFormat
    {
        [Description("Html In Body")]
        HtmlInBody = 1,

        [Description("Html As Excel")]
        HtmlAsExcel = 2,

        [Description("CSV")]
        Csv = 3
    }

    internal class BodyResult
    {
        public bool NotSend { get; set; }
        public string Body { get; set; }
    }
}