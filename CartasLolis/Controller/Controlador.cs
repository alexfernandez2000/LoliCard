using CartasLolis;
using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Controller
{
    public class Controlador
    {
        Form1 form;
        List<PictureBox> lpb;
        List<Label> llbDeck=new List<Label>();
        List<Carta>allCards=new List<Carta>();
        Carta[] cardsOnTable = new Carta[16];
        int cardSelected=-1;
        int enemyCardSelected=-2;
        Player player;
        Player enemy;
        public Controlador()
        {
            form = new Form1();
            insertStartedCards();
           allCards=deserealize().OrderBy(x => x.Nombre).ToList();
            //Creación del jugador
            List<Label>aux = form.Controls.OfType<Label>().ToList();
            foreach (Label item in aux)
            {
                try
                {
                   int s=int.Parse(item.Name.Substring(2));
                    llbDeck.Add(item);

                }
                catch (Exception e)
                {
                   
                }
            }
            llbDeck=llbDeck.OrderBy(x => x.Name).ToList();
            lpb =  form.Controls.OfType<PictureBox>().ToList();
            lpb = Merge(lpb);
            foreach (PictureBox item in lpb)
            {
                item.Click += PulsarCarta;
            }
            form.bt_Attack.Click += atackAction;
            form.bt_PasarTurno.Click += passTurn;
            startGame();
            Application.Run(form);

        }
        private void printDeckLabels()
        {
            for (int i = 11; i <= 15; i++)
            {
                if (cardsOnTable[i]!=null)
                {
                    llbDeck[i - 11].Text = cardsOnTable[i].Expenditure.ToString();
                }
                else
                {
                    llbDeck[i - 11].Text = null;
                }
            }
        }
        private void passTurn(object sender, EventArgs e)
        {
            player.Mana += 2;
            UpdateUserStats();
            rechargeActions();
            //Turno enemigo
            enemyTurn();
        }
        private void rechargeActions()
        {

            for (int i = 5; i <= 9; i++)
            {
                if (cardsOnTable[i]!=null)
                {
                    cardsOnTable[i].Actions = allCards.Find(x => x.Nombre.Equals(cardsOnTable[i].Nombre)).Actions;
                }
              
            }
            
        }
        private void enemyTurn()
        {
            //Primero saca cartas
            Boolean aux = true;
            while (aux)
            {
                Carta card = enemy.getRandomCardFromDeck();

                if (card.Expenditure <= enemy.Mana)
                {
                    //Coloca la carta en mano al juego si tiene espacio
                    for (int i = 0; i < 5; i++)
                    {
                        if (cardsOnTable[i] == null)
                        {
                            cardsOnTable[i] = card;
                            enemy.Mana -= card.Expenditure;
                            printCards();
                            break;
                        }
                    }
     //               Thread.Sleep(2000);
                }
                else
                {
                    aux = false;
                }
            }
            for (int i = 0; i < 5; i++)
            {
                if (cardsOnTable[i]!=null)
                {
                    for (int j = 0; j < cardsOnTable[i].Actions; j++)
                    {
                        
                        for (int k = 5; k < 10; k++)
                        {
                            if (cardsOnTable[k]!=null)
                            {
                                cardsOnTable[k].Hp -= cardsOnTable[i].Ap;
                                aux = true;
                                if (cardsOnTable[k].Hp <= 0)
                                {
                                    lpb[k + 1].Image = null;
                                    cardsOnTable[k] = null;                                    
                                    printCards();
                                }
                                break;
                            }                            
                        }
                        if (aux == false)
                        {
                            player.Heal -= cardsOnTable[i].Ap;
                            UpdateUserStats();
                            if (player.Heal <= 0)
                            {
                                //Derrota Jugador pierda
                                enemyCardSelected = -2;
                                MostrarInfoCartas();
                                //Enemigo derrotado
                                DialogResult dialogResult = MessageBox.Show("Continue?", "You Lose", MessageBoxButtons.YesNo);
                                if (dialogResult == DialogResult.Yes)
                                {
                                    //Reazer partida
                                    startGame();
                                }
                                else if (dialogResult == DialogResult.No)
                                {
                                    //Cerrar todo
                                    form.Close();
                                }
                                break;
                            }
                        }
                        aux = false;
                        if (player.Heal <= 0)
                        {
                            break;
                        }
          //              Thread.Sleep(2000);
                    }
                    if (player.Heal <= 0)
                    {
                        break;
                    }
                }
                if (player.Heal <= 0)
                {
                    break;
                }
            }
            enemy.Mana += 2;
        }
        //Ataca a las cartas o al enemigo
        private void atackAction(object sender, EventArgs e)
        {
            if (enemyCardSelected!=-1 && cardSelected!=-1 && cardsOnTable[cardSelected].Actions>0)
            {
                cardsOnTable[enemyCardSelected].Hp= cardsOnTable[enemyCardSelected].Hp- cardsOnTable[cardSelected].Ap;
                cardsOnTable[cardSelected].Actions--;
                if (cardsOnTable[enemyCardSelected].Hp<=0)
                {
                    //Carta eliminada
                    cardsOnTable[enemyCardSelected] = null;
                    lpb[enemyCardSelected + 1].Image=null;

                    enemyCardSelected = -2;
                    MostrarInfoCartas();
                    printCards();
                }
            }
            else if (enemyCardSelected == -1 && cardSelected != -1 && cardsOnTable[cardSelected].Actions > 0)
            {
               enemy.Heal=enemy.Heal - cardsOnTable[cardSelected].Ap;
                cardsOnTable[cardSelected].Actions--;

                if (enemy.Heal<=0)
                {
                    enemyCardSelected = -2;
                    MostrarInfoCartas();
                    //Enemigo derrotado
                    DialogResult dialogResult = MessageBox.Show("Continue?", "You Win", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        //Reazer partida
                        startGame();
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        //Cerrar todo
                        form.Close();
                    }

                }
                else
                {
                    MostrarInfoCartas();
                }
            }
        }
        //Comprueba todas las cartas

        //Orden de los pictureBox del form
        private List<PictureBox> Merge(List<PictureBox> array)
        {
            if (array.Count <= 1)
            {
                return array;
            }
            int arrayLengh = array.Count;
            List<PictureBox> array1 = Merge(array.GetRange(arrayLengh / 2, arrayLengh / 2 + arrayLengh % 2));
            List<PictureBox> array2 = Merge(array.GetRange(0, arrayLengh / 2));
            bool ordenado = false;
            int i = 0, j = 0, array2Lengh = array2.Count, array1Lengh = array1.Count;
            List<PictureBox> arrayOrdenado = new List<PictureBox>();
            while (ordenado == false)
            {

                if (i == array1Lengh)
                {
                    arrayOrdenado.Add(array2[j]);
                    j++;
                }
                else if (j == array2Lengh)
                {
                    arrayOrdenado.Add(array1[i]);
                    i++;
                }
                else if (int.Parse(array1[i].Name.Substring(2)) > int.Parse(array2[j].Name.Substring(2)))
                {
                    arrayOrdenado.Add(array2[j]);
                    j++;
                }
                else
                {
                    arrayOrdenado.Add(array1[i]);
                    i++;
                }
                if (arrayOrdenado.Count == arrayLengh)
                {
                    ordenado = true;
                }
            }
            return arrayOrdenado;

        }

        //Inicio de juego
        private void startGame()
        {
            player = new Player("Alex", 10,30, deckCreation());
            enemy = new Player("Enemy", 5, 30, deckCreation());
            for (int i = 0,x=cardsOnTable.Count(); i < x; i++)
            {
                cardsOnTable[i] = null;
            }
            insertImage("https://i.ytimg.com/vi/b5yhqSgSxFQ/hqdefault.jpg", 0);
            cardsOnTable[10] = player.getRandomCardFromDeck();
            form.pbMP.ForeColor=Color.Blue;
            form.pbMP.Style= ProgressBarStyle.Continuous;
            form.pbHP.ForeColor = Color.Red;
            form.pbHP.Style = ProgressBarStyle.Continuous;
            UpdateUserStats();
            MostrarInfoCartas();
            printDeckLabels();
            printCards();
            
        }
        //Muestra cartas en tablero
        private void printCards()
        {
            int i= 0;
            foreach (Carta cards in cardsOnTable)
            {
                if (cards!=null)
                {
                    insertImage(cards.Url,i+1);
                }
                else
                {
                    insertImage(null, i + 1);
                }
                i++;
            }
            
        }
        private void UpdateUserStats()
        {
           
            if (player.Heal<0)
            {
                form.lbHP.Text = "0";
                form.pbHP.Value = 0;
            }
           else
            {
                form.lbHP.Text = player.Heal.ToString();
                form.pbHP.Value = player.Heal;
            }
            form.lbMP.Text = player.Mana.ToString();
            form.pbMP.Value = player.Mana;
        }
        //Crea un mazo para la clase Player
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
        private void MostrarInfoCartas()
        {
            if (enemyCardSelected!=-2)
            {
                if (enemyCardSelected==-1)
                {
                    insertImage("https://i.ytimg.com/vi/b5yhqSgSxFQ/hqdefault.jpg", 17);
                    form.lbEnemyHp.Text ="HP: "+enemy.Heal;
                    form.lbEnemyAP.Text = null;
                    form.lbEnemyActions.Text = "Mana: "+enemy.Mana;
                }
                else
                {                  
                    insertImage(cardsOnTable[enemyCardSelected].Url, 17);
                    form.lbEnemyHp.Text = "HP: " + cardsOnTable[enemyCardSelected].Hp;
                    form.lbEnemyAP.Text = "AP: "+cardsOnTable[enemyCardSelected].Ap;
                    form.lbEnemyActions.Text = "Actions: " + +cardsOnTable[enemyCardSelected].Actions;
                }
            }
            else
            { 
                insertImage(null, 17);
                form.lbEnemyHp.Text = null;
                form.lbEnemyAP.Text = null;
                form.lbEnemyActions.Text = null;
            }
            
            if (cardSelected!=-1 && cardsOnTable[cardSelected]!=null)
            {
                insertImage(cardsOnTable[cardSelected].Url, 18);
                form.lbPlayerHp.Text = "HP: " + cardsOnTable[cardSelected].Hp;
                form.lbPlayerAP.Text = "AP: " + cardsOnTable[cardSelected].Ap;
                form.lbActionsPlayer.Text = "Actions: " + +cardsOnTable[cardSelected].Actions;
            }
            else
            {
                insertImage(null, 18);
                form.lbPlayerHp.Text = null;
                form.lbPlayerAP.Text = null;
                form.lbActionsPlayer.Text = null;
            }
        }
        private void PulsarCarta(object sender, EventArgs e)
        {
            //CardsOnTable 0-4 enemigo 5-9 jugador 10 a cojer 11-15 mano
            PictureBox prueba=(PictureBox) sender;
            int numCarta =int.Parse(prueba.Name.Substring(2));
            if (numCarta<6 && numCarta>0)
            {
                //Cartas enemigo
                if (cardsOnTable[numCarta-1]!=null)
                {
                    enemyCardSelected = numCarta-1;
                }
                else
                {
                    enemyCardSelected = -2;
                }
                MostrarInfoCartas();
            }
            else if(numCarta==0)
            {
                //Jugador enemigo
                
                enemyCardSelected = numCarta - 1;
                MostrarInfoCartas();

            }
            else if(numCarta!=11 && numCarta<11)
            {
                //Cartas aliadas en juego
                if (cardsOnTable[numCarta - 1] != null)
                {
                    cardSelected = numCarta - 1;
                    MostrarInfoCartas();
                }
                else
                {
                    cardSelected =- 1;

                }
            }
            else if(numCarta!=11&& numCarta<17)
            {
                //Cartas en mano
                if (cardsOnTable[numCarta-1 ]!=null)
                {
                    //Sacar carta al juego
                    for (int i = 5; i <= 9; i++)
                    {
                        if (cardsOnTable[i]==null && player.Mana>= cardsOnTable[numCarta-1].Expenditure)
                        {
                            player.Mana -= cardsOnTable[numCarta-1].Expenditure;
                            UpdateUserStats();
                            cardsOnTable[i] = cardsOnTable[numCarta - 1];
                            cardsOnTable[numCarta - 1] = null;
                            lpb[numCarta].Image=null;
                            printDeckLabels();
                            break;
                        }
                    }
                }

            }
            else if (numCarta==11)
            {
                //Siguiente carta a sacar
                for (int i = 11; i <=15 ; i++)
                {
                    if (cardsOnTable[i]==null)
                    {
                        cardsOnTable[i] = cardsOnTable[10];
                        cardsOnTable[10] = player.getRandomCardFromDeck();
                        printDeckLabels();
                        break;
                    }
                }
            }
            else
            {
                //PB de información
            }


            printCards();
        }

        //Creo una lista base de cartas para jugar
        public void insertStartedCards()
        {
            List<Carta> cards = new List<Carta>();
            Carta card;
            card = new Carta("Shiro", 10, 10,5, "https://www.anmosugoi.com/wp-content/uploads/2019/10/nogamenolife.png",1);
            cards.Add(card);
            
            card = new Carta("Kanna", 7, 7,3, "https://cutewallpaper.org/21/kanna-wallpapers/Desktop-Wallpaper-Cute-Kanna-Kamui-Kobayashi-San-Chi-No-.jpg", 1);
            cards.Add(card);

            card = new Carta("Schwi", 9, 9,4, "https://i.pinimg.com/originals/38/d3/6b/38d36b8052568be17d5dee0ee798fcdb.jpg", 1);
            cards.Add(card);

            card = new Carta("Nezuko", 6, 7,2, "https://static2.cbrimages.com/wordpress/wp-content/uploads/2020/02/nezuko-feature.jpg", 1);
            cards.Add(card);

            card = new Carta("Umaru", 7, 4,2, "https://i.pinimg.com/originals/e2/0a/0d/e20a0da2d36bdfa025b164f3745386c0.jpg", 1);
            cards.Add(card);

            card = new Carta("Eucliwood", 5, 8,2, "https://i.imgur.com/uHH9XvJ.jpg", 1);
            cards.Add(card);

            card = new Carta("Izuna", 8, 7,3, "https://i.pinimg.com/originals/91/c7/e3/91c7e3ade6c811211e1c132d332af969.jpg",1);
            cards.Add(card);

            card = new Carta("Koneko", 5, 6,1, "https://preview.redd.it/gbi4nrcdjwq41.jpg?auto=webp&s=189927c706df5b4da3859d4e25de57d38d9c5f21", 1);
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
        public void insertImage(string url,int pbPosition)
        {
            PictureBox image= lpb[pbPosition];
            if (url!=null)
            {
                image.Load(url);
                image.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                image.Image = null;
            }
            
            //pb= image;
            lpb[pbPosition] = image;
        }
    }
}
