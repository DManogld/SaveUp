using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SaveUp.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace SaveUp.ViewModels
{
    internal class RenouncesViewModel : ObservableObject, IQueryAttributable
    {
        public ObservableCollection<RenounceViewModel> AllRenounce { get; }
        public ICommand NewCommand { get; }
        public ICommand SelectRenounceCommand { get; }
        public decimal TotalPrice => AllRenounce.Sum(r => decimal.Parse(r.Preis));
        public ICommand Deleteall { get; set; }

        private decimal dailySavings;
        public decimal DailySavings
        {
            get { return dailySavings; }
            set { dailySavings = value; OnPropertyChanged(); }
        }

        // WeeklySavings property
        private decimal weeklySavings;
        public decimal WeeklySavings
        {
            get { return weeklySavings; }
            set { weeklySavings = value; OnPropertyChanged(); }
        }

        // MonthlySavings property
        private decimal monthlySavings;
        public decimal MonthlySavings
        {
            get { return monthlySavings; }
            set { monthlySavings = value; OnPropertyChanged(); }
        }


        public RenouncesViewModel()
        {

            AllRenounce = new ObservableCollection<RenounceViewModel>(Model.Renounce.LoadAll().Select(n => new RenounceViewModel(n)));
            NewCommand = new AsyncRelayCommand(NewRenounceAsync);
            Deleteall = new AsyncRelayCommand(DeleteAll);
            SelectRenounceCommand = new AsyncRelayCommand<RenounceViewModel>(SelectRenounceAsync);

        }

        /// <summary>
        /// Aktualisiert die täglichen, wöchentlichen und monatlichen Einsparungen
        /// basierend auf der Verzichtsliste.
        /// </summary>
        public void UpdateSavings()
        {
            CalculateDailySavings();
            CalculateWeeklySavings();
            CalculateMonthlySavings();
        }

        /// <summary>
        /// Berechnet die tägliche Ersparnis basierend auf dem aktuellen Datum.
        /// </summary>
        private void CalculateDailySavings()
        {
            DateTime today = DateTime.Today;
            decimal dailySavings = AllRenounce.Where(r => r.Date.Date == today).Sum(r => decimal.Parse(r.Preis));
            DailySavings = dailySavings;
        }

        /// <summary>
        ///  Berechnet die wöchentliche Einsparung basierend auf der aktuellen Woche.
        /// </summary>
        private void CalculateWeeklySavings()
        {
            DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            decimal weeklySavings = AllRenounce.Where(r => r.Date.Date >= startOfWeek.Date).Sum(r => decimal.Parse(r.Preis));
            WeeklySavings = weeklySavings;
        }

        /// <summary>
        /// Berechnet die monatliche Ersparnis basierend auf dem aktuellen Monat.
        /// </summary>
        private void CalculateMonthlySavings()
        {
            DateTime startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            decimal monthlySavings = AllRenounce.Where(r => r.Date.Date >= startOfMonth.Date).Sum(r => decimal.Parse(r.Preis));
            MonthlySavings = monthlySavings;
        }

        /// <summary>
        /// Löscht alle Renounce aus der Liste.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAll()
        {
            Renounce renounce = new Renounce();
            bool answer = await Application.Current.MainPage.DisplayAlert("Question?", "Wollen Sie wirklich alle Artikel Löschen?", "Yes", "No");
            if (answer)
            {
                renounce.DeleteAll();
                await UpdateRenouncesList();
            }
        }

        /// <summary>
        /// Navigiert zur neuen RenouncePage.
        /// </summary>
        /// <returns></returns>
        private async Task NewRenounceAsync()
        {
            await Shell.Current.GoToAsync(nameof(View.RenouncePage));
        }

        /// <summary>
        /// Wählt einen bestimmten Renounce aus und navigiert zu seiner Detailseite.
        /// </summary>
        /// <param name="renounce">Die Auserwählten renounce.</param>
        /// <returns></returns>
        private async Task SelectRenounceAsync(RenounceViewModel renounce)
        {
            if (renounce != null)
                await Shell.Current.GoToAsync($"{nameof(View.RenouncePage)}?load={renounce.Identifier}");
        }

        /// <summary>
        /// Aktualisiert die Renounce Liste und den Gesamtpreis und die Spareigenschaften.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateRenouncesList()
        {
            AllRenounce.Clear();
            foreach (var renounce in Model.Renounce.LoadAll())
            {
                if (renounce != null)
                    AllRenounce.Add(new RenounceViewModel(renounce));
            }
            OnPropertyChanged(nameof(AllRenounce));
            OnPropertyChanged(nameof(TotalPrice));
            UpdateSavings();
        }

        /// <summary>
        /// Wendet Abfrageattribute auf die Verzichtsliste an.
        /// </summary>
        /// <param name="query">Ein Wörterbuch mit anzuwendenden Abfrageattributen</param>
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
