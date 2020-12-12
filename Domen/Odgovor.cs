using System;
using System.Collections.Generic;
using System.Text;

namespace Domen
{
    [Serializable]
    public class Odgovor
    {
        public string Poruka { get; set; }
        public FormaPrikaz Forma { get; set; }
        
    }
    public enum FormaPrikaz
    {
        Ja,Protivnik,Rezultat
    }
}
