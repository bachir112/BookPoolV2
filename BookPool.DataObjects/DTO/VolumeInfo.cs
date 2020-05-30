using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookPool.DataObjects.DTO
{
    public class VolumeInfo
    {
        public string title { get; set; }
        public string subtitle { get; set; }
        public List<string> authors { get; set; }
        public string publisher { get; set; }
        public string publishedDate { get; set; }
        public string description { get; set; }
        //public IndustryIdentifiers industryIdentifiers { get; set; }
        public int pageCount { get; set; }
        public string printType { get; set; }
        public List<string> categories { get; set; }
        public decimal averageRating { get; set; }
        public ImageLinks imageLinks { get; set; }
        public string language { get; set; }
        public string previewLink { get; set; }
    }
}
