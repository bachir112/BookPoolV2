using BookPool.DataObjects.EDM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BookPoolV2.Global
{
    public class Globals
    {
        public static string baseURL = "https://localhost:44361/";

        static public string FacebookGraphAPIBaseUrl = "https://graph.facebook.com/";
        public const string DefaultFacebookFields = "id,name,email,gender,birthday,location,friends,first_name,last_name";

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

        public static async Task<List<UserCart>> GetBooksInCart(string UserID)
        {
            Dictionary<string, List<UserCart>> apiResults = new Dictionary<string, List<UserCart>>();
            List<UserCart> result = new List<UserCart>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Books/GetBooksInCart");
                httpRoute.Append("?");
                httpRoute.AppendFormat("UserID={0}", UserID);

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiResults = await response.Content.ReadAsAsync<Dictionary<string, List<UserCart>>>();
                    result = apiResults["results"];
                }
            }

            return result;
        }
    }
}