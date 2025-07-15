using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NeonNovaApp.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    [Authorize(Policy = "isAdmin")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = await _dashboardService.GetDashboardStatsAsync();
                return Ok(stats);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500,
                    new { message = "Error al obtener estadísticas del dashboard", detail = ex.Message });
            }
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUserStats()
        {
            try
            {
                var stats = await _dashboardService.GetUserStatsAsync();
                return Ok(stats);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500,
                    new { message = "Error al obtener estadísticas de usuarios", detail = ex.Message });
            }
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProductStats()
        {
            try
            {
                var stats = await _dashboardService.GetProductStatsAsync();
                return Ok(stats);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500,
                    new { message = "Error al obtener estadísticas de productos", detail = ex.Message });
            }
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategoryStats()
        {
            try
            {
                var stats = await _dashboardService.GetCategoryStatsAsync();
                return Ok(stats);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500,
                    new { message = "Error al obtener estadísticas de categorías", detail = ex.Message });
            }
        }
    }
}