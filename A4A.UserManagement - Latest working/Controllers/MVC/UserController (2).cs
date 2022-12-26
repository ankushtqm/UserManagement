using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace A4A.UM.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

       
        public ActionResult List()
        {
            ViewBag.Message = "Contact List";

            return View();
        }
        public ActionResult Show()
        {
            ViewBag.Message = "Contact Details";

            return View();
        }
    }
}