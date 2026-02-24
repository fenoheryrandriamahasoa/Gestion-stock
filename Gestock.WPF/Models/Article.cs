using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarcheApp.Models
{
    public class Article
    {
        public string Code { get; set; }
        public string Nom { get; set; }
        public double Prix { get; set; }
        public int Stock { get; set; }
        public string DateExpiration { get; set; }
        public string Categorie { get; set; }
    }
}
