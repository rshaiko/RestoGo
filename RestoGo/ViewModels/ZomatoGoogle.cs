using RestoGo.API;
using RestoGo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestoGo.ViewModels
{
    public class ZomatoGoogle
    {
        public GoogleMatrixData googleMatrixData { get; set; }
        public List<Restaurant> restaurants { get; set; }

    }
}