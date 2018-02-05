using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace lotfi.IPFiltering.In.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class MyIPFilteringMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configurationRoot;

        public MyIPFilteringMiddleware(RequestDelegate next, IConfiguration confiqurationRoot)
        {
            _next = next;
            _configurationRoot = confiqurationRoot;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (IpIsValid(httpContext.Connection.LocalIpAddress))
            {
                await _next(httpContext);
            }
            else
            {
                await httpContext.Response.WriteAsync("Your IP is not Valid");
            }
            
        }

        private bool IpIsValid(IPAddress localIpAddress)
        {
            var headerSection = _configurationRoot.GetSection("IpRange");
            var lower = headerSection["lowerInclusive"];
            var upper = headerSection["upperInclusive"];
            IPAddressRange range = new IPAddressRange(
                IPAddress.Parse(lower),
                IPAddress.Parse(upper));

            return range.IsInRange(localIpAddress);
        }
        private bool IpV4IsValid(string localIpAddress)
        {
            string[] arry = localIpAddress.Split('.');
            string s1="127", s2="0", s3="0";
            if (arry.Count()>4)
            {
                return false;
            }
            if (arry[0] != s1 || arry[1] != s2 || arry[2] != s3)
            {
                return false;
            }

            //if (localIpAddress.StartsWith("127.0.0."))
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}

            return true;
        }
        private bool IpV6IsValid(string localIpAddress)
        {
            //i don't know...!
            return false;
        }
    }
}
