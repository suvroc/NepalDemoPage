using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NepalDemoPage.Services
{
    public class HmacCalculator
    {
        public string Calculate(string[] input, string key)
        {
            var inputData = string.Join(",", input);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            var keyBytes = Encoding.UTF8.GetBytes(key);
            using (HMACSHA512 hmac = new HMACSHA512(keyBytes))
            {
                return BitConverter.ToString(hmac.ComputeHash(inputBytes)).Replace("-", "");
            }    
        }
    }
}
