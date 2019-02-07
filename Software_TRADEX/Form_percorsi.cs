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
    /// Form per il cambio del percorso delle cartelle PROGRAMMIpath e DATI
    /// - verifica che i percorsi portino a cartelle esistenti
    /// </summary>
    public partial class Form_percorsi : Form
    {
        /// <summary>
        /// Costruttore classico in cui vengono impostati i valori delle textbox
        /// </summary>
        public Form_percorsi()
        {
            InitializeComponent();
            textBox1.Text = Globals.PROGRAMMIpath;
            textBox2.Text = Globals.DATI;
        }

        /// <summary>
        /// Apre la finestra per navigare il filesystem e scrive il risultato nella textbox
        /// Se il percorso termina senza '\', ne aggiunge una (compatibilità col resto del codice)
        /// </summary>
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBox1.Text;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                if (!path[path.Length - 1].ToString().Equals(@"\"))
                {
                    path = path + "\\";
                }
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
        /// Quando viene modificato il testo della textbox verifica se esiste la cartella specificata.
        /// Di conseguenza imposta o no l'icona X
        /// </summary>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBox1.Text.ToString()))
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
        /// Esce dal form senza apportare modifiche
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Da all'utente la possibilità di scegliere se apportare le modifiche o no.
        /// In seguito aggiorna Globals.PROGRAMMIpath e Globals.DATI (se modificati) e riscrive il file SETTINGS.csv
        /// Chiude il form
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            string tb1 = textBox1.Text.ToString();
            string tb2 = textBox2.Text.ToString();
            if (!Directory.Exists(tb1) || !Directory.Exists(tb2))
            {
                string msg = "Uno dei percorsi inseriti non esiste, sei sicuro di voler continuare e salvare le modifiche?";
                System.Windows.MessageBoxResult me = System.Windows.MessageBox.Show(
                    msg, "Percorso inesistente", MessageBoxButton.YesNo,
                    MessageBoxImage.Question, MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
                Globals.log.Warn(msg);
                if (me == MessageBoxResult.No)
                {
                    return;
                }
            }
            if (!tb1.Equals(Globals.PROGRAMMIpath) || !tb2.Equals(Globals.DATI))
            {
                Globals.PROGRAMMIpath = tb1;
                Globals.DATI = tb2;
                MainWindow m = new MainWindow();
                m.scriviSETTINGS();
                Globals.log.Info("Percorsi settings");
            }
            this.Close();
        }
    }
}
