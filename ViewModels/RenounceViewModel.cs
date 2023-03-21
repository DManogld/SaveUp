using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace SaveUp.ViewModels
{
    internal class RenounceViewModel : ObservableObject, IQueryAttributable
    {
        public RenounceViewModel()
        {
            _renounce = new Model.Renounce();
            SetupCommands();
        }
        public RenounceViewModel(Model.Renounce resnounce)
        {
            _renounce = resnounce;
            SetupCommands();
        }


        private Model.Renounce _renounce;
        public string Text
        {
            get => _renounce.Text;
            set
            {
                if (_renounce.Text != value)
                {
                    _renounce.Text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }

        public string Preis
        {
            get => _renounce.Preis;
            set
            {
                if (_renounce.Preis != value)
                {
                    _renounce.Preis = value;
                    OnPropertyChanged(nameof(Preis));
                }
            }
        }

        public DateTime Date => _renounce.Date;
        public string Identifier => _renounce.Filename;
        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }


        /// <summary>
        /// SetupCommands-Methode wird verwendet,um neue AsyncRelayCommand-Objekte zu initialisieren,
        /// die Save- und Delete-Methoden ausführen.
        /// </summary>
        private void SetupCommands()
        {
            SaveCommand = new AsyncRelayCommand(Save);
            DeleteCommand = new AsyncRelayCommand(Delete);
        }

        /// <summary>
        /// Save-Methode aktualisiert das Datum des _renounce-Objekts, speichert es und navigiert 
        /// zur vorherigen Seite mit einem Query-Parameter, der das Dateinamen angibt.
        /// </summary>
        /// <returns></returns>
        private async Task Save()
        {
            _renounce.Date = DateTime.Now;
            _renounce.Save();
            await Shell.Current.GoToAsync($"..?saved={_renounce.Filename}");

        }

        /// <summary>
        /// Delete-Methode löscht das _renounce-Objekt und navigiert zur vorherigen Seite
        /// mit einem Query-Parameter, der das Dateinamen angibt.
        /// </summary>
        /// <returns></returns>
        private async Task Delete()
        {
            _renounce.Delete();
            await Shell.Current.GoToAsync($"..?deleted={_renounce.Filename}");
        }


        /// <summary>
        /// ApplyQueryAttributes-Methode prüft, ob der "load"-Query-Parameter im übergebenen "query" -Wörterbuch vorhanden ist
        /// und lädt die entsprechende Ressource, um die Anzeige zu aktualisieren.
        /// </summary>
        /// <param name="query"></param>
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("load"))
            {
                _renounce = Model.Renounce.Load(query["load"].ToString());
                RefreshProperties();
            }
        }

        /// <summary>
        /// Reload-Methode lädt die Daten des _renounce-Objekts neu und aktualisiert die angezeigten Eigenschaften.
        /// </summary>
        public void Reload()
        {
            _renounce = Model.Renounce.Load(_renounce.Filename);
            RefreshProperties();
        }


        /// <summary>
        /// RefreshProperties-Methode aktualisiert alle anzuzeigenden Eigenschaften des _renounce-Objekts.
        /// </summary>
        private void RefreshProperties()
        {
            OnPropertyChanged(nameof(Text));
            OnPropertyChanged(nameof(Preis));
            OnPropertyChanged(nameof(Date));
        }
    }
}
