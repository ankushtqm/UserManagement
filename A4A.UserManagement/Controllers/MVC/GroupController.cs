using System.Web.Mvc;

namespace A4A.UM.Controllers
{
    [Authorize]
    public class GroupController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            ViewBag.Message = "Groups List";

            return View();
        }
        public ActionResult Show()
        {
            ViewBag.Message = "Groups Details";

            return View();
        }
    }
}