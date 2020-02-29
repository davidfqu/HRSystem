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
    public class t_config_m1Controller : Controller
    {
        private HRSystemEntities db = new HRSystemEntities();

        // GET: t_config_m1
        public ActionResult Index()
        {
            return View(db.t_config_m1.ToList());
        }

        // GET: t_config_m1/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_config_m1 t_config_m1 = db.t_config_m1.Find(id);
            if (t_config_m1 == null)
            {
                return HttpNotFound();
            }
            return View(t_config_m1);
        }

        // GET: t_config_m1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: t_config_m1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "clave,f_objini,f_objfin,axo_activo,f_evalini1,f_evalfin1,f_evalini2,f_evalfin2,f_califica,f_calificafin")] t_config_m1 t_config_m1)
        {
            if (ModelState.IsValid)
            {
                db.t_config_m1.Add(t_config_m1);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(t_config_m1);
        }

        // GET: t_config_m1/Edit/5
        public ActionResult Edit(string id, string alert = "")
        {
            if(alert == "1")
            {
                ViewBag.Alert = "1";
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_config_m1 t_config_m1 = db.t_config_m1.Find(id);
            if (t_config_m1 == null)
            {
                return HttpNotFound();
            }
            return View(t_config_m1);
        }

        // POST: t_config_m1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "clave,f_objini,f_objfin,axo_activo,f_evalini1,f_evalfin1,f_evalini2,f_evalfin2,f_califica,f_calificafin")] t_config_m1 t_config_m1)
        {
            if (ModelState.IsValid)
            {
                db.Entry(t_config_m1).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit",new { id = t_config_m1.clave, alert = "1" });
            }
            return View(t_config_m1);
        }

        // GET: t_config_m1/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_config_m1 t_config_m1 = db.t_config_m1.Find(id);
            if (t_config_m1 == null)
            {
                return HttpNotFound();
            }
            return View(t_config_m1);
        }

        // POST: t_config_m1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            t_config_m1 t_config_m1 = db.t_config_m1.Find(id);
            db.t_config_m1.Remove(t_config_m1);
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
