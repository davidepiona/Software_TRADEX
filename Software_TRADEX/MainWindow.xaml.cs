﻿using System;
using System.Collections.Generic;
using System.IO;
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

namespace Software_TRADEX
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            if (Globals.PROGRAMMI == null)
            {
                ShowsNavigationUI = false;
                string set = Globals.SETTINGS;
                leggiSETTINGS(null, null);
                log4net.GlobalContext.Properties["LogFileName"] = Globals.LOG + @"\TRADE.log";
                Globals.log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                Globals.log.Info("Settings lette da: " + set);
                InitializeComponent();
            }
        }

        /// <summary>
        /// Scrive il file SETTINGS.csv
        /// </summary>
        public void scriviSETTINGS()
        {
            Console.WriteLine("Scrivo SETTINGS");
            Globals.log.Info("Scrivo SETTINGS");
            string[] lines = new string[4];
            lines[0] = Globals.PROGRAMMIpath;
            lines[1] = Globals.SETTINGS;
            lines[2] = Globals.DATI;
            lines[3] = Globals.ANTEPRIME.ToString();
            try
            {
                File.WriteAllLines(Globals.SETTINGS, lines);
            }
            catch (IOException)
            {
                string msg = "E00 - Il file " + Globals.SETTINGS + " non esiste o è aperto da un altro programma." +
                " \n\nNon è possibile salvare le nuove preferenze.";
                MessageBox.Show(msg, "E00 File bloccato", MessageBoxButton.OK,
                                MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                Globals.log.Error(msg);
            }
        }

        /// <summary>
        /// Lettura di SETTINGS.csv
        /// Se non è possibile aprire il file si apre il Form_initialSettings
        /// Se ci sono errori nel formato del file si utilizzano le impostazioni di defaut
        /// No logging perchè viene effettuato prima di aver impostato il logger
        /// </summary>
        public void leggiSETTINGS(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            Console.WriteLine("Leggo SETTINGS da "+ Globals.SETTINGS);
            if (!Globals.DEFAULT)
            {
                try
                {
                    var file = File.OpenRead(Globals.SETTINGS);
                    var reader = new StreamReader(file);
                    Globals.PROGRAMMIpath = reader.ReadLine();
                    Globals.SETTINGS = reader.ReadLine();
                    Globals.DATI = reader.ReadLine();
                    Globals.ANTEPRIME = reader.ReadLine().Equals("True") ? true : false;
                    file.Close();
                }
                catch (IOException)
                {
                    MessageBox.Show("E00 - non è stato possibile aprire il file " + Globals.SETTINGS +
                        " ", "E00"
                                         , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                }
                catch (NullReferenceException)
                {
                    MessageBox.Show("E36 - Il file " + Globals.SETTINGS +
                        " non è nel formato richiesto. \nVerranno caricate alcune impostazioni di base ma la funzionalità non è garantita.", "E36"
                        , MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
                }
            }
        }
    }
}
