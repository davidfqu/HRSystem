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
    public class t_objetivosController : Controller
    {
        private HRSystemEntities db = new HRSystemEntities();

        // GET: t_objetivos
        public ActionResult Index()
        {
            var t_objetivos = db.t_objetivos.Include(t => t.t_empleados).Include(t => t.t_plantas);
            return View(t_objetivos.ToList());
        }

        public ActionResult IndexModule1()
        {
            ViewBag.Objective = db.t_objetivos.Include(t => t.t_empleados).Include(t => t.t_plantas);
            ViewBag.ObjectivesDet = db.t_objetidet.Include(t => t.t_metricos).Include(t => t.t_objetivos);

            ViewBag.folio = new SelectList(db.t_objetivos, "folio", "folio");
            ViewBag.metrico = new SelectList(db.t_metricos, "metrico", "descrip");
            ViewBag.planta = new SelectList(db.t_plantas, "planta", "planta");
            return View();
        }

        // GET: t_objetivos/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_objetivos t_objetivos = db.t_objetivos.Find(id);
            if (t_objetivos == null)
            {
                return HttpNotFound();
            }
            return View(t_objetivos);
        }

        // GET: t_objetivos/Create
        public ActionResult Create()
        {
            ViewBag.empleado = new SelectList(db.t_empleados, "empleado", "nombre");
            ViewBag.planta = new SelectList(db.t_plantas, "planta", "nombre");
            return View();
        }

        // POST: t_objetivos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "planta,folio,empleado,fecha,axo,estatus,aprobado,f_aprobado,revision1,nota_r1,f_r1,revision2,nota_r2,f_r2,calificacion,f_id,f_enviado")] t_objetivos t_objetivos)
        {
            if (ModelState.IsValid)
            {
                db.t_objetivos.Add(t_objetivos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.empleado = new SelectList(db.t_empleados, "empleado", "nombre", t_objetivos.empleado);
            ViewBag.planta = new SelectList(db.t_plantas, "planta", "nombre", t_objetivos.planta);
            return View(t_objetivos);
        }

        // GET: t_objetivos/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_objetivos t_objetivos = db.t_objetivos.Find(id);
            if (t_objetivos == null)
            {
                return HttpNotFound();
            }
            ViewBag.empleado = new SelectList(db.t_empleados, "empleado", "nombre", t_objetivos.empleado);
            ViewBag.planta = new SelectList(db.t_plantas, "planta", "nombre", t_objetivos.planta);
            return View(t_objetivos);
        }

        public ActionResult Edit2(string empleado, int axo)
        {
            if (empleado == null && axo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_objetivos t_objetivos = db.t_objetivos.Where(x => x.empleado == empleado && x.axo == axo).Single();
            if (t_objetivos == null)
            {
                return HttpNotFound();
            }
            ViewBag.empleado = new SelectList(db.t_empleados, "empleado", "nombre", t_objetivos.empleado);
            ViewBag.planta = new SelectList(db.t_plantas, "planta", "nombre", t_objetivos.planta);
            return View(t_objetivos);
        }
        // POST: t_objetivos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "planta,folio,empleado,fecha,axo,estatus,aprobado,f_aprobado,revision1,nota_r1,f_r1,revision2,nota_r2,f_r2,calificacion,f_id,f_enviado")] t_objetivos t_objetivos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(t_objetivos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.empleado = new SelectList(db.t_empleados, "empleado", "nombre", t_objetivos.empleado);
            ViewBag.planta = new SelectList(db.t_plantas, "planta", "nombre", t_objetivos.planta);
            return View(t_objetivos);
        }

        // GET: t_objetivos/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_objetivos t_objetivos = db.t_objetivos.Find(id);
            if (t_objetivos == null)
            {
                return HttpNotFound();
            }
            return View(t_objetivos);
        }

        // POST: t_objetivos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            t_objetivos t_objetivos = db.t_objetivos.Find(id);
            db.t_objetivos.Remove(t_objetivos);
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
