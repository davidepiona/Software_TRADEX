using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Software_TRADEX
{
    public class Programma
    {
        /// <summary>
        /// Classe Progetto composta dagli elementi presenti nei file PROGRAMMI.csv
        /// Getter e Setter classici. 
        /// Sono presenti alcuni elementi aggiuntivi: 
        /// - dataModifica: ultima modifica della cartella relativa al programma; in seguito ad alcune operazioni viene trovata e aggiunta agli oggetti
        /// </summary>
        public int numero { get; set; }
        public string nome { get; set; }
        public string dataCreazione { get; set; }
        public string dataModifica { get; set; }
        public bool obsoleto { get; set; }
        public string nomeUtente { get; set; }
        public string password { get; set; }
        public string descrizione { get; set; }
        public bool presenzaCartella { get; set; }

        /// <summary>
        /// Inizializza gli attributi coi valori ricevuto
        /// </summary>
        public Programma(int numero, string nome, string dataCreazione, string dataModifica, bool obsoleto, string nomeUtente, string password, string descrizione, bool presenzaCartella)
        {
            this.numero = numero;
            this.nome = nome ?? throw new ArgumentNullException(nameof(nome));
            this.dataCreazione = dataCreazione ?? throw new ArgumentNullException(nameof(dataCreazione));
            this.dataModifica = dataModifica ?? throw new ArgumentNullException(nameof(dataModifica));
            this.obsoleto = obsoleto;
            this.nomeUtente = nomeUtente ?? throw new ArgumentNullException(nameof(nomeUtente));
            this.password = password ?? throw new ArgumentNullException(nameof(password));
            this.descrizione = descrizione ?? throw new ArgumentNullException(nameof(descrizione));
            this.presenzaCartella = presenzaCartella;
        }

        /// <summary>
        /// Metodo utilizzato per cercare i programmi nella lista. 
        /// Vengono utilizzate le parole contenute in numero, nome e descrizione.
        /// </summary>
        public string toName()
        {
            return numero + " " + nome + " " + descrizione;
        }
    }
}
