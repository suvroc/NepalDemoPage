using Microsoft.AspNetCore.Mvc;
using NepalDemoPage.Models;
using NepalDemoPage.Services;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NepalDemoPage.Controllers
{
    public class SewaController : Controller
    {
        public IActionResult eSewa()
        {
            ViewBag.BaseUrl = Request.Scheme + "://" + Request.Host.Value;
            return View();
        }

        public IActionResult eSewaWebhook()
        {
            ViewBag.QueryParams = Request.Query;

            foreach (var param in Request.Query)
            {
                param.ToString();
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
