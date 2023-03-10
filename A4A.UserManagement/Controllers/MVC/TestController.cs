using System.Web.Mvc;

namespace A4A.UM.Controllers
{
    [Authorize]
    public class TestController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }
        public ActionResult Test1()
        {
            return View();
        }
        public ActionResult GroupbyOBArray()
        {
            return View();
        }
        public ActionResult TestGroupby()
        {
            return View();
        }

        public ActionResult TestCompRegister()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}