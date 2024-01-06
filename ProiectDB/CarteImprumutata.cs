using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectDB
{
    public class CarteImprumutata
    {
        public string Titlu { get; set; }
        public string Autor { get; set; }
        public CarteImprumutata(string titlu, string autor)
        {
            Titlu = titlu;
            Autor = autor;
        }
    }
}
