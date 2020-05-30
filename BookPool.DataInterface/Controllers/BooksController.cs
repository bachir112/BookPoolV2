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
                        //googleBooksResult = googleBooksResult.Where(x => x.volumeInfo.industryIdentifiers.type.ToLower() != "other").Select(x => x).ToList();
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
