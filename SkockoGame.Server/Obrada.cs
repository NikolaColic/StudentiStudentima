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
        Igrac igrac1;
        Igrac igrac2;
        bool timer = false;
        BinaryFormatter formatter = new BinaryFormatter();
        NetworkStream stream1;
        NetworkStream stream2;
        public Obrada(Igrac igrac1, Igrac igrac2)
        {
            this.igrac1 = igrac1;
            this.igrac2 = igrac2;
            stream1 = new NetworkStream(igrac1.Soket);
            stream2 = new NetworkStream(igrac2.Soket);
            
        }
        

        public void KlijentObrada()
        {
            bool kraj = false;
            int brojac = 0;
            bool znak = false;
            while (!kraj)
            {
                try
                {
                  
                    if (!znak)
                    {
                        Thread.Sleep(3000);
                        PosaljiJednom("Vi ste na potezu", FormaPrikaz.Ja, stream1);
                        PosaljiJednom("Protivnik je na potezu", FormaPrikaz.Protivnik, stream2);
                        znak = true;
                    }
                    
                    Zahtev zahtev = (Zahtev)formatter.Deserialize(stream1);
                    if(zahtev.Pokusaj == "Isteklo je vreme")
                    {
                        brojac++;
                        znak = false;
                        
                        PosaljiJednom("Isteklo je vreme", FormaPrikaz.Ja, stream1);
                        PosaljiJednom("Protivniku je isteklo vreme!", FormaPrikaz.Protivnik, stream2);
                        if (brojac == 1) igrac1.BrojBodova = 0;
                        if (brojac == 2)
                        {
                            igrac2.BrojBodova = 0;
                            Thread.Sleep(2000);
                            PosaljiOba($"Gotova igra:\n{igrac1.Username}:\t{igrac1.BrojBodova}" +
                                $"\n{igrac2.Username}:\t{igrac2.BrojBodova}", FormaPrikaz.Rezultat);
                            return;
                        }
                        stream1 = new NetworkStream(igrac2.Soket);
                        stream2 = new NetworkStream(igrac1.Soket);
                        continue;
                    }
                    string poruka = Provera(zahtev.Pokusaj);
                    if (poruka.Contains("Pogodili"))
                    {
                        brojac++;
                        znak = false;
                        PosaljiJednom(poruka, FormaPrikaz.Ja, stream1);
                        PosaljiJednom($"Protivnik je ostvario sledeci rezultat: {igrac1.BrojBodova}", FormaPrikaz.Protivnik, stream2);
                        if (brojac == 2)
                        {
                            Thread.Sleep(2000);
                            PosaljiOba($"Gotova igra:\n{igrac1.Username}:\t{igrac1.BrojBodova}" +
                                $"\n{igrac2.Username}:\t{igrac2.BrojBodova}", FormaPrikaz.Rezultat);
                            return;
                        }
                        stream1 = new NetworkStream(igrac2.Soket);
                        stream2 = new NetworkStream(igrac1.Soket);
                        continue;
                    }
                    if (brojac == 1) igrac2.BrojBodova -= 2;
                    else igrac1.BrojBodova -= 2;
                    if(igrac1.BrojBodova == 0)
                    {
                        PosaljiJednom("Niste uspeli da pogodite", FormaPrikaz.Ja, stream1);
                        PosaljiJednom("Protivnik nije uspeo da resi", FormaPrikaz.Protivnik, stream2);
                        znak = false;
                        brojac++;
                        if (brojac == 2)
                        {
                            Thread.Sleep(2000);
                            PosaljiOba($"Gotova igra:\n{igrac1.Username}:\t{igrac1.BrojBodova}" +
                                $"\n{igrac2.Username}:\t{igrac2.BrojBodova}", FormaPrikaz.Rezultat);
                            return;
                        }
                        stream1 = new NetworkStream(igrac2.Soket);
                        stream2 = new NetworkStream(igrac1.Soket);
                        continue;
                    }

                    PosaljiJednom(poruka, FormaPrikaz.Ja, stream1);
                    PosaljiJednom("Protivnik je ostvario sledeci rezultat\n"+poruka, FormaPrikaz.Protivnik, stream2);


                }
                catch (SocketException)
                {
                    igrac1.Soket.Close();
                    igrac2.Soket.Close();
                    
                    kraj = true;
                }
                catch (IOException)
                {
                    igrac1.Soket.Close();
                    igrac2.Soket.Close();
                    kraj = true;
                }

            }
        }

        private string Provera(string pokusaj)
        {
            int tacno = ProveraTacno(pokusaj);
            if (tacno == 4) return $"Pogodili ste tacno resenje. Cestitamo! Osvojili ste {igrac1.BrojBodova}";
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

        private void PosaljiOba(string poruka, FormaPrikaz prikaz)
        {
            formatter.Serialize(stream1, KreirajOdgovor(poruka, prikaz));
            formatter.Serialize(stream2, KreirajOdgovor(poruka, prikaz));
        }
        private void PosaljiJednom(string poruka, FormaPrikaz prikaz, NetworkStream tok)
        {
            formatter.Serialize(tok, KreirajOdgovor(poruka, prikaz));
        }


        private Odgovor KreirajOdgovor(string poruka, FormaPrikaz prikaz)
        {
            return new Odgovor() { Forma = prikaz, Poruka = poruka };
        }

       
    }
}
