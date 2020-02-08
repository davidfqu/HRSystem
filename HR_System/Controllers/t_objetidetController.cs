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
    public class t_objetidetController : Controller
    {
        private HRSystemEntities db = new HRSystemEntities();

        // GET: t_objetidet
        public ActionResult Index()
        {
            var t_objetidet = db.t_objetidet.Include(t => t.t_metricos).Include(t => t.t_plantas);
            return View(t_objetidet.ToList());
        }

        // GET: t_objetidet/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_objetidet t_objetidet = db.t_objetidet.Find(id);
            if (t_objetidet == null)
            {
                return HttpNotFound();
            }
            return View(t_objetidet);
        }

        // GET: t_objetidet/Create
        public ActionResult Create(string empleado = "68690145")
        {
            var objetivoHeader = db.t_objetivos.Include(t => t.t_empleados).Include(t => t.t_plantas).Where(x => x.empleado == empleado).OrderByDescending(x => x.axo).Single();
            ViewBag.Objective = objetivoHeader;

            try
            {
                ViewBag.Manager = objetivoHeader.t_empleados.t_empleados2.nombre;
            }
            catch
            {
                ViewBag.Manager = null;
            }

            try
            {
                ViewBag.Manager2 = objetivoHeader.t_empleados.t_empleados2.t_empleados2.nombre;
            }
            catch
            {
                ViewBag.Manager2 = null;
            }


            var ObjectivesDet = db.t_objetidet.Include(t => t.t_metricos).Include(t => t.t_objetivos).Where(x => x.empleado == objetivoHeader.empleado && x.axo == objetivoHeader.axo);
            ViewBag.ObjectivesDet = ObjectivesDet;
            try
            {
                ViewBag.NextObjective = ObjectivesDet.OrderByDescending(x => x.consec).ToList().ElementAt(0).consec + 1;
            }
            catch
            {
                ViewBag.NextObjective = 1;
            }


            ViewBag.folio = new SelectList(db.t_objetivos, "folio", "folio");
            ViewBag.metrico = new SelectList(db.t_metricos, "metrico", "descrip");
            ViewBag.planta = new SelectList(db.t_plantas, "planta", "planta");
            return View();
        }

        // POST: t_objetidet/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "empleado,axo,consec,planta,fecha,objetivo,descrip,peso,metrico,cancelado,nota_cancel,f_cancel,u_cancel,resultado1,nota_r1,f_r1,resultado2,nota_r2,f_r2,f_id")] t_objetidet t_objetidet)
        {
            t_objetidet.fecha = System.DateTime.Now;
            if (ModelState.IsValid)
            {
                db.t_objetidet.Add(t_objetidet);
                db.SaveChanges();
                return RedirectToAction("Create");
            }

            ViewBag.metrico = new SelectList(db.t_metricos, "metrico", "descrip", t_objetidet.metrico);
            ViewBag.planta = new SelectList(db.t_plantas, "planta", "nombre", t_objetidet.planta);
            return View(t_objetidet);
        }

        // GET: t_objetidet/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_objetidet t_objetidet = db.t_objetidet.Find(id);
            if (t_objetidet == null)
            {
                return HttpNotFound();
            }
            ViewBag.metrico = new SelectList(db.t_metricos, "metrico", "descrip", t_objetidet.metrico);
            ViewBag.planta = new SelectList(db.t_plantas, "planta", "nombre", t_objetidet.planta);
            return View(t_objetidet);
        }

        // POST: t_objetidet/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "empleado,axo,consec,planta,fecha,objetivo,descrip,peso,metrico,cancelado,nota_cancel,f_cancel,u_cancel,resultado1,nota_r1,f_r1,resultado2,nota_r2,f_r2,f_id")] t_objetidet t_objetidet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(t_objetidet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.metrico = new SelectList(db.t_metricos, "metrico", "descrip", t_objetidet.metrico);
            ViewBag.planta = new SelectList(db.t_plantas, "planta", "nombre", t_objetidet.planta);
            return View(t_objetidet);
        }

        // GET: t_objetidet/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_objetidet t_objetidet = db.t_objetidet.Find(id);
            if (t_objetidet == null)
            {
                return HttpNotFound();
            }
            return View(t_objetidet);
        }

        // POST: t_objetidet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            t_objetidet t_objetidet = db.t_objetidet.Find(id);
            db.t_objetidet.Remove(t_objetidet);
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
