using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeManagement.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }

        // GET: /<controller>/
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statuscodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            

            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the page could not be found";
                    ViewBag.Path = statuscodeResult.OriginalPath;
                    ViewBag.QS = statuscodeResult.OriginalQueryString;
                    //logger registration
                    logger.LogWarning($"404 Error Occured. Path = {statuscodeResult.OriginalPath}" + 
                        $"and QueryString = {statuscodeResult.OriginalQueryString}");
                    break;
            }
            return View("NotFound");
        }
        [AllowAnonymous]
        [Route("Error")]
        public IActionResult Error()
        {
            var execptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            logger.LogError("The path " + execptionHandlerPathFeature.Path + " threw an exception " + execptionHandlerPathFeature.Error);
            ViewBag.ExceptionPath = execptionHandlerPathFeature.Path;
            ViewBag.ExceptionMessage = execptionHandlerPathFeature.Error;
            ViewBag.StackTrace = execptionHandlerPathFeature.Error.StackTrace;

            return View("Error");
        }
    }
}
