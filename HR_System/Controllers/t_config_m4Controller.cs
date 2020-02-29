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
    public class t_config_m4Controller : Controller
    {
        private HRSystemEntities db = new HRSystemEntities();

        // GET: t_config_m4
        public ActionResult Index()
        {
            return View(db.t_config_m4.ToList());
        }

        // GET: t_config_m4/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_config_m4 t_config_m4 = db.t_config_m4.Find(id);
            if (t_config_m4 == null)
            {
                return HttpNotFound();
            }
            return View(t_config_m4);
        }

        // GET: t_config_m4/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: t_config_m4/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "clave,f_inicio,f_final,cap_mrp,ind_alerta1")] t_config_m4 t_config_m4)
        {
            if (ModelState.IsValid)
            {
                db.t_config_m4.Add(t_config_m4);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(t_config_m4);
        }

        // GET: t_config_m4/Edit/5
        public ActionResult Edit(string id, string alert = "")
        {
            if (alert == "1")
            {
                ViewBag.Alert = "1";
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_config_m4 t_config_m4 = db.t_config_m4.Find(id);
            if (t_config_m4 == null)
            {
                return HttpNotFound();
            }
            return View(t_config_m4);
        }

        // POST: t_config_m4/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "clave,f_inicio,f_final,cap_mrp")] t_config_m4 t_config_m4, bool alert = false)

        {
            
            if(alert)
            {
                t_config_m4.ind_alerta1 = "1";
            }
            else
            {
                t_config_m4.ind_alerta1 = "1";
            }

            t_config_m4.cap_mrp = Convert.ToDecimal(Math.Round(Convert.ToDouble(t_config_m4.cap_mrp)));
            if (ModelState.IsValid)
            {
                db.Entry(t_config_m4).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = t_config_m4.clave, alert = "1" });
            }
            return View(t_config_m4);
        }

        // GET: t_config_m4/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_config_m4 t_config_m4 = db.t_config_m4.Find(id);
            if (t_config_m4 == null)
            {
                return HttpNotFound();
            }
            return View(t_config_m4);
        }

        // POST: t_config_m4/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            t_config_m4 t_config_m4 = db.t_config_m4.Find(id);
            db.t_config_m4.Remove(t_config_m4);
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
