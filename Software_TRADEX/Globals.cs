using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Software_TRADEX
{
    /// <summary>
    /// Classe statica che contiene oggetti accessibili da tutte le altre classi
    /// </summary>
    public static class Globals
    {
        public static List<Programma> PROGRAMMI;
        public static String PROGRAMMIpath = @"R:\TRADE\SOFTWARE_AZIENDALE\";
        public static String SETTINGS = @"R:\TRADE\TRADE\SETTINGS.csv";
        public static String DATI = @"R:\TRADE\TRADE\DATI\";
        public static String LOG = @"R:\TRADE\TRADE\";
        public static bool ANTEPRIME = true;
        public static bool DEFAULT = false;
        public static log4net.ILog log;

        //public const Int32 BUFFER_SIZE = 512; // Unmodifiable
        //public static Boolean IS_DATA_DIRTY = false; // Modifiable
        //public static String LAST_PROGRAM; // Modifiable
        //public static List<Clienti> CLIENTI;

        //public static String GITURL = "https://github.com/davidepiona/DATIsync.git";
        //public static String GITPATH = @"C:\Program Files\Git\cmd\git.exe";
        //public static bool SINCRONIZZAZIONE = true;
        //public static String DATIsync = @"C:\Users\attil\source\repos\Software_TRADEX\Software_TRADEX\DATIsync\";
        //public static readonly String DATIsync = @"C:\Users\attil\source\repos\ExpenseIt\ExpenseIt\DATIsync";// Unmodifiable
    }
}
