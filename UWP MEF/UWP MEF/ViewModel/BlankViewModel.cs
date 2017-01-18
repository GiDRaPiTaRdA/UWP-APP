using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Prism.Commands;
using PropertyChanged;
using UWP_MEF;
using UWP_MEF.View.SubPages;

namespace UWP_MEF.ViewModel
{
    
    public sealed class BlankViewModel:INotifyPropertyChanged
    {
        #region Variables
        private Frame rootFrame;

        private Hashtable frames;
        #endregion

        #region Properties
        // ReSharper disable once MemberCanBePrivate.Global
        public DelegateCommand Navigate { get; set; }
        // ReSharper disable once MemberCanBePrivate.Global
        public DelegateCommand GoForwardCommand { get; set; }
        // ReSharper disable once MemberCanBePrivate.Global
        public DelegateCommand GoBackCommand { get; set; }
        // ReSharper disable once MemberCanBePrivate.Global
        public DelegateCommand ToggleHamburgerMenuCommand { get; set; }
        // ReSharper disable once MemberCanBePrivate.Global
        public DelegateCommand<string> SelectHamburgerItemCommand { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public bool IsOpenHamburgerMenu { get; set; } = true;
        // ReSharper disable once MemberCanBePrivate.Global

        public Type FrameView { get; set; }
        #endregion

        #region Initialization
        public BlankViewModel()
        {
            this.Initialize();
        }
        private void Initialize()
        {
            this.rootFrame = Window.Current.Content as Frame;
            this.Navigate = new DelegateCommand(() => this.rootFrame.Navigate(typeof(MainPage)));
            this.frames = new Hashtable()
            {
                { "Frame 1", typeof(BlankPageFrame1) },
                { "Frame 2", typeof(BlankPageFrame2) },
                { "Frame 3", typeof(BlankPageFrame3) }
            };
            this.GoForwardCommand = new DelegateCommand(this.GoForward, () => this.rootFrame.CanGoForward);
            this.GoBackCommand = new DelegateCommand(this.GoBack);
            this.ToggleHamburgerMenuCommand = new DelegateCommand(this.ToggleHamburgerMenu);
            this.SelectHamburgerItemCommand =  new DelegateCommand<string>(this.SelectHamburgerItem);

            this.Subscribe();   // subscribe on navigated event

            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible; //backbutton
        }
        #endregion

        #region Binding methods
        private void GoForward()
        {
            if (this.rootFrame?.CanGoForward ?? false)
                this.rootFrame.GoForward();
        }
        private async void GoBack()
        {
            if (this.rootFrame?.CanGoBack ?? false)
                this.rootFrame.GoBack();
            else
            {
                if (await YesNoAsyncDialog("Do you want to leave the application?", "Leaving the app"))
                    Application.Current.Exit();
            }
        }
        private void ToggleHamburgerMenu()
        {
            this.IsOpenHamburgerMenu = !this.IsOpenHamburgerMenu;
            this.OnPropertyChanged(nameof(this.IsOpenHamburgerMenu));
        }
        private void SelectHamburgerItem(string key)
        {
            this.FrameView  = (Type)this.frames[key];
            this.OnPropertyChanged(nameof(this.FrameView));
        }
        #endregion

        #region Help methods
        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            this.GoForwardCommand.RaiseCanExecuteChanged();
        }
        private void Subscribe()=>
            this.rootFrame.Navigated += this.OnNavigated;
        private static async Task<bool> YesNoAsyncDialog(string message, string title)
        {
            var dialog = new MessageDialog(message, title);
            dialog.Commands.Add(new UICommand("Yes") { Id = 0 });
            dialog.Commands.Add(new UICommand("No") { Id = 1 });
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;
            var dialogResult = await dialog.ShowAsync();
            if ((int)dialogResult.Id == 0)
                return true;
            else
                return false;
        }
        #endregion

        #region INotify implemented
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}