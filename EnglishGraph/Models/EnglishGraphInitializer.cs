using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishGraph.Models
{
    public class EnglishGraphInitializer: CreateDatabaseIfNotExists<EnglishGraphContext>
    {
    }
}
