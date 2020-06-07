using BookPool.DataObjects.EDM;
using Microsoft.Ajax.Utilities;
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
        public JsonResult<Object> AddUserAddress(string UserID, string AddressTitle, string Address)
        {
            bool result = false;

            try
            {
                UsersAddress userAddress = new UsersAddress();
                userAddress.AspNetUserID = UserID;
                userAddress.AddressTitle = AddressTitle;
                userAddress.Address = Address;
                userAddress.DeliveringTo = true;

                using (var db = new BookPoolEntities())
                {
                    db.UsersAddresses.Where(x => x.AspNetUserID == UserID).Select(x => x).ForEach(x => x.DeliveringTo = false);
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
            List<UsersAddress> results = new List<UsersAddress>();

            try
            {
                using (var db = new BookPoolEntities())
                {
                    results = db.UsersAddresses.Where(x => x.AspNetUserID == UserID).Select(x => x).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { results });
        }


        [System.Web.Http.Route("api/Users/DeliveringToUserAddress")]
        [System.Web.Http.HttpGet]
        public JsonResult<Object> DeliveringToUserAddress(string UserID, int AddressID)
        {
            bool result = false;

            try
            {
                using (var db = new BookPoolEntities())
                {
                    db.UsersAddresses.Where(x => x.AspNetUserID == UserID).Select(x => x).ForEach(x => x.DeliveringTo = false);
                    UsersAddress usersAddress = db.UsersAddresses.First(x => x.ID == AddressID);
                    usersAddress.DeliveringTo = true;
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
    }
}
