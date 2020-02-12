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
    public class t_empleadosController : Controller
    {
        private HRSystemEntities db = new HRSystemEntities();

        // GET: t_empleados
        public ActionResult Index()
        {
            var t_empleados = db.t_empleados.Include(t => t.t_plantas).Include(t => t.t_usuarios).Include(t => t.t_empleados2);
            return View(t_empleados.ToList());
        }

        // GET: t_empleados/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_empleados t_empleados = db.t_empleados.Find(id);
            if (t_empleados == null)
            {
                return HttpNotFound();
            }
            return View(t_empleados);
        }

        // GET: t_empleados/Create
        public ActionResult Create()
        {
            ViewBag.planta = new SelectList(db.t_plantas, "planta", "nombre");
            ViewBag.usuario = new SelectList(db.t_usuarios, "usuario", "nombre");
            ViewBag.supervisor = new SelectList(db.t_empleados, "empleado", "nombre");
            return View();
        }

        // POST: t_empleados/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "empleado,nombre,supervisor,usuario,f_id,planta")] t_empleados t_empleados)
        {
            if (ModelState.IsValid)
            {
                db.t_empleados.Add(t_empleados);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.planta = new SelectList(db.t_plantas, "planta", "nombre", t_empleados.planta);
            ViewBag.usuario = new SelectList(db.t_usuarios, "usuario", "nombre", t_empleados.usuario);
            ViewBag.supervisor = new SelectList(db.t_empleados, "empleado", "nombre", t_empleados.supervisor);
            return View(t_empleados);
        }

        // GET: t_empleados/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_empleados t_empleados = db.t_empleados.Find(id);
            if (t_empleados == null)
            {
                return HttpNotFound();
            }
            ViewBag.planta = new SelectList(db.t_plantas, "planta", "nombre", t_empleados.planta);
            ViewBag.usuario = new SelectList(db.t_usuarios, "usuario", "nombre", t_empleados.usuario);
            ViewBag.supervisor = new SelectList(db.t_empleados, "empleado", "nombre", t_empleados.supervisor);
            return View(t_empleados);
        }

        // POST: t_empleados/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "empleado,nombre,supervisor,usuario,f_id,planta")] t_empleados t_empleados)
        {
            if (ModelState.IsValid)
            {
                db.Entry(t_empleados).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.planta = new SelectList(db.t_plantas, "planta", "nombre", t_empleados.planta);
            ViewBag.usuario = new SelectList(db.t_usuarios, "usuario", "nombre", t_empleados.usuario);
            ViewBag.supervisor = new SelectList(db.t_empleados, "empleado", "nombre", t_empleados.supervisor);
            return View(t_empleados);
        }

        // GET: t_empleados/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_empleados t_empleados = db.t_empleados.Find(id);
            if (t_empleados == null)
            {
                return HttpNotFound();
            }
            return View(t_empleados);
        }

        // POST: t_empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            t_empleados t_empleados = db.t_empleados.Find(id);
            db.t_empleados.Remove(t_empleados);
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
