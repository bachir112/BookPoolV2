using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookPool.DataObjects.DTO
{
    public class OrderResult
    {
        public int OrderID { get; set; }
        public decimal TotalPrice { get; set; }
        public string BuyerUserName { get; set; }
        public string BuyerPhoneNumber { get; set; }
        public string BuyerAddress { get; set; }
        public string SellerUserName { get; set; }
        public string SellerPhoneNumber { get; set; }
        public string SellerAddress { get; set; }
        public string BookName { get; set; }
        public decimal BookPrice { get; set; }
    }
}
