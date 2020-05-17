using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookPoolV2.Controllers
{
    public class MagazinesController : Controller
    {
        // GET: Magazines
        public ActionResult Index()
        {
            return View();
        }
    }
}