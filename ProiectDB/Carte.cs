using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectDB
{
    public class Carte
    {
        public string Titlu { get; set; }
        public string Autor { get; set; }
        public string Status { get; set; }
        public Carte(string titlu, string autor, string status)
        {
            Titlu = titlu;
            Autor = autor;
            Status = status;
        }
    }
}
