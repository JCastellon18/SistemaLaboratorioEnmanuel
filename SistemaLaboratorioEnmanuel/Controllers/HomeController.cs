using System.Web.Mvc;

namespace SistemaLaboratorioEnmanuel.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Dashboard");
        }
    }
}