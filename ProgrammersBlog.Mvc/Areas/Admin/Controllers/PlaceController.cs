using Microsoft.AspNetCore.Mvc;
using ProgrammersBlog.Services.Abstract;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammersBlog.Mvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PlaceController : Controller
    {
        private readonly IPlaceService _placeService;

        public PlaceController(IPlaceService placeService)
        {
            _placeService = placeService;
        }

        [HttpGet]
        public async  Task<IActionResult> Index()
        {
            var result = await _placeService.GetAllByNonDeleted();
            if (result.ResultStatus == ResultStatus.Success) ; return View(result.Data);
            return NotFound();

        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
    }
}
