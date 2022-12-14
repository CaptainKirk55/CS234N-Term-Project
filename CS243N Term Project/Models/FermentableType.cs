using System;
using System.Collections.Generic;

namespace CS243N_Term_Project.Models
{
    public partial class FermentableType
    {
        public FermentableType()
        {
            Fermentables = new HashSet<Fermentable>();
        }

        public int FermentableTypeId { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Fermentable> Fermentables { get; set; }
    }
}
