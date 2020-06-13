using BookPool.DataObjects.Global;
using BookPool.DataObjects.EDM;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.Text;
using BookPool.DataObjects.DTO;
using System.Threading.Tasks;

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


        [System.Web.Http.Route("api/Users/PlaceOrder")]
        [System.Web.Http.HttpGet]
        public async Task<JsonResult<Object>> PlaceOrder(string BuyerID, string BooksIDs)
        {
            string results = string.Empty;

            try
            {
                using (var db = new BookPoolEntities())
                {
                    decimal totalPrice = 0;
                    List<int> booksIDsInCart = BooksIDs.Split(',').Select(int.Parse).ToList();
                    foreach (var bookID in booksIDsInCart)
                    {
                        AvailableBook availableBook = db.AvailableBooks.FirstOrDefault(x => x.ID == bookID);
                        totalPrice += availableBook.Price;
                    }

                    OrderHeader orderHeader = new OrderHeader();
                    orderHeader.ClientUserID = BuyerID;
                    orderHeader.OrderedOn = DateTime.Now;
                    orderHeader.Status = Globals.OrderStatusPending;
                    orderHeader.TotalPrice = totalPrice;

                    
                    db.OrderHeaders.Add(orderHeader);

                    db.SaveChanges();

                    results = orderHeader.ID.ToString() + orderHeader.OrderedOn.ToString("ddMMyy");

                    UserCart userCart = db.UserCarts.FirstOrDefault(x => x.UserID == BuyerID);

                    foreach (var bookID in booksIDsInCart)
                    {
                        AvailableBook availableBook = db.AvailableBooks.FirstOrDefault(x => x.ID == bookID);
                        if(availableBook != null)
                        {
                            availableBook.SellingStatus = Global.Globals.BookSellingStatus_NotAvailable;

                            OrderDetail orderDetail = new OrderDetail();
                            orderDetail.BookName = availableBook.BookName;
                            orderDetail.BookPrice = availableBook.Price;
                            orderDetail.OrderHeaderID = orderHeader.ID;
                            orderDetail.Status = Globals.OrderStatusPending;
                            orderDetail.SellerUserID = availableBook.OwnerUserID;
                            orderDetail.BookID = availableBook.ID;

                            GoogleBook googleResult = new GoogleBook();
                            using (var client = new HttpClient())
                            {
                                client.BaseAddress = new Uri(DataObjects.Global.Globals.googleBaseAPI_SearchByID);
                                StringBuilder httpRoute = new StringBuilder();
                                httpRoute.Append(availableBook.GoogleID);

                                var response = await client.GetAsync(httpRoute.ToString());
                                if (response.IsSuccessStatusCode)
                                {
                                    googleResult = await response.Content.ReadAsAsync<GoogleBook>();
                                }
                            }

                            orderDetail.BookImage = googleResult.volumeInfo.imageLinks.thumbnail;
                            orderDetail.Authors = string.Join(",", googleResult.volumeInfo.authors);
                            orderDetail.GoogleID = googleResult.id;

                            db.OrderDetails.Add(orderDetail);

                            db.SaveChanges();
                        }


                        if (userCart != null)
                        {
                            List<int> userCartBooks = userCart.BooksIDsCSV.Split(',').Select(int.Parse).ToList();
                            userCartBooks.Remove(bookID);
                            userCart.BooksIDsCSV = string.Join(",", userCartBooks);
                            db.SaveChanges();
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Json((object)new { results });
        }


        [System.Web.Http.Route("api/Users/RemoveBookFromBasket")]
        [System.Web.Http.HttpGet]
        public JsonResult<Object> RemoveBookFromBasket(string UserID, int BookID)
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
                        booksIDsInCart.Remove(BookID);

                        userCart.BooksIDsCSV = string.Join(",", booksIDsInCart);
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
    }
}
