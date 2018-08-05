using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestoGo.Models
{
    public class Restaurant
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string URL { get; set; }
        public string Rating { get; set; }
        public string Ciusine { get; set; }
        public double AverageCost { get; set; }
        public double AggregateRating { get; set; }
        public string Votes { get; set; }
        public string Color { get; set; }

        //from Distance API
        public string TimeToGetTo { get; set; }
        public string Distance { get; set; }
    }
}