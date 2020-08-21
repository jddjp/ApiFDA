using System;
using System.Collections.Generic;

namespace ApiBack.Models
{
    public partial class ProcesoEnvioFirma
    {
       
        public int IdProcesoEnvioFirma { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Curp { get; set; }
        public string  NumControl { get; set; }
        public string TelefonoCasa { get; set; }
        public string TelefonoCelular { get; set; }
        public DateTime FechaEnvio { get; set; }
        public TimeSpan HoraEnvio { get; set; }
        public int Estatusfirma { get; set; }
        public string Email { get; set; }
        public string guid { get; set; }
        public string monto { get; set; }
        public string tipoc { get; set; }
        public int EstatusTransaccion { get; set; }
        



    }
}
