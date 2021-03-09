using Microsoft.AspNetCore.Mvc;
using NepalDemoPage.Models;
using NepalDemoPage.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NepalDemoPage.Controllers
{
    public class FonePayController : Controller
    {
        private string fonepaySecret;
        private string fonepayClientUrl;
        private string fonepayMerchantUrl;
        private string fonepayMerchantCode;
        private string fonepayUsername;
        private string fonepayPassword;


        public FonePayController(FonePayConfiguration configuration)
        {
            fonepaySecret = configuration.Secret;
            fonepayMerchantCode = configuration.MerchantCode;
            fonepayClientUrl = configuration.ClientUrl;
            fonepayMerchantUrl = configuration.MerchantUrl;
            fonepayUsername = configuration.Username;
            fonepayPassword = configuration.Password;
        }

        public IActionResult FonePay()
        {
            ViewBag.BaseUrl = Request.Scheme + "://" + Request.Host.Value;
            ViewBag.MerchantCode = fonepayMerchantCode;
            ViewBag.ClientUrl = fonepayClientUrl;
            return View();
        }

        [HttpGet]
        public string GenerateHMAC(HMACModel model)
        {
            return new HmacCalculator().Calculate(new string[]
            {
                model.Pid,
                model.Md,
                model.Prn,
                model.Amt,
                model.Crn,
                model.Dt,
                model.R1,
                model.R2,
                model.RU
            }, fonepaySecret);
        }

        public async Task<IActionResult> FonePayRedirect()
        {
            ViewBag.BaseUrl = Request.Scheme + "://" + Request.Host.Value;
            ViewBag.QueryParams = Request.Query;

            // ,,,,,,,

            var dv = new HmacCalculator().Calculate(new string[]
            {
                Request.Query["PRN"],
                Request.Query["PID"],
                Request.Query["PS"],
                Request.Query["RC"],
                Request.Query["UID"],
                Request.Query["BC"],
                Request.Query["INI"],
                Request.Query["P_AMT"],
                Request.Query["R_AMT"],
            }, fonepaySecret);

            ViewBag.ValidationResult = (dv == Request.Query["DV"]);

            // get status
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", Base64Encode(fonepayUsername + ":" + fonepayPassword));

            var values = new Dictionary<string, string>
            {
                { "prn", Request.Query["PRN"] },
                { "merchantCode", Request.Query["PID"] },
                { "amount", Request.Query["P_AMT"] }
            };
            var content = new StringContent(JsonConvert.SerializeObject(values, Formatting.None), System.Text.Encoding.UTF8, "application/json");
           
            string dv2 = new HmacCalculator().Calculate(new string[] {
                fonepayUsername,
                fonepayPassword,
                "POST",
                "application/json",
                "/merchant/merchantDetailsForThirdParty/txnVerification",
                JsonConvert.SerializeObject(values, Formatting.None)
            }, fonepaySecret); ;
            content.Headers.Add("auth", dv2);

            var response = client.PostAsync(fonepayMerchantUrl + "/convergent-merchant-web/api/merchant/merchantDetailsForThirdParty/txnVerification", content);

            ViewBag.StatusResponse = response.Result;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }

    public class FonePayConfiguration
    {
        public string Secret { get; set; }
        public string ClientUrl { get; set; }
        public string MerchantUrl { get; set; }
        public string MerchantCode { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
