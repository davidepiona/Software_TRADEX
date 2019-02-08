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
    /// Form per il cambio del percorso a cui trovare il file SETTINGS.csv (se non si trova nella cartella dell'eseguibile)
    /// - verifica che il percorso porti ad un file esistente
    /// </summary>
    public partial class Form_initialSettings : Form
    {
        /// <summary>
        /// Costruttore classico in cui viene impostato il valore della textbox
        /// </summary>
        public Form_initialSettings()
        {
            InitializeComponent();
            textBox1.Text = Globals.SETTINGS;
        }

        /// <summary>
        /// Quando viene modificato il testo della textbox verifica se esiste il file nel percorso specificato.
        /// Di conseguenza imposta o no l'icona X
        /// </summary>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!File.Exists(textBox1.Text.ToString()))
            {
                pictureBox2.Visible = true;
            }
            else
            {
                pictureBox2.Visible = false;
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
        /// Chiude forzatamente l'applicazione
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Informa l'utente se il percorso impostato porta ad un file esistente o no.
        /// In seguito aggiorna Globals.SETTINGS
        /// Chiude il form
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            string tb1 = textBox1.Text.ToString();

            if (!File.Exists(tb1))
            {
                string msg = "Il percorso impostato per il file 'SETTINGS.csv' non esiste.\n";
                MessageBoxResult me = System.Windows.MessageBox.Show(
                msg,
                "Percorso inesistente",
                MessageBoxButton.OK,
                MessageBoxImage.Error, System.Windows.MessageBoxResult.No, System.Windows.MessageBoxOptions.RightAlign);
                return;
            }
            if (!tb1.Equals(Globals.SETTINGS))
            {
                Globals.SETTINGS = tb1;
            }
            this.Close();
        }

        private void caricaDefault(object sender, EventArgs e)
        {
            Globals.DEFAULT = true;
            this.Close();
        }
    }
}
