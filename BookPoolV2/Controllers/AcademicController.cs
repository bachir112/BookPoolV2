using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BookPoolV2.Controllers
{
    public class AcademicController : Controller
    {
        // GET: Academic
        public async Task<ActionResult> Index()
        {
            ViewBag.UserAddresses = await Global.Globals.GetUserAddresses(User.Identity.GetUserId());
            return View();
        }
    }
}