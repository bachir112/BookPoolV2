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
        public async Task<JsonResult<Object>> GetBooks(string query = null, 
            string language = null, 
            Nullable<int> BookID = null,
            bool sortByPrice = false, 
            bool AvailableOnly = false, 
            bool sortByPopularity = false)
        {
            List<BookPoolResult> results = new List<BookPoolResult>();

            GoogleBooksResult googleResult = new GoogleBooksResult();
            List<GoogleBook> googleBooksResult = new List<GoogleBook>();
            List<GoogleBook> googleBooksNotAvailableResult = new List<GoogleBook>();

            List<BookPoolResult> dbResult = new List<BookPoolResult>();

            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(DataObjects.Global.Globals.googleBaseAPI);
                        StringBuilder httpRoute = new StringBuilder();
                        httpRoute.Append("?");
                        httpRoute.AppendFormat("q={0}", query);
                        httpRoute.Append("&");
                        httpRoute.Append("maxResults=40");
                        if(language != null)
                        {
                            string lan = "en";
                            switch (language)
                            {
                                case "english":
                                    lan = "en";
                                    break;
                                case "french":
                                    lan = "fr";
                                    break;
                                case "arabic":
                                    lan = "ar";
                                    break;
                                default:
                                    break;
                            }
                            httpRoute.Append("&");
                            httpRoute.AppendFormat("langRestrict={0}", lan);
                        }

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
                                    }).DistinctBy(x => x.ID).ToList();

                        if(dbResult.Count() == 0 && BookID != null)
                        {
                            int findBookID = Convert.ToInt32(BookID);
                            string googleBookID = db.AvailableBooks.FirstOrDefault(x => x.ID == findBookID).GoogleID;

                            using (var client = new HttpClient())
                            {
                                client.BaseAddress = new Uri(DataObjects.Global.Globals.googleBaseAPI_SearchByID);
                                StringBuilder httpRoute = new StringBuilder();
                                httpRoute.Append(googleBookID);

                                var response = await client.GetAsync(httpRoute.ToString());
                                if (response.IsSuccessStatusCode)
                                {
                                    var googleIDResult = await response.Content.ReadAsAsync<GoogleBook>();
                                    List<GoogleBook> googleBooksIDResult = new List<GoogleBook>();
                                    googleBooksIDResult.Add(googleIDResult);

                                    dbResult = (from googleBooks in googleBooksIDResult
                                                join availableBooks in db.AvailableBooks on googleBooks.id equals availableBooks.GoogleID
                                                join categories in db.Categories on availableBooks.CategoryID equals categories.ID
                                                join conditions in db.Conditions on availableBooks.BookConditionID equals conditions.ID
                                                join languages in db.Languages on availableBooks.BookLanguageID equals languages.ID
                                                join users in db.AspNetUsers on availableBooks.OwnerUserID equals users.Id
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
                                                }).DistinctBy(x => x.ID).ToList();
                                }
                            }
                        }
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

                if (string.IsNullOrEmpty(query))
                {
                    List<string> bookNames = new List<string>();
                    List<string> myGoogleBooksIDs = new List<string>();
                    using (var db = new BookPoolEntities())
                    {
                        myGoogleBooksIDs = db.AvailableBooks.OrderByDescending(x => x.ID).Select(x => x.GoogleID).ToList();
                        bookNames = db.AvailableBooks.OrderByDescending(x => x.ID).Select(x => x.BookName).ToList();
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
                                var googleBookResult = await response.Content.ReadAsAsync<GoogleBook>();
                                googleBooksResult.Add(googleBookResult);
                            }
                        }
                    }

                    foreach (var bookName in bookNames)
                    {
                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(DataObjects.Global.Globals.googleBaseAPI);
                            StringBuilder httpRoute = new StringBuilder();
                            httpRoute.Append("?");
                            httpRoute.AppendFormat("q={0}", bookName);
                            httpRoute.Append("&");
                            httpRoute.Append("maxResults=40");
                            if (language != null)
                            {
                                string lan = "en";
                                switch (language)
                                {
                                    case "english":
                                        lan = "en";
                                        break;
                                    case "french":
                                        lan = "fr";
                                        break;
                                    case "arabic":
                                        lan = "ar";
                                        break;
                                    default:
                                        break;
                                }
                                httpRoute.Append("&");
                                httpRoute.AppendFormat("langRestrict={0}", lan);
                            }

                            var response = await client.GetAsync(httpRoute.ToString());
                            if (response.IsSuccessStatusCode)
                            {
                                googleResult = await response.Content.ReadAsAsync<GoogleBooksResult>();
                                googleBooksNotAvailableResult.AddRange(googleResult.items.ToList<GoogleBook>());
                                googleBooksNotAvailableResult = googleBooksNotAvailableResult.Where(x => x.volumeInfo.authors != null).Select(x => x).ToList();
                            }
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
                                        SellingStatus = availableBooks.SellingStatus,
                                        BookLanguage = languages.LanguageName,
                                        PostedOn = availableBooks.PostedOn
                                    }).DistinctBy(x => x.ID).ToList();

                        if (language != null)
                        {
                            dbResult = dbResult.Where(x => x.BookLanguage.ToLower() == language.ToLower()).Select(x => x).ToList();
                        }

                        if (sortByPrice == true)
                        {
                            dbResult = dbResult.OrderBy(x => x.Price).Select(x => x).ToList();
                        }

                        //if (sortByPopularity == true)
                        //{
                        //    dbResult = dbResult.OrderByDescending(x => x.AverageRating).Select(x => x).ToList();
                        //}
                    }

                    var unavailableBooks = googleBooksNotAvailableResult.Where(x => !dbResult.Select(r => r.GoogleID).Contains(x.id)).Select(x => x).ToList();

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

                    //if (sortByPopularity == true)
                    //{
                    //    googleUnavailableBooks = googleUnavailableBooks.OrderByDescending(x => x.AverageRating).Select(x => x).ToList();
                    //}

                    if (AvailableOnly)
                    {
                        dbResult = dbResult.Where(x => x.SellingStatus == Global.Globals.BookSellingStatus_Available).Select(x => x).ToList();
                        results.AddRange(dbResult);
                    }
                    else
                    {
                        results.AddRange(dbResult);
                        results.AddRange(googleUnavailableBooks);
                    }

                }

                if (sortByPopularity == true)
                {
                    results = results.OrderByDescending(x => x.AverageRating).Select(x => x).ToList();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { results });
        }


        [System.Web.Http.Route("api/Books/GetAcademicBooks")]
        [System.Web.Http.HttpGet]
        public async Task<JsonResult<Object>> GetAcademicBooks(Nullable<int> Institution, Nullable<int> Course,
            string language = null,
            Nullable<int> BookID = null,
            bool sortByPrice = false,
            bool AvailableOnly = false,
            bool sortByPopularity = false)
        {
            List<BookPoolResult> results = new List<BookPoolResult>();

            GoogleBook googleResult = new GoogleBook();
            List<GoogleBook> googleBooksResult = new List<GoogleBook>();
            List<GoogleBook> googleBooksNotAvailableResult = new List<GoogleBook>();

            List<BookPoolResult> dbResult = new List<BookPoolResult>();

            try
            {
                string categoryName = string.Empty;
                List<string> myGoogleBooksIDs = new List<string>();
                using (var db = new BookPoolEntities())
                {
                    if (Institution == null)
                    {
                        myGoogleBooksIDs = db.AvailableBooks.Where(x => x.Academic == true).Select(x => x.GoogleID).ToList();
                    }

                    if (Institution != null && Course == null)
                    {
                        if(Institution == -1)
                        {
                            myGoogleBooksIDs = db.AvailableBooks.Where(x => x.Academic == true && x.Institution == "university").Select(x => x.GoogleID).ToList();
                        }
                        else if(Institution == -2)
                        {
                            myGoogleBooksIDs = db.AvailableBooks.Where(x => x.Academic == true && x.Institution == "school").Select(x => x.GoogleID).ToList();
                        }
                        else
                        {
                            myGoogleBooksIDs = db.AvailableBooks.Where(x => x.Academic == true && x.InstitutionID == Institution).Select(x => x.GoogleID).ToList();
                        }
                    }

                    if (Institution == null && Course != null)
                    {
                        myGoogleBooksIDs = db.AvailableBooks.Where(x => x.Academic == true && x.CourseID == Course).Select(x => x.GoogleID).ToList();
                    }

                    if(Institution != null && Course != null)
                    {
                        myGoogleBooksIDs = db.AvailableBooks.Where(x => x.Academic == true 
                                                                     && x.InstitutionID == Institution
                                                                     && x.CourseID == Course).Select(x => x.GoogleID).ToList();
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
                                    BookLanguage = languages.LanguageName,
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
                                }).DistinctBy(x => x.ID).ToList();
                }

                if (language != null)
                {
                    dbResult = dbResult.Where(x => x.BookLanguage.ToLower() == language.ToLower()).Select(x => x).ToList();
                }

                if (sortByPrice == true)
                {
                    dbResult = dbResult.OrderBy(x => x.Price).Select(x => x).ToList();
                }

                if (AvailableOnly)
                {
                    dbResult = dbResult.Where(x => x.SellingStatus == Global.Globals.BookSellingStatus_Available).Select(x => x).ToList();
                }

                if (sortByPopularity == true)
                {
                    dbResult = dbResult.OrderByDescending(x => x.AverageRating).Select(x => x).ToList();
                }

                results.AddRange(dbResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            return Json((object)new { results });
        }


        [System.Web.Http.Route("api/Books/FindMyBook")]
        [System.Web.Http.HttpGet]
        public async Task<JsonResult<Object>> FindMyBook(string query = null)
        {
            List<BookPoolResult> results = new List<BookPoolResult>();

            GoogleBooksResult googleResult = new GoogleBooksResult();
            List<GoogleBook> googleBooksResult = new List<GoogleBook>();
            List<GoogleBook> googleBooksNotAvailableResult = new List<GoogleBook>();

            try
            {
                if (!string.IsNullOrEmpty(query))
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

                    List<BookPoolResult> googleUnavailableBooks = googleBooksResult.Select(x => new BookPoolResult
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

                    results.AddRange(googleUnavailableBooks);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { results });
        }



        [System.Web.Http.Route("api/Books/GetBooksInCategory")]
        [System.Web.Http.HttpGet]
        public async Task<JsonResult<Object>> GetBooksInCategory(int CategoryID,
            string language = null,
            bool sortByPrice = false,
            bool AvailableOnly = false,
            bool sortByPopularity = false)
        {
            List<BookPoolResult> results = new List<BookPoolResult>();

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
                                }).DistinctBy(x => x.ID).ToList();
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
                        if (language != null)
                        {
                            string lan = "en";
                            switch (language)
                            {
                                case "english":
                                    lan = "en";
                                    break;
                                case "french":
                                    lan = "fr";
                                    break;
                                case "arabic":
                                    lan = "ar";
                                    break;
                                default:
                                    break;
                            }
                            httpRoute.Append("&");
                            httpRoute.AppendFormat("langRestrict={0}", lan);
                        }

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

                if (language != null)
                {
                    dbResult = dbResult.Where(x => x.BookLanguage.ToLower() == language.ToLower()).Select(x => x).ToList();
                }

                if (sortByPrice == true)
                {
                    dbResult = dbResult.OrderBy(x => x.Price).Select(x => x).ToList();
                }



                if (AvailableOnly)
                {
                    dbResult = dbResult.Where(x => x.SellingStatus == Global.Globals.BookSellingStatus_Available).Select(x => x).ToList();
                    results.AddRange(dbResult);
                }
                else
                {
                    results.AddRange(dbResult);
                    results.AddRange(googleUnavailableBooks);
                }


                if (sortByPopularity == true)
                {
                    results = results.OrderByDescending(x => x.AverageRating).Select(x => x).ToList();
                }

                //results.AddRange(dbResult);
                //results.AddRange(googleUnavailableBooks);
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
                                    SellingStatus = availableBooks.SellingStatus,
                                    PostedOn = availableBooks.PostedOn
                                }).DistinctBy(x => x.ID).ToList();
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
        public JsonResult<Object> SellMyBook(string UserID, 
                                             string GoogleID, 
                                             string BookName, 
                                             string Price, 
                                             string institutionType,
                                             int Condition, 
                                             int Category, 
                                             int Language, 
                                             bool Academic,
                                             Nullable<int> Institution = null,
                                             Nullable<int> Course = null)
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
                availableBook.PostedOn = DateTime.UtcNow;
                availableBook.InstitutionID = Institution;
                availableBook.CourseID = Course;
                availableBook.Institution = institutionType;

                using (var db = new BookPoolEntities())
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
                        List<int> booksIDsInCart = new List<int>();
                        if (!string.IsNullOrEmpty(userCart.BooksIDsCSV))
                        {
                            booksIDsInCart = userCart.BooksIDsCSV.Split(',').Select(int.Parse).ToList();
                        }

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

            List<int> booksIDsInCart = new List<int>();

            try
            {
                List<string> myGoogleBooksIDs = new List<string>();
                using (var db = new BookPoolEntities())
                {
                    string booksCSV = db.UserCarts.FirstOrDefault(x => x.UserID == UserID)?.BooksIDsCSV;
                    if(booksCSV != null)
                    {
                        booksIDsInCart = booksCSV.Split(',').Select(int.Parse).ToList();
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
                                where booksIDsInCart.Contains(availableBooks.ID)
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
                                }).DistinctBy(x => x.ID).ToList();
                }

                //var unavailableBooks = googleBooksResult.Where(x => !dbResult.Select(r => r.GoogleID).Contains(x.id)).Select(x => x).ToList();

                //List<BookPoolResult> googleUnavailableBooks = unavailableBooks.Select(x => new BookPoolResult
                //{
                //    Academic = false,
                //    BookConditionID = -1,
                //    BookLanguageID = -1,
                //    BookName = x.volumeInfo.title,
                //    CategoryID = -1,
                //    GoogleID = x.id,
                //    OwnerUserID = null,
                //    Price = 0,
                //    Authors = x.volumeInfo.authors,
                //    AverageRating = x.volumeInfo.averageRating,
                //    Categories = x.volumeInfo.categories,
                //    Description = x.volumeInfo.description,
                //    ImageURL = x.volumeInfo.imageLinks == null ? null : x.volumeInfo.imageLinks.thumbnail,
                //    PageCount = x.volumeInfo.pageCount,
                //    PreviewLink = x.volumeInfo.previewLink,
                //    PrintType = x.volumeInfo.printType,
                //    PublishedDate = x.volumeInfo.publishedDate,
                //    Publisher = x.volumeInfo.publisher,
                //    Subtitle = x.volumeInfo.subtitle,
                //    SellingStatus = Global.Globals.BookSellingStatus_NotAvailable
                //}).ToList();

                results.AddRange(dbResult);
                //results.AddRange(googleUnavailableBooks);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { results });
        }


        [System.Web.Http.Route("api/Books/GetMyOrdersHeaders")]
        [System.Web.Http.HttpGet]
        public JsonResult<Object> GetMyOrdersHeaders(string UserID)
        {
            List<object> results = new List<object>();
            List<OrderHeader> orderHeader = new List<OrderHeader>();

            try
            {
                using (var db = new BookPoolEntities())
                {
                    orderHeader = db.OrderHeaders.Where(x => x.ClientUserID == UserID).Select(x => x).OrderByDescending(x => x.OrderedOn).ToList();
                    results.AddRange(orderHeader);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { results });
        }


        [System.Web.Http.Route("api/Books/GetMyOrdersDetails")]
        [System.Web.Http.HttpGet]
        public JsonResult<Object> GetMyOrdersDetails(string OrderHeadersCSV)
        {
            List<object> results = new List<object>();
            List<OrderDetail> orderDetails = new List<OrderDetail>();

            try
            {
                using (var db = new BookPoolEntities())
                {
                    List<int> ordersHeaders = OrderHeadersCSV.Split(',').Select(int.Parse).ToList();
                    orderDetails = db.OrderDetails.Where(x => ordersHeaders.Contains(x.OrderHeaderID)).Select(x => x).ToList();
                    results.AddRange(orderDetails);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { results });
        }

    }
}
