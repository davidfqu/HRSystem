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
    public class t_objetivosController : Controller
    {
        private HRSystemEntities db = new HRSystemEntities();

         
        // GET: t_objetivos
        public ActionResult Index()
        {
            var t_objetivos = db.t_objetivos.Include(t => t.t_empleados).Include(t => t.t_plantas);
            return View(t_objetivos.ToList());
        }

        public ActionResult MenuModule1()
        {
            if (!Login())
                return RedirectToAction("NoUser", "Home", null);

            bool newobj = false;

            string empleado = Convert.ToString(Session["EmployeeNo"]);

            ViewBag.empleado = empleado;

            var t_objetivos = db.t_objetivos.Include(t => t.t_empleados).Include(t => t.t_plantas).Where(x => x.empleado == empleado).OrderByDescending(x => x.axo).ToList();
            var t_empleados = db.t_empleados.Include(t => t.t_plantas).Include(t => t.t_usuarios).Include(t => t.t_empleados2).Where(x => x.supervisor == empleado).ToList();
            var t_empleado = db.t_empleados.Find(empleado);

            ViewBag.Empoyee = t_empleado;
            try
            {
                ViewBag.Manager = t_empleado.t_empleados2.nombre;
            }
            catch
            {
                ViewBag.Manager = null;
            }

            try
            {
                ViewBag.Manager2 = t_empleado.t_empleados2.t_empleados2.nombre;
            }
            catch
            {
                ViewBag.Manager2 = null;
            }
            var t_config_m1 = db.t_config_m1.ToList().ElementAt(0);

            int i = 0;

            if (t_objetivos.Any())
            {
                
                foreach (var item in t_objetivos)
                    if (item.axo == t_config_m1.axo_activo)
                        i++;
            }
            else
            {
                newobj = true;
            }

            if (i == 0)
            {
                newobj = true;
            }
            
            if(System.DateTime.Now >= t_config_m1.f_objini && System.DateTime.Now <= t_config_m1.f_objfin && newobj)
            {
                ViewBag.NewObjective = "1";
                ViewBag.NewYear = t_config_m1.axo_activo;
            }
            if(t_empleados.Any())
            {
                ViewBag.DReports = "1";
            }

            empleadoTress add = new empleadoTress();
            
            ViewBag.DTRESS = add.datosTress(empleado.Substring(3, empleado.Length - 3), empleado.Substring(0,3));

            return View(t_objetivos);
        }

        public ActionResult DirectReports()
        {
            if (!Login())
                return RedirectToAction("NoUser", "Home", null);

            string empleado = Convert.ToString(Session["EmployeeNo"]);
            var t_objetivos = db.t_objetivos.Include(t => t.t_empleados).Include(t => t.t_plantas).Where(x => x.t_empleados.t_empleados2.empleado == empleado && x.estatus != "CA").ToList();


            return View(t_objetivos);
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
        public ActionResult Create([Bind(Include = "empleado,axo,planta,fecha,estatus,aprobado,f_aprobado,revision1,nota_r1,f_r1,revision2,nota_r2,f_r2,resultado_prom,calificacion,u_id,f_id,f_enviado,n_aprobado")] t_objetivos t_objetivos)
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

        public ActionResult CreateHeader(string empleado, decimal axo)
        {
            t_objetivos t_objetivos = new t_objetivos();
            t_objetivos.empleado = empleado;
            t_objetivos.axo = axo;
            t_objetivos.estatus = "PE";
            t_objetivos.fecha = System.DateTime.Now;
            t_objetivos.planta = empleado.Substring(0, 3);

            if (ModelState.IsValid)
            {
                db.t_objetivos.Add(t_objetivos);
                db.SaveChanges();
                return RedirectToAction("Create","t_objetidet", new {empleado = t_objetivos.empleado, axo = t_objetivos.axo });
            }
            
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
        public ActionResult SendToFirstApproval(string empleado, int axo)
        {
            if (empleado == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_objetivos t_objetivos = db.t_objetivos.Find(empleado, axo);
            if (t_objetivos == null)
            {
                return HttpNotFound();
            }
            t_objetivos.estatus = "EN";
            db.Entry(t_objetivos).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Approval", "t_objetidet", new { empleado = empleado, axo = axo });
          
        }

        public ActionResult Approve(string empleado, int axo)
        {
            if (empleado == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_objetivos t_objetivos = db.t_objetivos.Find(empleado, axo);
            if (t_objetivos == null)
            {
                return HttpNotFound();
            }

            t_objetivos.aprobado = Convert.ToString(Session["EmployeeNo"]);
            t_objetivos.estatus = "AP";

            db.Entry(t_objetivos).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Approval", "t_objetidet", new { empleado = empleado, axo = axo, ismanager = true });

        }

        public ActionResult Reject(string empleado, int axo, string n_aprobado)
        {
            if (empleado == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_objetivos t_objetivos = db.t_objetivos.Find(empleado, axo);
            if (t_objetivos == null)
            {
                return HttpNotFound();
            }
            t_objetivos.estatus = "PE";
            t_objetivos.n_aprobado = n_aprobado;
            db.Entry(t_objetivos).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Approval", "t_objetidet", new {empleado= empleado, axo = axo, ismanager = true });

        }
        // POST: t_objetivos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "empleado,axo,planta,fecha,estatus,aprobado,f_aprobado,revision1,nota_r1,f_r1,revision2,nota_r2,f_r2,resultado_prom,calificacion,u_id,f_id,f_enviado,n_aprobado")] t_objetivos t_objetivos)
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

        public bool Login()
        {
            string username = Convert.ToString(User.Identity.Name).Substring(11).ToLower();
            Session["userAccount"] = username;
            var t_usuarios = db.t_usuarios.Find(username);
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
