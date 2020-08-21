using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiBack.Models
{
    public partial class ResultadoFeedback
    {
        [Key]
        public int CodigoRespuesta { get; set; }
        public string DescRespuesta { get; set; }

     
    }
}
