using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace zad_c
{
    public partial class Form1 : Form
    {

        int[,] veze = new int[61, 61]; //pamti tezine. u [i , j] je spremljena tezina izmedu i. i j. vrha
        int[] brojac = new int[61]; //interni brojac prilikom spremanja susjeda
        int[,] susjedi = new int[61, 61]; //pamti susjede od svakog vrha. [i, j] znaci da je j susjed od i

        int[] prethodni = new int[61]; //pamti prethodni vrh od svakog vrha u najkracem putu
        int[] tren_tez = new int[61]; //ukupna tezina najkraceg puta do svakog vrha

        int[] oznacen = new int[61]; //prilikom izracuna, pamti za sve vrhove da li su vec posjeceni ili ne
        int[] tren_put = new int[61]; //interno pamti trenutačni put kroz graf prilikom izracuna

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string line;
            int start, end, x1, x2, pocetni, cilj;

            pocetni = Int32.Parse(textBox1.Text);
            cilj = Int32.Parse(textBox2.Text);

            for (int i = 0; i < 61; i++) //namjestiti inicijalne vrijednosti
            {
                brojac[i] = 0;
                oznacen[i] = 0;
                tren_tez[i] = 1000;
            }

            tren_tez[pocetni] = 0; //tezina pocetnog je nula
            oznacen[pocetni] = 1; //pocetni je posjecen odmah
            prethodni[pocetni] = 0; //pocetni nema prethodnog

                using (System.IO.StreamReader file = new System.IO.StreamReader("tezine.txt")) //citanje iz fajla
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        start = line.IndexOf("(") + 1; //parsira prvi vrh u zapisu u redu i pamti ga
                        end = line.IndexOf(",", start);
                        string prvi = line.Substring(start, end - start);
                        prvi = prvi.Replace("V", "");

                        start = line.IndexOf(",") + 1; //drugi
                        end = line.IndexOf(")", start);
                        string drugi = line.Substring(start, end - start);
                        drugi = drugi.Replace("V", "");

                        x1 = int.Parse(prvi);
                        x2 = int.Parse(drugi);

                        susjedi[x1, brojac[x1]] = x2; //susjed od prvog je drugi
                        susjedi[x2, brojac[x2]] = x1; //susjed od drugog je prvi. Pamti se oba smjera radi lakseg izracuna

                        brojac[x1]++; //brojac broja susjeda
                        brojac[x2]++;

                        veze[x1, x2] = int.Parse(line.Substring(line.LastIndexOf('=') + 1)); //sprema tezinu
                        veze[x2, x1] = veze[x1, x2];
                    }
                }

            bool provjera = true;
            int tren_vrh = pocetni; //postavljanje pocetnog kao trenutnog vrha
            int temp_tez, min=1000, k=0;

            while (provjera)
            {
                if (prov_oznaceni()) provjera = false; //funkcija provjerava da li su posjeceni svi vrhovi, ako jesu zavrsava petlju.
                else {
                    min = 0;
                    for (int i=0; i<61; i++)
                    {
                    if (veze[tren_vrh, susjedi[tren_vrh, i]] != 0) //ako je tezina razlicita od nula, postoji veza izmedu vrhova
                    {

                        temp_tez = tren_tez[tren_vrh] + veze[tren_vrh, susjedi[tren_vrh, i]]; //uzima se ukupna tezina do svakog od susjeda od trenutacnog vrha

                        if (temp_tez < tren_tez[susjedi[tren_vrh, i]]) //ako je manja od dosadanje ukupne tezine do dotičnog vrha, uzima se kao nova
                        {
                            prethodni[susjedi[tren_vrh, i]] = tren_vrh; //tren. vrh se postavlja kao novi prethodni vrh u najkracem putu
                            tren_tez[susjedi[tren_vrh, i]] = temp_tez;
                        }
                            if (tren_tez[susjedi[tren_vrh, i]] < tren_tez[min] && oznacen[susjedi[tren_vrh, i]] == 0) min = susjedi[tren_vrh, i]; //uzima se sljedeci najmanji susjed
                        }
                    }
                    oznacen[tren_vrh] = 1; //trenutačni vrh se postavlja da je posjecen
                    if (prov_susjeda(tren_vrh)) //provjerava se posjecenost susjeda tren vrha. Ako su svi posjeceni, vraca se unatrag na prethodni vrh u privremenom putu.
                    {
                        if (k > 0)
                        {
                            k = k - 1;
                            min = tren_put[k];
                        }
                    }
                    else {
                        tren_put[k] = tren_vrh; //tren vrh se postavlja u privremeni put
                        k++;
                    }
                    tren_vrh = min; //sljedeci vrh je odabrani najmanji susjed
                }
            }

            provjera = true;
            string ispis = pocetni.ToString();
            int[] put = new int[61];
            int prebroj = 0;
            tren_vrh = cilj; //najkraci put do cilja se iscitava od odozada

            while (provjera)
            {
                if (tren_vrh != pocetni)
                {
                    put[prebroj] = tren_vrh;
                    tren_vrh = prethodni[tren_vrh]; //iscitavaju se prethodni od vrha, i tako od cilja do pocetnog.
                    prebroj++;
                } else
                {
                    put[prebroj] = tren_vrh;
                    tren_vrh = prethodni[tren_vrh]; //spremi se i pocetni
                    prebroj++;
                    provjera = false;
                }
            }

            for(int i = prebroj-2; i >= 0; i--)
            {
                ispis += "->" + put[i].ToString(); //put od cilja do pocetnog se obrnuto sprema u ispis
            }

            label2.Text = "Put: " + ispis;
            label4.Text = "Tezina: " + tren_tez[cilj].ToString();

        }

        public bool prov_oznaceni()
        {
            bool temp = true;
            for (int i = 1; i < 8; i++)
                if (oznacen[i] == 0) temp = false;
            return temp;
        }

        public bool prov_susjeda(int tren)
        {
            for (int i = 0; i < brojac[tren]; i++)
                if (oznacen[susjedi[tren, i]] == 0) return false;
            return true;
        }
    }
}
