﻿
using EmployeeBook.Dto;
using EmployeeBook.ImageService;
using EmployeeBook.Models;
using EmployeeBook.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;
using System.Web;

namespace EmployeeBook.Controllers
{
    [Authorize(Roles = "User, Admin")]
    public class ProfileController : Controller
    {
        private readonly IDbContext _dbContext;
        private readonly IImageService _imageService;
        public ProfileController(IDbContext dbContext, IImageService imageService)
        {
            _dbContext = dbContext;
            _imageService = imageService;
        }
        public IActionResult GetProfile()
        {
            string data = Request.Cookies["Data"];
            var dataGuid = Guid.Parse(data);
            var profile = _dbContext.GetProfile(dataGuid);
            if(profile.FirstName == null)
            {
               ViewBag.ErrorMessage = "Please first create profile";
                return RedirectToAction("ListOfProfiles");
            }
            return View(profile);
        }
        [HttpGet]
        public IActionResult EditPerson(Guid Id)
        {
                var profile = new Person();
                string data = "";
                if (Id != Guid.Empty)
                {
                    profile = _dbContext.GetProfileByProfileId(Id);
                }
                else
                {
                    data = Request.Cookies["Data"];
                    profile = _dbContext.GetProfile(Guid.Parse(data));
                }
                if (profile.FirstName == null)
                {
                    ViewBag.ErrorMessage = "First Create Profile";
                    return View("EditPerson");
                }
                string fileName = "profile_picture.jpg";
                var personDto = new PersonImage() { FirstName = profile.FirstName, LastName = profile.LastName, Email = profile.Email, PersonCode = profile.PersonCode, TelephoneNumber = profile.TelephoneNumber, ProfilePicture = null};
                ViewModel newViewModel = new ViewModel()
                {
                    personImage = personDto,
                    Id = profile.Id
                };
                return View(newViewModel);
        }
        [HttpPost]
        public IActionResult EditPerson(ViewModel viewModel)
        {
            var allowedExtensions = new[] { ".jpg", ".png" };
            if (viewModel.personImage.ProfilePicture != null)
            {
                var fileExtension = Path.GetExtension(viewModel.personImage.ProfilePicture.FileName)?.ToLower();

                if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
                {
                    ViewBag.ErrorMessage = "Wrong profile picture format";
                    return View("EditPerson");
                }
            }
            var byteImage = _imageService.GetByteArray(viewModel.personImage.ProfilePicture);
            var newProfile = new Person() { Id = viewModel.Id, FirstName = viewModel.personImage.FirstName, LastName = viewModel.personImage.LastName, Email = viewModel.personImage.Email, PersonCode = viewModel.personImage.PersonCode, TelephoneNumber = viewModel.personImage.TelephoneNumber, ProfilePicture = byteImage };
            _dbContext.EditPerson(newProfile);
                return RedirectToAction("ListOfProfiles");
        }
        [HttpGet]
        public IActionResult ListOfProfiles()
        {
  
            return View(_dbContext.ShowProfiles());
        }
        public IActionResult CreateProfile(PersonImage personImage)
        {
            if (ModelState.IsValid)
            {
                string data = Request.Cookies["Data"];
                Guid dataGuid = Guid.Parse(data);
                if (!_dbContext.CheckUserHasProfile(dataGuid))
                {
                    var pic = _imageService.GetByteArray(personImage.ProfilePicture);
                    _dbContext.CreateProfile(personImage, dataGuid, pic);
                    return RedirectToAction("ListOfProfiles");
                }
                ViewBag.ErrorMessage = "Profile for user already exists";
            }
            return View("CreateProfile");
        }
        [HttpPost]
        public IActionResult DeleteProfile(Guid Id)
        {
            _dbContext.DeleteProfile(Id);
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
