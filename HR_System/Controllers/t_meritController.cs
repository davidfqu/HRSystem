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
    public class t_meritController : Controller
    {
        private HRSystemEntities db = new HRSystemEntities();

        // GET: t_merit
        public ActionResult Index()
        {
            var t_merit = db.t_merit.Include(t => t.t_empleados).Include(t => t.t_empleados1);
            return View(t_merit.ToList());
        }

        public ActionResult IndexModule4D()
        {
            return View();
        }


        public ActionResult IndexModule4()
        {
            if (!Login())
                return RedirectToAction("NoUser", "Home", null);

            string empleado = Convert.ToString(Session["EmployeeNo"]);
            var t_merit = db.t_merit.Include(t => t.t_empleados).Include(t => t.t_empleados1).Where(x => x.supervisor == empleado && x.axo == System.DateTime.Now.Year).ToList();
            
            if(!t_merit.Any())
                return RedirectToAction("Index", "Home", null);

            var t_meridet = db.t_meridet.Include(t => t.t_califica).Include(t => t.t_empleados).Include(t => t.t_jobcode).Include(t => t.t_merit).Where(x => x.supervisor == empleado && x.axo == System.DateTime.Today.Year).ToList();
            var directs = new List<MyDirects>();

            string estado = "";
            string iconocal = "";
            string colorcal = "";

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

                switch (item.calificacion)
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

                MyDirects ndirect = new MyDirects();
                empleadoTress add = new empleadoTress();
                add = add.datosTress(item.empleado.Substring(3, item.empleado.Length - 3), item.empleado.Substring(0, 3));
                ndirect.empleado = item.empleado;
                ndirect.nombre = item.nombre;
                ndirect.estatusm4 = estado;
                ndirect.axom4 = item.axo;
                ndirect.puesto = item.puesto;
                ndirect.foto = add.btImagen;
                ndirect.meritrec = Convert.ToDecimal(item.sugerido_porc);
                ndirect.iconoresult = iconocal;
                ndirect.coloriconoresult = colorcal;
                directs.Add(ndirect);

            }


            ViewBag.Directos = directs;

            ViewBag.merit = t_merit.ElementAt(0);
            ViewBag.available = t_merit.ElementAt(0).budget_imp - t_merit.ElementAt(0).budget_spen;
            ViewBag.percent = Convert.ToString(Math.Round((Convert.ToDouble(t_merit.ElementAt(0).budget_spen / t_merit.ElementAt(0).budget_imp)) * 100));

            return View();
        }

        // GET: t_merit/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_merit t_merit = db.t_merit.Find(id);
            if (t_merit == null)
            {
                return HttpNotFound();
            }
            return View(t_merit);
        }

        // GET: t_merit/Create
        public ActionResult Create()
        {
            ViewBag.autoriza = new SelectList(db.t_empleados, "empleado", "nombre");
            ViewBag.supervisor = new SelectList(db.t_empleados, "empleado", "nombre");
            return View();
        }

        // POST: t_merit/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "supervisor,axo,nombre,depto,budget_imp,budget_porc,budget_spen,estatus,notas,u_id,f_id,autoriza,ind_autoriza,f_autoriza,n_autoriza")] t_merit t_merit)
        {
            if (ModelState.IsValid)
            {
                db.t_merit.Add(t_merit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.autoriza = new SelectList(db.t_empleados, "empleado", "nombre", t_merit.autoriza);
            ViewBag.supervisor = new SelectList(db.t_empleados, "empleado", "nombre", t_merit.supervisor);
            return View(t_merit);
        }

        // GET: t_merit/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_merit t_merit = db.t_merit.Find(id);
            if (t_merit == null)
            {
                return HttpNotFound();
            }
            ViewBag.autoriza = new SelectList(db.t_empleados, "empleado", "nombre", t_merit.autoriza);
            ViewBag.supervisor = new SelectList(db.t_empleados, "empleado", "nombre", t_merit.supervisor);
            return View(t_merit);
        }

        // POST: t_merit/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "supervisor,axo,nombre,depto,budget_imp,budget_porc,budget_spen,estatus,notas,u_id,f_id,autoriza,ind_autoriza,f_autoriza,n_autoriza")] t_merit t_merit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(t_merit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.autoriza = new SelectList(db.t_empleados, "empleado", "nombre", t_merit.autoriza);
            ViewBag.supervisor = new SelectList(db.t_empleados, "empleado", "nombre", t_merit.supervisor);
            return View(t_merit);
        }

        // GET: t_merit/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            t_merit t_merit = db.t_merit.Find(id);
            if (t_merit == null)
            {
                return HttpNotFound();
            }
            return View(t_merit);
        }

        // POST: t_merit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            t_merit t_merit = db.t_merit.Find(id);
            db.t_merit.Remove(t_merit);
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
