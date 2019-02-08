using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Software_TRADEX
{
    /// <summary>
    /// Form che permette di selezionare le cartelle di origine e destinazione del processo di 
    /// conversione dal file TXT usato dalla vecchia versione di TRADEX al file csv usato nella nuova versione
    /// </summary>
    public partial class Form_conversioneDaTXT : Form
    {
        /// <summary>
        /// Costruttore classico in cui vengono impostati i valori delle textbox
        /// </summary>
        public Form_conversioneDaTXT()
        {
            InitializeComponent();
            textBox1.Text = @"T:\PROGETTI\TRADEX\BASE_DATI\tradex.txt";
            textBox2.Text = Globals.DATI;
        }

        /// <summary>
        /// Quando viene modificato il testo della textbox verifica se esiste il file nel percorso specificato.
        /// Di conseguenza imposta o no l'icona X
        /// </summary>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!File.Exists(textBox1.Text.ToString()))
            {
                pictureBox4.Visible = true;
            }
            else
            {
                pictureBox4.Visible = false;
            }
        }

        /// <summary>
        /// Quando viene modificato il testo della textbox verifica se esiste la cartella specificata.
        /// Di conseguenza imposta o no l'icona X
        /// </summary>
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBox2.Text.ToString()))
            {
                pictureBox5.Visible = true;
            }
            else
            {
                pictureBox5.Visible = false;
            }
        }

        /// <summary>
        /// Apre la finestra per navigare il filesystem e scrive il risultato nella textbox
        /// </summary>
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = textBox1.Text;
            openFileDialog1.RestoreDirectory = false;
            openFileDialog1.FileName = textBox1.Text.Split('\\').Last();
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = openFileDialog1.FileName;
                textBox1.Text = path;
            }
        }

        /// <summary>
        /// Apre la finestra per navigare il filesystem e scrive il risultato nella textbox
        /// Se il percorso termina senza '\', ne aggiunge una (compatibilità col resto del codice)
        /// </summary>
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBox2.Text;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                if (!path[path.Length - 1].ToString().Equals(@"\"))
                {
                    path = path + "\\";
                }
                textBox2.Text = path;
            }
        }

        /// <summary>
        /// Esce dal form senza apportare modifiche
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Da all'utente la possibilità di scegliere se effettuare l'importazione o no.
        /// Se i percorsi immessi portano a cartelle esistenti effettua la conversione.
        /// A conversione terminata un MessageBox visualizza l'esito ottenuto.
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            string originFile = textBox1.Text.ToString();
            string tb2 = textBox2.Text.ToString();
            string destFile = tb2 + "PROGRAMMI.csv";
            if (!File.Exists(originFile) || !Directory.Exists(tb2))
            {
                string msg = "Uno dei percorsi inseriti non esiste, impossibile eseguire l'operazione.";
                System.Windows.MessageBoxResult me = System.Windows.MessageBox.Show(
                    msg, "Percorso inesistente", MessageBoxButton.OK,
                    MessageBoxImage.Question, MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
                Globals.log.Warn(msg);
                return;
            }
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
                System.Windows.MessageBox.Show("Convertito TXT da " + originFile + " a " + destFile, "TXT convertito", MessageBoxButton.OK,
                                MessageBoxImage.Information, MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
                Globals.log.Info("Convertito TXT da " + originFile + " a " + destFile);
            }
            catch (IOException)
            {
                string msg = "E18 - errore nella conversione file";
                System.Windows.MessageBox.Show(msg, "E18", MessageBoxButton.OK,
                                MessageBoxImage.Error, MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
            this.Close();
        }
    }

}
