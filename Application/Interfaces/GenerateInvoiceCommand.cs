using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;
using Application.Services;

namespace Application.Interfaces
{
    public class GenerateInvoiceCommand : IGenerateInvoiceCommand
    {
        private readonly IInvoiceService _service;
        private readonly IPdfGenerator _pdfGen;

        public GenerateInvoiceCommand(
            IInvoiceService service,
            IPdfGenerator pdfGen)
        {
            _service = service;
            _pdfGen = pdfGen;
        }

        public async Task<byte[]> ExecuteAsync()
        {
            List<Domain.Entities.Product> products = await _service.ObtenerFacturasAsync();
            return _pdfGen.GenerateProductReport(products);
        }
    }
}

