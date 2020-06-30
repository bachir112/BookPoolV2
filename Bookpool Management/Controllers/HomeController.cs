using BookPool.DataObjects.DTO;
using BookPool.DataObjects.EDM;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Bookpool_Management.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            using (var db = new BookPoolEntities())
            {
                ViewBag.Users = db.AspNetUsers.ToList();
            }


            //Books For Sale
            GoogleBook googleResult = new GoogleBook();
            List<GoogleBook> googleBooksResult = new List<GoogleBook>();

            List<BookPoolResult> dbResult = new List<BookPoolResult>();

            List<string> myGoogleBooksIDs = new List<string>();
            using (var db = new BookPoolEntities())
            {
                myGoogleBooksIDs = db.AvailableBooks.Select(x => x.GoogleID).ToList();
            }

            foreach (var googleID in myGoogleBooksIDs)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BookPool.DataObjects.Global.Globals.googleBaseAPI_SearchByID);
                    StringBuilder httpRoute = new StringBuilder();
                    httpRoute.Append(googleID);

                    var response = await client.GetAsync(httpRoute.ToString());
                    if (response.IsSuccessStatusCode)
                    {
                        googleResult = await response.Content.ReadAsAsync<GoogleBook>();
                        googleBooksResult.Add(googleResult);
                    }
                }
            }

            using (var db = new BookPoolEntities())
            {
                ViewBag.AvailableBooks = (from googleBooks in googleBooksResult
                                          join availableBooks in db.AvailableBooks on googleBooks.id equals availableBooks.GoogleID
                                          join ownerUser in db.AspNetUsers on availableBooks.OwnerUserID equals ownerUser.Id

                                          select new BookPoolResult
                                          {
                                              ID = availableBooks.ID,
                                              Academic = availableBooks.Academic,
                                              BookConditionID = availableBooks.BookConditionID,
                                              BookLanguageID = availableBooks.BookLanguageID,
                                              BookName = availableBooks.BookName,
                                              CategoryID = availableBooks.CategoryID,
                                              GoogleID = availableBooks.GoogleID,
                                              OwnerUserID = availableBooks.OwnerUserID,
                                              Price = availableBooks.Price,
                                              Authors = googleBooks.volumeInfo.authors,
                                              AverageRating = googleBooks.volumeInfo.averageRating,
                                              Categories = googleBooks.volumeInfo.categories,
                                              Description = googleBooks.volumeInfo.description,
                                              ImageURL = googleBooks.volumeInfo.imageLinks.thumbnail.Replace("http:", "https:"),
                                              PageCount = googleBooks.volumeInfo.pageCount,
                                              PreviewLink = googleBooks.volumeInfo.previewLink,
                                              PrintType = googleBooks.volumeInfo.printType,
                                              PublishedDate = googleBooks.volumeInfo.publishedDate,
                                              Publisher = googleBooks.volumeInfo.publisher,
                                              Subtitle = googleBooks.volumeInfo.subtitle,
                                              SellingStatus = availableBooks.SellingStatus,
                                              PostedOn = availableBooks.PostedOn,
                                              OwnerUserName = ownerUser.Email,
                                              OwnerPhoneNumber = ownerUser.PhoneNumber
                                          }).DistinctBy(x => x.ID).ToList();
            }



            //Books For Search
            GoogleBook googleSearchResult = new GoogleBook();
            List<GoogleBook> googleSearchBooksResult = new List<GoogleBook>();

            List<BookPoolResult> dbSearchResult = new List<BookPoolResult>();

            List<string> myGoogleSearchBooksIDs = new List<string>();
            using (var db = new BookPoolEntities())
            {
                myGoogleSearchBooksIDs = db.SearchForBooks.Select(x => x.GoogleID).ToList();
            }

            foreach (var googleID in myGoogleSearchBooksIDs)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BookPool.DataObjects.Global.Globals.googleBaseAPI_SearchByID);
                    StringBuilder httpRoute = new StringBuilder();
                    httpRoute.Append(googleID);

                    var response = await client.GetAsync(httpRoute.ToString());
                    if (response.IsSuccessStatusCode)
                    {
                        googleSearchResult = await response.Content.ReadAsAsync<GoogleBook>();
                        googleSearchBooksResult.Add(googleSearchResult);
                    }
                }
            }

            using (var db = new BookPoolEntities())
            {
                ViewBag.SearchBooks = (from googleBooks in googleSearchBooksResult
                                       join searchBooks in db.SearchForBooks on googleBooks.id equals searchBooks.GoogleID
                                          join userToSearch in db.AspNetUsers on searchBooks.AspNetUserID equals userToSearch.Id

                                       select new BookPoolResult
                                          {
                                              ID = searchBooks.ID,
                                              BookName = googleBooks.volumeInfo.title,
                                              GoogleID = googleBooks.id,
                                              Authors = googleBooks.volumeInfo.authors,
                                              AverageRating = googleBooks.volumeInfo.averageRating,
                                              Categories = googleBooks.volumeInfo.categories,
                                              Description = googleBooks.volumeInfo.description,
                                              ImageURL = googleBooks.volumeInfo.imageLinks.thumbnail.Replace("http:", "https:"),
                                              PageCount = googleBooks.volumeInfo.pageCount,
                                              PreviewLink = googleBooks.volumeInfo.previewLink,
                                              PrintType = googleBooks.volumeInfo.printType,
                                              PublishedDate = googleBooks.volumeInfo.publishedDate,
                                              Publisher = googleBooks.volumeInfo.publisher,
                                              Subtitle = googleBooks.volumeInfo.subtitle,
                                              OwnerUserName = userToSearch.Email,
                                              OwnerPhoneNumber = userToSearch.PhoneNumber
                                          }).DistinctBy(x => x.ID).ToList();
            }

            using (var db = new BookPoolEntities())
            {
                ViewBag.Orders = (from orders in db.OrderHeaders 
                                 join details in db.OrderDetails on orders.ID equals details.OrderHeaderID
                                 join userSeller in db.AspNetUsers on details.SellerUserID equals userSeller.Id
                                 join userBuyer in db.AspNetUsers on orders.ClientUserID equals userBuyer.Id
                                 select new OrderResult
                                 {
                                     BookName = details.BookName,
                                     BookPrice = details.BookPrice,
                                     BuyerAddress = "",
                                     BuyerPhoneNumber = userBuyer.PhoneNumber,
                                     BuyerUserName = userBuyer.UserName,
                                     SellerUserName = userSeller.UserName,
                                     SellerPhoneNumber = userSeller.PhoneNumber,
                                     OrderID = orders.ID,
                                     TotalPrice = orders.TotalPrice
                                 }).ToList();
            }



            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}