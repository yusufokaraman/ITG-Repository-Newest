using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using ProgrammersBlog.Entities.Concrete;
using ProgrammersBlog.Entities.Dtos;
using ProgrammersBlog.Mvc.Areas.Admin.Models;
using ProgrammersBlog.Mvc.Helpers.Abstract;
using ProgrammersBlog.Services.Abstract;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammersBlog.Mvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PlaceController : BaseController
    {
        private readonly IPlaceService _placeService;
        private readonly ICategoryService _categoryService;
        private readonly IToastNotification _toastNotification;

        public PlaceController(IPlaceService placeService, ICategoryService categoryService, UserManager<User> userManager, IMapper mapper, IImageHelper imageHelper, IToastNotification toastNotification) : base(userManager, mapper, imageHelper)
        {
            _placeService = placeService;
            _categoryService = categoryService;
            _toastNotification = toastNotification;
        }
        [Authorize(Roles = "SuperAdmin,Place.Read")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _placeService.GetAllByNonDeletedAsync();
            if (result.ResultStatus == ResultStatus.Success) return View(result.Data);
            return NotFound();
        }
        [Authorize(Roles = "SuperAdmin,Place.Create")]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var result = await _categoryService.GetAllByNonDeletedAndActiveAsync();
            if (result.ResultStatus == ResultStatus.Success)
            {
                return View(new PlaceAddViewModel
                {
                    Categories = result.Data.Categories
                });
            }

            return NotFound();
        }
        [Authorize(Roles = "SuperAdmin,Place.Create")]
        [HttpPost]
        public async Task<IActionResult> Add(PlaceAddViewModel placeAddViewModel)
        {
            if (ModelState.IsValid)
            {
                var placeAddDto = Mapper.Map<PlaceAddDto>(placeAddViewModel);
                //var imageResult = await ImageHelper.Upload(articleAddViewModel.Title,
                //    articleAddViewModel.ThumbnailFile, PictureType.Post);
                //articleAddDto.Thumbnail = imageResult.Data.FullName;
                var result = await _placeService.AddAsync(placeAddDto, LoggedInUser.UserName, LoggedInUser.Id);
                if (result.ResultStatus == ResultStatus.Success)
                {
                    _toastNotification.AddSuccessToastMessage(result.Message, new ToastrOptions
                    {
                        Title = "Başarılı İşlem!"
                    });
                    return RedirectToAction("Index", "Place");
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }

            var categories = await _categoryService.GetAllByNonDeletedAndActiveAsync();
            placeAddViewModel.Categories = categories.Data.Categories;
            return View(placeAddViewModel);
        }
        [Authorize(Roles = "SuperAdmin,Place.Update")]
        [HttpGet]
        public async Task<IActionResult> Update(int placeId)
        {
            var placeResult = await _placeService.GetPlaceUpdateDtoAsync(placeId);
            var categoriesResult = await _categoryService.GetAllByNonDeletedAndActiveAsync();
            if (placeResult.ResultStatus == ResultStatus.Success && categoriesResult.ResultStatus == ResultStatus.Success)
            {
                var placeUpdateViewModel = Mapper.Map<PlaceUpdateViewModel>(placeResult.Data);
                placeUpdateViewModel.Categories = categoriesResult.Data.Categories;
                return View(placeUpdateViewModel);
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "SuperAdmin,Place.Update")]
        [HttpPost]
        public async Task<IActionResult> Update(PlaceUpdateViewModel placeUpdateViewModel)
        {
            if (ModelState.IsValid)
            {
                bool isNewThumbnailUploaded = false;
                var oldThumbnail = placeUpdateViewModel.PlacePicture;
                if (placeUpdateViewModel.ThumbnailFile != null)
                {
                    //var uploadedImageResult = await ImageHelper.Upload(articleUpdateViewModel.Title,
                    //    articleUpdateViewModel.ThumbnailFile, PictureType.Post);
                    //articleUpdateViewModel.Thumbnail = uploadedImageResult.ResultStatus == ResultStatus.Success
                    //    ? uploadedImageResult.Data.FullName
                    //    : "postImages/defaultThumbnail.jpg";
                    if (oldThumbnail != "postImages/defaultThumbnail.jpg")
                    {
                        isNewThumbnailUploaded = true;
                    }
                }
                var placeUpdateDto = Mapper.Map<PlaceUpdateDto>(placeUpdateViewModel);
                var result = await _placeService.UpdateAsync(placeUpdateDto, LoggedInUser.UserName);
                if (result.ResultStatus == ResultStatus.Success)
                {
                    if (isNewThumbnailUploaded)
                    {
                        ImageHelper.Delete(oldThumbnail);
                    }
                    _toastNotification.AddSuccessToastMessage(result.Message, new ToastrOptions
                    {
                        Title = "Başarılı İşlem!"
                    });
                    return RedirectToAction("Index", "Place");
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }

            var categories = await _categoryService.GetAllByNonDeletedAndActiveAsync();
            placeUpdateViewModel.Categories = categories.Data.Categories;
            return View(placeUpdateViewModel);
        }


    }
}
