using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPersonnel.Models.Dettes
{
    public class DetteRestant
    {
        public int IdDettesRestants { get; set; }
        public int EmployeId { get; set; }

        public decimal DettesRestants { get; set; }
    }
}
