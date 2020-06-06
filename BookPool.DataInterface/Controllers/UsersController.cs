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
    public class UsersController : ApiController
    {
        [System.Web.Http.Route("api/Users/AddUserAddress")]
        [System.Web.Http.HttpGet]
        public JsonResult<Object> SellMyBook(string UserID, string AddressTitle, string Address)
        {
            bool result = false;

            try
            {
                UsersAddress userAddress = new UsersAddress();
                userAddress.AspNetUserID = UserID;
                userAddress.AddressTitle = AddressTitle;
                userAddress.Address = Address;

                using (var db = new BookPoolEntities())
                {
                    db.UsersAddresses.Add(userAddress);
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

        [System.Web.Http.Route("api/Users/GetUserAddresses")]
        [System.Web.Http.HttpGet]
        public JsonResult<Object> GetUserAddresses(string UserID)
        {
            List<UsersAddress> userAddress = new List<UsersAddress>();

            try
            {
                using (var db = new BookPoolEntities())
                {
                    userAddress = db.UsersAddresses.Where(x => x.AspNetUserID == UserID).Select(x => x).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { userAddress });
        }
    }
}
