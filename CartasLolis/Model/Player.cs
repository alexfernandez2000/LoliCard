using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Player
    {
        private string name;
        private int mana;
        private int heal;
        private List<Carta> deck;

        public string Name { get => name; set => name = value; }
        public int Mana { get => mana; set => mana = value; }
        public int Heal { get => heal; set => heal = value; }
        public List<Carta> Mazo { get => deck; set => deck = value; }

        public Player()
        {

        }
        public Player(string name, int mana, int heal, List<Carta> deck)
        {
            this.name = name;
            this.mana = mana;
            this.heal = heal;
            this.deck = deck;
        }
    }
}
