using Microsoft.AspNetCore.Mvc;
using ProgrammersBlog.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammersBlog.Mvc.Controllers
{
    public class PlaceController : Controller
    {
        private readonly IPlaceService _placeService;
        //private readonly PlaceRightSideBarWidgetOptions _placeRightSideBarWidgetOptions;
        public PlaceController(IPlaceService placeService)
        {
            _placeService = placeService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
