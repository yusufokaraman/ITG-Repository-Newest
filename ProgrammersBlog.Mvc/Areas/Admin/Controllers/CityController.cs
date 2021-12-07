using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using ProgrammersBlog.Entities.Dtos;
using ProgrammersBlog.Mvc.Areas.Admin.Models;
using ProgrammersBlog.Services.Abstract;
using ProgrammersBlog.Shared.Utilities.Extensions;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProgrammersBlog.Mvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CityController : Controller
    {
        private readonly ICityService _cityService;
        private readonly IToastNotification _toastNotification;

        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }
        [Authorize(Roles = "SuperAdmin,City.Read")]
        public async Task<IActionResult> Index()
        {
            var result = await _cityService.GetAllAsync();

            return View(result.Data);
        }
        [Authorize(Roles = "SuperAdmin,City.Create")]
        [HttpGet]
        public IActionResult Add()
        {
            return PartialView("_CityAddPartial");
        }
        //[Authorize(Roles = "SuperAdmin,City.Create")]
        //[HttpPost]
        ////public async Task<IActionResult> Add(CityAddDto cityAddDto)
        ////{
        ////    if (ModelState.IsValid)
        ////    {
        ////        var result = await _cityService.AddAsync(cityAddDto, LoggedInUser.UserName);
        ////        if (result.ResultStatus == ResultStatus.Success)
        ////        {
        ////            var cityAddAjaxModel = JsonSerializer.Serialize(new CityAddAjaxViewModel
        ////            {
        ////                CityDto = result.Data,
        ////                CityAddPartial = await this.RenderViewToStringAsync("_CityAddPartial", cityAddDto)
        ////            });
        ////            return Json(cityAddAjaxModel);
        ////        }
        ////    }
        ////    var cityAddAjaxErrorModel = JsonSerializer.Serialize(new CityAddAjaxViewModel
        ////    {
        ////        CityAddPartial = await this.RenderViewToStringAsync("_CityAddPartial", cityAddDto)
        ////    });
        ////    return Json(cityAddAjaxErrorModel);

        ////}

    }
}
