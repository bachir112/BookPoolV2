using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookPool.DataObjects.Global
{
    public class Globals
    {
        public static string googleBaseAPI = "https://www.googleapis.com/books/v1/volumes";
        public static string googleBaseAPI_SearchByID = "https://www.googleapis.com/books/v1/volumes/";

        public static string OrderStatusPending = "Pending";
        public static string OrderStatusCancled = "Canceled";
        public static string OrderStatusCompleted = "Completed";
    }
}
