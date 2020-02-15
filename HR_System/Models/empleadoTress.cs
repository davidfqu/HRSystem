using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HR_System.Models
{
    public class empleadoTress
    {
        public string empleado { get; set; }
        public string depto { get; set; }
        public string puesto{ get; set; }
        public Nullable<System.DateTime> fechaIngreso { get; set; }
        public Nullable<decimal> salario { get; set; }
        public Byte[] btImagen { get; set; }
        

        public empleadoTress datosTress(string empleado, string planta)
        {
            empleadoTress empleadodatos = new empleadoTress();
            if (planta == "686")
            {
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
                                        WHERE co.CB_CODIGO = " + empleado + @" and co.CB_ACTIVO = 'S' 
                                        ORDER BY PU_DESCRIP,CB_TURNO";

                var datos = db.Query(selectedQueryString).ToList();
                if (datos.Any())
                {
                    empleadodatos.puesto = datos.ElementAt(0).PUESTO;
                    empleadodatos.depto = datos.ElementAt(0).CB_NIVEL2;
                    empleadodatos.btImagen = (Byte[])datos.ElementAt(0)[6];

                    return empleadodatos;
                }
            }
            
            return empleadodatos;
        }
    }


}