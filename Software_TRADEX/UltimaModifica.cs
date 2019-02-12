using Software_TRADEX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Software_TRADEX
{
    class UltimaModifica
    {
        /// <summary>
        /// Classe per la ricerca e memorizzazione dell'ultima modifica avvenuta in una cartella.
        /// </summary>
        private Dictionary<string, DateTime> allDate = new Dictionary<string, DateTime>();
        DateTime dtNew = new DateTime();
        private string attenzionePath;

        /// <summary>
        /// Costruttore vuoto
        /// </summary>
        public UltimaModifica()
        {
            this.attenzionePath = "";
        }

        /// <summary>
        /// Metodo che scrive nei programmi ricevuti le rispettive ultime modifiche che si hanno in memoria poi controlla se esiste una cartella per quel programma
        /// La scrittura avviene in un formato che permette di ordinarli in modo da evidenziare gli ultimi programmi su cui si è lavorato
        /// Il controllo di presenza della cartella verifica anche che la cartella contenga almeno un elemento
        /// </summary>
        public void aggiornoModifiche(List<Programma> programmi)
        {
            Console.WriteLine("Aggiorno Modifiche");
            Globals.log.Info("Aggiorno Modifiche");
            foreach (Programma p in programmi)
            {
                if (allDate.TryGetValue("Id" + p.numero, out DateTime ultima))
                {
                    p.dataModifica = ultima.ToString("yyyy/MM/dd HH:mm:ss");
                }
            }
            foreach (Programma p in programmi)
            {
                string path = Globals.PROGRAMMIpath + "Id" + p.numero;
                if (Directory.Exists(path) && Directory.GetFileSystemEntries(path, "*.*").Count() > 0)
                {
                    p.presenzaCartella = true;
                }
                else
                {
                    p.presenzaCartella = false;
                }
            }
        }

        /// <summary>
        /// Metodo che fa partire una ricerca all'interno di tutti i file in tutte le cartelle (ricrosivamente) 
        /// a partire dalla cartella 'path'. Viene memorizzata l'ultima modifica più recente.
        /// Vengono controllate solo le modifiche dei file e non delle cartelle per cercare di non essere ingannati dalle cartelle copiate.
        /// Restituisce false se viene generata qualche eccezione.
        /// Utilizza i metodi ausiliari: modificheByFile, ProcessDirectory e ProcessFile.
        /// </summary>
        public bool ricercaLenta(string path)
        {
            try
            {
                System.Windows.MessageBox.Show(path);

                allDate = new Dictionary<string, DateTime>();
                if (Directory.Exists(path))
                {
                    foreach (string proj in Directory.GetDirectories(path))
                    {
                        DateTime res = modificheByFile(proj);

                        allDate.Add(proj.Split('\\').Last(), res);
                        Console.WriteLine("CARTELLA: <" + proj + "> - DATA PIU' RECENTE: <" + res + ">");
                        Globals.log.Info("CARTELLA: <" + proj + "> - DATA PIU' RECENTE: <" + res + ">");
                    }
                }
            }
            catch (Exception e)
            {

                string msg = "E10 - Eccezione nella ricerca ultime modifiche: " + e + "\n" + attenzionePath;
                System.Windows.MessageBox.Show(msg);
                Console.WriteLine(msg);
                Globals.log.Error(msg);
                return false;
            }
            return true;

        }

        /// <summary>
        /// Chiama i metodi ProcessDirectory o ProcessFile a seconda di che tipo di elemento riceve
        /// </summary>
        public DateTime modificheByFile(string proj)
        {
            dtNew = new DateTime();
            if (Directory.Exists(proj))
            {
                ProcessDirectory(proj);
            }
            else if (File.Exists(proj))
            {
                ProcessFile(proj);
            }
            return dtNew;
        }

        /// <summary>
        /// Processa tutti i file nella cartella che è stata passata e
        /// agisce ricorsivamente nello stesso modo in ogni sotto-directory
        /// </summary>
        public void ProcessDirectory(string targetDirectory)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }

        /// <summary>
        /// Azione di confronto su ogni singolo file per valutare se la sua data di modifica 
        /// sia la più recente e nel caso memorizzarla.
        /// </summary>
        private void ProcessFile(string path)
        {
            try
            {
                DateTime dt = File.GetLastWriteTime(path);

                if (DateTime.Compare(dtNew, dt) < 0)
                {
                    dtNew = dt;
                }
            }
            catch (PathTooLongException)
            {
                Globals.log.Error("Errore, PathTooLongException: " + path);
            }
        }
    }
}