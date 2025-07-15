using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace NeonNovaApp.Extensions;

public class DataSeeder
{
    public static async Task SeedUsers(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();

        if (userManager.Users.Any())
            return;

        var usuariosParaCrear = new List<(Users Usuario, string Password)>
        {
            (new Users
            {
                UserName = "josehuanca612@gmail.com",
                Email = "josehuanca612@gmail.com",
                EmailConfirmed = true,
                FirstName = "Jose Armando",
                LastName = "Huanca Otiniano",
            }, "Jose2025!"),

            (new Users
            {
                UserName = "rogerik@hotmail.com",
                Email = "rogerik@hotmail.com",
                EmailConfirmed = true,
                FirstName = "Roger Erik",
                LastName = "Concepcion Leon",
            }, "RogerErik2025!"),

            (new Users
            {
                UserName = "jhon3122ahre@gmail.com",
                Email = "jhon3122ahre@gmail.com",
                EmailConfirmed = true,
                FirstName = "Jhon Wilson",
                LastName = "Rodriguez Quezada",
            }, "JhonMojamed2025!"),

            (new Users
            {
                UserName = "ryan@gmail.com",
                Email = "ryan@gmail.com",
                EmailConfirmed = true,
                FirstName = "Raydberg Gabriel",
                LastName = "Chuquival Gil",
            }, "Ryan2025!"),

            (new Users
            {
                UserName = "carlos@example.com",
                Email = "carlos@example.com",
                EmailConfirmed = true,
                FirstName = "Carlos",
                LastName = "Mendoza",
            }, "Carlos123!"),
            (new Users
            {
                UserName = "luisa@example.com",
                Email = "luisa@example.com",
                EmailConfirmed = true,
                FirstName = "Luisa",
                LastName = "Fernandez",
            }, "Luisa123!"),

            (new Users
            {
                UserName = "roberto@example.com",
                Email = "roberto@example.com",
                EmailConfirmed = true,
                FirstName = "Roberto",
                LastName = "Gomez",
            }, "Roberto123!"),

            (new Users
            {
                UserName = "patricia@gmail.com",
                Email = "patricia@gmail.com",
                EmailConfirmed = true,
                FirstName = "Patricia",
                LastName = "Sanchez",
            }, "Patricia123!"),

            (new Users
            {
                UserName = "miguel@hotmail.com",
                Email = "miguel@hotmail.com",
                EmailConfirmed = true,
                FirstName = "Miguel",
                LastName = "Torres",
            }, "Miguel123!"),

            (new Users
            {
                UserName = "diana@example.com",
                Email = "diana@example.com",
                EmailConfirmed = true,
                FirstName = "Diana",
                LastName = "Ramirez",
            }, "Diana123!")
        };

        foreach (var (usuario, password) in usuariosParaCrear)
        {
            await userManager.CreateAsync(usuario, password);
        }
    }
}