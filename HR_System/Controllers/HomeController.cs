using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using HR_System.Models;
using System.Globalization;
using WebMatrix.Data;

namespace HR_System.Controllers
{
    public class HomeController : Controller
    {
        private HRSystemEntities db = new HRSystemEntities();

        public ActionResult Index()
        {
            if (!Login())
                return RedirectToAction("NoUser", "Home", null);

            return View();
        }

        public ActionResult NoUser()
        {
            return View();
        }
        public ActionResult MyInfo()
        {
            return View();
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
                                        co.CB_NIVEL2,n3.TB_ELEMENT AS AREA,co2.CB_E_MAIL,im.IM_BLOB 
                                        from [Tress_MedlineMXL].[dbo].COLABORA co 
                                        inner join [Tress_MedlineMXL].[dbo].PUESTO po on co.CB_PUESTO = po.PU_CODIGO 
                                        inner join [Tress_MedlineMXL].[dbo].NIVEL2 n2 on co.CB_NIVEL2 = n2.TB_CODIGO 
                                        inner join [Tress_MedlineMXL].[dbo].NIVEL3 n3 on co.CB_NIVEL3 = n3.TB_CODIGO 
                                        inner join [Tress_MedlineMXL].[dbo].NIVEL4 n4 on co.CB_NIVEL4 = n4.TB_CODIGO 
                                        inner join [Tress_MedlineMXL].[dbo].TURNO tu on co.CB_TURNO = tu.TU_CODIGO 
										left join [Tress_MedlineMXL].[dbo].COLABORA co2 on co.CB_NIVEL4 = co2.CB_CODIGO
                                        left join [Tress_MedlineMXL].[dbo].IMAGEN im ON co.CB_CODIGO = im.CB_CODIGO 
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

            Byte[] btImga = (Byte[])datos.ElementAt(0)[6];
            ViewBag.userImage = "data:image/png;base64," + Convert.ToBase64String(btImga, 0, btImga.Length);


            return empleado;
        }
    }
}