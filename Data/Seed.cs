using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortalAcademico.Models;

namespace PortalAcademico.Data;

public static class Seed
{
    public static async Task Run(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;
        var ctx = sp.GetRequiredService<ApplicationDbContext>();
        await ctx.Database.MigrateAsync();

        var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole>>();
        var userMgr = sp.GetRequiredService<UserManager<IdentityUser>>();

        const string roleName = "Coordinador";
        if (!await roleMgr.RoleExistsAsync(roleName))
            await roleMgr.CreateAsync(new IdentityRole(roleName));

        // Usuario coordinador
        var email = "coordinador@uni.edu";
        var user = await userMgr.FindByEmailAsync(email);
        if (user == null)
        {
            user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            await userMgr.CreateAsync(user, "Passw0rd!");
            await userMgr.AddToRoleAsync(user, roleName);
        }

        // Cursos iniciales
        if (!await ctx.Cursos.AnyAsync())
        {
            var cursos = new[]
            {
                new Curso { Codigo="MAT101", Nombre="Cálculo I", Creditos=4, CupoMaximo=30, HorarioInicio=new TimeSpan(8,0,0),  HorarioFin=new TimeSpan(10,0,0), Activo=true },
                new Curso { Codigo="PRO201", Nombre="Programación I", Creditos=3, CupoMaximo=35, HorarioInicio=new TimeSpan(10,0,0), HorarioFin=new TimeSpan(12,0,0), Activo=true },
                new Curso { Codigo="BD301",  Nombre="Bases de Datos", Creditos=4, CupoMaximo=25, HorarioInicio=new TimeSpan(14,0,0), HorarioFin=new TimeSpan(16,0,0), Activo=true },
            };
            ctx.Cursos.AddRange(cursos);
            await ctx.SaveChangesAsync();
        }
    }
}
