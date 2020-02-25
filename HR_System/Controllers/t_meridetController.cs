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
    public class t_meridetController : Controller
    {
        private HRSystemEntities db = new HRSystemEntities();

        // GET: t_meridet
        public ActionResult Index()
        {
            var t_meridet = db.t_meridet.Include(t => t.t_califica).Include(t => t.t_empleados).Include(t => t.t_jobcode).Include(t => t.t_merit);
            return View(t_meridet.ToList());
        }

        public ActionResult DirectReports()
        {
            if (!Login())
                return RedirectToAction("NoUser", "Home", null);

            string empleado = Convert.ToString(Session["EmployeeNo"]);

            //Si el empleado que quiere ingresar no esta como supervisor y el año de los objetivos no es el actual, regresar al menu
            var t_merit = db.t_merit.Include(t => t.t_empleados).Include(t => t.t_empleados1).Where(x => x.supervisor == empleado && x.axo == System.DateTime.Now.Year).ToList();

            if (!t_merit.Any())
                return RedirectToAction("Index", "Home", null);
            
            var t_meridet = db.t_meridet.Include(t => t.t_califica).Include(t => t.t_empleados).Include(t => t.t_jobcode).Include(t => t.t_merit).Where(x => x.supervisor == empleado && x.axo == System.DateTime.Today.Year).ToList();

            var directs = new List<MyDirects>();

            string estado = "";

            foreach (var item in t_meridet)
            {
                switch (item.estatus)
                {
                    case "PE":
                        estado = "Waiting For Aproval";
                        break;

                    case "AP":
                        estado = "Approved";
                        break;

                    default:
                        estado = "No Merit";
                        break;
                }

                MyDirects ndirect = new MyDirects();
                empleadoTress add = new empleadoTress();
                add = add.datosTress(item.empleado.Substring(3, item.empleado.Length - 3), item.empleado.Substring(0, 3));
                ndirect.empleado = item.empleado;
                ndirect.nombre = item.nombre;
                ndirect.estatusm4 = estado;
                ndirect.axom4 = item.axo;
                ndirect.puesto = add.puesto;
                ndirect.foto = add.btImagen;
                ndirect.meritrec = Convert.ToDecimal(item.sugerido_porc);
                directs.Add(ndirect);
             
            }


            ViewBag.Directos = directs;
            return View();
        }

        public ActionResult MeritProcess(string empleado, decimal axo, string supervisor)
        {
            if (!Login())
                return RedirectToAction("NoUser", "Home", null);
            
            var t_merit = db.t_merit.Include(t => t.t_empleados).Include(t => t.t_empleados1).Where(x => x.supervisor == supervisor && x.axo == System.DateTime.Now.Year).ToList();

            if (!t_merit.Any())
                return RedirectToAction("Index", "Home", null);


            ViewBag.spent = t_merit.ElementAt(0).budget_spen;
            ViewBag.available = t_merit.ElementAt(0).budget_imp - t_merit.ElementAt(0).budget_spen;
            ViewBag.percent = Convert.ToString(Math.Round((ViewBag.available / t_merit.ElementAt(0).budget_imp) * 100));

            var t_meridet = db.t_meridet.Find(supervisor, axo, empleado);

            MyDirects ndirect = new MyDirects();
            empleadoTress add = new empleadoTress();
            add = add.datosTress(empleado.Substring(3, empleado.Length - 3), empleado.Substring(0, 3));
            ndirect.empleado = empleado;
            ndirect.nombre = t_meridet.nombre;
            ndirect.depto = add.depto;
            ndirect.puesto = add.puesto;
            ndirect.foto = add.btImagen;
            string iconocal = "";
            string colorcal = "";
            switch (t_meridet.calificacion)
            {
                case "EE":
                    iconocal = "<i class='fas fa-medal'></i>";
                    colorcal = "warning-color-dark";
                    break;
                case "ME":
                    iconocal = "<i class='fas fa-thumbs-up'></i>";
                    colorcal = "success-color";
                    break;
                case "NI":
                    iconocal = "<i class='fas fa-exclamation'></i>";
                    colorcal = "warning-color";
                    break;
                case "FE":
                    iconocal = "<i class='fas fa-thumbs-down'></i>";
                    colorcal = "danger-color";
                    break;
                default:
                    iconocal = "<i class='fas fa-minus'></i>";
                    colorcal = "stylish-color";
                    break;
            }

            ViewBag.colorcal = colorcal;
            ViewBag.iconocal = iconocal;

            try
            {
                ndirect.manager1 = t_meridet.t_empleados.t_empleados2.nombre;
            }
            catch
            {
                ndirect.manager1 = null;
            }
            try
            {
                ndirect.manager2 = t_meridet.t_empleados.t_empleados2.t_empleados2.nombre;
            }
            catch
            {
                ndirect.manager2 = null;
            }

            ViewBag.direct = ndirect;
            ViewBag.merit = t_meridet;
            ViewBag.bamount = Math.Truncate(100*(Convert.ToDouble((t_meridet.salario_axo * t_meridet.budget_porc) / 100)))/100 ;
            ViewBag.maxmerit = ViewBag.bamount * 2;


            decimal meritg1 = Convert.ToDecimal((t_meridet.t_califica.rango_ini * t_merit.ElementAt(0).budget_porc)) / 100;
            decimal meritg2 = Convert.ToDecimal((t_meridet.t_califica.rango_fin * t_merit.ElementAt(0).budget_porc)) / 100;

            
            ViewBag.meritg1 = meritg1;
            ViewBag.meritg2 = meritg2;
            return View();
        }

        public ActionResult Approve(String empleado, decimal merit = 0, decimal lump = 0, string comments = "", decimal meritper = 0, decimal lumpper = 0)
        {
            if (!Login())
                return RedirectToAction("NoUser", "Home", null);

            String supervisor = Convert.ToString(Session["EmployeeNo"]);

            bool completed = true;

            var t_merit = db.t_merit.Include(t => t.t_empleados).Include(t => t.t_empleados1).Where(x => x.supervisor == supervisor && x.axo == System.DateTime.Now.Year).ToList();

            if (!t_merit.Any())
                return RedirectToAction("Index", "Home", null);

            t_meridet t_meridet = db.t_meridet.Find(supervisor,System.DateTime.Now.Year,empleado);

            t_meridet.salario_nuevo = t_meridet.salario_axo + merit;
            t_meridet.sugerido_imp = merit;
            t_meridet.sugerido_porc = meritper;
            t_meridet.lump_porc = lumpper;
            t_meridet.lump_imp = lump;
            t_meridet.estatus = "AP";
            t_meridet.nota = comments;
            t_meridet.u_id = supervisor;
            t_meridet.f_id = System.DateTime.Now;
            db.Entry(t_meridet).State = EntityState.Modified;
            db.SaveChanges();

            var t_meridet2 = db.t_meridet.Where(x => x.supervisor == supervisor && x.axo == System.DateTime.Now.Year);

            foreach (var item in t_meridet2)
            {
                if (item.estatus != "AP")
                    completed = false;
            }

            if(completed)
            {
                t_merit t_merit2 = db.t_merit.Find(supervisor, System.DateTime.Now.Year);
                t_merit2.estatus = "AP";
                db.Entry(t_merit2).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("IndexModule4", "t_merit", null);
        }

        // GET: t_meridet/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_meridet t_meridet = db.t_meridet.Find(id);
            if (t_meridet == null)
            {
                return HttpNotFound();
            }
            return View(t_meridet);
        }

        // GET: t_meridet/Create
        public ActionResult Create()
        {
            ViewBag.calificacion = new SelectList(db.t_califica, "califica", "descrip");
            ViewBag.empleado = new SelectList(db.t_empleados, "empleado", "nombre");
            ViewBag.jobcode = new SelectList(db.t_jobcode, "clave", "descrip");
            ViewBag.supervisor = new SelectList(db.t_merit, "supervisor", "nombre");
            return View();
        }

        // POST: t_meridet/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "supervisor,axo,empleado,nombre,ind_elegible,jobcode,puesto,calificacion,salario_axo,budget_porc,sugerido_imp,sugerido_porc,lump_imp,lump_porc,salario_nuevo,market_mcr,merit_guide,estatus,nota,out_guide,u_id,f_id")] t_meridet t_meridet)
        {
            if (ModelState.IsValid)
            {
                db.t_meridet.Add(t_meridet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.calificacion = new SelectList(db.t_califica, "califica", "descrip", t_meridet.calificacion);
            ViewBag.empleado = new SelectList(db.t_empleados, "empleado", "nombre", t_meridet.empleado);
            ViewBag.jobcode = new SelectList(db.t_jobcode, "clave", "descrip", t_meridet.jobcode);
            ViewBag.supervisor = new SelectList(db.t_merit, "supervisor", "nombre", t_meridet.supervisor);
            return View(t_meridet);
        }

        // GET: t_meridet/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_meridet t_meridet = db.t_meridet.Find(id);
            if (t_meridet == null)
            {
                return HttpNotFound();
            }
            ViewBag.calificacion = new SelectList(db.t_califica, "califica", "descrip", t_meridet.calificacion);
            ViewBag.empleado = new SelectList(db.t_empleados, "empleado", "nombre", t_meridet.empleado);
            ViewBag.jobcode = new SelectList(db.t_jobcode, "clave", "descrip", t_meridet.jobcode);
            ViewBag.supervisor = new SelectList(db.t_merit, "supervisor", "nombre", t_meridet.supervisor);
            return View(t_meridet);
        }

        // POST: t_meridet/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "supervisor,axo,empleado,nombre,ind_elegible,jobcode,puesto,calificacion,salario_axo,budget_porc,sugerido_imp,sugerido_porc,lump_imp,lump_porc,salario_nuevo,market_mcr,merit_guide,estatus,nota,out_guide,u_id,f_id")] t_meridet t_meridet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(t_meridet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.calificacion = new SelectList(db.t_califica, "califica", "descrip", t_meridet.calificacion);
            ViewBag.empleado = new SelectList(db.t_empleados, "empleado", "nombre", t_meridet.empleado);
            ViewBag.jobcode = new SelectList(db.t_jobcode, "clave", "descrip", t_meridet.jobcode);
            ViewBag.supervisor = new SelectList(db.t_merit, "supervisor", "nombre", t_meridet.supervisor);
            return View(t_meridet);
        }

        // GET: t_meridet/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_meridet t_meridet = db.t_meridet.Find(id);
            if (t_meridet == null)
            {
                return HttpNotFound();
            }
            return View(t_meridet);
        }

        // POST: t_meridet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            t_meridet t_meridet = db.t_meridet.Find(id);
            db.t_meridet.Remove(t_meridet);
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
                        ViewBag.userImage = "data:image/png;base64," + Convert.ToBase64String(add.btImagen, 0, add.btImagen.Length);
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
