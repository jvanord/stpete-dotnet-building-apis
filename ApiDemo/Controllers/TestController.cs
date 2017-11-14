using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo.Controllers
{
    public class TestController : Controller
    {
		/// <summary>
		/// Returns a simple SPA for testing this API.
		/// </summary>
		/// <returns></returns>
		[HttpGet, Route("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}