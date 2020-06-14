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
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index(string query = null,
            string language = null,
            bool sortByPrice = false,
            bool AvailableOnly = false,
            bool sortByPopularity = false)
        {
            ViewBag.Query = query;
            ViewBag.UserAddresses = await Global.Globals.GetUserAddresses(User.Identity.GetUserId());
            ViewBag.UserCartCookie = await Global.Globals.GetCart(User.Identity.GetUserId());

            ViewBag.Language = language;
            ViewBag.SortByPrice = sortByPrice;
            ViewBag.AvailableOnly = AvailableOnly;
            ViewBag.SortByPopularity = sortByPopularity;

            if (string.IsNullOrEmpty(query))
            {
                query = string.Empty;
            }

            Dictionary<string, List<BookPoolResult>> apiResults = new Dictionary<string, List<BookPoolResult>>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Books/GetBooks");
                httpRoute.Append("?");
                httpRoute.AppendFormat("query={0}", query);
                if (language != null)
                {
                    httpRoute.Append("&");
                    httpRoute.AppendFormat("language={0}", language);
                }
                if (sortByPrice)
                {
                    httpRoute.Append("&");
                    httpRoute.AppendFormat("sortByPrice={0}", sortByPrice);
                }
                if (AvailableOnly)
                {
                    httpRoute.Append("&");
                    httpRoute.AppendFormat("AvailableOnly={0}", AvailableOnly);
                }
                if (sortByPopularity)
                {
                    httpRoute.Append("&");
                    httpRoute.AppendFormat("sortByPopularity={0}", sortByPopularity);
                }

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiResults = await response.Content.ReadAsAsync<Dictionary<string, List<BookPoolResult>>>();
                    ViewBag.Books = apiResults["results"];
                }
            }

            Dictionary<string, List<Category>> apiCategoriesResults = new Dictionary<string, List<Category>>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Values/GetCategories");

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiCategoriesResults = await response.Content.ReadAsAsync<Dictionary<string, List<Category>>>();
                    ViewBag.Categories = apiCategoriesResults["results"];
                }
            }


            return View();
        }

        public async Task<ActionResult> About()
        {
            ViewBag.UserAddresses = await Global.Globals.GetUserAddresses(User.Identity.GetUserId());
            return View();
        }

        public async Task<ActionResult> Contact()
        {
            ViewBag.UserAddresses = await Global.Globals.GetUserAddresses(User.Identity.GetUserId());
            return View();
        }

        public async Task<ActionResult> MyWishes()
        {
            ViewBag.UserAddresses = await Global.Globals.GetUserAddresses(User.Identity.GetUserId());

            Dictionary<string, List<BookPoolResult>> apiResults = new Dictionary<string, List<BookPoolResult>>();
            List<BookPoolResult> result = new List<BookPoolResult>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Books/GetBooksForSearch");
                httpRoute.Append("?");
                httpRoute.AppendFormat("UserID={0}", User.Identity.GetUserId());

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiResults = await response.Content.ReadAsAsync<Dictionary<string, List<BookPoolResult>>>();
                    ViewBag.MyBooks = apiResults["results"];
                }
            }

            return View();
        }

        public async Task<ActionResult> MyBooks()
        {
            ViewBag.UserAddresses = await Global.Globals.GetUserAddresses(User.Identity.GetUserId());
            Dictionary<string, List<BookPoolResult>> apiResults = new Dictionary<string, List<BookPoolResult>>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Books/GetMyBooks");
                httpRoute.Append("?");
                httpRoute.AppendFormat("UserID={0}", User.Identity.GetUserId());

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiResults = await response.Content.ReadAsAsync<Dictionary<string, List<BookPoolResult>>>();
                    ViewBag.MyBooks = apiResults["results"];
                }
            }

            Dictionary<string, List<Category>> apiCategoriesResults = new Dictionary<string, List<Category>>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Values/GetCategories");

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiCategoriesResults = await response.Content.ReadAsAsync<Dictionary<string, List<Category>>>();
                    ViewBag.Categories = apiCategoriesResults["results"];
                }
            }

            Dictionary<string, List<Language>> apiLanguagesResults = new Dictionary<string, List<Language>>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Values/GetLanguages");

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiLanguagesResults = await response.Content.ReadAsAsync<Dictionary<string, List<Language>>>();
                    ViewBag.Languages = apiLanguagesResults["results"];
                }
            }

            Dictionary<string, List<Condition>> apiConditionsResults = new Dictionary<string, List<Condition>>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Values/GetConditions");

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiConditionsResults = await response.Content.ReadAsAsync<Dictionary<string, List<Condition>>>();
                    ViewBag.Conditions = apiConditionsResults["results"];
                }
            }

            Dictionary<string, List<Institution>> apiInstitutionsResults = new Dictionary<string, List<Institution>>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Values/GetInstitutions");

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiInstitutionsResults = await response.Content.ReadAsAsync<Dictionary<string, List<Institution>>>();
                    ViewBag.Institutions = apiInstitutionsResults["results"];
                }
            }

            Dictionary<string, List<Cours>> apiCoursesResults = new Dictionary<string, List<Cours>>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Values/GetCourses");

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiCoursesResults = await response.Content.ReadAsAsync<Dictionary<string, List<Cours>>>();
                    ViewBag.Courses = apiCoursesResults["results"];
                }
            }

            using (var db = new BookPoolEntities())
            {
                string thisUserID = User.Identity.GetUserId();
                ViewBag.AspNetUser = db.AspNetUsers.First(x => x.Id == thisUserID);
            }

            return View();
        }
        
        public async Task<ActionResult> Product(int bookID, string query = null)
        {
            ViewBag.UserAddresses = await Global.Globals.GetUserAddresses(User.Identity.GetUserId());
            ViewBag.UserCartCookie = await Global.Globals.GetCart(User.Identity.GetUserId());

            Dictionary<string, List<BookPoolResult>> apiResults = new Dictionary<string, List<BookPoolResult>>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Books/GetBooks");
                httpRoute.Append("?");
                httpRoute.AppendFormat("query={0}", query);
                httpRoute.Append("&");
                httpRoute.AppendFormat("BookID={0}", bookID);

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiResults = await response.Content.ReadAsAsync<Dictionary<string, List<BookPoolResult>>>();
                    ViewBag.AvailableBook = apiResults["results"].First(x => x.ID == bookID);
                    ViewBag.SuggestedBooks = apiResults["results"].Where(x => x.ID != bookID).Select(x => x).ToList();
                }
            }

            return View();
        }

        public async Task<ActionResult> FilteredByCategory(int categoryID,
            string language = null,
            bool sortByPrice = false,
            bool AvailableOnly = false,
            bool sortByPopularity = false)
        {
            ViewBag.UserAddresses = await Global.Globals.GetUserAddresses(User.Identity.GetUserId());
            ViewBag.UserCartCookie = await Global.Globals.GetCart(User.Identity.GetUserId());

            ViewBag.CategoryID = categoryID;
            ViewBag.Language = language;
            ViewBag.SortByPrice = sortByPrice;
            ViewBag.AvailableOnly = AvailableOnly;
            ViewBag.SortByPopularity = sortByPopularity;

            Dictionary<string, List<BookPoolResult>> apiResults = new Dictionary<string, List<BookPoolResult>>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Books/GetBooksInCategory");
                httpRoute.Append("?");
                httpRoute.AppendFormat("CategoryID={0}", categoryID);
                if (language != null)
                {
                    httpRoute.Append("&");
                    httpRoute.AppendFormat("language={0}", language);
                }
                if (sortByPrice)
                {
                    httpRoute.Append("&");
                    httpRoute.AppendFormat("sortByPrice={0}", sortByPrice);
                }
                if (AvailableOnly)
                {
                    httpRoute.Append("&");
                    httpRoute.AppendFormat("AvailableOnly={0}", AvailableOnly);
                }
                if (sortByPopularity)
                {
                    httpRoute.Append("&");
                    httpRoute.AppendFormat("sortByPopularity={0}", sortByPopularity);
                }

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiResults = await response.Content.ReadAsAsync<Dictionary<string, List<BookPoolResult>>>();
                    ViewBag.Books = apiResults["results"];
                }
            }


            Dictionary<string, List<Category>> apiCategoriesResults = new Dictionary<string, List<Category>>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Values/GetCategories");

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiCategoriesResults = await response.Content.ReadAsAsync<Dictionary<string, List<Category>>>();
                    ViewBag.Category = apiCategoriesResults["results"].First(x => x.ID == categoryID);
                }
            }

            return View();
        }

        public async Task<ActionResult> MyOrders()
        {
            ViewBag.UserAddresses = await Global.Globals.GetUserAddresses(User.Identity.GetUserId());

            Dictionary<string, List<OrderHeader>> apiOrderHeaderResults = new Dictionary<string, List<OrderHeader>>();
            List<OrderHeader> resultOrderHeader = new List<OrderHeader>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Books/GetMyOrdersHeaders");
                httpRoute.Append("?");
                httpRoute.AppendFormat("UserID={0}", User.Identity.GetUserId());

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiOrderHeaderResults = await response.Content.ReadAsAsync<Dictionary<string, List<OrderHeader>>>();
                    resultOrderHeader = apiOrderHeaderResults["results"];
                    ViewBag.OrdersHeaders = apiOrderHeaderResults["results"];
                }
            }

            List<int> ordersHeadersListOfIDs = resultOrderHeader.Select(x => x.ID).ToList();
            string csvHeadersIDs = string.Join(",", ordersHeadersListOfIDs);
            Dictionary<string, List<OrderDetail>> apiOrderDetailsResults = new Dictionary<string, List<OrderDetail>>();
            List<OrderDetail> resultOrderDetails = new List<OrderDetail>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Books/GetMyOrdersDetails");
                httpRoute.Append("?");
                httpRoute.AppendFormat("OrderHeadersCSV={0}", csvHeadersIDs);

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiOrderDetailsResults = await response.Content.ReadAsAsync<Dictionary<string, List<OrderDetail>>>();
                    ViewBag.OrdersDetails = apiOrderDetailsResults["results"];
                    //resultOrderDetails = apiOrderDetailsResults["results"];
                }
            }

            return View();
        }

        public async Task<ActionResult> FindMyBook(string query = null)
        {
            //ViewBag.UserAddresses = await Global.Globals.GetUserAddresses(User.Identity.GetUserId());

            Dictionary<string, List<BookPoolResult>> apiResults = new Dictionary<string, List<BookPoolResult>>();
            List<BookPoolResult> result = new List<BookPoolResult>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Books/FindMyBook");
                httpRoute.Append("?");
                httpRoute.AppendFormat("query={0}", query);

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiResults = await response.Content.ReadAsAsync<Dictionary<string, List<BookPoolResult>>>();
                    result = apiResults["results"];
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> SellMyBook(string googleID, 
                                                   string bookName, 
                                                   string price, 
                                                   string condition, 
                                                   string category,
                                                   string language,
                                                   string academic,
                                                   string institution,
                                                   string course,
                                                   string institutionType)
        {
            Dictionary<string, List<BookPoolResult>> apiResults = new Dictionary<string, List<BookPoolResult>>();
            List<BookPoolResult> result = new List<BookPoolResult>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Books/SellMyBook");
                httpRoute.Append("?");
                httpRoute.AppendFormat("UserID={0}", User.Identity.GetUserId());
                httpRoute.Append("&");
                httpRoute.AppendFormat("GoogleID={0}", googleID);
                httpRoute.Append("&");
                httpRoute.AppendFormat("BookName={0}", bookName);
                httpRoute.Append("&");
                httpRoute.AppendFormat("Price={0}", price);
                httpRoute.Append("&");
                httpRoute.AppendFormat("Condition={0}", condition);
                httpRoute.Append("&");
                httpRoute.AppendFormat("Category={0}", category);
                httpRoute.Append("&");
                httpRoute.AppendFormat("Language={0}", language);
                httpRoute.Append("&");
                httpRoute.AppendFormat("Academic={0}", academic);
                httpRoute.Append("&");
                httpRoute.AppendFormat("Institution={0}", institution);
                httpRoute.Append("&");
                httpRoute.AppendFormat("Course={0}", course);
                httpRoute.Append("&");
                httpRoute.AppendFormat("institutionType={0}", institutionType);

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiResults = await response.Content.ReadAsAsync<Dictionary<string, List<BookPoolResult>>>();
                    result = apiResults["results"];
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public async Task<ActionResult> DeleteMyBook(int MyBookID)
        {
            Dictionary<string, bool> apiResults = new Dictionary<string, bool>();
            bool result = false;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Books/DeleteMyBook");
                httpRoute.Append("?");
                httpRoute.AppendFormat("UserID={0}", User.Identity.GetUserId());
                httpRoute.Append("&");
                httpRoute.AppendFormat("MyBookID={0}", MyBookID);

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiResults = await response.Content.ReadAsAsync<Dictionary<string, bool>>();
                    result = apiResults["results"];
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public async Task<ActionResult> DeleteSearchForBook(int MyBookID)
        {
            Dictionary<string, bool> apiResults = new Dictionary<string, bool>();
            bool result = false;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Books/DeleteSearchForBook");
                httpRoute.Append("?");
                httpRoute.AppendFormat("UserID={0}", User.Identity.GetUserId());
                httpRoute.Append("&");
                httpRoute.AppendFormat("MyBookID={0}", MyBookID);

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiResults = await response.Content.ReadAsAsync<Dictionary<string, bool>>();
                    result = apiResults["results"];
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<ActionResult> SearchForThisBook(string BookID)
        {
            Dictionary<string, bool> apiResults = new Dictionary<string, bool>();
            bool result = false;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Books/SearchForThisBook");
                httpRoute.Append("?");
                httpRoute.AppendFormat("UserID={0}", User.Identity.GetUserId());
                httpRoute.Append("&");
                httpRoute.AppendFormat("GoogleID={0}", BookID);

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiResults = await response.Content.ReadAsAsync<Dictionary<string, bool>>();
                    result = apiResults["results"];
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<ActionResult> AddBookToCart(string BookID)
        {
            Dictionary<string, bool> apiResults = new Dictionary<string, bool>();
            bool result = false;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Books/AddBookToCart");
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


        [HttpPost]
        public async Task<ActionResult> AddUserAddress(string AddressTitle, string Address)
        {
            Dictionary<string, bool> apiResults = new Dictionary<string, bool>();
            bool result = false;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Users/AddUserAddress");
                httpRoute.Append("?");
                httpRoute.AppendFormat("UserID={0}", User.Identity.GetUserId());
                httpRoute.Append("&");
                httpRoute.AppendFormat("AddressTitle={0}", AddressTitle);
                httpRoute.Append("&");
                httpRoute.AppendFormat("Address={0}", Address);

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiResults = await response.Content.ReadAsAsync<Dictionary<string, bool>>();
                    result = apiResults["results"];
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<ActionResult> DeliveringToUserAddress(string AddressID)
        {
            Dictionary<string, bool> apiResults = new Dictionary<string, bool>();
            bool result = false;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Users/DeliveringToUserAddress");
                httpRoute.Append("?");
                httpRoute.AppendFormat("UserID={0}", User.Identity.GetUserId());
                httpRoute.Append("&");
                httpRoute.AppendFormat("AddressID={0}", AddressID);

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiResults = await response.Content.ReadAsAsync<Dictionary<string, bool>>();
                    result = apiResults["results"];
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> PartialUserAddresses()
        {
            Dictionary<string, List<UsersAddress>> apiResults = new Dictionary<string, List<UsersAddress>>();
            List<UsersAddress> result = new List<UsersAddress>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Users/GetUserAddresses");
                httpRoute.Append("?");
                httpRoute.AppendFormat("UserID={0}", User.Identity.GetUserId());

                var response = await client.GetAsync(httpRoute.ToString());
                if (response.IsSuccessStatusCode)
                {
                    apiResults = await response.Content.ReadAsAsync<Dictionary<string, List<UsersAddress>>>();
                    result = apiResults["results"];
                }
            }

            return View(result);
        }

    }
}