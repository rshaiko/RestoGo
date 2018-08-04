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
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        string url, json;

        public ActionResult Index()
        {

            ViewBag.Message = "Your application description page.";

            //private static readonly HttpClient client = new HttpClient();

            // GET: Restaurant
            
                string url = "https://developers.zomato.com/api/v2.1/geocode?lat=45.7&lon=-73&apikey=" + "4d72030018edd126975c251c29c50f70";
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
                            rest.Rating = item.restaurant.user_rating.aggregate_rating;

                            list.Add(rest);
                        }

                    }
                    Console.WriteLine("OK");
                    return View(list);

                }

                catch (Exception)
                {
                    //no distances found
                    Console.WriteLine("Error");
                    return View();
                }
            

            
            
            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Search()
        {
            ViewBag.Message = "RestoGo - Advanced search";

            double[] userCoordinates = { 0, 0 };

            if (User.Identity.IsAuthenticated)
            {
                // get current user coordinates
                string userName = System.Web.HttpContext.Current.User.Identity.Name;
                var curUser = db.Users.Where(a => a.Email == userName).SingleOrDefault();
                url = "http://maps.google.com/maps/api/geocode/json?address=" + curUser.Zip;
                try
                {
                    json = Api.getJson(url);
                    GoogleCoordinates coord = JsonConvert.DeserializeObject<GoogleCoordinates>(json);
                    if (coord.status != "OK") throw new Exception();
                    userCoordinates[0] = coord.results[0].geometry.location.lat;
                    userCoordinates[1] = coord.results[0].geometry.location.lng;

                }
                catch (Exception)
                {
                    //no coordinates found
                    Console.WriteLine("Coordinates fail!!!");
                    // using MONTREAL DOWNTOWN
                    userCoordinates[0] = 45.7058381;
                    userCoordinates[1] = -73.47426;

                }
            }
            else
            {
                // using MONTREAL DOWNTOWN
                userCoordinates[0] = 45.7058381;
                userCoordinates[1] = -73.47426;
            }
            ViewBag.Coord = userCoordinates;



            url = "https://maps.googleapis.com/maps/api/distancematrix/json?origins=H8N2K5&destinations=H8N2K7|H8P1E5&key=" + Globals.DistanceAPIkey;
            try
            {
                json = Api.getJson(url);
                GoogleMatrixData goo = JsonConvert.DeserializeObject<GoogleMatrixData>(json);
                if (goo.rows[0].elements[0].status != "OK") throw new Exception();
                //tbGoogleDistanceTo.Content = goo.rows[0].elements[0].distance.text.ToString();
                //tbGoogleTimeTo.Content = goo.rows[0].elements[0].duration.text.ToString();
                //int.TryParse(goo.rows[0].elements[0].distance.value + "", out meterGoo[0]);
                //int.TryParse(goo.rows[0].elements[0].duration.value + "", out secGoo[0]);

                Console.WriteLine("distance OK");
                return View(goo);

            }

            catch (Exception)
            {
                //no distances found
                Console.WriteLine("No distance!!!");
                return View();
            }
        }
    }
}