using Newtonsoft.Json;
using RestoGo.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestoGo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string url = "https://maps.googleapis.com/maps/api/distancematrix/json?origins=H8N2K5&destinations=H8N2K7|H8P1E5&key=" + Globals.DistanceAPIkey;
            try
            {
                string json = Api.getJson(url);
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