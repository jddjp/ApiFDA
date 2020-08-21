using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ApiBack.Models
{
    public class DocsExtra
    {
        [Key]
        public string PrimerNombr { get; set; }
        public string SegundoNombr { get; set; }
        public string ApellidoPatern { get; set; }
        public string ApellidoMatern { get; set; }
        public string CUR { get; set; }
        public string TelefonoCas { get; set; }
        public string TelefonoCelula { get; set; }
        public string Emai { get; set; }
        public string NumeroPagare { get; set; }
        public string IdCredi { get; set; }
        public string monto { get; set; }
        public string tipo { get; set; }
        public string metodoaviso { get; set; }
        public string pdfcontent9 { get; set; }
        public string pdfcontent10 { get; set; }
        public string pdfcontent11 { get; set; }
        public string pdfcontent12 { get; set; }

    }
}
