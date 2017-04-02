using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using SieveofEratosthenes.Commands;
using System.Linq;

namespace SieveofEratosthenes.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        #region Commands

        public RelayCommand GeneratePrimeNumbersCommand { get; private set; }
        public RelayCommand PreviousPageCommand { get; private set; }
        public RelayCommand NextPageCommand { get; private set; }
        public RelayCommand FirstPageCommand { get; private set; }
        public RelayCommand LastPageCommand { get; private set; }
        public RelayCommand DisplayRequestedPageCommand { get; private set; }
        public RelayCommand ExportToCsvCommand { get; private set; }

        #endregion

        #region Constructor

        public HomeViewModel()
        {
            PreviousPageCommand = new RelayCommand(ShowPreviousPage);
            NextPageCommand = new RelayCommand(ShowNextPage);
            FirstPageCommand = new RelayCommand(ShowFirstPage);
            LastPageCommand = new RelayCommand(ShowLastPage);
            DisplayRequestedPageCommand = new RelayCommand( DisplayUserRequestedPageData);
            GeneratePrimeNumbersCommand = new RelayCommand(GeneratePrimeNumbers);
            ExportToCsvCommand = new RelayCommand(ExportDataToCsv);
        }

        #endregion

        #region Properties

        private bool _isDataVisible;
        public bool IsDataVisible
        {
            get { return _isDataVisible; }
            set { _isDataVisible = value;
            NotifyPropertyChanged("IsDataVisible");
            }
        }

        private ObservableCollection<int> _displayData;
        public ObservableCollection<int> DisplayData
        {
            get { return _displayData; }
            set
            {
                if (_displayData != value)
                {
                    _displayData = value;
                    NotifyPropertyChanged("DisplayData");
                }
            }
        }

        private string _waitMessage;
        public string WaitMessage
        {
            get { return _waitMessage; }
            set
            {
                if (_waitMessage != value)
                {
                    _waitMessage = value;
                    NotifyPropertyChanged("WaitMessage");
                }
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if(_isBusy != value)
                {
                    _isBusy = value;
                    NotifyPropertyChanged("IsBusy");
                }
            }
        }

        private int _currentPageIndex = -1;
        public int CurrentPageIndex
        {
            get { return _currentPageIndex; }
            set
            {
                if (_currentPageIndex != value)
                {
                    _currentPageIndex = value;
                    NotifyPropertyChanged("CurrentPage");

                    SetDataPagerButtonVisibility();
                }
            }
        }

        public int _currentPage = 1;
        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (value > 0 && value <= TotalPages)
                {
                    _currentPage = value;
                    NotifyPropertyChanged("CurrentPage");
                }
            }
        }

        private int _totalPages;
        public int TotalPages
        {
            get { return _totalPages; }
            private set
            {
                _totalPages = value;
                NotifyPropertyChanged("TotalPages");
            }
        }

        private string _maxPrimeNumber;
        public string MaxPrimeNumber
        {
            get { return _maxPrimeNumber; }
            set
            {
                if (_maxPrimeNumber != value)
                {
                    _maxPrimeNumber = value;
                    NotifyPropertyChanged("MaxPrimeNumber");
                }
            }
        }


        #endregion

        #region Private Variables

        private readonly BackgroundWorker _worker = new BackgroundWorker();
        private const int ItemsPerPage = 100;
        private List<int> _listOfPrimeNumbers;

        #endregion

        #region Private Methods

        private void GeneratePrimeNumbers()
        {
            try
            {
                _worker.DoWork += worker_GeneratePrimeNumbers;
                _worker.RunWorkerCompleted += worker_GeneratePrimeNumbersCompleted;
                IsBusy = true;
                WaitMessage = "Please wait while we fetch the primes for you";
                _worker.RunWorkerAsync();
                if (_displayData != null)
                    _displayData.Clear();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private void worker_GeneratePrimeNumbersCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CalculateTotalPages();
            ShowFirstPage();
            SetDataPagerButtonVisibility();
            IsBusy = false;
            IsDataVisible = true;
        }

        private void SetDataPagerButtonVisibility()
        {
            if (CurrentPageIndex == 0)
            {
                FirstPageCommand.SetCanExecute(false);
                PreviousPageCommand.SetCanExecute(false);

                if (TotalPages == CurrentPageIndex + 1)
                {
                    NextPageCommand.SetCanExecute(false);
                    LastPageCommand.SetCanExecute(false);
                }
                else
                {
                    NextPageCommand.SetCanExecute(true);
                    LastPageCommand.SetCanExecute(true);
                }
            }
            else if(CurrentPageIndex + 1 == TotalPages)
            {
                NextPageCommand.SetCanExecute(false);
                LastPageCommand.SetCanExecute(false);

                FirstPageCommand.SetCanExecute(true);
                PreviousPageCommand.SetCanExecute(true);
            }
            else
            {
                NextPageCommand.SetCanExecute(true);
                LastPageCommand.SetCanExecute(true);

                FirstPageCommand.SetCanExecute(true);
                PreviousPageCommand.SetCanExecute(true);
            }
        }

        private void worker_GeneratePrimeNumbers(object sender, DoWorkEventArgs e)
        {
            try
            {
                var upperLimit = Convert.ToInt32(MaxPrimeNumber);

                var indexNumber = 2; //Assumption based on Sieve

                var numberBitArray = new BitArray(upperLimit + 1);

                _listOfPrimeNumbers = new List<int>();

                /* Set all but 0 and 1 to prime status */
                numberBitArray.SetAll(true);
                numberBitArray[0] = numberBitArray[1] = false;

                while (indexNumber <= upperLimit)
                {
                    if (numberBitArray[indexNumber])
                    {
                        _listOfPrimeNumbers.Add(indexNumber);
                        int i = 2;
                        while (indexNumber * i <= upperLimit)
                        {
                            numberBitArray[indexNumber * i] = false;
                            i++;
                        }
                    }
                    indexNumber++;
                }

                NotifyPropertyChanged("ListOfPrimeNumbers");
                
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public void ShowPreviousPage()
        {
            CurrentPage--;
            CurrentPageIndex--;

            if (_displayData != null && _displayData.Any())
            {
                _displayData.Clear();

                var showDataFrom = (_currentPageIndex == 0) ? 0 : CurrentPageIndex*ItemsPerPage;
                var showDataTill = (_currentPageIndex == 0)
                                       ? ItemsPerPage - 1
                                       : ItemsPerPage*(CurrentPageIndex + 1) - 1;

                for (int i = showDataFrom; i <= showDataTill; i++)
                {
                    if (_listOfPrimeNumbers.ElementAtOrDefault(i) != 0)
                    {
                        _displayData.Add(_listOfPrimeNumbers[i]);
                    }
                }
                NotifyPropertyChanged("DisplayData");
            }

            CurrentPage = CurrentPageIndex + 1;
        }

        public void ShowNextPage()
        {
            CurrentPage++;
            CurrentPageIndex++;
            
            if (_displayData != null && _displayData.Any())
            {
                _displayData.Clear();

                var showDataFrom = CurrentPageIndex*ItemsPerPage;
                var showDataTill = ItemsPerPage*(CurrentPageIndex + 1) - 1;

                for (int i = showDataFrom; i <= showDataTill; i++)
                {
                    if (_listOfPrimeNumbers.ElementAtOrDefault(i) != 0)
                    {
                        _displayData.Add(_listOfPrimeNumbers[i]);
                    }
                }
                NotifyPropertyChanged("DisplayData");
            }

            CurrentPage = CurrentPageIndex + 1;
        }

        public void ShowFirstPage()
        {
            CurrentPage = 1;
            CurrentPageIndex = 0;

            if(_displayData != null && _displayData.Any())
                _displayData.Clear();

            if(_displayData == null)
                _displayData = new ObservableCollection<int>();

            for (int i = 0; i < ItemsPerPage ; i++)
            {
                if(_listOfPrimeNumbers.ElementAtOrDefault(i) != 0)
                {
                    _displayData.Add(_listOfPrimeNumbers[i]);
                }
            }
            CurrentPage = CurrentPageIndex + 1;
            NotifyPropertyChanged("DisplayData");
        }

        public void ShowLastPage()
        {
            CurrentPage = TotalPages;
            CurrentPageIndex = TotalPages - 1;
            
            if (_displayData != null && _displayData.Any())
                _displayData.Clear();

            var showDataFrom = CurrentPageIndex * ItemsPerPage;
            var showDataTill = ItemsPerPage * (CurrentPageIndex + 1) - 1;

            for (int i = showDataFrom; i < showDataTill; i++)
            {
                if (_listOfPrimeNumbers.ElementAtOrDefault(i) != 0)
                {
                    _displayData.Add(_listOfPrimeNumbers[i]);
                }
            }
            NotifyPropertyChanged("DisplayData");
            NotifyPropertyChanged("CurrentPageIndex");
        }

        private void  DisplayUserRequestedPageData()
        {
            CurrentPageIndex = CurrentPage - 1;
            if (_displayData != null && _displayData.Any())
                _displayData.Clear();

            var showDataFrom = CurrentPageIndex * ItemsPerPage;
            var showDataTill = ItemsPerPage * (CurrentPageIndex + 1) - 1;

            for (int i = showDataFrom; i <= showDataTill; i++)
            {
                if (_listOfPrimeNumbers != null &&_listOfPrimeNumbers.ElementAtOrDefault(i) != 0)
                {
                    _displayData.Add(_listOfPrimeNumbers[i]);
                }
            }
            NotifyPropertyChanged("DisplayData");
            NotifyPropertyChanged("CurrentPage");

        }

        private void CalculateTotalPages()
        {
            if (_listOfPrimeNumbers.Count % ItemsPerPage == 0)
            {
                TotalPages = (_listOfPrimeNumbers.Count / ItemsPerPage);
            }
            else
            {
                TotalPages = (_listOfPrimeNumbers.Count / ItemsPerPage) + 1;
            }
        }
        
        private void ExportDataToCsv()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog {Filter = "csv files (*.csv)|*.csv"};
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var sw = new StreamWriter(saveFileDialog.FileName)) 
                    {
                        if(_listOfPrimeNumbers != null && _listOfPrimeNumbers.Any())
                        {
                            for (int i = 1; i <= _listOfPrimeNumbers.Count; i++)
                            {
                                if(i % 50 == 0)
                                {
                                    sw.WriteLine(_listOfPrimeNumbers[i - 1].ToString());
                                }
                                else
                                {
                                    sw.Write(_listOfPrimeNumbers[i - 1].ToString());
                                    if (i != _listOfPrimeNumbers.Count)
                                        sw.Write(",");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion

        #region Implement Interface

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }
}
