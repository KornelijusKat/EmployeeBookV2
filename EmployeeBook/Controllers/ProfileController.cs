
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
            var port = new Person();
            string data = "";
            if (!string.IsNullOrEmpty(stringValue))
            {
                port = _dbContext.GetProfileByProfileId(Guid.Parse(stringValue));
            }
            else
            {
                data = Request.Cookies["Data"];
                port = _dbContext.GetProfile(Guid.Parse(data));
            }
                string fileName = "profile_picture.jpg";
                var newFile = _imageService.ConvertToIFormFile(port.ProfilePicture, fileName);
                var personDto = new PersonImage() { FirstName = port.FirstName, LastName = port.LastName, Email = port.Email, PersonCode = port.PersonCode, TelephoneNumber = port.TelephoneNumber, ProfilePicture = newFile };
            ViewModel newViewModel = new ViewModel()
            {
                personImage = personDto,
                Id = port.Id
                
            };
                return View(newViewModel);
        }
        [HttpPost]
        public IActionResult EditPerson(ViewModel viewModel)
        { 
                var byteImage = _imageService.GetByteArray(viewModel.personImage.ProfilePicture);
            var l = new Person() { Id = viewModel.Id, FirstName = viewModel.personImage.FirstName, LastName = viewModel.personImage.LastName, Email = viewModel.personImage.Email, PersonCode = viewModel.personImage.PersonCode, TelephoneNumber = viewModel.personImage.TelephoneNumber, ProfilePicture = byteImage };
            if (l.FirstName == null)
            {
                ViewBag.ErrorMessage = "First Create Profile";
                return View("EditPerson");
            }
            _dbContext.EditPerson(l);
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
        [HttpDelete]
        public IActionResult DeleteProfile([FromBody] string Id)
        {
            using (MySqlConnection sqlCon = new MySqlConnection(@"server = localhost; port = 3306;user=root; password=test;database=worker"))
            {
                sqlCon.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM persons WHERE UserId = @UserId", sqlCon);
                cmd.Parameters.AddWithValue("UserId", Guid.Parse(Id));
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Profile/ListOfProfiles");
        }
    }
}
