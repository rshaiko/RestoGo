using Newtonsoft.Json;
using RestoGo.API;
using RestoGo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestoGo.Controllers
{
    public class SearchController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        string url, json;
        double[] userCoordinates = { 0, 0 };
        public ActionResult Index()
        {
            ViewBag.Message = "RestoGo - Advanced search";
            ViewBag.FormData = new string[]{ "","",""};
            if (User.Identity.IsAuthenticated)
            {
                // get current user coordinates
                string userName = System.Web.HttpContext.Current.User.Identity.Name;
                var curUser = db.Users.Where(a => a.Email == userName).SingleOrDefault();
                ViewBag.UserAddress = new string[]{curUser.Address, curUser.City, curUser.Zip };
            }
            else
            {
                ViewBag.UserAddress = null;
            }
            ViewBag.SearchError = "";
            return View();
        }

        [HttpPost]
        public ActionResult Index(string addr, string city, string prov, string zip)
        {
            ViewBag.UserZip = "H2Z1A7";
            string[] formData = { addr, city, zip };
            ViewBag.FormData = formData;
            ViewBag.SearchError = "";
            ViewBag.FormattedAddress = "";
            double[] searchCoordinates = { 0, 0 };
            string searchAddress="";
            if (zip != "")
            {
                searchAddress = zip;
            }
            else if (addr != "" || city != "")
            {
                searchAddress = addr + " " + city + " " + prov;
                searchAddress = searchAddress.Trim();
            }
            else
            {
                ViewBag.SearchError = "Error - no address submitted";
                //no coordinates found
                return View();
            }

            url = "http://maps.google.com/maps/api/geocode/json?address=" + searchAddress;
            try
            {
                json = Api.getJson(url);
                GoogleCoordinates coord = JsonConvert.DeserializeObject<GoogleCoordinates>(json);
                if (coord.status != "OK") throw new Exception();
                searchCoordinates[0] = coord.results[0].geometry.location.lat;
                searchCoordinates[1] = coord.results[0].geometry.location.lng;
                ViewBag.FormattedAddress = coord.results[0].formatted_address;

            }
            catch (Exception)
            {
                ViewBag.SearchError = "Error - Location not found, please verify the address";
                //no coordinates found
                return View();
            }

            // GET: Restaurant
            url = "https://developers.zomato.com/api/v2.1/geocode?lat="+ searchCoordinates[0] + "&lon="+ searchCoordinates[1] + "&apikey=" + Globals.RestoAPIkey;
            List<Restaurant> list = new List<Restaurant>();
            try
            {
                string json = Api.getJson(url);
                dynamic obj = JsonConvert.DeserializeObject(json);

                dynamic obj2 = obj.nearby_restaurants;
                foreach (var item in obj2)
                {
                    Restaurant rest = new Restaurant();
                    if (item.restaurant != null)
                    {
                        rest.Name = item.restaurant.name;
                        rest.Address = item.restaurant.location.address;
                        rest.City = item.restaurant.location.city;
                        rest.ZipCode = item.restaurant.location.zipcode;
                        rest.URL = item.restaurant.url;
                        rest.Rating = item.restaurant.user_rating.rating_text;
                        rest.Ciusine = item.restaurant.cuisines;
                        rest.AverageCost = item.restaurant.average_cost_for_two;
                        rest.Color = item.restaurant.user_rating.rating_color;
                        rest.AggregateRating = item.restaurant.user_rating.aggregate_rating;
                        rest.Votes = item.restaurant.user_rating.votes;
                        rest.Latitude = item.restaurant.location.latitude;
                        rest.Longitude = item.restaurant.location.longitude;
                        list.Add(rest);
                    }

                }

                //Adding time and distance
                string[] arrDest = new string[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    arrDest[i] = list[i].ZipCode;
                }
                string destinations = String.Join("|", arrDest);

                string zipFrom = "H2Z1A7";
                if (User.Identity.IsAuthenticated)
                {
                    string userName = System.Web.HttpContext.Current.User.Identity.Name;
                    var curUser = db.Users.Where(a => a.Email == userName).SingleOrDefault();
                    zipFrom = curUser.Zip;
                    ViewBag.UserZip = curUser.Zip;
                }
                url = "https://maps.googleapis.com/maps/api/distancematrix/json?origins="+zipFrom+"&destinations=" + destinations + "&key=" + Globals.DistanceAPIkey;
                try
                {
                    json = Api.getJson(url);
                    GoogleMatrixData goo = JsonConvert.DeserializeObject<GoogleMatrixData>(json);
                    if (goo.status != "OK") throw new Exception();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (goo.rows[0].elements[i].status != "OK")
                        {
                            list[i].Distance = "n/d";
                            list[i].TimeToGetTo = "n/d";
                        }
                        else
                        {
                            list[i].Distance = goo.rows[0].elements[i].distance.text.ToString();
                            list[i].TimeToGetTo = goo.rows[0].elements[i].duration.text.ToString();
                        }
                    }
                    return View(list);
                }

                catch (Exception)
                {
                    //no distances found
                    Console.WriteLine("No distance!!!");
                    return View();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
                return View();
            }
        }
    }
}