using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookPool.DataObjects.DTO
{
    class UserCartDTO
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public string BooksIDsCSV { get; set; }
    }
}
