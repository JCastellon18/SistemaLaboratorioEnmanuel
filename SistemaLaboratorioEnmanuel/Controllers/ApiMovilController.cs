using SistemaLaboratorioEnmanuel.Models;
using System.Linq;
using System.Web.Mvc;

namespace SistemaLaboratorioEnmanuel.Controllers
{
    public class ApiMovilController : Controller
    {
        private LaboratorioBDEntities db = new LaboratorioBDEntities();
        // GET: ApiMovil

        [HttpGet]
        public ActionResult Index()
        {
            var listaInventario = db.sp_Insumos().ToList();

            return Json(listaInventario, JsonRequestBehavior.AllowGet);

        }
    }
}