﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using SieveofEratosthenes.Commands;
using System.Linq;

namespace SieveofEratosthenes.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        #region Commands

        public RelayCommand GeneratePrimeNumbersCommand { get; set; }
        public RelayCommand PreviousPageCommand { get; private set; }
        public RelayCommand NextPageCommand { get; private set; }
        public RelayCommand FirstPageCommand { get; private set; }
        public RelayCommand LastPageCommand { get; private set; }

        #endregion

        #region Constructor

        public HomeViewModel()
        {
            PreviousPageCommand = new RelayCommand(ShowPreviousPage);
            NextPageCommand = new RelayCommand(ShowNextPage);
            FirstPageCommand = new RelayCommand(ShowFirstPage);
            LastPageCommand = new RelayCommand(ShowLastPage);
            GeneratePrimeNumbersCommand = new RelayCommand(GeneratePrimeNumbers);
        }

        #endregion

        #region Public Methods

        #endregion

        #region Properties

        private string _helloMessage = "Hello from VM";
        public string HelloMessage
        {
            get { return _helloMessage; }
            set { 
                _helloMessage = "Hii from command";
                NotifyPropertyChanged("HelloMessage");
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

        public int CurrentPage
        {
            get { return _currentPageIndex + 1; }
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
        private int _itemsPerPage = 100;
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
        }

        private void SetDataPagerButtonVisibility()
        {
            if (CurrentPageIndex == 0)
            {
                FirstPageCommand.SetCanExecute(false);
                PreviousPageCommand.SetCanExecute(false);

                if (TotalPages == CurrentPage)
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
            else if(CurrentPage == TotalPages)
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
                var stopWatch = new Stopwatch();

                stopWatch.Start();

                var upperLimit = Convert.ToInt32(MaxPrimeNumber);

                var indexNumber = 2; //Assumption based on Sieve

                var numberBitArray = new BitArray((int)(upperLimit + 1));

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

                stopWatch.Stop();

                var time = stopWatch.ElapsedMilliseconds;

                NotifyPropertyChanged("ListOfPrimeNumbers");
                
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public void ShowPreviousPage()
        {
            CurrentPageIndex--;
            if (_displayData != null && _displayData.Any())
            {
                _displayData.Clear();

                var showDataFrom = (_currentPageIndex == 0) ? 0 : CurrentPageIndex*_itemsPerPage;
                var showDataTill = (_currentPageIndex == 0)
                                       ? _itemsPerPage - 1
                                       : _itemsPerPage*(CurrentPageIndex + 1) - 1;

                for (int i = showDataFrom; i <= showDataTill; i++)
                {
                    if (_listOfPrimeNumbers.ElementAtOrDefault(i) != 0)
                    {
                        _displayData.Add(_listOfPrimeNumbers[i]);
                    }
                }
                NotifyPropertyChanged("DisplayData");
            }
        }

        public void ShowNextPage()
        {
            CurrentPageIndex++;
            if (_displayData != null && _displayData.Any())
            {
                _displayData.Clear();

                var showDataFrom = CurrentPageIndex*_itemsPerPage;
                var showDataTill = _itemsPerPage*(CurrentPageIndex + 1) - 1;

                for (int i = showDataFrom; i <= showDataTill; i++)
                {
                    if (_listOfPrimeNumbers.ElementAtOrDefault((int) i) != 0)
                    {
                        _displayData.Add(_listOfPrimeNumbers[(int) i]);
                    }
                }
                NotifyPropertyChanged("DisplayData");
            }
        }

        public void ShowFirstPage()
        {
            CurrentPageIndex = 0;
            if(_displayData != null && _displayData.Any())
                _displayData.Clear();

            if(_displayData == null)
                _displayData = new ObservableCollection<int>();

            for (int i = 0; i < _itemsPerPage ; i++)
            {
                if(_listOfPrimeNumbers.ElementAtOrDefault((int) i) != 0)
                {
                    _displayData.Add(_listOfPrimeNumbers[(int) i]);
                }
            }
            NotifyPropertyChanged("DisplayData");
        }

        public void ShowLastPage()
        {
            CurrentPageIndex = TotalPages - 1;
            if (_displayData != null && _displayData.Any())
                _displayData.Clear();

            var showDataFrom = CurrentPageIndex * _itemsPerPage;
            var showDataTill = _itemsPerPage * (CurrentPageIndex + 1) - 1;

            for (int i = showDataFrom; i < showDataTill; i++)
            {
                if (_listOfPrimeNumbers.ElementAtOrDefault((int) i) != 0)
                {
                    _displayData.Add(_listOfPrimeNumbers[(int) i]);
                }
            }
            NotifyPropertyChanged("DisplayData");
        }

        private void CalculateTotalPages()
        {
            if (_listOfPrimeNumbers.Count % _itemsPerPage == 0)
            {
                TotalPages = (_listOfPrimeNumbers.Count / (int) _itemsPerPage);
            }
            else
            {
                TotalPages = (_listOfPrimeNumbers.Count / (int) _itemsPerPage) + 1;
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