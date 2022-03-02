using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace WebSite.Controllers
{
    public class DownloadController : Controller
    {
        public IActionResult Index()
        {
            var fileName = "gimp-2.8.22-setup.exe";
            var filePath = Path.Combine("SoftwareDownload", fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/x-msdownload", fileName);
        }
    }
}