using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace MaterailReport.Models.OldDB
{
    public class MRContext : DbContext
    {
        public MRContext() : base("server=WIN-QSRMNQOT9SQ;database=material_report;user id=BAA;password=Artyom1") { }

        public DbSet<ConstituentComponentType> ConstituentComponentTypes { get; set; }
    }
}