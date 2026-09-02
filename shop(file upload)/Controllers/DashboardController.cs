using Microsoft.AspNetCore.Mvc;
using shop_file_upload_.services;

namespace shop_file_upload_.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IdashboardService _dashboardService;

        public DashboardController(IdashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public IActionResult Index()
        {
            var model = _dashboardService.ShowChart();
            return View(model);
        }
    }
}
