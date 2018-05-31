using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Rating.Models
{
    public class IndividualContext : DbContext,
        IDisposable
    {
        public DbSet<Professor> Professors { get; set; }

        public new void Dispose()
        {
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        ~IndividualContext()
        {
            base.Dispose();
        }
    }

    //public class Initializer : DropCreateDatabaseAlways<IndividualContext>
    //{
    //    protected override void Seed(IndividualContext context)
    //    {
    //        context.Professors.Add(new Professor());
    //    }
    //}
}