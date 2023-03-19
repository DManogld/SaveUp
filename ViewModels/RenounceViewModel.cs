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

        private void SetupCommands()
        {
            SaveCommand = new AsyncRelayCommand(Save);
            DeleteCommand = new AsyncRelayCommand(Delete);
        }

        private async Task Save()
        {
            _renounce.Date = DateTime.Now;
            _renounce.Save();
            await Shell.Current.GoToAsync($"..?saved={_renounce.Filename}");

        }

        private async Task Delete()
        {
            _renounce.Delete();
            await Shell.Current.GoToAsync($"..?deleted={_renounce.Filename}");
        }


        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("load"))
            {
                _renounce = Model.Renounce.Load(query["load"].ToString());
                RefreshProperties();
            }
        }

        public void Reload()
        {
            _renounce = Model.Renounce.Load(_renounce.Filename);
            RefreshProperties();
        }

        private void RefreshProperties()
        {
            OnPropertyChanged(nameof(Text));
            OnPropertyChanged(nameof(Preis));
            OnPropertyChanged(nameof(Date));
        }



    }
}
