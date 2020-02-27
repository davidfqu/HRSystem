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
        public ActionResult NewUser()
        {
            ViewBag.planta = new SelectList(db.t_plantas, "planta", "nombre");

            List<SelectListItem> roles = new List<SelectListItem>();
            roles.Add(new SelectListItem() { Text = "User", Value = "USER" });
            roles.Add(new SelectListItem() { Text = "Manager", Value = "MGR" });
            roles.Add(new SelectListItem() { Text = "Administrator", Value = "ADMCO" });
            ViewBag.rol = new SelectList(roles, "Value", "Text");

            ViewBag.supervisor = new SelectList(db.t_empleados,"empleado", "nombre");
            ViewBag.manager = new SelectList(db.t_empleados.Include(t => t.t_usuarios).Where(x => x.t_usuarios.rol == "MGR"), "empleado","nombre");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewUser(string planta, string empleado, string usuario, string email, string rol, string supervisor, string manager)
        {
           
            return View();
        }


        public ActionResult Index()
        {
            var t_usuarios = db.t_usuarios.Include(t => t.t_plantas);
            return View(t_usuarios.ToList());
        }

        // GET: t_usuarios/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_usuarios t_usuarios = db.t_usuarios.Find(id);
            if (t_usuarios == null)
            {
                return HttpNotFound();
            }
            return View(t_usuarios);
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
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_usuarios t_usuarios = db.t_usuarios.Find(id);
            if (t_usuarios == null)
            {
                return HttpNotFound();
            }
            ViewBag.planta = new SelectList(db.t_plantas, "planta", "nombre", t_usuarios.planta);
            return View(t_usuarios);
        }

        // POST: t_usuarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "usuario,nombre,planta,email,activo,rol,f_id")] t_usuarios t_usuarios)
        {
            if (ModelState.IsValid)
            {
                db.Entry(t_usuarios).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.planta = new SelectList(db.t_plantas, "planta", "nombre", t_usuarios.planta);
            return View(t_usuarios);
        }

        // GET: t_usuarios/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_usuarios t_usuarios = db.t_usuarios.Find(id);
            if (t_usuarios == null)
            {
                return HttpNotFound();
            }
            return View(t_usuarios);
        }

        // POST: t_usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            t_usuarios t_usuarios = db.t_usuarios.Find(id);
            db.t_usuarios.Remove(t_usuarios);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
