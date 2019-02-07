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
    /// Form per la creazione di un nuovo programma
    /// - verifica che siano immessi dati adeguati
    /// - aggiunge il programma alla lista e al file PROGRAMMI.csv
    /// - crea la cartella e il file .docx
    /// </summary>
    public partial class Form_NuovoProgramma : Form
    {

        private int ultimoProgramma;
        /// <summary>
        /// Costruttore a cui viene passato l'indice dell'ultimo programma in lista
        /// </summary>
        /// <param name="ultimoProgramma"></param>
        public Form_NuovoProgramma(int ultimoProgramma)
        {
            this.ultimoProgramma = ultimoProgramma;
            InitializeComponent();
        }

        /// <summary>
        /// Esce dal form senza apportare modifiche
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Metodo che prova a creare un nuovo programma.
        /// - se ci sono stringhe vuote le sostituisce con "."
        /// - aggiunge il programma al file PROGRAMMI.csv
        /// - crea la cartella per i file (se non esiste)
        /// Chiude il form
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            string nome = textBox1.Text.ToString();
            string descrizione = textBox2.Text.ToString();
            string nomeUtente = textBox3.Text.ToString();
            string password = textBox4.Text.ToString();
            if (nome.Equals(""))
                nome = ".";
            if (descrizione.Equals(""))
                descrizione = ".";
            if (nomeUtente.Equals(""))
                nomeUtente = ".";
            if (password.Equals(""))
                password = ".";
            string data = (DateTime.Now).ToString();
            int i = 0;
            int num = ultimoProgramma + 1;
            string file = Globals.DATI + "PROGRAMMI.csv";
            try
            {
                string programDetails = num + "," +
                                        nome + "," +
                                        data + "," +
                                        "." + "," +
                                        "False" + "," +
                                        nomeUtente + "," +
                                        password + "," +
                                        descrizione + Environment.NewLine;
                Console.WriteLine("Aggiugo il programma: "+programDetails);
                File.AppendAllText(file, programDetails);
                string msg = "Nuovo programma; avrà l'indice: " + num;
                Console.WriteLine(msg);
                Globals.log.Info(msg);
                try
                {
                    Directory.CreateDirectory(Globals.PROGRAMMIpath + @"\" + "Id" + num);
                }
                catch (IOException)
                {
                    string msg2 = "E15 - La cartella " + Globals.PROGRAMMIpath + @"\" + "Id" + num + " non è stata creata per un problema";
                    MessageBox.Show(msg2, "E15"
                                         , MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                    Globals.log.Error(msg2);
                }
                string fileName = Globals.PROGRAMMIpath + @"\" + "Id" + num + @"\programma.docx";
                try
                {
                    var doc = Xceed.Words.NET.DocX.Create(fileName);
                    doc.InsertParagraph("Id" + num + "  -  " + nome).Bold();
                    doc.InsertParagraph("\n DATA CREAZIONE: " + data);
                    doc.InsertParagraph("\n NOME UTENTE: " + nomeUtente);
                    doc.InsertParagraph("\n PASSWORD: " + password);
                    doc.InsertParagraph("\n DESCRIZIONE: " + descrizione);
                    doc.Save();
                }
                catch (IOException)
                {
                    string msg2 = "E16 - Il file " + fileName + " non è stato creato per un problema";
                    MessageBox.Show(msg2, "E16", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                    Globals.log.Error(msg2);
                }
                Globals.log.Info("Aggiunto programma");
                this.Close();
            }
            catch (IOException)
            {
                string msg = "E17 - Il file " + file + " non esiste o è aperto da un altro programma";
                MessageBox.Show(msg, "E17", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
        }
    }
}
