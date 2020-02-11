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
        public ActionResult Create()
        {
            Login();
            string empleado = Convert.ToString(Session["EmployeeNo"]);
            int totalweight = 0;
            var objetivoHeader = db.t_objetivos.Include(t => t.t_empleados).Include(t => t.t_plantas).Where(x => x.empleado == empleado).OrderByDescending(x => x.axo).ToList().ElementAt(0);

            if (objetivoHeader.estatus == "EN")
                RedirectToAction("Approval", new {empleado = objetivoHeader.empleado });

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


            ViewBag.folio = new SelectList(db.t_objetivos, "folio", "folio");
            ViewBag.metrico = new SelectList(db.t_metricos, "metrico", "descrip");
            ViewBag.planta = new SelectList(db.t_plantas, "planta", "planta");
            return View();
        }

        public ActionResult Approval(string empleado)
        {
            Login();
        
            var objetivoHeader = db.t_objetivos.Include(t => t.t_empleados).Include(t => t.t_plantas).Where(x => x.empleado == empleado).OrderByDescending(x => x.axo).ToList().ElementAt(0);
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

            var ObjectivesDet = db.t_objetidet.Include(t => t.t_metricos).Include(t => t.t_objetivos).Where(x => x.empleado == objetivoHeader.empleado && x.axo == objetivoHeader.axo);
            
            ViewBag.ObjectivesDet = ObjectivesDet;
           
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
                return RedirectToAction("Create");
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

        public ActionResult DeleteObjective(string empleado, int axo, int consec)
        {
            t_objetidet t_objetidet = db.t_objetidet.Find(empleado,axo,consec);
            db.t_objetidet.Remove(t_objetidet);
            db.SaveChanges();
            return RedirectToAction("Create");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public void Login()
        {
            string[] infoTressEmpleado;
            string username = Convert.ToString(User.Identity.Name).Substring(11).ToLower();
            Session["userAccount"] = username;
            var t_usuarios = db.t_usuarios.Find(username);
            if (t_usuarios == null)
            {
                //si usuario no esta
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
                }
                else
                {
                    //si el nombre del usuario no existe en empleados
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
