using System;
using System.Collections.Generic;

namespace ApiBack.Models
{
    public partial class State
    {
        public int Result { get; set; }
        public string ResultDate { get; set; }
        public string Date { get; set; }
        public string SubstateComment { get; set; }
        public int ExternalId { get; set; }
        public string Guid { get; set; }
        public string CancelCode { get; set; }
        public string SubstateDate { get; set; }
        public int SubstateValue { get; set; }
        public int Value { get; set; }
        public string Comment { get; set; }
        public string CancelReason { get; set; }
    }
}
