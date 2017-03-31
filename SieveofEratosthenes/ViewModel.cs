using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SieveofEratosthenes
{
    public class ViewModel
    {
        #region Private Variables

        private bool canExecute = true;

        #endregion

        #region Private Methods

        public void Test(object obj)
        {
            HelloMessage = "Hiii from command binding";
        }

        #endregion

        #region Properties

        public bool CanExecute
        {
            get
            {
                return this.canExecute;
            }

            set
            {
                if (this.canExecute == value)
                {
                    return;
                }

                this.canExecute = value;
            }
        }

        private string _helloMessage = "Hello from VM";
        public string HelloMessage
        {
            get { return _helloMessage; }
            set { _helloMessage = "Hii from command";}
        }

        #endregion

        #region Public Methods

        #endregion

        //#region Commands

        //public RelayCommand TestButtonCommand { get; set; }

        //#endregion

        //#region Constructor

        //public ViewModel()
        //{
        //    TestButtonCommand = new RelayCommand(Test, param => this.CanExecute);
        //}

        //#endregion
    }

    //public class RelayCommand : ICommand
    //{
    //    private Action<object> execute;

    //    private Predicate<object> canExecute;

    //    private event EventHandler CanExecuteChangedInternal;

    //    public RelayCommand(Action<object> execute)
    //        : this(execute, DefaultCanExecute)
    //    {
    //    }

    //    public RelayCommand(Action<object> execute, Predicate<object> canExecute)
    //    {
    //        if (execute == null)
    //        {
    //            throw new ArgumentNullException("execute");
    //        }

    //        if (canExecute == null)
    //        {
    //            throw new ArgumentNullException("canExecute");
    //        }

    //        this.execute = execute;
    //        this.canExecute = canExecute;
    //    }

    //    public event EventHandler CanExecuteChanged
    //    {
    //        add
    //        {
    //            CommandManager.RequerySuggested += value;
    //            this.CanExecuteChangedInternal += value;
    //        }

    //        remove
    //        {
    //            CommandManager.RequerySuggested -= value;
    //            this.CanExecuteChangedInternal -= value;
    //        }
    //    }

    //    public bool CanExecute(object parameter)
    //    {
    //        return this.canExecute != null && this.canExecute(parameter);
    //    }

    //    public void Execute(object parameter)
    //    {
    //        this.execute(parameter);
    //    }

    //    public void OnCanExecuteChanged()
    //    {
    //        EventHandler handler = this.CanExecuteChangedInternal;
    //        if (handler != null)
    //        {
    //            handler.Invoke(this, EventArgs.Empty);
    //        }
    //    }

    //    public void Destroy()
    //    {
    //        this.canExecute = _ => false;
    //        this.execute = _ => { return; };
    //    }

    //    private static bool DefaultCanExecute(object parameter)
    //    {
    //        return true;
    //    }
    //}
}
