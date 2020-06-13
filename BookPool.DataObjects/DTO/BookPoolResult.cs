using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookPool.DataObjects.DTO
{
    public class BookPoolResult
    {
        public int ID { get; set; }
        public string BookName { get; set; }
        public decimal Price { get; set; }
        public int BookConditionID { get; set; }
        public string BookCondition { get; set; }
        public int BookLanguageID { get; set; }
        public string BookLanguage { get; set; }
        public bool Academic { get; set; }
        public int CategoryID { get; set; }
        public string Category { get; set; }
        public string GoogleID { get; set; }
        public string OwnerUserID { get; set; }
        public string OwnerUser { get; set; }
        public string Subtitle { get; set; }
        public List<string> Authors { get; set; }
        public string Publisher { get; set; }
        public string PublishedDate { get; set; }
        public string Description { get; set; }
        public int PageCount { get; set; }
        public string PrintType { get; set; }
        public List<string> Categories { get; set; }
        public decimal AverageRating { get; set; }
        public string ImageURL { get; set; }
        public string PreviewLink { get; set; }
        public string SellingStatus { get; set; }
        public Nullable<DateTime> PostedOn { get; set; }
    }
}
