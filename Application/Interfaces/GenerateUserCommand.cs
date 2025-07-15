using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public class GenerateUserCommand : IGenerateUserCommand
    {
    private readonly IUserService _service;
    private readonly IPdfGeneratorUsers _pdfGen;

    public GenerateUserCommand(
        IUserService service,
        IPdfGeneratorUsers pdfGen)
    {
        _service = service;
        _pdfGen = pdfGen;
    }

    public async Task<byte[]> ExecuteAsync()
    {
        List<Domain.Entities.Users> users = await _service.ObtenerReporteAsync();
        return _pdfGen.GenerateProductReport(users);
    }
}
}

