using System;
using System.Collections.Generic;

namespace CS243N_Term_Project.Models
{
    public partial class HopType
    {
        public HopType()
        {
            Hops = new HashSet<Hop>();
        }

        public int HopTypeId { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Hop> Hops { get; set; }
    }
}
