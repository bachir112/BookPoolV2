using BookPool.DataObjects.EDM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BookPoolV2.Global
{
    public class Globals
    {
        //dev
        //public static string baseURL = "http://localhost/BookPool.DataInterface/";

        //prod
        public static string baseURL = "http://localhost:44361/";

        static public string FacebookGraphAPIBaseUrl = "https://graph.facebook.com/";
        public const string DefaultFacebookFields = "id,name,email,gender,birthday,location,friends,first_name,last_name";

        public static string BookSellingStatus_Available = "Available";
        public static string BookSellingStatus_NotAvailable = "NotAvailable";

        public static async Task<List<UsersAddress>> GetUserAddresses(string userID)
        {
            Dictionary<string, List<UsersAddress>> apiResults = new Dictionary<string, List<UsersAddress>>();
            List<UsersAddress> result = new List<UsersAddress>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Users/GetUserAddresses");
                httpRoute.Append("?");
                httpRoute.AppendFormat("UserID={0}", userID);

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiResults = await response.Content.ReadAsAsync<Dictionary<string, List<UsersAddress>>>();
                    result = apiResults["results"];
                }
            }

            return result;
        }

        public static async Task<List<Dictionary<string, string>>> GetCart(string UserID)
        {
            Dictionary<string, List<Dictionary<string, string>>> apiResults = new Dictionary<string, List<Dictionary<string, string>>>();
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Books/GetCart");
                httpRoute.Append("?");
                httpRoute.AppendFormat("UserID={0}", UserID);

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiResults = await response.Content.ReadAsAsync<Dictionary<string, List<Dictionary<string, string>>>>();
                    result = apiResults["results"];
                }
            }

            return result;
        }

        static public bool sendEmail(string title, string message, string fullName, string sendTo)
        {
            bool result = false;

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("bookpool.me@gmail.com");
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
                SmtpServer.Credentials = new System.Net.NetworkCredential("bookpool.me@gmail.com", "AnAbach@123");
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