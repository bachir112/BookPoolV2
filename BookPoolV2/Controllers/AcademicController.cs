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
    public class AcademicController : Controller
    {
        // GET: Academic
        public async Task<ActionResult> Index(Nullable<int> Institution, Nullable<int> Course,
            string language = null,
            bool sortByPrice = false,
            bool AvailableOnly = false,
            bool sortByPopularity = false)
        {
            ViewBag.UserAddresses = await Global.Globals.GetUserAddresses(User.Identity.GetUserId());
            ViewBag.UserCartCookie = await Global.Globals.GetCart(User.Identity.GetUserId());

            ViewBag.Institution = Institution;
            ViewBag.Course = Course;

            ViewBag.Language = language;
            ViewBag.SortByPrice = sortByPrice;
            ViewBag.AvailableOnly = AvailableOnly;
            ViewBag.SortByPopularity = sortByPopularity;

            Dictionary<string, List<BookPoolResult>> apiResults = new Dictionary<string, List<BookPoolResult>>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Global.Globals.baseURL);
                StringBuilder httpRoute = new StringBuilder();
                httpRoute.Append("api/Books/GetAcademicBooks");
                httpRoute.Append("?");
                httpRoute.AppendFormat("Institution={0}", Institution);
                httpRoute.Append("&");
                httpRoute.AppendFormat("Course={0}", Course);
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

            return View();
        }
    }
}