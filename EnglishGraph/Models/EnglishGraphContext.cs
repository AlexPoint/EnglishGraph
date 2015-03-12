using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class EnglishGraphContext: DbContext
    {

        public EnglishGraphContext() : base("EnglishGraphContext") { }


        public DbSet<Word> Words { get; set; }
    }
}
