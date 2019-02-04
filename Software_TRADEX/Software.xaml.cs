using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Software_TRADEX
{
    /// <summary>
    /// Logica di interazione per Software.xaml
    /// ORGANIZZAZIONE:
    /// - COSTRUTTORI
    /// - FUNZIONI PRINCIPALI
    /// - CONTROLLI GENERALI
    /// - BOTTONI
    /// - INTERAZIONE CON L'UTENTE
    /// - MENU DI IMPOSTAZIONI
    /// </summary>
    public partial class Software : Page
    {

        //private bool back = false;
        private int ProgSelezionato;
        private List<String> progetti = new List<String>();
        //private List<Progetto> progettiSync = new List<Progetto>();
        private UltimaModifica ultimaModifica;

        public Software()
        {
            InitializeComponent();
            PreviewKeyDown += new KeyEventHandler(PreviewKeyDown2);
            readPrograms();
            createList();
            this.ultimaModifica = new UltimaModifica();
        }

        /// <summary>
        /// Procedura inziale: 
        /// - legge i progetti da file
        /// - legge le ultime modifiche da file (poi mette i risultati in progetti)
        /// - controlla la sincronizzazione
        /// - crea la lista
        /// - initcheck: guarda se coincidono il numero di progetti e l'ultimo progetto
        /// - imposta la visibilità degli elementi
        /// </summary>
        private void initialize()
        {
            //readProjects();
            //ProgSelezionato = Globals.CLIENTI[num_cliente].getlastId();
            //ultimaModifica = new UltimaModifica(Globals.CLIENTI[num_cliente]);
            //Globals.log.Info("Leggo date.csv");
            //if (!ultimaModifica.readByCSV(Globals.DATI + Globals.CLIENTI[num_cliente].getNomeCliente() + "date.csv"))
            //{
            //    string msg = "E02 - Il file " + Globals.DATI + Globals.CLIENTI[num_cliente].getNomeCliente() +
            //        "date.csv" + " non esiste o è aperto da un altro programma.\n\nLe ultime modifiche dei progetti non " +
            //        "saranno caricate da file.";
            //    MessageBox.Show(msg, "E02", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            //    Globals.log.Error(msg);
            //}
            //ultimaModifica.aggiornoModifiche(progetti);
            //CheckBox_sync_ultima_modifica();
            //createList();
            //Label titolo = this.FindName("titolo") as Label;
            //titolo.Content = titolo.Content.ToString() + " " + Globals.CLIENTI[num_cliente].getNomeCliente().Replace("_", "__");
            //PreviewKeyDown += new KeyEventHandler(PreviewKeyDown2);
            //InitCheck();
            //SetVisibility();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                            FUNZIONI PRINCIPALI                             ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Legge i progetti da file .csv e li salva nella lista progetti.
        /// </summary>
        private void readPrograms()
        {
            int j = 0;
            try
            {
                if (Globals.PROGRAMMI == null)
                {
                    Globals.log.Info("Lettura PROGRAMMI.csv");
                    var file = File.OpenRead(Globals.DATI + @"PROGRAMMI.csv");
                    var reader = new StreamReader(file);
                    Globals.PROGRAMMI = new List<Programma>();
                    while (!reader.EndOfStream)
                    {
                        string[] line = reader.ReadLine().Split(',');
                        if (line.Length == 8)
                        {
                            Globals.PROGRAMMI.Add(new Programma(Int32.Parse(line[0]), line[1], line[2], line[3], line[4].Equals("True"), line[5], line[6], line[7]));
                        }
                        j++;
                    }
                    file.Close();
                }
            }
            catch (IOException)
            {
                string msg = "E00 - Il file " + Globals.DATI + @"PROGRAMMI.csv" +
                    " non esiste o è aperto da un altro programma. \n L'APPLICAZIONE SARA' CHIUSA";
                MessageBox.Show(msg, "E00"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Fatal(msg);
                Environment.Exit(0);
            }
            catch (FormatException)
            {
                string msg = "E00 - Il file " + Globals.DATI + @"PROGRAMMI.csv" +
                    " è in un formato non corretto. \nProblema riscontrato al programma numero: " + j;
                MessageBox.Show(msg, "E00", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
        }

        /// <summary>
        /// Aggiunge la lista di progetti alla DataGrid dopo averla svuotata.
        /// Mentre li scorre cerca quello che era stato aperto per ultimo e quando lo trova lo seleziona.
        /// </summary>
        private void createList()
        {
            Console.WriteLine("Create List");
            Globals.log.Info("Create List");
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            dataGrid.SelectionChanged += new SelectionChangedEventHandler(ChangePreview);
            dataGrid.Items.Clear();
            int i = 0;
            foreach (Programma p in Globals.PROGRAMMI)
            {
                dataGrid.Items.Add(p);
                i++;
                //if (p.numero.Equals(Globals.PROGRAMMI[num_cliente].getlastId()))
                //{
                //    dataGrid.SelectedIndex = i;
                //    dataGrid.ScrollIntoView(progetti[i]);
                //}
            }
        }

        /// <summary>
        /// Aggiorna gli elementi nella DataGrid:
        /// - controlla la sincronizzazione
        /// - aggiorna le ultime modifiche in progetti
        /// - aggiunge tutti i progetti presenti e FILTRATI dopo aver svuotato la DataGrid
        /// - mentre scorre i progetti restituisce il primo visualizzato per permettere di selezionarlo durante la ricerca
        /// </summary>
        private Programma updateList(string filter)
        {
            Console.WriteLine("Update list1");
            Globals.log.Info("Update list1");
            Programma primo = new Programma(0, "", "", "", false, "", "", "");
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            ultimaModifica.aggiornoModifiche(Globals.PROGRAMMI);
            if (dataGrid != null)
            {
                dataGrid.Items.Clear();
                int i = 0;
                foreach (Programma p in Globals.PROGRAMMI)
                {
                    if (p.toName().IndexOf(filter, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                    {
                        if (i == 0)
                        {
                            primo = p;
                        }
                        dataGrid.Items.Add(p);
                        i++;
                    }
                }
            }
            return primo;
        }

        /// <summary>
        /// Aggiorna gli elementi nella DataGrid DOPO AVER CREATO NUOVI PROGETTI:
        /// - LEGGE I PROGETTI DA FILE (unica cosa in più del precedente)
        /// - controlla la sincronizzazione
        /// - aggiorna le ultime modifiche in progetti
        /// - aggiunge tutti i progetti presenti dopo aver svuotato la DataGrid
        /// - mentre scorre i progetti cerca quello che era stato aperto per ultimo e quando lo trova lo seleziona.
        /// </summary>
        //private void updateListNewProject(object sender, System.Windows.Forms.FormClosedEventArgs e)
        //{
        //Console.WriteLine("UpdateList2");
        //Globals.log.Info("UpdateList2");
        //progetti = new List<Progetto>();
        //readProjects();
        //DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
        //ultimaModifica.aggiornoModifiche(progetti);
        //if (dataGrid != null)
        //{
        //    int i = 0;
        //    dataGrid.Items.Clear();
        //    foreach (Progetto p in progetti)
        //    {
        //        dataGrid.Items.Add(p);
        //        if (p.numero.Equals(Globals.CLIENTI[num_cliente].getlastId()))
        //        {
        //            dataGrid.SelectedIndex = i;
        //            dataGrid.ScrollIntoView(progetti[i]);
        //        }
        //        i++;
        //    }
        //}
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                             CONTROLLI GENERALI                             ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Dopo aver caricato la pagina da il focus alla textbox e controlla se è necessario tornare a Clienti_Home
        /// </summary>
        public void Progetti_Home_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = this.FindName("TextBox") as TextBox;
            textBox.Focus();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                                BOTTONI                                     ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Bottone apertura cartella del filesystem.
        /// Imposta questo come ultimo progetto aperto.
        /// </summary>
        private void Button_Open_Folder(object sender, RoutedEventArgs e)
        {
            //Globals.PROGRAMMI[num_cliente].setLastId(ProgSelezionato);
            string path = Globals.PROGRAMMIpath + "Id" + ProgSelezionato;
            Console.WriteLine(path);
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
            TextBox textBox = this.FindName("TextBox") as TextBox;
            textBox.Focus();
        }

        /// <summary>
        /// Bottone per la creazione di un nuovo progetto.
        /// Chiama il FORM Form1. (se non ci sono le condizioni può portare a Clienti_Home)
        /// </summary>
        private void Button_New_Project(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Apre il file docx attualmente visualizzato
        /// </summary>
        private void Button_Apri_Docx(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Globals.PROGRAMMIpath + "Id" + ProgSelezionato + @"\programma.docx");
        }

        private void Button_Convert(object sender, RoutedEventArgs e)
        {
            string originFile = Globals.DATI + "tradex.txt";
            string destFile = Globals.DATI + "PROGRAMMI.csv";
            List<Programma> programmi = new List<Programma>();
            if (File.Exists(originFile))
            {

                StreamReader objReader = new StreamReader(originFile);
                do
                {
                    string num = objReader.ReadLine();
                    objReader.Peek();
                    string nome = objReader.ReadLine();
                    objReader.Peek();
                    string utente = objReader.ReadLine();
                    objReader.Peek();
                    string pwd = objReader.ReadLine();
                    objReader.Peek();
                    string data = objReader.ReadLine();
                    objReader.Peek();
                    if (nome.Equals(".") && utente.Equals(".") && pwd.Equals(".") && data.Equals("."))
                    {
                        break;
                    }
                    Programma prog = new Programma(Int32.Parse(num), nome, data, ".", false, utente, pwd, ".");
                    programmi.Add(prog);
                } while (objReader.Peek() != -1);
                objReader.Close();
            }
            string[] lines = new string[programmi.Count];
            int i = 0;
            foreach (Programma p in programmi)
            {
                lines[i] = p.numero + "," + p.nome + "," + p.dataCreazione + "," + p.dataModifica
                    + "," + p.obsoleto + "," + p.nomeUtente + "," + p.password + "," + p.descrizione;
                i++;
            }
            try
            {
                File.WriteAllLines(destFile, lines);
            }
            catch (IOException)
            {
                string msg = "E00 - errore nella conversione file";
                MessageBox.Show(msg, "E00", MessageBoxButton.OK,
                                MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
        }

        /// <summary>
        /// Bottone che attiva il controllo delle ultime modifiche di tutti i progetti nella cartella di questo cliente.
        /// (su un altro thread)
        /// Le aggiorna in progetti e ricarica la DataGrid. (se tutto è andato bene)
        /// [Disabilita i bottoni di sincronizzazione]
        /// </summary>
        private void Button_Ultime_Modifiche(object sender, RoutedEventArgs e)
        {
            Button buttonModifiche = this.FindName("BottModifiche") as Button;
            buttonModifiche.IsEnabled = false;
            Task.Factory.StartNew(() =>
            {
                if (ultimaModifica.ricercaLenta(Globals.PROGRAMMIpath))
                {
                    if (!ultimaModifica.writeInCSV(Globals.DATI + "date.csv"))
                    {
                        string msg = "E00 - Il file " + Globals.DATI + "date.csv"
                            + " non esiste o è aperto da un altro programma. \n\nNon è stato possibile salvare i dati relativi alle" +
                            " ultime modifiche.";
                        MessageBox.Show(msg, "E00"
                                         , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        Globals.log.Error(msg);
                    }
                    ultimaModifica.aggiornoModifiche(Globals.PROGRAMMI);
                }
                else
                {
                    string msg = "E00 non riuscito aggiornamento ultime modifiche";
                    MessageBox.Show(msg, "E00"
                                         , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    Globals.log.Error(msg);
                }

            }).ContinueWith(task =>
            {
                buttonModifiche.IsEnabled = true;
                //updateList("");
                string msg = "Le ultime modifiche di tutti i progetti di Id" + ProgSelezionato +
                    " sono state aggiornate e caricate nel relativo file csv.";
                MessageBox.Show(msg, "Modifiche aggiornate"
                                     , MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Info(msg);
                updateList("");
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                        INTERAZIONE CON L'UTENTE                            ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Al doppio click sulla riga apre la cartella del filesystem.
        /// </summary>
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            Button_Open_Folder(sender, null);
        }

        /// <summary>
        /// Funzioni per consentire di navigare la DataGrid con le freccie su e giù mentre si effettua una ricerca.
        /// Con invio si apre la cartella del filesystem del progetto selezionato
        /// </summary>
        private void PreviewKeyDown2(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
                dataGrid.Focus();
                if (dataGrid.SelectedIndex > 0)
                {
                    dataGrid.SelectedIndex = dataGrid.SelectedIndex - 1;
                    dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                }
            }
            if (e.Key == Key.Down)
            {
                DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
                dataGrid.SelectedIndex = dataGrid.SelectedIndex + 1;
                if (dataGrid.SelectedItem != null)
                {
                    dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                }
            }
            if (e.Key == Key.Enter)
            {
                Button_Open_Folder(null, null);
            }
        }

        /// <summary>
        /// Richiamato ogni volta che cambia la selezione nella DataGrid.
        /// - aggiorna ProgSelezionato.
        /// - carica immagine di anteprima e docx (se disponibili)
        /// NullReferenceException spesso sollevata perchè quando si cerca si ricarica la DataGrid e 
        /// per un momento non c'è nessun progetto selezionato. 
        /// </summary>
        private void ChangePreview(object sender, EventArgs e)
        {
            Console.WriteLine("Change Preview");
            Globals.log.Info("Change Preview");
            try
            {
                ProgSelezionato = (((Programma)((DataGrid)sender).SelectedValue).numero);
                RichTextBox richTextBox = this.FindName("richTextBox") as RichTextBox;
                Button button = this.FindName("buttonOpenDocx") as Button;

                richTextBox.Visibility = Visibility.Visible;
                button.Visibility = Visibility.Visible;

                string file = Globals.PROGRAMMIpath + "Id" + ProgSelezionato + @"\programma.docx";
                if (File.Exists(file))
                {
                    var doc = Xceed.Words.NET.DocX.Load(file);
                    richTextBox.Document.Blocks.Clear();
                    richTextBox.AppendText(doc.Text);
                }
                else
                {
                    richTextBox.Document.Blocks.Clear();
                    richTextBox.Visibility = Visibility.Hidden;
                    button.Visibility = Visibility.Hidden;
                }
            }
            catch (NullReferenceException nre)
            {
                //Console.WriteLine("ECCEZIONE: " + nre);
                Globals.log.Warn("Eccezione in changePreview: " + nre);
            }

        }

        /// <summary>
        /// Richiamato ogni volta che viene modificato il testo nella TextBox di ricerca.
        /// Aggiorna e filtra la lista e poi seleziona la prima riga visualizzata.
        /// </summary>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Programma p = updateList(((TextBox)sender).Text);
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            dataGrid.SelectedIndex = 0;
            dataGrid.ScrollIntoView(p);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                          MENU DI IMPOSTAZIONI                              ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Apre il FORM Form_percorsi per modificare i path in cui il programma cerca i file.
        /// </summary>
        private void Menu_percorsi(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Apre il FORM Form_github per modificare il path di git.exe e del repository github.
        /// </summary>
        private void Menu_github(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Imposta la visibilità dell'anteprima dell'immagine e del docx.
        /// Aggiorna la variabile Globals.ANTEPRIME e scrive sul .csv.
        /// </summary>
        private void Menu_anteprima(object sender, RoutedEventArgs e)
        {


        }

        /// <summary>
        /// Imposta la visibilità dei bottoni di sincronizzazione.
        /// Aggiorna la variabile Globals.SINCRONIZZAZIONE e scrive sul .csv.
        /// </summary>
        private void Menu_sync(object sender, RoutedEventArgs e)
        {


        }

        /// <summary>
        /// Apre il FORM Form_aggiornaCSV importare file .csv per MATRIX e renderli in formato DATA.
        /// </summary>
        private void Menu_importa_CSV(object sender, RoutedEventArgs e)
        {

        }
    }




}