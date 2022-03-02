using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace WebSiteCustomer.Controllers
{
    public class ViewDataCollectedController : Controller
    {
        private readonly IStringLocalizer<ViewDataCollectedController> _localizer;

        public ViewDataCollectedController(IStringLocalizer<ViewDataCollectedController> localizer)
        {

        }

        public IActionResult Index()
        {
            return View();
        }
    }
}