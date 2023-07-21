
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
        private IDbContext _dbContext;
        private IImageService _imageService;
        public ProfileController(IDbContext dbContext, IImageService imageService)
        {
            _dbContext = dbContext;
            _imageService = imageService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetProfile()
        {
            string data = Request.Cookies["Data"];
            var dataGuid = Guid.Parse(data);
            var port = _dbContext.GetProfile(dataGuid);
            if(port.FirstName == null)
            {
               ViewBag.ErrorMessage = "Please first create profile";
                return RedirectToAction("ListOfProfiles");
            }
            return View(port);
        }
        [HttpGet]
        public IActionResult EditPerson([FromQuery]string sdata)
        {
            Request.Query.TryGetValue("stringValue", out var stringValue);
            var profile = new Person();
            string data = "";
            if (!string.IsNullOrEmpty(stringValue))
            {
                profile = _dbContext.GetProfileByProfileId(Guid.Parse(stringValue));
            }
            else
            {
                data = Request.Cookies["Data"];
                profile = _dbContext.GetProfile(Guid.Parse(data));
            }
            if(profile.FirstName == null)
            {
                ViewBag.ErrorMessage = "First Create Profile";
                return View("EditPerson");
            }
                string fileName = "profile_picture.jpg";
                var newFile = _imageService.ConvertToIFormFile(profile.ProfilePicture, fileName);
                var personDto = new PersonImage() { FirstName = profile.FirstName, LastName = profile.LastName, Email = profile.Email, PersonCode = profile.PersonCode, TelephoneNumber = profile.TelephoneNumber, ProfilePicture = newFile };
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
