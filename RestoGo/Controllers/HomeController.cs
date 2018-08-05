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

            ViewBag.Message = "RestoGo - Find your best match resto";

            //private static readonly HttpClient client = new HttpClient();

            // GET: Restaurant
            
                string url = "https://developers.zomato.com/api/v2.1/geocode?lat=45.50884&lon=-73.58781&apikey=" + "4d72030018edd126975c251c29c50f70";
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
                            rest.Ciusine = item.restaurant.cuisines ;
                            rest.AverageCost = item.restaurant.average_cost_for_two;
                            rest.Color = item.restaurant.user_rating.rating_color;
                            rest.AggregateRating = item.restaurant.user_rating.aggregate_rating;
                            rest.Votes = item.restaurant.user_rating.votes;
                            list.Add(rest);
                        }

                    }
                
                //Adding time and distance
                string[] arrDest = new string[list.Count];
                for(int i = 0; i < list.Count; i++)
                {
                    arrDest[i] = list[i].ZipCode;
                }
                string destinations = String.Join("|", arrDest);

                url = "https://maps.googleapis.com/maps/api/distancematrix/json?origins=H8N2K5&destinations="+destinations+"&key=" + Globals.DistanceAPIkey;
                try
                {
                    json = Api.getJson(url);
                    GoogleMatrixData goo = JsonConvert.DeserializeObject<GoogleMatrixData>(json);
                    if(goo.status !="OK")throw new Exception();
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

        
    }
}