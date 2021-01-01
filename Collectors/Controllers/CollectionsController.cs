using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Controllers
{
    public class CollectionsController : Controller
    {
        public IActionResult Index()
        {       
            //GetAvaibleCollections();
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
