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

            //if (User.Identity.IsAuthenticated)
            //{
            //    // get current user coordinates
            //    string userName = System.Web.HttpContext.Current.User.Identity.Name;
            //    var curUser = db.Users.Where(a => a.Email == userName).SingleOrDefault();
            //    ViewBag.CurUser = curUser;
            //    url = "http://maps.google.com/maps/api/geocode/json?address=" + curUser.Zip;
            //    try
            //    {
            //        json = Api.getJson(url);
            //        GoogleCoordinates coord = JsonConvert.DeserializeObject<GoogleCoordinates>(json);
            //        if (coord.status != "OK") throw new Exception();
            //        userCoordinates[0] = coord.results[0].geometry.location.lat;
            //        userCoordinates[1] = coord.results[0].geometry.location.lng;

            //    }
            //    catch (Exception)
            //    {
            //        //no coordinates found
            //        Console.WriteLine("Coordinates fail!!!");
            //        // using MONTREAL DOWNTOWN
            //        userCoordinates[0] = 45.7058381;
            //        userCoordinates[1] = -73.47426;

            //    }
            //}
            //else
            //{
            //    // using MONTREAL DOWNTOWN
            //    userCoordinates[0] = 45.7058381;
            //    userCoordinates[1] = -73.47426;
            //}
            //ViewBag.Coord = userCoordinates;



            //url = "https://maps.googleapis.com/maps/api/distancematrix/json?origins=H8N2K5&destinations=H8N2K7|H8P1E5&key=" + Globals.DistanceAPIkey;
            //try
            //{
            //    json = Api.getJson(url);
            //    GoogleMatrixData goo = JsonConvert.DeserializeObject<GoogleMatrixData>(json);
            //    if (goo.rows[0].elements[0].status != "OK") throw new Exception();
            //    //tbGoogleDistanceTo.Content = goo.rows[0].elements[0].distance.text.ToString();
            //    //tbGoogleTimeTo.Content = goo.rows[0].elements[0].duration.text.ToString();
            //    //int.TryParse(goo.rows[0].elements[0].distance.value + "", out meterGoo[0]);
            //    //int.TryParse(goo.rows[0].elements[0].duration.value + "", out secGoo[0]);

            //    Console.WriteLine("distance OK");
            //    return View(goo);

            //}

            //catch (Exception)
            //{
            //    //no distances found
            //    Console.WriteLine("No distance!!!");
            //    return View();
            //}
            ViewBag.SearchError = "";
            return View();
        }

        [HttpPost]
        public ActionResult Index(string addr, string city, string prov, string zip)
        {
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
                Console.WriteLine("Coordinates fail!!!");
                // using MONTREAL DOWNTOWN
                searchCoordinates[0] = 45.7058381;
                searchCoordinates[1] = -73.47426;
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


                Console.WriteLine("OK");
                return View(list);

            }

            catch (Exception)
            {

                Console.WriteLine("Error");
                return View();
            }
        }
    }
}