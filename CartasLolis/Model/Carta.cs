using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
   public class Carta : ICloneable
    {
        private string nombre;
        private int hp;
        private int ap;
        private int expenditure;
        private int actions;
        private string url;

        public string Nombre { get => nombre; set => nombre = value; }
        public int Hp { get => hp; set => hp = value; }
        public int Ap { get => ap; set => ap = value; }
        public int Expenditure { get => expenditure; set => expenditure = value; }
        public string Url { get => url; set => url = value; }
        public int Actions { get => actions; set => actions = value; }

        public Carta(string nombre, int hp, int ap, int expenditure, string url, int actions)
        {
            Nombre = nombre;
            Hp = hp;
            Ap = ap;
            Expenditure = expenditure;
            Url = url;
            Actions = actions;
        }
        public Carta Clone()
        {
            return (Carta)this.MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }

        public Carta()
        {
        }
    }
}
