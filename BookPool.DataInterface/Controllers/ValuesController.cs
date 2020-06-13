using BookPool.DataObjects.EDM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace BookPool.DataInterface.Controllers
{
    public class ValuesController : ApiController
    {
        [System.Web.Http.Route("api/Values/GetCategories")]
        [System.Web.Http.HttpGet]
        public JsonResult<Object> GetCategories()
        {
            List<object> results = new List<object>();
            List<Category> categories = new List<Category>();

            try
            {
                using(var db = new BookPoolEntities())
                {
                    categories = db.Categories.ToList();
                }

                results.AddRange(categories);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { results });
        }

        [System.Web.Http.Route("api/Values/GetLanguages")]
        [System.Web.Http.HttpGet]
        public JsonResult<Object> GetLanguages()
        {
            List<object> results = new List<object>();
            List<Language> languages = new List<Language>();

            try
            {
                using (var db = new BookPoolEntities())
                {
                    languages = db.Languages.ToList();
                }

                results.AddRange(languages);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { results });
        }

        [System.Web.Http.Route("api/Values/GetConditions")]
        [System.Web.Http.HttpGet]
        public JsonResult<Object> GetConditions()
        {
            List<object> results = new List<object>();
            List<Condition> conditions = new List<Condition>();

            try
            {
                using (var db = new BookPoolEntities())
                {
                    conditions = db.Conditions.ToList();
                }

                results.AddRange(conditions);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { results });
        }

        [System.Web.Http.Route("api/Values/GetInstitutions")]
        [System.Web.Http.HttpGet]
        public JsonResult<Object> GetInstitutions()
        {
            List<object> results = new List<object>();
            List<Institution> institutions = new List<Institution>();

            try
            {
                using (var db = new BookPoolEntities())
                {
                    institutions = db.Institutions.ToList();
                }

                results.AddRange(institutions);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { results });
        }

        [System.Web.Http.Route("api/Values/GetCourses")]
        [System.Web.Http.HttpGet]
        public JsonResult<Object> GetCourses()
        {
            List<object> results = new List<object>();
            List<Cours> courses = new List<Cours>();

            try
            {
                using (var db = new BookPoolEntities())
                {
                    courses = db.Courses.ToList();
                }

                results.AddRange(courses);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { results });
        }

    }
}
