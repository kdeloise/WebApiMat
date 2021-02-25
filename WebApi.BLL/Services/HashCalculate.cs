using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WebApi.BLL.Services
{
    public class HashCalculate
    {
        public static string CalculateMd5(byte[] fileBytes)
        {
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(fileBytes);
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }
}
