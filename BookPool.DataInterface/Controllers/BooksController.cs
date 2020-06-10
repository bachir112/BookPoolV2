using BookPool.DataObjects.DTO;
using BookPool.DataObjects.EDM;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace BookPool.DataInterface.Controllers
{
    public class BooksController : ApiController
    {
        [System.Web.Http.Route("api/Books/GetBooks")]
        [System.Web.Http.HttpGet]
        public async Task<JsonResult<Object>> GetBooks(string query = "an economist walks into")
        {
            List<object> results = new List<object>();

            GoogleBooksResult googleResult = new GoogleBooksResult();
            List<GoogleBook> googleBooksResult = new List<GoogleBook>();

            List<BookPoolResult> dbResult = new List<BookPoolResult>();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(DataObjects.Global.Globals.googleBaseAPI);
                    StringBuilder httpRoute = new StringBuilder();
                    httpRoute.Append("?");
                    httpRoute.AppendFormat("q={0}", query);
                    httpRoute.Append("&");
                    httpRoute.Append("maxResults=40");

                    var response = await client.GetAsync(httpRoute.ToString());
                    if (response.IsSuccessStatusCode)
                    {
                        googleResult = await response.Content.ReadAsAsync<GoogleBooksResult>();
                        googleBooksResult = googleResult.items.ToList<GoogleBook>();
                        googleBooksResult = googleBooksResult.Where(x => x.volumeInfo.authors != null).Select(x => x).ToList();
                    }
                }

                using (var db = new BookPoolEntities())
                {
                    dbResult = (from googleBooks in googleBooksResult
                                join availableBooks in db.AvailableBooks on googleBooks.id equals availableBooks.GoogleID
                                join categories in db.Categories on availableBooks.CategoryID equals categories.ID
                                join conditions in db.Conditions on availableBooks.BookConditionID equals conditions.ID
                                join languages in db.Languages on availableBooks.BookLanguageID equals languages.ID
                                join users in db.AspNetUsers on availableBooks.OwnerUserID equals users.Id
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
                                    OwnerUser = users.UserName,
                                    Price = availableBooks.Price,
                                    Authors = googleBooks.volumeInfo.authors,
                                    AverageRating = googleBooks.volumeInfo.averageRating,
                                    Categories = googleBooks.volumeInfo.categories,
                                    Description = googleBooks.volumeInfo.description,
                                    ImageURL = googleBooks.volumeInfo.imageLinks.thumbnail,
                                    PageCount = googleBooks.volumeInfo.pageCount,
                                    PreviewLink = googleBooks.volumeInfo.previewLink,
                                    PrintType = googleBooks.volumeInfo.printType,
                                    PublishedDate = googleBooks.volumeInfo.publishedDate,
                                    Publisher = googleBooks.volumeInfo.publisher,
                                    Subtitle = googleBooks.volumeInfo.subtitle,
                                    BookCondition = conditions.ConditionName,
                                    BookLanguage = languages.LanguageName,
                                    Category = categories.CategoryName,
                                    SellingStatus = availableBooks.SellingStatus
                                }).ToList();
                }

                var unavailableBooks = googleBooksResult.Where(x => !dbResult.Select(r => r.GoogleID).Contains(x.id)).Select(x => x).ToList();

                List<BookPoolResult> googleUnavailableBooks = unavailableBooks.Select(x => new BookPoolResult
                {
                    Academic = false,
                    BookConditionID = -1,
                    BookLanguageID = -1,
                    BookName = x.volumeInfo.title,
                    CategoryID = -1,
                    GoogleID = x.id,
                    OwnerUserID = null,
                    Price = -1,
                    Authors = x.volumeInfo.authors,
                    AverageRating = x.volumeInfo.averageRating,
                    Categories = x.volumeInfo.categories,
                    Description = x.volumeInfo.description,
                    ImageURL = x.volumeInfo.imageLinks == null ? null : x.volumeInfo.imageLinks.thumbnail,
                    PageCount = x.volumeInfo.pageCount,
                    PreviewLink = x.volumeInfo.previewLink,
                    PrintType = x.volumeInfo.printType,
                    PublishedDate = x.volumeInfo.publishedDate,
                    Publisher = x.volumeInfo.publisher,
                    Subtitle = x.volumeInfo.subtitle
                }).ToList();

                results.AddRange(dbResult);
                results.AddRange(googleUnavailableBooks);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { results });
        }


        [System.Web.Http.Route("api/Books/GetBooksInCategory")]
        [System.Web.Http.HttpGet]
        public async Task<JsonResult<Object>> GetBooksInCategory(int CategoryID)
        {
            List<object> results = new List<object>();

            GoogleBook googleResult = new GoogleBook();
            List<GoogleBook> googleBooksResult = new List<GoogleBook>();

            List<BookPoolResult> dbResult = new List<BookPoolResult>();

            try
            {
                string categoryName = string.Empty;
                List<string> myGoogleBooksIDs = new List<string>();
                using (var db = new BookPoolEntities())
                {
                    myGoogleBooksIDs = db.AvailableBooks.Where(x => x.CategoryID == CategoryID).Select(x => x.GoogleID).ToList();
                    categoryName = db.Categories.FirstOrDefault(x => x.ID == CategoryID)?.CategoryName;
                }

                foreach (var googleID in myGoogleBooksIDs)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(DataObjects.Global.Globals.googleBaseAPI_SearchByID);
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
                    dbResult = (from googleBooks in googleBooksResult
                                join availableBooks in db.AvailableBooks on googleBooks.id equals availableBooks.GoogleID

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
                                    ImageURL = googleBooks.volumeInfo.imageLinks.thumbnail,
                                    PageCount = googleBooks.volumeInfo.pageCount,
                                    PreviewLink = googleBooks.volumeInfo.previewLink,
                                    PrintType = googleBooks.volumeInfo.printType,
                                    PublishedDate = googleBooks.volumeInfo.publishedDate,
                                    Publisher = googleBooks.volumeInfo.publisher,
                                    Subtitle = googleBooks.volumeInfo.subtitle,
                                    SellingStatus = availableBooks.SellingStatus
                                }).ToList();
                }


                List<GoogleBook> ListGoogleBooksUnavailableResult = new List<GoogleBook>();
                int startIndex = 0;
                while (ListGoogleBooksUnavailableResult.Count() < 10)
                {
                    List<GoogleBook> googleBooksUnavailableResult = new List<GoogleBook>();
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(DataObjects.Global.Globals.googleBaseAPI);
                        StringBuilder httpRoute = new StringBuilder();
                        httpRoute.Append("?");
                        httpRoute.AppendFormat("q={0}", categoryName);
                        httpRoute.Append("&");
                        httpRoute.Append("maxResults=40");
                        httpRoute.Append("&");
                        httpRoute.AppendFormat("startIndex={0}", startIndex);

                        var response = await client.GetAsync(httpRoute.ToString());
                        if (response.IsSuccessStatusCode)
                        {
                            var googleCategoryResult = await response.Content.ReadAsAsync<GoogleBooksResult>();
                            googleBooksUnavailableResult = googleCategoryResult.items.ToList<GoogleBook>();
                            googleBooksUnavailableResult = googleBooksUnavailableResult.Where(x => x.volumeInfo.authors != null).Select(x => x).ToList();
                        }
                    }

                    ListGoogleBooksUnavailableResult.AddRange(googleBooksUnavailableResult);

                    startIndex += 40;
                }

                var unavailableBooks = ListGoogleBooksUnavailableResult.Where(x => !dbResult.Select(r => r.GoogleID).Contains(x.id)).Select(x => x).ToList();

                List<BookPoolResult> googleUnavailableBooks = unavailableBooks.Select(x => new BookPoolResult
                {
                    Academic = false,
                    BookConditionID = -1,
                    BookLanguageID = -1,
                    BookName = x.volumeInfo.title,
                    CategoryID = -1,
                    GoogleID = x.id,
                    OwnerUserID = null,
                    Price = -1,
                    Authors = x.volumeInfo.authors,
                    AverageRating = x.volumeInfo.averageRating,
                    Categories = x.volumeInfo.categories,
                    Description = x.volumeInfo.description,
                    ImageURL = x.volumeInfo.imageLinks == null ? null : x.volumeInfo.imageLinks.thumbnail,
                    PageCount = x.volumeInfo.pageCount,
                    PreviewLink = x.volumeInfo.previewLink,
                    PrintType = x.volumeInfo.printType,
                    PublishedDate = x.volumeInfo.publishedDate,
                    Publisher = x.volumeInfo.publisher,
                    Subtitle = x.volumeInfo.subtitle
                }).ToList();

                results.AddRange(dbResult);
                results.AddRange(googleUnavailableBooks);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { results });
        }


        [System.Web.Http.Route("api/Books/GetMyBooks")]
        [System.Web.Http.HttpGet]
        public async Task<JsonResult<Object>> GetMyBooks(string UserID)
        {
            List<object> results = new List<object>();

            GoogleBook googleResult = new GoogleBook();
            List<GoogleBook> googleBooksResult = new List<GoogleBook>();

            List<BookPoolResult> dbResult = new List<BookPoolResult>();

            try
            {
                List<string> myGoogleBooksIDs = new List<string>();
                using (var db = new BookPoolEntities())
                {
                    myGoogleBooksIDs = db.AvailableBooks.Where(x => x.OwnerUserID == UserID).Select(x => x.GoogleID).ToList();
                }

                foreach(var googleID in myGoogleBooksIDs)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(DataObjects.Global.Globals.googleBaseAPI_SearchByID);
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
                    dbResult = (from googleBooks in googleBooksResult
                                join availableBooks in db.AvailableBooks on googleBooks.id equals availableBooks.GoogleID

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
                                    ImageURL = googleBooks.volumeInfo.imageLinks.thumbnail,
                                    PageCount = googleBooks.volumeInfo.pageCount,
                                    PreviewLink = googleBooks.volumeInfo.previewLink,
                                    PrintType = googleBooks.volumeInfo.printType,
                                    PublishedDate = googleBooks.volumeInfo.publishedDate,
                                    Publisher = googleBooks.volumeInfo.publisher,
                                    Subtitle = googleBooks.volumeInfo.subtitle,
                                    SellingStatus = availableBooks.SellingStatus
                                }).ToList();
                }

                var unavailableBooks = googleBooksResult.Where(x => !dbResult.Select(r => r.GoogleID).Contains(x.id)).Select(x => x).ToList();

                List<BookPoolResult> googleUnavailableBooks = unavailableBooks.Select(x => new BookPoolResult
                {
                    Academic = false,
                    BookConditionID = -1,
                    BookLanguageID = -1,
                    BookName = x.volumeInfo.title,
                    CategoryID = -1,
                    GoogleID = x.id,
                    OwnerUserID = null,
                    Price = -1,
                    Authors = x.volumeInfo.authors,
                    AverageRating = x.volumeInfo.averageRating,
                    Categories = x.volumeInfo.categories,
                    Description = x.volumeInfo.description,
                    ImageURL = x.volumeInfo.imageLinks == null ? null : x.volumeInfo.imageLinks.thumbnail,
                    PageCount = x.volumeInfo.pageCount,
                    PreviewLink = x.volumeInfo.previewLink,
                    PrintType = x.volumeInfo.printType,
                    PublishedDate = x.volumeInfo.publishedDate,
                    Publisher = x.volumeInfo.publisher,
                    Subtitle = x.volumeInfo.subtitle
                }).ToList();

                results.AddRange(dbResult);
                results.AddRange(googleUnavailableBooks);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { results });
        }


        [System.Web.Http.Route("api/Books/SellMyBook")]
        [System.Web.Http.HttpGet]
        public JsonResult<Object> SellMyBook(string UserID, string GoogleID, string BookName, string Price, int Condition, int Category, int Language, bool Academic)
        {
            bool result = false;

            try
            {
                AvailableBook availableBook = new AvailableBook();
                availableBook.OwnerUserID = UserID;
                availableBook.GoogleID = GoogleID;
                availableBook.BookName = BookName;
                availableBook.Price = Convert.ToDecimal(Price);
                availableBook.CategoryID = Category;
                availableBook.BookConditionID = Condition;
                availableBook.BookLanguageID = Language;
                availableBook.Academic = Academic;
                availableBook.SellingStatus = Global.Globals.BookSellingStatus_Available;

                using(var db = new BookPoolEntities())
                {
                    db.AvailableBooks.Add(availableBook);
                    db.SaveChanges();
                }

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { result });
        }


        [System.Web.Http.Route("api/Books/DeleteMyBook")]
        [System.Web.Http.HttpGet]
        public JsonResult<Object> DeleteMyBook(string UserID, int MyBookID)
        {
            bool result = false;

            try
            {
                using (var db = new BookPoolEntities())
                {
                    AvailableBook availableBook = db.AvailableBooks.First(x => x.OwnerUserID == UserID && x.ID == MyBookID);
                    db.AvailableBooks.Remove(availableBook);
                    db.SaveChanges();
                }

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { result });
        }



        [System.Web.Http.Route("api/Books/DeleteSearchForBook")]
        [System.Web.Http.HttpGet]
        public JsonResult<Object> DeleteSearchForBook(string UserID, int MyBookID)
        {
            bool result = false;

            try
            {
                using (var db = new BookPoolEntities())
                {
                    string goolgleBookID = db.SearchForBooks.First(x => x.AspNetUserID == UserID && x.ID == MyBookID).GoogleID;
                    List<SearchForBook> searchForBooks = db.SearchForBooks.Where(x => x.AspNetUserID == UserID && x.GoogleID == goolgleBookID).Select(x => x).ToList();
                    db.SearchForBooks.RemoveRange(searchForBooks);
                    db.SaveChanges();
                }

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { result });
        }


        [System.Web.Http.Route("api/Books/SearchForThisBook")]
        [System.Web.Http.HttpGet]
        public JsonResult<Object> SearchForThisBook(string UserID, string GoogleID)
        {
            bool result = false;

            try
            {
                using (var db = new BookPoolEntities())
                {
                    SearchForBook searchForBook = new SearchForBook();
                    searchForBook.AspNetUserID = UserID;
                    searchForBook.GoogleID = GoogleID;
                    db.SearchForBooks.Add(searchForBook);
                    db.SaveChanges();
                }

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { result });
        }


        [System.Web.Http.Route("api/Books/GetBooksForSearch")]
        [System.Web.Http.HttpGet]
        public async Task<JsonResult<object>> GetBooksForSearch(string UserID)
        {
            GoogleBook googleResult = new GoogleBook();
            List<GoogleBook> googleBooksResult = new List<GoogleBook>();

            List<BookPoolResult> results = new List<BookPoolResult>();

            try
            {
                List<string> myGoogleBooksIDs = new List<string>();
                using (var db = new BookPoolEntities())
                {
                    myGoogleBooksIDs = db.SearchForBooks.Where(x => x.AspNetUserID == UserID).Select(x => x.GoogleID).ToList();
                }

                foreach (var googleID in myGoogleBooksIDs)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(DataObjects.Global.Globals.googleBaseAPI_SearchByID);
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
                    results = (from googleBooks in googleBooksResult
                               join searchForBooks in db.SearchForBooks on googleBooks.id equals searchForBooks.GoogleID
                               select new BookPoolResult
                                {
                                   ID = searchForBooks.ID,
                                   BookName = googleBooks.volumeInfo.title,
                                   Authors = googleBooks.volumeInfo.authors,
                                   AverageRating = googleBooks.volumeInfo.averageRating,
                                   Categories = googleBooks.volumeInfo.categories,
                                   Description = googleBooks.volumeInfo.description,
                                   ImageURL = googleBooks.volumeInfo.imageLinks.thumbnail,
                                   PageCount = googleBooks.volumeInfo.pageCount,
                                   PreviewLink = googleBooks.volumeInfo.previewLink,
                                   PrintType = googleBooks.volumeInfo.printType,
                                   PublishedDate = googleBooks.volumeInfo.publishedDate,
                                   Publisher = googleBooks.volumeInfo.publisher,
                                   Subtitle = googleBooks.volumeInfo.subtitle
                               }).DistinctBy(x => x.BookName).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { results });
        }


        [System.Web.Http.Route("api/Books/AddBookToCart")]
        [System.Web.Http.HttpGet]
        public JsonResult<Object> AddBookToCart(string UserID, int BookID)
        {
            bool results = false;

            try
            {
                using (var db = new BookPoolEntities())
                {
                    UserCart userCart = db.UserCarts.FirstOrDefault(x => x.UserID == UserID);

                    if (userCart != null)
                    {
                        List<int> booksIDsInCart = userCart.BooksIDsCSV.Split(',').Select(int.Parse).ToList();
                        if (!booksIDsInCart.Contains(BookID))
                        {
                            booksIDsInCart.Add(BookID);

                            userCart.BooksIDsCSV = string.Join(",", booksIDsInCart);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        userCart = new UserCart();
                        userCart.UserID = UserID;
                        userCart.BooksIDsCSV = BookID.ToString();

                        db.UserCarts.Add(userCart);
                        db.SaveChanges();
                    }
                }

                results = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { results });
        }


        [System.Web.Http.Route("api/Books/GetCart")]
        [System.Web.Http.HttpGet]
        public JsonResult<Object> GetCart(string UserID)
        {
            List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> userCookieCart = new List<Dictionary<string, string>>();
            try
            {
                using (var db = new BookPoolEntities())
                {
                    UserCart userCart = db.UserCarts.FirstOrDefault(x => x.UserID == UserID);
                    if(userCart != null)
                    {
                        List<int> booksIDsInCart = userCart.BooksIDsCSV.Split(',').Select(int.Parse).ToList();
                        foreach (var bookID in booksIDsInCart)
                        {
                            AvailableBook availableBook = db.AvailableBooks.FirstOrDefault(x => x.ID == bookID && x.SellingStatus == Global.Globals.BookSellingStatus_Available);
                            if (availableBook != null)
                            {
                                Dictionary<string, string> bookInCookie = new Dictionary<string, string>();
                                bookInCookie.Add("BookID", bookID.ToString());
                                bookInCookie.Add("Price", availableBook.Price.ToString());

                                userCookieCart.Add(bookInCookie);
                            }
                        }
                    }
                    results.AddRange(userCookieCart);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { results });
        }



        [System.Web.Http.Route("api/Books/GetBooksInCart")]
        [System.Web.Http.HttpGet]
        public async Task<JsonResult<Object>> GetBooksInCart(string UserID)
        {
            List<object> results = new List<object>();

            GoogleBook googleResult = new GoogleBook();
            List<GoogleBook> googleBooksResult = new List<GoogleBook>();

            List<BookPoolResult> dbResult = new List<BookPoolResult>();

            try
            {
                List<string> myGoogleBooksIDs = new List<string>();
                using (var db = new BookPoolEntities())
                {
                    string booksCSV = db.UserCarts.FirstOrDefault(x => x.UserID == UserID)?.BooksIDsCSV;
                    if(booksCSV != null)
                    {
                        List<int> booksIDsInCart = booksCSV.Split(',').Select(int.Parse).ToList();
                        myGoogleBooksIDs = db.AvailableBooks.Where(x => booksIDsInCart.Contains(x.ID)).Select(x => x.GoogleID).ToList();
                    }
                }

                foreach (var googleID in myGoogleBooksIDs)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(DataObjects.Global.Globals.googleBaseAPI_SearchByID);
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
                    dbResult = (from googleBooks in googleBooksResult
                                join availableBooks in db.AvailableBooks on googleBooks.id equals availableBooks.GoogleID
                                where availableBooks.SellingStatus == Global.Globals.BookSellingStatus_Available
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
                                    ImageURL = googleBooks.volumeInfo.imageLinks.thumbnail,
                                    PageCount = googleBooks.volumeInfo.pageCount,
                                    PreviewLink = googleBooks.volumeInfo.previewLink,
                                    PrintType = googleBooks.volumeInfo.printType,
                                    PublishedDate = googleBooks.volumeInfo.publishedDate,
                                    Publisher = googleBooks.volumeInfo.publisher,
                                    Subtitle = googleBooks.volumeInfo.subtitle
                                }).ToList();
                }

                var unavailableBooks = googleBooksResult.Where(x => !dbResult.Select(r => r.GoogleID).Contains(x.id)).Select(x => x).ToList();

                List<BookPoolResult> googleUnavailableBooks = unavailableBooks.Select(x => new BookPoolResult
                {
                    Academic = false,
                    BookConditionID = -1,
                    BookLanguageID = -1,
                    BookName = x.volumeInfo.title,
                    CategoryID = -1,
                    GoogleID = x.id,
                    OwnerUserID = null,
                    Price = -1,
                    Authors = x.volumeInfo.authors,
                    AverageRating = x.volumeInfo.averageRating,
                    Categories = x.volumeInfo.categories,
                    Description = x.volumeInfo.description,
                    ImageURL = x.volumeInfo.imageLinks == null ? null : x.volumeInfo.imageLinks.thumbnail,
                    PageCount = x.volumeInfo.pageCount,
                    PreviewLink = x.volumeInfo.previewLink,
                    PrintType = x.volumeInfo.printType,
                    PublishedDate = x.volumeInfo.publishedDate,
                    Publisher = x.volumeInfo.publisher,
                    Subtitle = x.volumeInfo.subtitle
                }).ToList();

                results.AddRange(dbResult);
                results.AddRange(googleUnavailableBooks);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { results });
        }

    }
}
