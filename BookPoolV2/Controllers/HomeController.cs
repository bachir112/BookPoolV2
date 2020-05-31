﻿using BookPool.DataObjects.DTO;
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
        public async Task<ActionResult> Index(string query = null)
        {
            ViewBag.Query = query;

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

        public async Task<ActionResult> MyBooks()
        {
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

            return View();
        }
        
        public async Task<ActionResult> Product(int bookID, string query = null)
        {
            Dictionary<string, List<BookPoolResult>> apiResults = new Dictionary<string, List<BookPoolResult>>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Books/GetBooks");
                httpRoute.Append("?");
                httpRoute.AppendFormat("query={0}", query);

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

        public ActionResult FilteredProducts()
        {
            return View();
        }

        public ActionResult MyOrders()
        {
            return View();
        }

        public async Task<ActionResult> FindMyBook(string query = null)
        {
            Dictionary<string, List<BookPoolResult>> apiResults = new Dictionary<string, List<BookPoolResult>>();
            List<BookPoolResult> result = new List<BookPoolResult>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Books/GetBooks");
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
                                                   string academic)
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

    }
}