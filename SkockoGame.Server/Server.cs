using Domen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace SkockoGame.Server
{
    public class Server
    {
        Socket osluskujuciSoket;
        //Ovo se dodaje tek kada se dodaje za ime
        List<Socket> listaSoketa = new List<Socket>();
        BinaryFormatter formatter = new BinaryFormatter();
        
        private static Server instance;
        private Server()
        {

        }
        public static Server Instance
        {
            get
            {
                if (instance is null) instance = new Server();
                return instance;
            }
        }

        public bool PokreniServer()
        {
            try
            {
                osluskujuciSoket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint parametri = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9090);
                osluskujuciSoket.Bind(parametri);
                osluskujuciSoket.Listen(5);
                listaSoketa.Add(osluskujuciSoket);
                Thread t = new Thread(Osluskuj);
                t.Start();
                return true;
            }
            catch (SocketException)
            {
                osluskujuciSoket.Close();
                return false;
            }
            catch (IOException)
            {
                osluskujuciSoket.Close();
                return false;
            }
        }
        public bool ZaustaviServer()
        {
            foreach(var sok in listaSoketa)
            {
                sok.Close();
            }
            return true;
        }

        private void Osluskuj()
        {
            bool kraj = false;
            while (!kraj)
            {
                try
                {
                    Socket klijentskiSoket = osluskujuciSoket.Accept();
                    new Thread(() => ProveraIme(klijentskiSoket)).Start();
                    listaSoketa.Add(klijentskiSoket);

                }
                catch (SocketException)
                {
                    kraj = true;
                }
                catch (IOException)
                {
                    kraj = true;
                }
            }

        }

        private void ProveraIme(Socket socket)
        {
            try
            {
                NetworkStream tok = new NetworkStream(socket);
                Zahtev zahtev = (Zahtev)formatter.Deserialize(tok);
                Igrac igrac = new Igrac() { Soket = socket, Username = zahtev.Pokusaj };
                Obrada obrada = new Obrada(igrac);
                obrada.KlijentObrada();
            }
            catch (SocketException)
            {
                return;
            }
            catch (IOException)
            {
                return;
            }
        }
    }
}
