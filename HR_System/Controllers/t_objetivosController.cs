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

            decimal lastyearobj = 0;

            bool newobj = false;

            string empleado = Convert.ToString(Session["EmployeeNo"]);

            ViewBag.empleado = empleado;

            var t_objetivos = db.t_objetivos.Include(t => t.t_empleados).Include(t => t.t_plantas).Where(x => x.empleado == empleado).OrderByDescending(x => x.axo).ToList();

            var t_empleados = db.t_empleados.Include(t => t.t_plantas).Include(t => t.t_usuarios).Include(t => t.t_empleados2).Where(x => x.supervisor == empleado).ToList();

            var t_config_m1 = db.t_config_m1.ToList().ElementAt(0);

            if(t_objetivos.Any())
            {
                lastyearobj = t_objetivos.ElementAt(0).axo;
                if (lastyearobj != t_config_m1.axo_activo)
                    newobj = true;
            }
            else
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
            return RedirectToAction("Approval", "t_objetidet", new { empleado = empleado, axo = axo });

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
            return RedirectToAction("Approval", "t_objetidet", new {empleado= empleado, axo = axo });

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
            string[] infoTressEmpleado;
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

                    string numempleado = empleado[0].empleado.Substring(3, 5);
                    if (empleado[0].planta == "686")
                    {
                        infoTressEmpleado = datosTress686(numempleado);
                        ViewBag.userJobPosition = infoTressEmpleado[2];
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

        public string[] datosTress686(string id)
        {

            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            var db = WebMatrix.Data.Database.Open("686TressConn");



            var selectedQueryString = @"SELECT co.cb_nombres + ' ' +co.CB_APE_PAT + ' ' +co.CB_APE_MAT as NOMBRE
                                        ,co.CB_TURNO,po.PU_DESCRIP AS PUESTO,
                                        co.CB_NIVEL2,n3.TB_ELEMENT AS AREA,co2.CB_E_MAIL
                                        from [Tress_MedlineMXL].[dbo].COLABORA co 
                                        inner join [Tress_MedlineMXL].[dbo].PUESTO po on co.CB_PUESTO = po.PU_CODIGO 
                                        inner join [Tress_MedlineMXL].[dbo].NIVEL2 n2 on co.CB_NIVEL2 = n2.TB_CODIGO 
                                        inner join [Tress_MedlineMXL].[dbo].NIVEL3 n3 on co.CB_NIVEL3 = n3.TB_CODIGO 
                                        inner join [Tress_MedlineMXL].[dbo].NIVEL4 n4 on co.CB_NIVEL4 = n4.TB_CODIGO 
                                        inner join [Tress_MedlineMXL].[dbo].TURNO tu on co.CB_TURNO = tu.TU_CODIGO 
										left join [Tress_MedlineMXL].[dbo].COLABORA co2 on co.CB_NIVEL4 = co2.CB_CODIGO

                                        WHERE co.CB_CODIGO = " + id + @" and co.CB_ACTIVO = 'S' 
                                        ORDER BY PU_DESCRIP,CB_TURNO";

            var datos = db.Query(selectedQueryString);


            string[] empleado = new string[6];

            if (datos.Any())
            {
                for (int i = 0; i < 6; i++)
                {
                    empleado[i] = datos.ElementAt(0)[i];
                }
                empleado[0] = cultInfo.ToTitleCase(empleado[0].ToString().ToLower());
            }


            return empleado;
        }
    }
}
