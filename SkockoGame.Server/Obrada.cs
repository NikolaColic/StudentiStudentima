using Domen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace SkockoGame.Server
{
    public class Obrada
    {
        string resenje = "Herc Herc Pik Tref";
        Igrac igrac;
        BinaryFormatter formatter = new BinaryFormatter();
        NetworkStream stream;
        public Obrada(Igrac igrac)
        {
            this.igrac = igrac;
            stream = new NetworkStream(igrac.Soket);
            
        }

        public void KlijentObrada()
        {
            bool kraj = false;
            formatter.Serialize(stream, new Odgovor() { Forma = FormaPrikaz.Ja, Poruka = $"Dobrodosao {igrac.Username}" });
            while (!kraj)
            {
                try
                {
                    Zahtev zahtev = (Zahtev)formatter.Deserialize(stream);
                    string poruka = Provera(zahtev.Pokusaj);
                    if (poruka.Contains("Pogodili"))
                    {
                        Odgovor o1 = new Odgovor() { Poruka = poruka, Forma = FormaPrikaz.Ja };
                        formatter.Serialize(stream, o1);
                        return;
                    }
                    igrac.BrojBodova -= 2;
                    if(igrac.BrojBodova == 0)
                    {
                        Odgovor o2 = new Odgovor() { Poruka = "Niste uspeli da resite", Forma = FormaPrikaz.Ja };
                        formatter.Serialize(stream, o2);
                        return;
                    }
                    Odgovor o = new Odgovor() { Poruka = poruka, Forma = FormaPrikaz.Ja };
                    formatter.Serialize(stream, o);
                    

                }
                catch (SocketException)
                {
                    igrac.Soket.Close();
                    kraj = true;
                }
                catch (IOException)
                {
                    igrac.Soket.Close();
                    kraj = true;
                }

            }
        }

        private string Provera(string pokusaj)
        {
            int tacno = ProveraTacno(pokusaj);
            if (tacno == 4) return $"Pogodili ste tacno resenje. Cestitamo! Osvojili ste {igrac.BrojBodova}";
            int loseMesto = ProveraPogodjeno(pokusaj) - tacno;
            return $"Tacno mesto:\t{tacno}\nNije pravo mesto:\t{loseMesto}";
        }

        private int ProveraTacno(string pokusaj)
        {
            var pokusajNiz = pokusaj.Split(' ');
            var resenjeNiz = resenje.Split(' ');
            int brojac = 0;
            for(int i =0; i < pokusajNiz.Length-1; i++)
            {
                if (pokusajNiz[i] == resenjeNiz[i]) brojac++;
            }
            return brojac;
        }
        private int ProveraPogodjeno(string pokusaj)
        {
            //Kreirati dva niza kao gore
            var pokusajNiz = pokusaj.Split(' ');
            var resenjeNiz = resenje.Split(' ');
            //Kreirati
            string[] pokusajBezPonavljanja = pokusajNiz.Distinct().ToArray();
            string[] resenjeBezPonavljanja = resenjeNiz.Distinct().ToArray();
            int brojac = 0;
            for(int i =0; i< pokusajBezPonavljanja.Length; i++)
            {
                for(int j =0; j< resenjeBezPonavljanja.Length; j++)
                {
                    if(pokusajBezPonavljanja[i] == resenjeBezPonavljanja[j])
                    {
                        int brojPokusaj = pokusajNiz.Count((el) => el == pokusajBezPonavljanja[i]);
                        int brojResenje = resenjeNiz.Count((el) => el == pokusajBezPonavljanja[i]);
                        //if (brojPokusaj >= brojResenje) brojac += brojResenje;
                        //else brojac += brojPokusaj;
                        brojac += (brojPokusaj >= brojResenje) ? brojResenje : brojPokusaj;

                    }
                }
            }
            return brojac;
        }
    }
}
