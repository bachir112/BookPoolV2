using BookPool.DataObjects.EDM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookPool.DataInterface.Global
{
    public class Globals
    {
        public static string BookSellingStatus_Available = "Available";
        public static string BookSellingStatus_NotAvailable = "NotAvailable";

        public static decimal GetBookpoolCharges()
        {
            decimal bookpoolCharges = 0;

            using (var db = new BookPoolEntities())
            {
                bookpoolCharges = db.BookPoolCharges.First().Charge;
            }

            return bookpoolCharges;
        }
    }
}