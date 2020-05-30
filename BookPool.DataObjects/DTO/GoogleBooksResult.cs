using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookPool.DataObjects.DTO
{
    public class GoogleBooksResult
    {
        public string kind { get; set; }
        public Nullable<int> totalItems { get; set; }
        public List<GoogleBook> items { get; set; }
    }
}
