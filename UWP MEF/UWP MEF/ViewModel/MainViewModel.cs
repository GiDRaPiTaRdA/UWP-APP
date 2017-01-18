using System;
using System.Threading.Tasks;
using Windows.Graphics.Printing3D;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Prism.Commands;
using UWP_MEF.View;

namespace UWP_MEF.ViewModel
{
    public class MainViewModel
    {
        Frame rootFrame;

        // ReSharper disable once MemberCanBePrivate.Global
        public DelegateCommand Navigate { get; set; }
        // ReSharper disable once MemberCanBePrivate.Global
        public DelegateCommand GoForwardCommand { get; set; }
        // ReSharper disable once MemberCanBePrivate.Global
        public DelegateCommand GoBackCommand { get; set; }

        public MainViewModel()
        {
            this.Initialize();
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            this.GoForwardCommand.RaiseCanExecuteChanged();
        }
        private void GoForward()
        {
            if(this.rootFrame?.CanGoForward??false)
                this.rootFrame.GoForward();
        }
        private async void GoBack()
        {
            if (this.rootFrame?.CanGoBack ?? false)
                this.rootFrame.GoBack();
            else
            {
                if (await this.YesNoAsyncDialog("Do you want to leave the application?", "Leaving the app"))
                    Application.Current.Exit();
            }
        }
        private void Initialize()
        {
            this.rootFrame = Window.Current.Content as Frame;
            this.Navigate = new DelegateCommand(() => this.rootFrame.Navigate(typeof(BlankPage1)));
            this.GoForwardCommand = new DelegateCommand(this.GoForward, () => this.rootFrame.CanGoForward);
            this.GoBackCommand = new DelegateCommand(this.GoBack);
            this.Subscribe();
        }
        private void Subscribe()
        {
            this.rootFrame.Navigated += this.OnNavigated;
        }
        private async Task<bool> YesNoAsyncDialog(string message, string title)
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


    }
}