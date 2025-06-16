using Microsoft.AspNetCore.Mvc;

namespace HR_Operations_System.Controllers
{
    public class EmployeesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }


}
