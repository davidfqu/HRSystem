using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HR_System.Models;
using System.Globalization;

namespace HR_System.Controllers
{
    public class t_objetidetController :  Controller
    {
        private HRSystemEntities db = new HRSystemEntities();
      
        // GET: t_objetidet
        public ActionResult Index()
        {
            var t_objetidet = db.t_objetidet.Include(t => t.t_plantas);
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
        public ActionResult Create(string empleado, decimal axo)
        {
            if (!Login())
                return RedirectToAction("NoUser", "Home", null);

             empleado = Convert.ToString(Session["EmployeeNo"]);
            int totalweight = 0;
            var objetivoHeader = db.t_objetivos.Include(t => t.t_empleados).Include(t => t.t_plantas).Where(x => x.empleado == empleado && x.axo == axo).ToList().ElementAt(0);

            if (objetivoHeader.estatus != "PE")
                return RedirectToAction("Approval", new {empleado = objetivoHeader.empleado, axo = objetivoHeader.axo });

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
            
            var ObjectivesDet = db.t_objetidet.Include(t => t.t_objetivos).Where(x => x.empleado == objetivoHeader.empleado && x.axo == objetivoHeader.axo);
            foreach( var item in ObjectivesDet){
                totalweight = totalweight + Convert.ToInt16(item.peso);
            }

            ViewBag.TotalWeight = totalweight;
            ViewBag.ObjectivesQty = ObjectivesDet.ToList().Count();
            ViewBag.ObjectivesDet = ObjectivesDet;
            try
            {
                ViewBag.NextObjective = ObjectivesDet.OrderByDescending(x => x.consec).ToList().ElementAt(0).consec + 1;
            }
            catch
            {
                ViewBag.NextObjective = 1;
            }

            if(objetivoHeader.n_aprobado != null)
            {
                ViewBag.Rechazado = "1";
            }
            else
                ViewBag.Rechazado = "0";

            empleadoTress add = new empleadoTress();

            add = add.datosTress(empleado.Substring(3, empleado.Length - 3), empleado.Substring(0, 3));

            ViewBag.infoTress = add;

            ViewBag.folio = new SelectList(db.t_objetivos, "folio", "folio");
            ViewBag.planta = new SelectList(db.t_plantas, "planta", "planta");
            return View();
        }

        public ActionResult Approval(string empleado, decimal axo, bool ismanager  = false)
        {
            if (!Login())
                return RedirectToAction("NoUser", "Home", null);

            if (!ismanager)
            empleado = Convert.ToString(Session["EmployeeNo"]);

            var objetivoHeader = db.t_objetivos.Include(t => t.t_empleados).Include(t => t.t_plantas).Where(x => x.empleado == empleado && x.axo == axo).ToList().ElementAt(0);
            ViewBag.Objective = objetivoHeader;

            try
            {
                ViewBag.Manager = objetivoHeader.t_empleados.t_empleados2.nombre;

                if (objetivoHeader.t_empleados.t_empleados2.empleado == Convert.ToString(Session["EmployeeNo"]))
                    ViewBag.FirstManager = true;
                else
                    ViewBag.FirstManager = false;
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

            var ObjectivesDet = db.t_objetidet.Include(t => t.t_objetivos).Where(x => x.empleado == objetivoHeader.empleado && x.axo == objetivoHeader.axo);
            
            ViewBag.ObjectivesDet = ObjectivesDet;

            empleadoTress add = new empleadoTress();

            add = add.datosTress(empleado.Substring(3, empleado.Length - 3), empleado.Substring(0, 3));

            ViewBag.infoTress = add;

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
            t_objetidet.f_id = System.DateTime.Now; 
            if (ModelState.IsValid)
            {
                db.t_objetidet.Add(t_objetidet);
                db.SaveChanges();
                return RedirectToAction("Create", new {emplado = t_objetidet.axo, axo = t_objetidet.axo});
            }

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
            t_objetidet.f_id = System.DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Entry(t_objetidet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create", new { emplado = t_objetidet.axo, axo = t_objetidet.axo });
            }

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

        public ActionResult DeleteObjective(string empleado, int axo, int consec)
        {
            t_objetidet t_objetidet = db.t_objetidet.Find(empleado,axo,consec);
            db.t_objetidet.Remove(t_objetidet);
            db.SaveChanges();
            return RedirectToAction("Create", new { emplado = t_objetidet.axo, axo = t_objetidet.axo });
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
                    }

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
