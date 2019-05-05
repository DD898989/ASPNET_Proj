using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace MvcApplication00.Models
{
    public class Class1_Film
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
    }

    public class FilmDBContext : DbContext
    {
        public DbSet<Class1_Film> Films { get; set; }
    }
}