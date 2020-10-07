using BookPool.DataObjects.EDM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace BookPool.DataInterface.Global
{
    public class Globals
    {
        public static string BookSellingStatus_Available = "Available";
        public static string BookSellingStatus_NotAvailable = "NotAvailable";

        public static string BookpoolEmailAddress = ConfigurationManager.AppSettings["BookpoolEmailAddress"];
        public static string BookpoolEmailAddressPassword = ConfigurationManager.AppSettings["BookpoolEmailAddressPassword"];

        public static decimal GetBookpoolCharges()
        {
            decimal bookpoolCharges = 0;

            using (var db = new BookPoolEntities())
            {
                bookpoolCharges = db.BookPoolCharges.First().Charge;
            }

            return bookpoolCharges;
        }

        static public bool sendEmail(string title, string message, string fullName, string sendTo)
        {
            bool result = false;

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress(BookpoolEmailAddress);
                mail.To.Add(sendTo);
                mail.Subject = title;

                string body = string.Empty;
                using (StreamReader reader = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/bookpool_email.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{Name}", fullName);
                body = body.Replace("{Title}", title);
                body = body.Replace("{Message}", message);

                mail.Body = body;

                mail.IsBodyHtml = true;
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(BookpoolEmailAddress, BookpoolEmailAddressPassword);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }
    }
}