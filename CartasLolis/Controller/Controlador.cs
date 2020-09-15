using CartasLolis;
using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Controller
{
    public class Controlador
    {
        Form1 form;
        List<PictureBox> lpb;
        List<Carta>allCards=new List<Carta>();
        Carta auxCard;
        Player player;
        Player enemy;
        public Controlador()
        {
            form = new Form1();
            insertStartedCards();
           allCards=deserealize();
            //Creación del jugador
            player = new Player("Alex", 0, 30, deckCreation());

            lpb =  form.Controls.OfType<PictureBox>().ToList();
            foreach (PictureBox item in lpb)
            {
                item.Click += PulsarCarta;
            }
            Application.Run(form);

        }
        private List<Carta> deckCreation()
        {
            Random r = new Random();
            List<Carta> cl = new List<Carta>();
            Carta caux;
            for (int i = 0; i < 5; i++)
            {
                
                caux = allCards[r.Next(0,allCards.Count-1)];
                if (cl.Find(x => x.Nombre == caux.Nombre)==null)
                {
                    cl.Add(caux);
                }
                else
                {
                    i--;
                }
            }
            return cl;
        }
        private void PulsarCarta(object sender, EventArgs e)
        {
            PictureBox prueba=(PictureBox) sender;
            int numCarta =int.Parse(prueba.Name.Substring(2));
            if (numCarta<6)
            {
            //Cartas enemigo
            }
            else if(numCarta!=11 && numCarta<11)
            { 
            //Cartas aliadas en juego
            }
            else if(numCarta!=11)
            { 
            //Cartas en mano
            }
            else
            {
            //Siguiente carta a sacar
            }
           
            
            prueba.SizeMode = PictureBoxSizeMode.Zoom;
        }
        //Creo una lista base de cartas para jugar
        public void insertStartedCards()
        {
            List<Carta> cards = new List<Carta>();
            Carta card;
            card = new Carta("Shiro", 10, 10,5, "https://www.anmosugoi.com/wp-content/uploads/2019/10/nogamenolife.png");
            cards.Add(card);
            
            card = new Carta("Kanna", 7, 7,3, "https://cutewallpaper.org/21/kanna-wallpapers/Desktop-Wallpaper-Cute-Kanna-Kamui-Kobayashi-San-Chi-No-.jpg");
            cards.Add(card);

            card = new Carta("Schwi", 9, 9,4, "https://i.pinimg.com/originals/38/d3/6b/38d36b8052568be17d5dee0ee798fcdb.jpg");
            cards.Add(card);

            card = new Carta("Nezuko", 6, 7,2, "https://static2.cbrimages.com/wordpress/wp-content/uploads/2020/02/nezuko-feature.jpg");
            cards.Add(card);

            card = new Carta("Umaru", 7, 4,2, "https://i.pinimg.com/originals/e2/0a/0d/e20a0da2d36bdfa025b164f3745386c0.jpg");
            cards.Add(card);

            card = new Carta("Eucliwood", 5, 8,2, "https://i.imgur.com/uHH9XvJ.jpg");
            cards.Add(card);

            card = new Carta("Izuna", 8, 7,3, "https://i.pinimg.com/originals/91/c7/e3/91c7e3ade6c811211e1c132d332af969.jpg");
            cards.Add(card);

            card = new Carta("Koneko", 5, 6,1, "https://preview.redd.it/gbi4nrcdjwq41.jpg?auto=webp&s=189927c706df5b4da3859d4e25de57d38d9c5f21");
            cards.Add(card);
            serialize(cards);
        }
        //Inserta una lista de cartas a un fichero xml.
        public void serialize(List<Carta> cards)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(List<Carta>)); 
            StreamWriter myWriter = new StreamWriter("Cartas.xml");
            mySerializer.Serialize(myWriter, cards);
            myWriter.Close();
        }
        //Obtiene una lista de cartas de un fichero xml.
        public List<Carta> deserealize()
        {
            var mySerializer = new XmlSerializer(typeof(List<Carta>));
            // To read the file, create a FileStream.
            var myFileStream = new FileStream("Cartas.xml", FileMode.Open);
            // Call the Deserialize method and cast to the object type.
            List<Carta> myObject = (List<Carta>)mySerializer.Deserialize(myFileStream);
            return myObject;
        }
        public void insertarImagen()
        {
            PictureBox ima= form.pb7;
            ima.Load("https://www.anmosugoi.com/wp-content/uploads/2019/10/nogamenolife.png");
            ima.SizeMode = PictureBoxSizeMode.Zoom;
            form.pb7 = ima;
            
        }
    }
}
