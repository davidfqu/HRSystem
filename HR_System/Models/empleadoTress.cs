using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HR_System.Models
{
    public class empleadoTress
    {
        public string empleado { get; set; }
        public string nombre { get; set; }
        public string depto { get; set; }
        public string puesto{ get; set; }
        public Nullable<System.DateTime> fechaIngreso { get; set; }
        public Nullable<decimal> salario { get; set; }
        public Byte[] btImagen { get; set; }
        public Nullable<System.DateTime> fechaSalario { get; set; }

        public empleadoTress MeritYear(int Year)
        {
            empleadoTress empleadodatos = new empleadoTress();
            var db = WebMatrix.Data.Database.Open("HRSystemEntities");
            var selectedQueryString = "sp_view_merit_year(@Year)";
            
            var datos = db.Query(selectedQueryString,Year).ToList();
            if (datos.Any())
            {
                empleadodatos.nombre = datos.ElementAt(0).NOMBRE;
                empleadodatos.puesto = datos.ElementAt(0).PUESTO;
                empleadodatos.depto = datos.ElementAt(0).CB_NIVEL2;
                empleadodatos.btImagen = (Byte[])datos.ElementAt(0)[6];
                empleadodatos.salario = datos.ElementAt(0)[7];
                empleadodatos.fechaIngreso = datos.ElementAt(0)[8];
                empleadodatos.fechaSalario = datos.ElementAt(0)[9];
                return empleadodatos;
            }
            

            return empleadodatos;
        }
        public empleadoTress datosTress(string empleado, string planta)
        {
            empleadoTress empleadodatos = new empleadoTress();
            if (planta == "686")
            {
                var db = WebMatrix.Data.Database.Open("686TressConn");
                var selectedQueryString = @"SELECT co.cb_nombres + ' ' +co.CB_APE_PAT + ' ' +co.CB_APE_MAT as NOMBRE
                                        ,co.CB_TURNO,po.PU_DESCRIP AS PUESTO,
                                         n2.TB_ELEMENT as DEPTO, n3.TB_ELEMENT AS AREA,co2.CB_E_MAIL,im.IM_BLOB, co.CB_SALARIO salario, co.CB_FEC_ANT f_anti, co.CB_FEC_SAL f_sal
                                        from [Tress_MedlineMXL].[dbo].COLABORA co 
                                        inner join [Tress_MedlineMXL].[dbo].PUESTO po on co.CB_PUESTO = po.PU_CODIGO 
                                        inner join [Tress_MedlineMXL].[dbo].NIVEL2 n2 on co.CB_NIVEL2 = n2.TB_CODIGO 
                                        inner join [Tress_MedlineMXL].[dbo].NIVEL3 n3 on co.CB_NIVEL3 = n3.TB_CODIGO 
                                        inner join [Tress_MedlineMXL].[dbo].NIVEL4 n4 on co.CB_NIVEL4 = n4.TB_CODIGO 
                                        inner join [Tress_MedlineMXL].[dbo].TURNO tu on co.CB_TURNO = tu.TU_CODIGO 
										left join [Tress_MedlineMXL].[dbo].COLABORA co2 on co.CB_NIVEL4 = co2.CB_CODIGO
                                        left join [Tress_MedlineMXL].[dbo].IMAGEN im ON co.CB_CODIGO = im.CB_CODIGO  
                                        WHERE co.CB_CODIGO = " + empleado + @" and co.CB_ACTIVO = 'S' 
                                        ORDER BY PU_DESCRIP,CB_TURNO";

                var datos = db.Query(selectedQueryString).ToList();
                if (datos.Any())
                {
                    empleadodatos.nombre = datos.ElementAt(0).NOMBRE;
                    empleadodatos.puesto = datos.ElementAt(0).PUESTO;
                    empleadodatos.depto = datos.ElementAt(0).DEPTO;
                    empleadodatos.btImagen = (Byte[])datos.ElementAt(0)[6];
                    empleadodatos.salario = datos.ElementAt(0)[7];
                    empleadodatos.fechaIngreso = datos.ElementAt(0)[8];
                    empleadodatos.fechaSalario = datos.ElementAt(0)[9];
                    return empleadodatos;
                }
            }
            
            return empleadodatos;
        }
    }


}