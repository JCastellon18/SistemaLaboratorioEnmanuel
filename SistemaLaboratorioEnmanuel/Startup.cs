using Microsoft.Owin;
using Owin;
using SistemaLaboratorioEnmanuel.Models;
using QuestPDF.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


[assembly: OwinStartupAttribute(typeof(SistemaLaboratorioEnmanuel.Startup))]
namespace SistemaLaboratorioEnmanuel
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            QuestPDF.Settings.License = LicenseType.Community;
            CrearRolesConUsuario();
        }

        private void CrearRolesConUsuario()
        {
            //Acceder al modelo de seguridad
            ApplicationDbContext context = new ApplicationDbContext();

            //Manejadores de roles y usuarios
            var manejadorRol = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var manejadorUsuario = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            //verificamos la existencia del rol
            if (!manejadorRol.RoleExists("Admin"))
            {
                //Crear el nuevo rol por primera vez
                var rol = new IdentityRole();

                //Establecer valores
                rol.Name = "Admin";
                manejadorRol.Create(rol);

                //Asignar a su primer usuario
                var user = new ApplicationUser();

                //Asignamos los valores
                user.UserName = "denis.lopez@gmail.com";
                user.Email = "denis.lopez@gmail.com";
                string pwd = "Facil123*";

                //Proceder a agregar usuario
                var verificar = manejadorUsuario.Create(user, pwd);

                //asignar el usuario con su respectivo rol
                if (verificar.Succeeded)
                {
                    var result = manejadorUsuario.AddToRole(user.Id, "Admin");
                }
            }


            //Para el rol trabajador
            if (!manejadorRol.RoleExists("trabajador"))
            {
                //Crear el nuevo rol por primera vez
                var rol = new IdentityRole();

                //Establecer valores
                rol.Name = "trabajador";
                manejadorRol.Create(rol);

                //Asignar a su primer usuario
                var user = new ApplicationUser();

                //Asignamos los valores
                user.UserName = "denis.lopez2@gmail.com";
                user.Email = "denis.lopez2@gmail.com";
                string pwd = "Facil123*";

                //Proceder a agregar usuario
                var verificar = manejadorUsuario.Create(user, pwd);

                //asignar el usuario con su respectivo rol
                if (verificar.Succeeded)
                {
                    var result = manejadorUsuario.AddToRole(user.Id, "trabajador");
                }
            }
        }
    }
}
