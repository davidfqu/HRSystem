using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HR_System.Models
{
    public class MyDirectsObjectives
    {
        public string empleado { get; set; }
        public string nombre { get; set; }

        public decimal axo { get; set; }
        public string estatus { get; set; }
        public string puesto { get; set; }
        public Byte[] foto { get; set; }
    }
}