using BookPool.DataObjects.DTO;
using BookPool.DataObjects.EDM;
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
    public class CheckoutController : Controller
    {
        // GET: Checkout
        public async Task<ActionResult> Index()
        {
            ViewBag.UserAddresses = await Global.Globals.GetUserAddresses(User.Identity.GetUserId());

            Dictionary<string, List<BookPoolResult>> apiResults = new Dictionary<string, List<BookPoolResult>>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Books/GetBooksInCart");
                httpRoute.Append("?");
                httpRoute.AppendFormat("UserID={0}", User.Identity.GetUserId());

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiResults = await response.Content.ReadAsAsync<Dictionary<string, List<BookPoolResult>>>();
                    ViewBag.BooksInCart = apiResults["results"];
                }
            }


            using (var db = new BookPoolEntities())
            {
                string thisUserID = User.Identity.GetUserId();
                ViewBag.AspNetUser = db.AspNetUsers.First(x => x.Id == thisUserID);
            }

            return View();
        }

        public async Task<ActionResult> PlacedOrder(string books, string FirstName, string LastName, string Email, string PhoneNumber)
        {
            ViewBag.UserAddresses = await Global.Globals.GetUserAddresses(User.Identity.GetUserId());

            using (var db = new BookPoolEntities())
            {
                string thisUserID = User.Identity.GetUserId();
                AspNetUser aspNetUser = db.AspNetUsers.First(x => x.Id == thisUserID);
                aspNetUser.FirstName = FirstName;
                aspNetUser.LastName = LastName;
                aspNetUser.Email = Email;
                aspNetUser.PhoneNumber = PhoneNumber;

                db.SaveChanges();
            }

            Dictionary<string, string> apiResults = new Dictionary<string, string>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Users/PlaceOrder");
                httpRoute.Append("?");
                httpRoute.AppendFormat("BuyerID={0}", User.Identity.GetUserId());
                httpRoute.Append("&");
                httpRoute.AppendFormat("BooksIDs={0}", books);

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiResults = await response.Content.ReadAsAsync<Dictionary<string, string>>();
                    ViewBag.OrderNumber = apiResults["results"];
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> RemoveBookFromBasket(int BookID)
        {
            Dictionary<string, bool> apiResults = new Dictionary<string, bool>();
            bool result = false;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Users/RemoveBookFromBasket");
                httpRoute.Append("?");
                httpRoute.AppendFormat("UserID={0}", User.Identity.GetUserId());
                httpRoute.Append("&");
                httpRoute.AppendFormat("BookID={0}", BookID);

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiResults = await response.Content.ReadAsAsync<Dictionary<string, bool>>();
                    result = apiResults["results"];
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}