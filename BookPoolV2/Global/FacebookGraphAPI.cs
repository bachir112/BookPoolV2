using BookPoolV2.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace BookPoolV2.Global
{
    public class FacebookGraphAPI
    {
        public FacebookGraphAPI(string accessToken, string baseURL, string clientID = null, string fields = null)
        {
            AccessToken = accessToken;
            BaseURL = baseURL;
            ClientID = clientID;
            Fields = fields;
        }

        async public Task<FacebookAccount> GetUserInfo(FacebookGraphAPI FGA)
        {
            FacebookAccount facebookAccountInfo = new FacebookAccount();

            using (var client = new HttpClient())
            {
                string uri = FGA.BaseURL + FGA.ClientID + "?" + "access_token=" + FGA.AccessToken + "&fields=" + FGA.Fields;
                var responseResult = await client.GetStringAsync(uri);

                facebookAccountInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookAccount>(responseResult);
                facebookAccountInfo.Picture = Global.Globals.FacebookGraphAPIBaseUrl + facebookAccountInfo.Id + "/picture?type=large";
            }

            return facebookAccountInfo;
        }

        public string AccessToken { get; set; }
        public string BaseURL { get; set; }
        public string ClientID { get; set; }
        public string Fields { get; set; }
    }
}