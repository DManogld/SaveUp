using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SaveUp.ViewModels
{
    internal class RenouncesViewModel : IQueryAttributable
    {
        public ObservableCollection<RenounceViewModel> AllRenounce { get; }
        public ICommand NewCommand { get; }
        public ICommand SelectRenounceCommand { get; }

        public RenouncesViewModel()
        {
            AllRenounce = new ObservableCollection<RenounceViewModel>(Model.Renounce.LoadAll().Select(n => new RenounceViewModel(n)));
            NewCommand = new AsyncRelayCommand(NewRenounceAsync);
            SelectRenounceCommand = new AsyncRelayCommand<RenounceViewModel>(SelectRenounceAsync);
        }

        private async Task NewRenounceAsync()
        {
            await Shell.Current.GoToAsync(nameof(View.RenouncePage));
        }

        private async Task SelectRenounceAsync(RenounceViewModel renounce)
        {
            if (renounce != null)
                await Shell.Current.GoToAsync($"{nameof(View.RenouncePage)}?load={renounce.Identifier}");
        }

        void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("deleted"))
            {
                string renounceId = query["deleted"].ToString();
                RenounceViewModel matchedRenounce = AllRenounce.Where((n) => n.Identifier == renounceId).FirstOrDefault();

                // If resource exists, delete it
                if (matchedRenounce != null)
                    AllRenounce.Remove(matchedRenounce);
            }
            else if (query.ContainsKey("saved"))
            {
                string renounceId = query["saved"].ToString();
                RenounceViewModel matchedRenounce = AllRenounce.Where((n) => n.Identifier == renounceId).FirstOrDefault();

                // If resource is found, update it
                if (matchedRenounce != null)
                {
                    matchedRenounce.Reload();
                    AllRenounce.Move(AllRenounce.IndexOf(matchedRenounce), 0);
                }
                // If resource isn't found, it's new; add it.
                else
                    AllRenounce.Insert(0, new RenounceViewModel(Model.Renounce.Load(renounceId)));
            }
        }
    }
}
