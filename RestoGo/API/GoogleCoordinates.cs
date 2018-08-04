using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestoGo.API
{
    public class GoogleCoordinates
    {
        public Results [] results { get; set; }
        public string status { get; set; }
    }

    public class Results
    {
        public Address_components [] address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }

    }

    public class Geometry
    {
        public Location location { get; set; }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Address_components
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public string [] types { get; set; }

    }
}