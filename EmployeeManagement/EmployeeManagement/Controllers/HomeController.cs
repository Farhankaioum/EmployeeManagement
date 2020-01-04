using EmployeeManagement.Models;
using EmployeeManagement.Security;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace EmployeeManagement.Controllers
{
   
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger<HomeController> logger;

       public readonly IDataProtector protector;

        public HomeController(IEmployeeRepository employeeRepository, IHostingEnvironment hostingEnvironment,
                                                            ILogger<HomeController> logger,
                     IDataProtectionProvider dataProtectionProvider,
                     DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;

            this.protector = dataProtectionProvider.CreateProtector
                (dataProtectionPurposeStrings.EmployeeRouteValue);
        }
        //[Route("")]
        //[Route("~/")]
        //[Route("[controller]/[action]")]
        //[Route("Index")]
       [AllowAnonymous]
        public ViewResult Index()
        {
            //return _employeeRepository.GetEmployee(1).Name;
            var model = _employeeRepository.GetAllEmployee().Select( e =>
                {
                    e.EncryptedId = protector.Protect(e.Id.ToString());
                    return e;
                });
            return View(model);
            
        }
        [AllowAnonymous]
        public ViewResult Details(string id)
        {
            string decryptedId = protector.Unprotect(id);
            int decryptedIntId = Convert.ToInt32(decryptedId);
            Employee employee = _employeeRepository.GetEmployee(decryptedIntId);
            if (employee == null)
            {
                
                Response.StatusCode = 404;
                return View("EmployeeNotFound", decryptedIntId);
            }
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel
            {
                Employee = _employeeRepository.GetEmployee(decryptedIntId),
                Title = "Employee Details"
            };
            //if(homeDetailsViewModel.Employee == null)
            //{
            //    Response.StatusCode = 404;
            //      return View("EmployeeNotFound", id.Value);
            //}

            return View(homeDetailsViewModel);
        }
        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFilename = ProcessUploadFile(model);
               
                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFilename
                };
                _employeeRepository.Add(newEmployee);
                return RedirectToAction("details", new {id = newEmployee.Id});
                
            }
           
                return View();
        }

        //Employee Editing area start
       [HttpGet]
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath

            };

            return View(employeeEditViewModel);
        }
        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;


                if (model.Photo !=null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    employee.PhotoPath = ProcessUploadFile(model);
                }
                Employee updatedEmployee = _employeeRepository.Update(employee);

                return RedirectToAction("index");
            }

            return View(model);
        }

        private string ProcessUploadFile(EmployeeCreateViewModel model)
        {
            string uniqueFilename = null;
           if(model.Photo != null)
            {
                string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueFilename = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFilename);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
                    
            }
            return uniqueFilename;
        }

        // for delete employee data
        public IActionResult Delete(int id)
        {
           var emp = _employeeRepository.Delete(id);
            return RedirectToAction("index");
        }
    }
}
