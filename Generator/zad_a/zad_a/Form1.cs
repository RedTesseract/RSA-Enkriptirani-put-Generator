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
using IntXLib;

namespace zad_a
{
    public partial class Form1 : Form
    {
        int[] put = new int[16]; //kriptirani put
        string[] decPut = new string[16]; //dekriptirani put
        int[] n = new int[60]; //komponenta N ključa
        uint[] pub = new uint[60]; //javna komponenta ključa
        uint[] priv = new uint[60]; //privatna komp ključa
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string line;
            int start, end, brojac=0;

                using (System.IO.StreamReader file = new System.IO.StreamReader("put.txt"))
                {
                    while ((line = file.ReadLine()) != null) //čita kriptirani put i sprema brojke u niz "put"
                    {
                        start = line.IndexOf(".") + 1;
                        end = line.Length;
                        string brojs = line.Substring(start, end - start);
                        brojs = brojs.Replace(" ", "");
                        put[brojac] = Int32.Parse(brojs);
                        brojac++;
                    }
                }

            brojac = 0;
            
            using (System.IO.StreamReader file = new System.IO.StreamReader("vrhovi.txt")) //čita i sprema ključeve
            {
                while ((line = file.ReadLine()) != null)
                {

                    start = line.IndexOf("(") + 1; //čita N iz .txt
                    end = line.IndexOf(",");
                    string brojs = line.Substring(start, end - start);
                    n[brojac] = Int32.Parse(brojs);

                    start = line.IndexOf(",") + 1; //čita javni
                    end = line.IndexOf(")");
                    brojs = line.Substring(start, end - start);
                    pub[brojac] = uint.Parse(brojs);

                    start = line.LastIndexOf(",") + 1; //čita privatni
                    end = line.LastIndexOf(")");
                    brojs = line.Substring(start, end - start);
                    priv[brojac] = uint.Parse(brojs);

                    brojac++;
                }
            }

            int tren = 1;
            decPut[0] = "V01"; //početni vrh
            listBox1.Items.Add("v1");
            string temp;

            for(int i=0; i<15; i++)
            {
                IntX dekript = (IntX.Pow(put[i], priv[tren-1]) % n[tren-1]); //RSA dekripcija sljedeceg u nizu

                temp = dekript.ToString();
                tren = Int32.Parse(temp);

                if(tren<10) decPut[i + 1] = "V0" + tren.ToString(); //provjera da li je vrh manji od 10 samo radi ispisa s nulom ispred
                else decPut[i + 1] = "V" + tren.ToString();

                listBox1.Items.Add("v" + dekript.ToString());
            }

            int suma = 0, tezina;
            for (int i = 0; i < 14; i++)
            {
                using (System.IO.StreamReader file = new System.IO.StreamReader("tezine.txt"))
                {
                        while ((line = file.ReadLine()) != null) //čita težine
                    {

                        if (line.Contains(decPut[i]) && line.Contains(decPut[i + 1])){ //ako linija sadrži trenutačnog i sljedeceg u nizu, uzima njihovu tezinu 
                            tezina = int.Parse(line.Substring(line.LastIndexOf('=') + 1));
                            suma = suma + tezina; //tezina se pribraja sumi puta
                        }
                    }
                }
            }
            label2.Text = "Tezina: " + suma; //ispis tezine puta
        }
    }
}
