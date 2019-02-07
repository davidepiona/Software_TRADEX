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
        private int ProgSelezionato;
        private UltimaModifica ultimaModifica;

        /// <summary>
        /// Il costruttore inizializza il componenti. Legge i programmi da file e li scrive nella DataGrid.
        /// </summary>
        public Software()
        {
            InitializeComponent();
            PreviewKeyDown += new KeyEventHandler(PreviewKeyDown2);
            Loaded += Software_Loaded;
            readPrograms();
            createList();
            this.ultimaModifica = new UltimaModifica();
            SetVisibility();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                            FUNZIONI PRINCIPALI                             ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Legge i programmi da file .csv e li salva nella lista programmi.
        /// </summary>
        private void readPrograms()
        {
            int j = 0;
            try
            {
                if (Globals.PROGRAMMI == null)
                {
                    Globals.PROGRAMMI = new List<Programma>();
                    Globals.log.Info("Lettura PROGRAMMI.csv");
                    var file = File.OpenRead(Globals.DATI + @"PROGRAMMI.csv");
                    var reader = new StreamReader(file);
                    while (!reader.EndOfStream)
                    {
                        string[] line = reader.ReadLine().Split(',');
                        if (line.Length == 8)
                        {
                            Globals.PROGRAMMI.Add(new Programma(Int32.Parse(line[0]), line[1], line[2], line[3], line[4].Equals("True"), line[5], line[6], line[7]));
                        }
                        j++;
                        Console.WriteLine("LETTO"+j);
                    }
                    file.Close();
                }
            }
            catch (IOException)
            {
                string msg = "E01 - Il file " + Globals.DATI + @"PROGRAMMI.csv non esiste o è aperto da un altro programma. \n";
                MessageBox.Show(msg, "E01"
                                     , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Fatal(msg);
            }
            catch (FormatException)
            {
                string msg = "E02 - Il file " + Globals.DATI + @"PROGRAMMI.csv" +
                    " è in un formato non corretto. \nProblema riscontrato al programma numero: " + j;
                MessageBox.Show(msg, "E02", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
        }

        /// <summary>
        /// Aggiunge la lista di programmi alla DataGrid dopo averla svuotata.
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
            }
        }

        /// <summary>
        /// Aggiorna gli elementi nella DataGrid:
        /// - aggiorna le ultime modifiche in programmi
        /// - aggiunge tutti i programmi presenti e FILTRATI dopo aver svuotato la DataGrid
        /// - mentre scorre i programmi restituisce il primo visualizzato per permettere di selezionarlo durante la ricerca
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
        /// Aggiorna gli elementi nella DataGrid DOPO AVER CREATO NUOVI PROGRAMMI:
        /// - LEGGE I PROGRAMMI DA FILE (unica cosa in più del precedente)
        /// - aggiorna le ultime modifiche in programmi
        /// - aggiunge tutti i programmi presenti dopo aver svuotato la DataGrid
        /// - seleziona l'ultimo programma della lista (quello appena creato in teoria)
        /// </summary>
        private void readAgainListPrograms(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            Console.WriteLine("UpdateList2");
            Globals.log.Info("UpdateList2");
            Globals.PROGRAMMI = null;
            readPrograms();
            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
            if (dataGrid != null)
            {
                dataGrid.Items.Clear();
                int i = 0;
                foreach (Programma p in Globals.PROGRAMMI)
                {
                    dataGrid.Items.Add(p);
                    i++;
                }
                dataGrid.SelectedIndex = Globals.PROGRAMMI.Last().numero - 1;
                dataGrid.ScrollIntoView(Globals.PROGRAMMI.Last());
            }
        }
        /// <summary>
        /// Metodo per la riscrittura di Globals.PROGRAMMI nel file PROGRAMMI.csv
        /// </summary>
        private void scriviCSV()
        {
            List<string> lines = new List<string>();
            int i = 0;
            foreach (Programma p in Globals.PROGRAMMI)
            {
                lines.Add(p.numero + "," + p.nome + "," + p.dataCreazione + "," + p.dataModifica
                    + "," + p.obsoleto + "," + p.nomeUtente + "," + p.password + "," + p.descrizione);
                i++;
            }
            try
            {
                File.WriteAllLines(Globals.DATI+ "PROGRAMMI.csv", lines);
            }
            catch (IOException)
            {
                string msg = "E03 - errore nella scrittura del file";
                MessageBox.Show(msg, "E03", MessageBoxButton.OK,
                                MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                             CONTROLLI GENERALI                             ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Dopo aver caricato la pagina da il focus alla textbox
        /// </summary>
        public void Software_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = this.FindName("TextBox") as TextBox;
            textBox.Focus();
        }

        /// <summary>
        /// Impostazione iniziale della visibilità dei bottoni secondo le impostazioni
        /// </summary>
        private void SetVisibility()
        {
            Console.WriteLine("Set visibility");
            Globals.log.Info("Set visibility");
            MenuItem ma = this.FindName("Menu_anteprima_check") as MenuItem;
            ma.IsChecked = Globals.ANTEPRIME;
            if (!Globals.ANTEPRIME)
            {
                RichTextBox richTextBox = this.FindName("richTextBox") as RichTextBox;
                Button button = this.FindName("buttonOpenDocx") as Button;
                richTextBox.Visibility = Visibility.Hidden;
                button.Visibility = Visibility.Hidden;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                                BOTTONI                                     ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Bottone apertura cartella del filesystem. Poi restituisce il focus alla barra di ricerca.
        /// </summary>
        private void Button_Open_Folder(object sender, RoutedEventArgs e)
        {
            string path = Globals.PROGRAMMIpath + "Id" + ProgSelezionato;
            Console.WriteLine(path);
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
            else
            {
                string msg = "E04 - La cartella " + path + " che si è cercato di aprire non esiste.";
                MessageBox.Show(msg, "E04", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Warn(msg);
            }
            TextBox textBox = this.FindName("TextBox") as TextBox;
            textBox.Focus();
        }

        /// <summary>
        /// Bottone per la creazione di un nuovo programma.
        /// Chiama il FORM Form_NuovoProgramma. 
        /// </summary>
        private void Button_New_Program(object sender, RoutedEventArgs e)
        {
            Form_NuovoProgramma form = new Form_NuovoProgramma(Globals.PROGRAMMI.Last().numero);
            form.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.readAgainListPrograms);
            form.ShowDialog();
        }
        
        /// <summary>
        /// Controlla se esiste un file programma.docx nel programma attualmente visualizzato 
        /// - se esiste: apre il file docx
        /// - se non esiste : crea un file docx con tutte le informazioni del programma
        /// </summary>
        private void Button_Apri_Docx(object sender, RoutedEventArgs e)
        {
            string file = Globals.PROGRAMMIpath + "Id" + ProgSelezionato + @"\programma.docx";
            if (File.Exists(file))
            {
                System.Diagnostics.Process.Start(file);
            }
            else
            {
                Programma programma= Globals.PROGRAMMI.Find(x => x.numero.Equals(ProgSelezionato));
                if (programma != null)
                {
                    try
                    {
                        var doc = Xceed.Words.NET.DocX.Create(file);
                        doc.InsertParagraph("Id" + ProgSelezionato + "  -  "+ programma.nome).Bold();
                        doc.InsertParagraph("\n DATA CREAZIONE: " + programma.dataCreazione);
                        doc.InsertParagraph("\n OBSOLETO: " + programma.obsoleto.ToString());
                        doc.InsertParagraph("\n NOME UTENTE: " + programma.nomeUtente);
                        doc.InsertParagraph("\n PASSWORD: " + programma.password);
                        doc.InsertParagraph("\n DESCRIZIONE: " + programma.descrizione);
                        doc.Save();
                    }
                    catch (IOException)
                    {
                        string msg = "E05 - Il file " + file + " non è stato creato per un problema";
                        MessageBox.Show(msg, "E05", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                        Globals.log.Error(msg);
                    }
                    DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
                    ChangePreview(dataGrid, null);
                }
            }
        }

        /// <summary>
        /// Bottone che attiva il controllo delle ultime modifiche di tutti i programmi nella cartella di questo cliente.
        /// (su un altro thread)
        /// Le aggiorna in programmi e ricarica la DataGrid. (se tutto è andato bene)
        /// </summary>
        private void Button_Ultime_Modifiche(object sender, RoutedEventArgs e)
        {
            Button buttonModifiche = this.FindName("BottModifiche") as Button;
            buttonModifiche.IsEnabled = false;
            Task.Factory.StartNew(() =>
            {
                if (ultimaModifica.ricercaLenta(Globals.PROGRAMMIpath))
                {
                    ultimaModifica.aggiornoModifiche(Globals.PROGRAMMI);
                    scriviCSV();
                }
                else
                {
                    string msg = "E06 non riuscito aggiornamento ultime modifiche";
                    MessageBox.Show(msg, "E06"
                                         , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                    Globals.log.Error(msg);
                }
            }).ContinueWith(task =>
            {
                buttonModifiche.IsEnabled = true;
                string msg = "Le ultime modifiche di tutti i programmi di Id" + ProgSelezionato +
                    " sono state aggiornate e caricate nel relativo file csv.";
                MessageBox.Show(msg, "Modifiche aggiornate"
                                     , MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Info(msg);
                updateList("");
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Bottone che attiva la scrittura del file PROGRAMMI.csv
        /// </summary>
        private void Button_Save(object sender, RoutedEventArgs e)
        {
            scriviCSV();
            Console.WriteLine("Salvato PROGRAMMI.csv");
            Globals.log.Info("Salvato PROGRAMMI.csv");
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                        INTERAZIONE CON L'UTENTE                            ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Al doppio click sulla riga apre la cartella del filesystem.
        /// </summary>
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("duoble");
            Button_Open_Folder(sender, null);
        }

        /// <summary>
        /// Funzioni per consentire di navigare la DataGrid con le freccie su e giù mentre si effettua una ricerca.
        /// Con invio si apre la cartella del filesystem del programma selezionato
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
        /// - carica docx di anteprima (se disponibile)
        /// NullReferenceException spesso sollevata perchè quando si cerca si ricarica la DataGrid e 
        /// per un istante non c'è nessun programma selezionato. 
        /// </summary>
        private void ChangePreview(object sender, EventArgs e)
        {
            Console.WriteLine("Change Preview");
            Globals.log.Info("Change Preview");
            RichTextBox richTextBox = this.FindName("richTextBox") as RichTextBox;
            Button button = this.FindName("buttonOpenDocx") as Button;
            try
            {
                ProgSelezionato = (((Programma)((DataGrid)sender).SelectedValue).numero);
                if (Globals.ANTEPRIME)
                {
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
                        //button.Visibility = Visibility.Hidden;
                    }
                }
            }
            catch (NullReferenceException nre)
            {
                richTextBox.Visibility = Visibility.Hidden;
                button.Visibility = Visibility.Visible;
                Globals.log.Warn("Eccezione in changePreview: " + nre);
            }
            if (!Globals.ANTEPRIME)
            {
                richTextBox.Visibility = Visibility.Hidden;
                button.Visibility = Visibility.Hidden;
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

        //ESPERIMENTO PER RICONOSCERE QUANDO CAMBIA LO STATO DELLE CHECKBOX - NON MOLTO FUNZIONANTE
        //private void CheckBoxChanged(object sender, RoutedEventArgs e)
        //{
        //    //bool value = ((DataGridCell)sender).Content.ToString().Split(':').Last().Equals("True") ? true : false;
        //    //Globals.PROGRAMMI[ProgSelezionato].obsoleto = value;
        //    MessageBox.Show("scrivo");
        //    scriviCSV();
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////                          MENU DI IMPOSTAZIONI                              ///////////////////               
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Apre il FORM Form_percorsi per modificare i path in cui il programma cerca i file.
        /// </summary>
        private void Menu_percorsi(object sender, RoutedEventArgs e)
        {
            Form_percorsi form = new Form_percorsi();
            form.FormClosed
                    += new System.Windows.Forms.FormClosedEventHandler(this.readAgainListPrograms);
            form.ShowDialog();
        }

        /// <summary>
        /// Imposta la visibilità dell'anteprima del docx.
        /// Aggiorna la variabile Globals.ANTEPRIME e scrive sul .csv.
        /// </summary>
        private void Menu_anteprima(object sender, RoutedEventArgs e)
        {
            bool value = ((MenuItem)sender).IsChecked;
            RichTextBox richTextBox = this.FindName("richTextBox") as RichTextBox;
            Button button = this.FindName("buttonOpenDocx") as Button;
            if (value != Globals.ANTEPRIME)
            {
                Globals.ANTEPRIME = value;
                MainWindow m = new MainWindow();
                m.scriviSETTINGS();
            }
            if (value)
            {
                button.Visibility = Visibility.Visible;
                richTextBox.Visibility = Visibility.Visible;
                DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
                ChangePreview(dataGrid, null);
            }
            else
            {
                richTextBox.Visibility = Visibility.Hidden;
                button.Visibility = Visibility.Hidden;
            }

        }

        /// <summary>
        /// Apre il FORM Form_conversioneDaTXT importare file TXT per TRADEX e renderli in formato .csv.
        /// </summary>
        private void Menu_converti_TXT(object sender, RoutedEventArgs e)
        {
            Form_conversioneDaTXT form = new Form_conversioneDaTXT();
            form.ShowDialog();
        }

        /// <summary>
        /// Se l'utente conferma crea un file programma.docx per ogni programma con i dati del programma.
        /// </summary>
        private void Menu_DOCX(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show("Sei sicuro di voler CREARE un file programma.docx in ogni programma?",
                "Creare TUTTI i DOCX?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            if (dialogResult == MessageBoxResult.Yes)
            {
                foreach (Programma p in Globals.PROGRAMMI)
                {
                    if (p != null)
                    {
                        string file = Globals.PROGRAMMIpath + "Id" + p.numero + @"\programma.docx";
                        if (!File.Exists(file))
                        {
                            try
                            {
                                var doc = Xceed.Words.NET.DocX.Create(file);
                                doc.InsertParagraph("Id" + ProgSelezionato + "  -  " + p.nome).Bold();
                                doc.InsertParagraph("\n DATA CREAZIONE: " + p.dataCreazione);
                                doc.InsertParagraph("\n OBSOLETO: " + p.obsoleto.ToString());
                                doc.InsertParagraph("\n NOME UTENTE: " + p.nomeUtente);
                                doc.InsertParagraph("\n PASSWORD: " + p.password);
                                doc.InsertParagraph("\n DESCRIZIONE: " + p.descrizione);
                                doc.Save();
                                string msg = "Il file " + file + " è stato creato";
                                Globals.log.Info(msg);
                            }
                            catch (IOException)
                            {
                                string msg = "E07 - Il file " + file + " NON è stato creato";
                                //MessageBox.Show(msg, "E07", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                                Globals.log.Info(msg);
                            }
                            DataGrid dataGrid = this.FindName("dataGrid") as DataGrid;
                            ChangePreview(dataGrid, null);
                        }
                    }
                }
            }
        }
    }




}