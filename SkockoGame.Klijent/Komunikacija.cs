using Domen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace SkockoGame.Klijent
{
    public class Komunikacija
    {
        private static Komunikacija instance;
        private Socket klijentskiSoket;
        private BinaryFormatter formatter = new BinaryFormatter();
        private NetworkStream tok;
        public FrmKlijent forma;
        private Komunikacija()
        {

        }
        public static Komunikacija Instance
        {
            get
            {
                if (instance is null) instance = new Komunikacija();
                return instance;
            }
        }

        internal void PrijemPoruke()
        {
            bool kraj = false;
            while (!kraj)
            {
                try
                {

                    Odgovor odgovor = (Odgovor)formatter.Deserialize(tok);
                    switch (odgovor.Forma)
                    {
                        case (FormaPrikaz.Ja):
                            forma.DodajPorukuJa(odgovor.Poruka);
                            break;
                        case (FormaPrikaz.Protivnik):
                            forma.DodajPorukuProtivnik(odgovor.Poruka);
                            break;
                        case (FormaPrikaz.Rezultat):
                            forma.DodajPorukuRezultat(odgovor.Poruka);
                            break;
                    }

                }
                catch (SocketException)
                {
                    kraj = true;
                    

                }
                catch (IOException)
                {

                    MessageBox.Show("Pukao je server!");
                    klijentskiSoket.Close();
                    kraj = true;

                }
            }
        }

        public bool PoveziSe()
        {
            try
            {
                klijentskiSoket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                klijentskiSoket.Connect("localhost", 9090);
                tok = new NetworkStream(klijentskiSoket);
                return true;
            }
            catch (SocketException)
            {
                return false;

            }
        }
        public string PosaljiPoruku(string poruka)
        {
            try
            {
                Zahtev zahtev = new Zahtev() { Pokusaj = poruka };
                formatter.Serialize(tok, zahtev);
                //string odgovor = (string)formatter.Deserialize(tok);
                //return odgovor;
                return "";
            }
            catch (SocketException)
            {
                return null;

            }
            catch (IOException)
            {
                MessageBox.Show("Pukao je server!");
                klijentskiSoket.Close();
                return null;

            }
        }
    }
}
