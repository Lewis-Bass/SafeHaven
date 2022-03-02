using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebSiteCustomer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()               
                .Build();

    }
}

// .ConfigureKestrel((context, options) =>
//                {
//    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
//    options.Limits.MaxResponseBufferSize = 1024000;
//    options.Limits.Http2.HeaderTableSize = 1024000;
//})