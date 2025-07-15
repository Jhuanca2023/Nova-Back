using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NeonNovaApp.Controllers
{
    [ApiController]
    [Route("api/report")]
    [Authorize(Policy = "isAdmin")]
    public class ReportController : ControllerBase
    {
        private readonly IGenerateInvoiceCommand _invoiceCommand;
        private readonly IGenerateUserCommand _userCommand;

        public ReportController(IGenerateInvoiceCommand invoiceCommand, IGenerateUserCommand userCommand)
        {
            _invoiceCommand = invoiceCommand;
            _userCommand = userCommand;
        }

        [HttpGet("generate-product")]
        public async Task<IActionResult> GenerateProducts()
        {
            byte[] pdf = await _invoiceCommand.ExecuteAsync();
            return File(pdf, "application/pdf", "ReporteProductos.pdf");
        }

        [HttpGet("generate-user")]
        public async Task<IActionResult> GenerateUser()
        {
            byte[] pdf = await _userCommand.ExecuteAsync();
            return File(pdf, "application/pdf", "ReporteUsuarios.pdf");
        }
    }
}