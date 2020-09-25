using ReactiveUI;
using ReactiveUIReleaseTest.UWP.VM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ReactiveUIReleaseTest.UWP.Pages
{
    public static class VisibilitylHelper
    {
        public static Visibility ToVisibility(this bool self, bool revers = false)
        {
            if (self != revers)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }
        public static bool ToBool(this Visibility self, bool revers = false)
        {
            if (self == Visibility.Visible)
                return !revers;
            else
                return revers;
        }
    }
    public class TcpClientBaseClass : ReactiveUserControl<TestSocketVM>
    {
    }
    public sealed partial class TestSocketButton : TcpClientBaseClass
    {
        public TestSocketButton()
        {
            this.InitializeComponent();
            ViewModel = new TestSocketVM();
            this.WhenActivated(dis =>
            {
                this.Bind(ViewModel, vm => vm.HostName, v => v.HostName.Text).DisposeWith(dis);
                this.OneWayBind(ViewModel, vm => vm.IsConnect, v => v.Connect2Server.Visibility, x => x.ToVisibility(true)).DisposeWith(dis);
                this.OneWayBind(ViewModel, vm => vm.IsConnect, v => v.Disconnect4Server.Visibility, x => x.ToVisibility()).DisposeWith(dis);
                this.OneWayBind(ViewModel, vm => vm.IsConnect, v => v.HostName.IsEnabled, ic => !ic).DisposeWith(dis);
                this.OneWayBind(ViewModel, vm => vm.IsConnect, v => v.Port.IsEnabled, ic => !ic).DisposeWith(dis);
                this.Bind(ViewModel, vm => vm.TextMessage, v => v.TextMessage.Text).DisposeWith(dis);
                this.Bind(ViewModel, vm => vm.Port, v => v.Port.Text, p => p.ToString(), vp =>
                {
                    uint val = 1000;
                    if (uint.TryParse(vp, out val))
                        return val;
                    return 1000;
                }
                ).DisposeWith(dis);
                this.OneWayBind(ViewModel, vm => vm.InComes, v => v.InComes.ItemsSource);
                this.BindCommand(ViewModel, vm => vm.Connect2Server, v => v.Connect2Server).DisposeWith(dis);
                this.BindCommand(ViewModel, vm => vm.Disconnect4Server, v => v.Disconnect4Server).DisposeWith(dis);
                this.BindCommand(ViewModel, vm => vm.SendMessage, v => v.SendMessageBtn).DisposeWith(dis);

                ViewModel.Connect2Server.SelectMany(res =>
                {
                    var dialog = res ?
                        new MessageDialog("Connetion Successful", "We Connect to Server!") :
                        new MessageDialog("Connetion Failed", "Ah, ah, ah, you didn't say the magic word!");

                    return dialog.ShowAsync().ToObservable();
                }).Subscribe().DisposeWith(dis);
                ViewModel.Disconnect4Server.SelectMany(res => new MessageDialog("Disconnected", "Yes,Yeah, now you can connect new server!").ShowAsync().ToObservable()).Subscribe().DisposeWith(dis);
            });
        }
        private void Clean_Click(object sender, RoutedEventArgs e) => TextMessage.Text = "";
    }
}
