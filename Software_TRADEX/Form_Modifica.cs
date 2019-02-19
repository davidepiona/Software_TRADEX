using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Software_TRADEX
{
    /// <summary>
    /// Form per la modifica di un programma esistente
    /// - verifica che siano immessi dati adeguati
    /// - aggiunge il programma alla lista e al file PROGRAMMI.csv
    /// - aggiorna o crea il file .docx
    /// </summary>
    public partial class Form_Modifica : Form
    {
        private Programma prog;

        /// <summary>
        /// Costruttore a cui viene passato  il programma da modificare
        /// Visualizza i dati relativi a quel programma
        /// </summary>
        public Form_Modifica(Programma prog)
        {
            this.prog = prog;
            InitializeComponent();
            label5.Text = "Id" + prog.numero;
            textBox1.Text = prog.nome;
            textBox2.Text = prog.descrizione;
            textBox3.Text = prog.nomeUtente;
            textBox4.Text = prog.password;
        }

        /// <summary>
        /// Esce dal form senza apportare modifiche
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Metodo che apporta le modifiche all'interno di un programma.
        /// - se ci sono stringhe vuote le sostituisce con "."
        /// - sostituisce il programma in Globals.PROGRAMMI
        /// - Appende le modifiche in fondo al file di word (o lo crea se non esiste)
        /// - riscrive il file PROGRAMMI.csv
        /// Chiude il form
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            prog.nome = textBox1.Text.ToString();
            prog.descrizione = textBox2.Text.ToString();
            prog.nomeUtente = textBox3.Text.ToString();
            prog.password = textBox4.Text.ToString();
            if (prog.nome.Equals(""))
                prog.nome = ".";
            if (prog.descrizione.Equals(""))
                prog.descrizione = ".";
            if (prog.nomeUtente.Equals(""))
                prog.nomeUtente = ".";
            if (prog.password.Equals(""))
                prog.password = ".";
            prog.dataModifica = (DateTime.Now).ToString();
            Globals.PROGRAMMI[Globals.PROGRAMMI.FindIndex(r => r.numero == prog.numero)] = prog;
            string msg = "Aggiornato il programma con indice " + prog.numero;
            Console.WriteLine(msg);
            Globals.log.Info(msg);
            string fileName = Globals.PROGRAMMIpath + @"\" + "Id" + prog.numero + @"\programma.docx";
            try
            {
                if (!File.Exists(fileName))
                {
                    var doc = Xceed.Words.NET.DocX.Create(fileName);
                    doc.InsertParagraph("\nId" + prog.numero + "  -  " + prog.nome).Bold();
                    doc.InsertParagraph("\n DATA CREAZIONE: " + prog.dataCreazione);
                    doc.InsertParagraph("\n NOME UTENTE: " + prog.nomeUtente);
                    doc.InsertParagraph("\n PASSWORD: " + prog.password);
                    doc.InsertParagraph("\n DESCRIZIONE: " + prog.descrizione);
                    doc.Save();
                }
                else
                {
                    var doc = Xceed.Words.NET.DocX.Load(fileName);
                    doc.InsertParagraph("Aggiornamento del " + prog.dataModifica).Bold();
                    doc.InsertParagraph("\nId" + prog.numero + "  -  " + prog.nome).Bold();
                    doc.InsertParagraph("\n DATA CREAZIONE: " + prog.dataCreazione);
                    doc.InsertParagraph("\n NOME UTENTE: " + prog.nomeUtente);
                    doc.InsertParagraph("\n PASSWORD: " + prog.password);
                    doc.InsertParagraph("\n DESCRIZIONE: " + prog.descrizione);
                    doc.Save();
                }
            }
            catch (IOException)
            {
                string msg2 = "E31 - Il file " + fileName + " non è stato modificato per un problema";
                MessageBox.Show(msg2, "E31", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg2);
            }
            if (scriviCSV())
            {
                Globals.log.Info("Programma " + prog.numero + " modificato correttamente");
                System.Windows.MessageBox.Show("Programma " + prog.numero + "modificato correttamente", "Modificato", System.Windows.MessageBoxButton.OK,
                       System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
            }
            else
            {
                Globals.log.Error("Programma " + prog.numero + " NON modificato");
                System.Windows.MessageBox.Show("Programma " + prog.numero + " NON modificato", "NON modificato", System.Windows.MessageBoxButton.OK,
                       System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
            }
            this.Close();

        }

        /// <summary>
        /// Metodo per l'eliminazione del programma selezionato
        /// - chiede all'utente conferma dell'operazione
        /// - rimuove il programma da Globals.PROGRAMMI
        /// - riscrive il file PROGRAMMI.csv
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            System.Windows.MessageBoxResult dialogResult = System.Windows.MessageBox.Show("Sei sicuro di voler ELIMINARE il programma " +
                prog.nome + " con codice Id" + prog.numero + "?",
               "Conferma Eliminazione", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.RightAlign);
            if (dialogResult == System.Windows.MessageBoxResult.Yes)
            {
                if (prog.numero != 0)
                {
                    Console.WriteLine("Rimozione programma " + prog.numero);
                    Globals.log.Info("Rimozione programma " + prog.numero);
                    Globals.PROGRAMMI.Remove(prog);
                    if (scriviCSV())
                    {
                        Globals.log.Info("Programma eliminato");
                        System.Windows.MessageBox.Show("Programma eliminato", "Eliminato", System.Windows.MessageBoxButton.OK,
                               System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
                    }
                    else
                    {
                        Globals.log.Error("Programma NON eliminato");
                        System.Windows.MessageBox.Show("Programma NON eliminato", "NON Eliminato", System.Windows.MessageBoxButton.OK,
                               System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
                    }
                }
            }
            this.Close();
        }

        /// <summary>
        /// Metodo per la riscrittura di Globals.PROGRAMMI nel file PROGRAMMI.csv
        /// </summary>
        private bool scriviCSV()
        {
            List<string> lines = new List<string>();
            int i = 0;
            foreach (Programma p in Globals.PROGRAMMI)
            {
                lines.Add(p.numero + "," + p.nome + "," + p.dataCreazione + "," + p.dataModifica
                    + "," + p.obsoleto + "," + p.nomeUtente + "," + p.password + "," + p.descrizione + "," + p.presenzaCartella);
                i++;
            }
            try
            {
                File.WriteAllLines(Globals.DATI + "PROGRAMMI.csv", lines);
            }
            catch (IOException)
            {
                string msg = "E32 - errore nella scrittura del file";
                System.Windows.MessageBox.Show(msg, "E32", System.Windows.MessageBoxButton.OK,
                                System.Windows.MessageBoxImage.Error, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Metodo che impedisce all'utente di inserire caratteri "nuova linea" 
        /// che creerebbero problemi nel filce .csv 
        /// </summary>
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBox2.Text[textBox2.Text.Length - 1].ToString().Equals("\r") ||
                    textBox2.Text[textBox2.Text.Length - 1].ToString().Equals("\r\n") ||
                    textBox2.Text[textBox2.Text.Length - 1].ToString().Equals("\n"))
                {
                    textBox2.Text = textBox2.Text.Remove(textBox2.Text.Length - 1);
                    textBox2.SelectionStart = textBox2.Text.Length + 1;
                }
            }
            catch (IndexOutOfRangeException)
            {
                //la stringa è vuota, no problem
            }
        }
    }
}
