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

        public ActionResult AutorizeMP(string supervisor, decimal axo)
        {
            if(!Login())
                return RedirectToAction("NoUser", "Home", null);

            t_merit t_merit = db.t_merit.Find(supervisor, axo);
            t_merit.estatus = "AU";
            t_merit.ind_autoriza = "1";
            t_merit.f_autoriza = System.DateTime.Now;
            db.Entry(t_merit).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("IndexModule4", "t_merit", new {empleado = t_merit.supervisor });
        }

        public ActionResult Reject(string supervisor, decimal axo, string n_autoriza)
        {
            if (!Login())
                return RedirectToAction("NoUser", "Home", null);

            t_merit t_merit = db.t_merit.Find(supervisor, axo);
            t_merit.ind_autoriza = "2";
            t_merit.f_autoriza = System.DateTime.Now;
            t_merit.n_autoriza = n_autoriza;
            db.Entry(t_merit).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("IndexModule4", "t_merit", new { empleado = t_merit.supervisor });
        }

        public ActionResult AddComments(string supervisor, decimal axo, string comments)
        {
            if (!Login())
                return RedirectToAction("NoUser", "Home", null);

            t_merit t_merit = db.t_merit.Find(supervisor, axo);
            t_merit.n_autoriza = comments;
            t_merit.f_autoriza = System.DateTime.Now;
            db.Entry(t_merit).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("IndexModule4", "t_merit", new { empleado = t_merit.supervisor });
        }

        public ActionResult IndexModule4D()
        {
            if (!Login())
                return RedirectToAction("NoUser", "Home", null);

            string planta = Convert.ToString(Session["Plant"]);

            var configplanta = db.t_plantas.Find(planta);

            if (Convert.ToString(Session["userAccount"]) != configplanta.direct_email)
                return RedirectToAction("Index", "Home", null);

            string empleado = Convert.ToString(Session["EmployeeNo"]);

            var t_merit = db.t_merit.Include(t => t.t_empleados).Include(t => t.t_empleados1).Where(x=> x.axo == System.DateTime.Now.Year && (x.autoriza == empleado || x.supervisor == empleado)).OrderBy(x => x.supervisor).ToList();
            var t_meridet = db.t_meridet.Include(t => t.t_califica).Include(t => t.t_empleados).Include(t => t.t_jobcode).Include(t => t.t_merit).Where(x => x.supervisor == empleado && x.axo == System.DateTime.Today.Year).OrderBy(x => x.supervisor).ToList();
            var t_meridet2 = db.t_meridet.Include(t => t.t_califica).Include(t => t.t_empleados).Include(t => t.t_jobcode).Include(t => t.t_merit).Where(x => x.axo == System.DateTime.Today.Year && x.supervisor.Substring(0,3) == planta).OrderBy(x => x.supervisor).ToList();


            double approve = 0;
            decimal totalbudget = 0;
            decimal totalspent = 0;
            decimal totalavailable = 0;
            bool mstatus = true;
            string smstatus = "";
            string istatus = "";
            string estado = "";
            string iconocal = "";
            string colorcal = "";
            var departments = new List<MyDirects>();
            var directs = new List<MyDirects>();

            foreach(var item in t_meridet2)
            {
                if (item.estatus == "AP")
                    approve++;
            }

            foreach (var item in t_merit)
            {
                totalbudget = Convert.ToDecimal(totalbudget + item.budget_imp);
                totalspent = Convert.ToDecimal(totalspent + item.budget_spen);

                switch (item.estatus)
                {
                    case "PE":
                        istatus = "In Process";
                        mstatus = false;
                        break;
                    case "AP":
                        istatus = "Completed";
                        mstatus = false;
                        break;
                    default:
                        istatus = "Autorized";
                        break;
                }
                
                MyDirects ndirect = new MyDirects();
                empleadoTress add = new empleadoTress();
                add = add.datosTress(item.supervisor.Substring(3, item.supervisor.Length - 3), item.supervisor.Substring(0, 3));

                ndirect.empleado = item.supervisor;
                ndirect.depto = item.depto;
                ndirect.nombre = item.nombre;
                ndirect.foto = add.btImagen;
                ndirect.dextra1 = Convert.ToDecimal(item.budget_imp);
                ndirect.dextra2 = Convert.ToDecimal(item.budget_spen);
                ndirect.estatusm4 = istatus;
                ndirect.sextra1 = (Math.Round((ndirect.dextra2 / ndirect.dextra1)*100)).ToString() + "%";
                departments.Add(ndirect);

            }

            if (mstatus)
                smstatus = "Completed";
            else
                smstatus = "In Process";

            foreach (var item in t_meridet)
            {
                switch (item.estatus)
                {
                    case "PE":
                        estado = "Pending";
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
                ndirect.manager1 = item.supervisor;
                ndirect.empleado = item.empleado;
                ndirect.nombre = item.nombre;
                ndirect.estatusm4 = estado;
                ndirect.axom4 = item.axo;
                ndirect.puesto = item.puesto;
                ndirect.foto = add.btImagen;
                ndirect.meritrec = Convert.ToDecimal(item.sugerido_porc);
                ndirect.iconoresult = iconocal;
                ndirect.coloriconoresult = colorcal;
                ndirect.lump = Convert.ToDecimal(item.lump_imp);
                directs.Add(ndirect);


            }

            totalavailable = totalbudget - totalspent;
            ViewBag.budgetper = t_merit.ElementAt(0).budget_porc;
            ViewBag.tbudget = totalbudget;
            ViewBag.tspent = totalspent;
            ViewBag.tavailable = totalavailable;
            ViewBag.statusmerit = smstatus;

            ViewBag.ApproveCount = approve;
            string aper = Math.Round((approve / t_meridet2.Count) * 100).ToString() + "%";
            ViewBag.ApprovePer = aper;
            ViewBag.MCount = t_meridet2.Count;

            ViewBag.percent = Math.Round((ViewBag.tspent / ViewBag.tbudget) * 100);
            ViewBag.departments = departments;
            ViewBag.directs = directs;


            return View();
        }


        public ActionResult IndexModule4(string empleado = "")
        {
            if (!Login())
                return RedirectToAction("NoUser", "Home", null);
            
            if (empleado == "")
            {
                empleado = Convert.ToString(Session["EmployeeNo"]);

                var configplanta = db.t_plantas.Find(Convert.ToString(Session["Plant"]));

                if (Convert.ToString(Session["userAccount"]) == configplanta.direct_email)
                    return RedirectToAction("IndexModule4D", "t_merit", null);
            }
            
            
            var t_merit = db.t_merit.Include(t => t.t_empleados).Include(t => t.t_empleados1).Where(x => x.supervisor == empleado && x.axo == System.DateTime.Now.Year).ToList();
            
            if(!t_merit.Any())
                return RedirectToAction("Index", "Home", null);

            var t_meridet = db.t_meridet.Include(t => t.t_califica).Include(t => t.t_empleados).Include(t => t.t_jobcode).Include(t => t.t_merit).Where(x => x.supervisor == empleado && x.axo == System.DateTime.Today.Year).ToList();
            var directs = new List<MyDirects>();

            string estado = "";
            string iconocal = "";
            string colorcal = "";

            double approve = 0;

            foreach (var item in t_meridet)
            {
                switch (item.estatus)
                {
                    case "PE":
                        estado = "Pending";
                        break;

                    case "AP":
                        estado = "Approved";
                        approve++;
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
                ndirect.manager1 = item.supervisor;
                ndirect.empleado = item.empleado;
                ndirect.nombre = item.nombre;
                ndirect.estatusm4 = estado;
                ndirect.axom4 = item.axo;
                ndirect.puesto = item.puesto;
                ndirect.foto = add.btImagen;
                ndirect.meritrec = Convert.ToDecimal(item.sugerido_porc);
                ndirect.iconoresult = iconocal;
                ndirect.coloriconoresult = colorcal;
                ndirect.lump = Convert.ToDecimal(item.lump_imp);
                directs.Add(ndirect);


            }

            ViewBag.ApproveCount = approve;
            ViewBag.Directos = directs;
            string aper = Math.Round((approve / directs.Count) * 100 ).ToString() +"%";
            ViewBag.ApprovePer = aper;

            ViewBag.merit = t_merit.ElementAt(0);
            ViewBag.available = t_merit.ElementAt(0).budget_imp - t_merit.ElementAt(0).budget_spen;
            ViewBag.percent = Convert.ToString(Math.Round(Convert.ToDouble((t_merit.ElementAt(0).budget_spen / t_merit.ElementAt(0).budget_imp)* 100)));

            string statusmerit = "";

            switch (t_merit.ElementAt(0).estatus)
            {
                case "PE":
                    statusmerit = "In Process";
                    break;

                case "AP":
                    statusmerit = "Completed";
                    break;

                case "AU":
                    statusmerit = "Autorized";
                    break;

                case "AA":
                    statusmerit = "Aplied";
                    break;

                default:
                    statusmerit = "No Merit";
                    break;
            }

            

            ViewBag.statusmerit = statusmerit;

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

        public ActionResult Start(string sparame, int sparamx)
        {
            if (!Login())
                return RedirectToAction("NoUser", "Home", null);

            var budgetAxo = db.t_budget_pta.Where(x => x.axo == sparamx).ToList();
            decimal dBudget = 0;
            if (budgetAxo.Any())
            {
                if (decimal.TryParse(budgetAxo[0].budget.ToString(), out dBudget))
                    ViewBag.Budget = dBudget;
            }


            ViewBag.userNo = sparame;
            ViewBag.Axo = sparamx;
            var meritList = db.sp_view_merit_year(sparamx).ToList();
            ViewBag.Merit = meritList;
            
            ViewBag.craeteMerit = true;
            if (meritList.ToList().Count > 0)
                ViewBag.craeteMerit = false;

            empleadoTress add = new empleadoTress();

            add = add.datosTress(sparame.Substring(3, sparame.Length - 3), sparame.Substring(0, 3));
            ViewBag.infoTress = add;

            return View();
        }
        private bool Elegible(DateTime adIngreso,DateTime adSalario)
        {
            bool bReturn = false;
            //fecha de ingreso
            int iYear1 = adIngreso.Year;
            int iYearA = DateTime.Today.Year;

            if (iYearA > iYear1)
                bReturn = true;

            //fecha del ultimo ajuste salarial
            int iYear2 = adSalario.Year;
            if (iYearA > iYear1)
                bReturn = true;

            return bReturn;
        }
        public ActionResult CreateAll(decimal sparamx)
        {
            //list of employees / local & tress store procedure query
            
            var budgetAxo = db.t_budget_pta.Where(x => x.axo == sparamx).ToList();
            decimal dBudget = 0;
            if (budgetAxo.Any())
            {
                if (!decimal.TryParse(budgetAxo[0].budget.ToString(), out dBudget))
                    return HttpNotFound();
            }
            
            var spPlanners = db.sp_view_merit_planners().ToList();
            if (spPlanners.ToList().Count() > 0)
            {
                for (int x = 0; x < spPlanners.ToList().Count(); x++)
                {
                    var sSuper = spPlanners[x].empleado.ToString();
                    var sName = spPlanners[x].nombre.ToString();
                    var sDepto = spPlanners[x].TB_ELEMENT.ToString();
                    var sManager = string.Empty;
                    if (spPlanners[x].gerente != null)
                        sManager = spPlanners[x].gerente.ToString();
                    else
                        sManager = null;
                    decimal dSalary = 0;
                    decimal dSalaryBudget = 0;
                    if (!decimal.TryParse(spPlanners[x].salary.ToString(), out dSalary)) dSalary = 0;

                    if (dSalary > 0)
                        dSalaryBudget = dSalary * (dBudget / 100);
                    else
                        continue;
                    
                    var spDirect = db.sp_view_merit_dreport(sSuper).ToList();
                    if (spPlanners.ToList().Count() > 0)
                    {
                        //saver t_merit
                        t_merit dbMerit = new t_merit();
                        dbMerit.supervisor = sSuper;
                        dbMerit.axo = sparamx;
                        dbMerit.nombre = sName;
                        dbMerit.depto = sDepto;
                        dbMerit.budget_imp = dSalaryBudget;
                        dbMerit.budget_porc = dBudget;
                        dbMerit.budget_spen = 0;
                        dbMerit.autoriza = sManager;
                        dbMerit.estatus = "PE";
                        dbMerit.u_id = Convert.ToString(Session["userAccount"]);
                        dbMerit.f_id = System.DateTime.Now;
                        
                        db.Entry(dbMerit).State = EntityState.Added;
                        if(db.SaveChanges() > 0)
                        {    
                            //save t_meridet
                            for (int i = 0; i < spDirect.ToList().Count(); i++)
                            {
                                t_meridet dbMeridet = new t_meridet();
                                dbMeridet.supervisor = sSuper;
                                dbMeridet.axo = sparamx;
                                dbMeridet.empleado = spDirect[i].empleado.ToString();
                                dbMeridet.nombre = spDirect[i].nombre.ToString();
                                dbMeridet.jobcode = spDirect[i].jobcode.ToString();
                                decimal dMRP = 0;
                                if (!decimal.TryParse(spDirect[i].mrp.ToString(), out dMRP)) dMRP = 0;
                                dbMeridet.market_mcr = dMRP;
                                dbMeridet.puesto = spDirect[i].PU_DESCRIP.ToString();
                                dbMeridet.calificacion = spDirect[i].califica.ToString();
                                decimal dSalaryD = 0;
                                if (!decimal.TryParse(spDirect[i].salary.ToString(), out dSalaryD)) dSalaryD = 0;
                                dbMeridet.salario_axo = dSalaryD;
                                dbMeridet.budget_porc = dBudget;
                                dbMeridet.estatus = "PE";
                                dbMeridet.sugerido_imp = 0;
                                dbMeridet.sugerido_porc = 0;
                                dbMeridet.lump_imp = 0;
                                dbMeridet.lump_porc = 0;
                                dbMeridet.market_mcr = 0;//get mrc from job
                                dbMeridet.out_guide = "0";
                                dbMeridet.salario_nuevo = 0;
                                DateTime dtSalario = DateTime.Parse(spDirect[i].CB_FEC_ANT.ToString());
                                DateTime dtIngreso = DateTime.Parse(spDirect[i].CB_FEC_ING.ToString());
                                if (Elegible(dtSalario, dtSalario))
                                    dbMeridet.ind_elegible = "1";
                                else
                                    dbMeridet.ind_elegible = "0";
                                dbMeridet.u_id = Convert.ToString(Session["userAccount"]);
                                dbMeridet.f_id = DateTime.Now;

                                db.Entry(dbMeridet).State = EntityState.Added;
                                db.SaveChanges();
                            }
                        }
                    }
                }    
            }
            
            return RedirectToAction("Start","t_merit",new { sparame = Session["userNo"], sparamx= sparamx});

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
                    Session["Plant"] = t_usuarios.planta;
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
