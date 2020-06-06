using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookPoolV2.Classes
{
    public class FacebookAccount
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public Location Location { get; set; }
        public string Birthday { get; set; }
        public string Gender { get; set; }
        public string Picture { get; set; }
    }

    public class Location
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}