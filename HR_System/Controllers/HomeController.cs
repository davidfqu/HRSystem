﻿using System;
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
                ViewBag.userRol = t_usuarios.rol.ToString();
                
                var empleado = db.t_empleados.Where(x => x.usuario == username).ToList();
                if (empleado.Any())
                {
                    var config1 = db.t_config_m1.Where(x => x.clave == "01").ToList();
                    ViewBag.Axo = config1[0].axo_activo;

                    Session["userNo"] = empleado[0].empleado;
                    ViewBag.userNo = empleado[0].empleado;
                    string numempleado = empleado[0].empleado;
                    if (empleado[0].planta == "686")
                    {

                        empleadoTress add = new empleadoTress();

                        add = add.datosTress(numempleado.Substring(3, numempleado.Length - 3), numempleado.Substring(0, 3));
                        ViewBag.userJobPosition = add.puesto;
                        ViewBag.userImage = "data:image/png;base64," + Convert.ToBase64String(add.btImagen, 0,add.btImagen.Length);
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