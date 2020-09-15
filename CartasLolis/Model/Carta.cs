using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
   public class Carta
    {
        private string nombre;
        private int hp;
        private int ap;
        private int expenditure;
        private string url;

        public string Nombre { get => nombre; set => nombre = value; }
        public int Hp { get => hp; set => hp = value; }
        public int Ap { get => ap; set => ap = value; }
        public int Expenditure { get => expenditure; set => expenditure = value; }
        public string Url { get => url; set => url = value; }

        public Carta(string nombre, int hp, int ap, int expenditure, string url)
        {
            Nombre = nombre;
            Hp = hp;
            Ap = ap;
            Expenditure = expenditure;
            Url = url;
        }

        public Carta()
        {
        }
    }
}
