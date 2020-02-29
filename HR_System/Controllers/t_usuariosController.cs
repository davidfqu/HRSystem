using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HR_System.Models;

namespace HR_System.Controllers
{
    public class t_usuariosController : Controller
    {
        private HRSystemEntities db = new HRSystemEntities();

        // GET: t_usuarios
        public ActionResult NewUser(string alert = "")
        {
            if (!Login())
                return RedirectToAction("NoUser", "Home", null);

            t_usuarios user = db.t_usuarios.Find(Convert.ToString(Session["userAccount"]));

            if(user.rol != "ADMCO")
            {
                return RedirectToAction("Index", "Home", null);
            }


            if (alert == "1")
            {
                ViewBag.Alert = "1";
            }
            if (alert == "2")
            {
                ViewBag.Alert = "2";
            }
            if (alert == "3")
            {
                ViewBag.Alert = "3";
            }

            ViewBag.planta = new SelectList(db.t_plantas, "planta", "nombre");

            List<SelectListItem> roles = new List<SelectListItem>();
            roles.Add(new SelectListItem() { Text = "User", Value = "USER" });
            roles.Add(new SelectListItem() { Text = "Manager", Value = "MGR" });
            roles.Add(new SelectListItem() { Text = "Compensation Admin", Value = "ADMCO" });
            roles.Add(new SelectListItem() { Text = "HR Admin", Value = "ADMHR" });
            ViewBag.rol = new SelectList(roles, "Value", "Text");

            var empleadoslist = db.t_empleados.OrderBy(x => x.nombre).ToList();
            List<SelectListItem> liempleados = new List<SelectListItem>();

            liempleados.Add(new SelectListItem { Text = "N/A", Value = "" });
            foreach(var x in empleadoslist)
            {
                liempleados.Add(new SelectListItem { Text = x.nombre, Value = x.empleado});
            }

            ViewBag.supervisor = liempleados;

            var managerlist = db.t_empleados.Include(t => t.t_usuarios).Where(x => x.t_usuarios.rol == "MGR").OrderBy(x => x.nombre).ToList();
            List<SelectListItem> limanager = new List<SelectListItem>();

            limanager.Add(new SelectListItem { Text = "N/A", Value = "" });
            foreach (var x in managerlist)
            {
                limanager.Add(new SelectListItem { Text = x.nombre, Value = x.empleado });
            }

            ViewBag.manager = limanager;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewUser(string planta, int iempleado, string usuario, string email, string rol, string supervisor, string manager)
        {
            string empleado = Convert.ToString(iempleado);
            usuario = usuario.ToLower();

            if(existeUser(usuario))
            {
               
                return RedirectToAction("NewUser","t_usuarios", new {alert = "1" });
            }

            if (existeEmployee(planta+empleado))
            {
                return RedirectToAction("NewUser", "t_usuarios", new { alert = "2" });
            }

            empleadoTress add = new empleadoTress();

            add = add.datosTress(empleado, planta);

            if (add.nombre == null)
            {
                return RedirectToAction("NewUser", "t_usuarios", new { alert = "3" });
            }

            t_usuarios t_usuarios = new t_usuarios();
            t_empleados t_empleado = new t_empleados();

            t_usuarios.usuario = usuario;
            t_usuarios.nombre = add.nombre;
            t_usuarios.planta = planta;
            t_usuarios.email = email;
            t_usuarios.activo = "1";
            t_usuarios.rol = rol;
            t_usuarios.f_id = System.DateTime.Now;

            t_empleado.empleado = planta + empleado;
            t_empleado.nombre = add.nombre;
            t_empleado.usuario = usuario;
            t_empleado.f_id = System.DateTime.Now;
            t_empleado.planta = planta;
            if(supervisor=="")
            {
                t_empleado.supervisor = null;
            }
            else
            {
                t_empleado.supervisor = supervisor;
            }
            
            if(manager == "")
            {
                t_empleado.gerente = null;
            }
            else
            {
                t_empleado.gerente = manager;
            }
           


            if (ModelState.IsValid)
            {
                db.t_usuarios.Add(t_usuarios);
                db.SaveChanges();
                db.t_empleados.Add(t_empleado);
                db.SaveChanges();
                return RedirectToAction("Index", "t_usuarios", null); 
            }
           
            return View();
        }

        public bool existeUser(string usuario)
        {
            var validateName = db.t_usuarios.FirstOrDefault
                                (x => x.usuario == usuario);
            if (validateName != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool existeEmployee(string empleado)
        {
            var validateName = db.t_empleados.FirstOrDefault
                                (x => x.empleado == empleado);
            if (validateName != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ActionResult Index()
        {
            if (!Login())
                return RedirectToAction("NoUser", "Home", null);

            t_usuarios user = db.t_usuarios.Find(Convert.ToString(Session["userAccount"]));

            if (user.rol != "ADMCO")
            {
                return RedirectToAction("Index", "Home", null);
            }
            
            var t_empleados = db.t_empleados.Include(t => t.t_plantas).Include(t => t.t_usuarios).Include(t => t.t_empleados2).Include(t => t.t_empleados21).Where(x=> x.t_usuarios.activo=="1").ToList();
            return View(t_empleados);
        }

        // GET: t_usuarios/Details/5
        public ActionResult Details(string empleado)
        {
            MyDirects ndirect = new MyDirects();
            empleadoTress add = new empleadoTress();
            add = add.datosTress(empleado.Substring(3, empleado.Length - 3), empleado.Substring(0, 3));

            t_empleados t_empleados = db.t_empleados.Find(empleado);

            ndirect.nombre = t_empleados.nombre;
            ndirect.empleado = t_empleados.empleado;
            ndirect.usuario = t_empleados.t_usuarios.usuario;
            ndirect.sextra1 = t_empleados.t_usuarios.email;
            ndirect.depto = add.depto;
            ndirect.puesto = add.puesto;
            try
            {
                ndirect.manager1 = t_empleados.t_empleados21.nombre;
            }
           catch
            {
                ndirect.manager1 = "";
            }

            ViewBag.details = ndirect;

            return View();
        }

        // GET: t_usuarios/Create
        public ActionResult Create()
        {
            ViewBag.planta = new SelectList(db.t_plantas, "planta", "nombre");
            return View();
        }

        // POST: t_usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "usuario,nombre,planta,email,activo,rol,f_id")] t_usuarios t_usuarios)
        {
            if (ModelState.IsValid)
            {
                db.t_usuarios.Add(t_usuarios);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.planta = new SelectList(db.t_plantas, "planta", "nombre", t_usuarios.planta);
            return View(t_usuarios);
        }

        // GET: t_usuarios/Edit/5
        public ActionResult Edit(string empleado)
        {
            if (!Login())
                return RedirectToAction("NoUser", "Home", null);

            t_usuarios user = db.t_usuarios.Find(Convert.ToString(Session["userAccount"]));

            if (user.rol != "ADMCO")
            {
                return RedirectToAction("Index", "Home", null);
            }

            if (empleado == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_empleados t_empleados = db.t_empleados.Find(empleado);
            if (t_empleados == null)
            {
                return HttpNotFound();
            }

            ViewBag.planta = new SelectList(db.t_plantas, "planta", "nombre");

            List<SelectListItem> roles = new List<SelectListItem>();
            roles.Add(new SelectListItem() { Text = "User", Value = "USER" });
            roles.Add(new SelectListItem() { Text = "Manager", Value = "MGR" });
            roles.Add(new SelectListItem() { Text = "Compensation Admin", Value = "ADMCO" });
            roles.Add(new SelectListItem() { Text = "HR Admin", Value = "ADMHR" });
            ViewBag.rol = new SelectList(roles, "Value", "Text");

            var empleadoslist = db.t_empleados.OrderBy(x => x.nombre).ToList();
            List<SelectListItem> liempleados = new List<SelectListItem>();
            try
            {
                liempleados.Add(new SelectListItem { Text = t_empleados.t_empleados2.nombre, Value = t_empleados.supervisor });
            }
            catch
            {

            }
            
            liempleados.Add(new SelectListItem { Text = "N/A", Value = "" });
            foreach (var x in empleadoslist)
            {
                liempleados.Add(new SelectListItem { Text = x.nombre, Value = x.empleado });
            }

            ViewBag.supervisor = liempleados;

            var managerlist = db.t_empleados.Include(t => t.t_usuarios).Where(x => x.t_usuarios.rol == "MGR").OrderBy(x => x.nombre).ToList();
            List<SelectListItem> limanager = new List<SelectListItem>();

            try
            {
                limanager.Add(new SelectListItem { Text = t_empleados.t_empleados21.nombre, Value = t_empleados.gerente });
            }
            catch
            {

            }
            limanager.Add(new SelectListItem { Text = "N/A", Value = "" });
            foreach (var x in managerlist)
            {
                limanager.Add(new SelectListItem { Text = x.nombre, Value = x.empleado });
            }

            ViewBag.manager = limanager;


            return View(t_empleados);
        }

        // POST: t_usuarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string planta, string empleado, string usuario, string email, string rol, string supervisor, string manager)
        {
            t_usuarios t_usuarios = db.t_usuarios.Find(usuario);
            t_empleados t_empleados = db.t_empleados.Find(planta + empleado);

            t_usuarios.email = email;
            t_usuarios.rol = rol;

            t_empleados.supervisor = supervisor;
            t_empleados.gerente = manager;

            if (ModelState.IsValid)
            {
                db.Entry(t_empleados).State = EntityState.Modified;
                db.Entry(t_usuarios).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index", "t_usuarios", null);
            }
            return RedirectToAction("Index", "t_usuarios", null);
        }

        // GET: t_usuarios/Delete/5
        public ActionResult Delete(string empleado)
        {
            if (!Login())
                return RedirectToAction("NoUser", "Home", null);

            t_usuarios user = db.t_usuarios.Find(Convert.ToString(Session["userAccount"]));

            if (user.rol != "ADMCO")
            {
                return RedirectToAction("Index", "Home", null);
            }

            MyDirects ndirect = new MyDirects();
            empleadoTress add = new empleadoTress();
            add = add.datosTress(empleado.Substring(3, empleado.Length - 3), empleado.Substring(0, 3));

            t_empleados t_empleados = db.t_empleados.Find(empleado);

            ndirect.nombre = t_empleados.nombre;
            ndirect.empleado = t_empleados.empleado;
            ndirect.usuario = t_empleados.t_usuarios.usuario;
            ndirect.sextra1 = t_empleados.t_usuarios.email;
            ndirect.depto = add.depto;
            ndirect.puesto = add.puesto;
            try
            {
                ndirect.manager1 = t_empleados.t_empleados21.nombre;
            }
            catch
            {
                ndirect.manager1 = "";
            }

            ViewBag.details = ndirect;

            return View();
        }

        // POST: t_usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string usuario)
        {
            if (!Login())
                return RedirectToAction("NoUser", "Home", null);

            t_usuarios user = db.t_usuarios.Find(Convert.ToString(Session["userAccount"]));

            if (user.rol != "ADMCO")
            {
                return RedirectToAction("Index", "Home", null);
            }

            t_usuarios t_usuarios = db.t_usuarios.Find(usuario);
            t_usuarios.activo = "0";
            db.Entry(t_usuarios).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "t_usuarios", null);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public bool Login()
        {
            string username = Convert.ToString(User.Identity.Name).Substring(11).ToLower();
            Session["userAccount"] = username;
           
            var t_usuarios = db.t_usuarios.Find(username);
            Session["Rol"] = t_usuarios.rol;
            if (t_usuarios == null)
            {
                //si usuario no esta
                return false;
            }
            else
            {
                var nombreusuario = t_usuarios.nombre.Split(' ');
                ViewBag.userFirstName = nombreusuario[0];
                var empleado = db.t_empleados.Where(x => x.usuario == username).ToList();
                if (empleado.Any())
                {
                    Session["userNo"] = empleado[0].empleado;

                    string numempleado = empleado[0].empleado;
                    if (empleado[0].planta == "686")
                    {

                        empleadoTress add = new empleadoTress();

                        add = add.datosTress(numempleado.Substring(3, numempleado.Length - 3), numempleado.Substring(0, 3));
                        ViewBag.userJobPosition = add.puesto;
                        ViewBag.userImage = "data:image/png;base64," + Convert.ToBase64String(add.btImagen, 0, add.btImagen.Length);
                    }
                    Session["Plant"] = t_usuarios.planta;
                    Session["EmployeeNo"] = empleado[0].empleado;
                    return true;
                }
                else
                {
                    //si el nombre del usuario no existe en empleados
                    return false;
                }

            }

        }
    }
}
