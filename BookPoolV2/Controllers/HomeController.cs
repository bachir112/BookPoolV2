using BookPool.DataObjects.DTO;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BookPoolV2.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index(string query = null)
        {
            Dictionary<string, List<BookPoolResult>> apiResults = new Dictionary<string, List<BookPoolResult>>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Books/GetBooks");
                if(query != null)
                {
                    httpRoute.Append("?");
                    httpRoute.AppendFormat("query={0}", query);
                }

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiResults = await response.Content.ReadAsAsync<Dictionary<string, List<BookPoolResult>>>();
                    ViewBag.Books = apiResults["results"];
                }
            }

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult MyWishes()
        {
            return View();
        }

        public ActionResult MyBooks()
        {
            return View();
        }
        
        public ActionResult Product()
        {
            return View();
        }

        public ActionResult FilteredProducts()
        {
            return View();
        }

        public ActionResult MyOrders()
        {
            return View();
        }
    }
}