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
        /// Classe Progetto composta dagli elementi presenti nei file *CLIENTE*.csv
        /// Getter e Setter classici. 
        /// Sono presenti alcuni elementi aggiuntivi: 
        /// - modifica: ultima modifica della cartella relativa al progetto; in seguito ad alcune operazioni viene trovata e aggiunta agli oggetti
        /// - sync: valore che indica lo stato di aggiornamento tra copia locale e copia in DATIsync
        /// - sigla: necessario per aver la prima colonna della tabella compatta (concatenazione di suffisso e numero)
        /// </summary>
        public int numero { get; set; }
        public string nome { get; set; }
        public string dataCreazione { get; set; }
        public string dataModifica { get; set; }
        public bool obsoleto { get; set; }
        public string nomeUtente { get; set; }
        public string password { get; set; }
        public string descrizione { get; set; }

        /// <summary>
        /// Inizializza gli attributi coi valori ricevuto e inizializza dataModifica a null
        /// </summary>
        public Programma(int numero, string nome, string dataCreazione, string dataModifica, bool obsoleto, string nomeUtente, string password, string descrizione)
        {
            this.numero = numero;
            this.nome = nome ?? throw new ArgumentNullException(nameof(nome));
            this.dataCreazione = dataCreazione ?? throw new ArgumentNullException(nameof(dataCreazione));
            this.dataModifica = dataModifica ?? throw new ArgumentNullException(nameof(dataModifica));
            this.obsoleto = obsoleto;
            this.nomeUtente = nomeUtente ?? throw new ArgumentNullException(nameof(nomeUtente));
            this.password = password ?? throw new ArgumentNullException(nameof(password));
            this.descrizione = descrizione ?? throw new ArgumentNullException(nameof(descrizione));
        }

        public string toName()
        {
            return numero + " " + nome + " " + "descrizione";
        }
    }
}
