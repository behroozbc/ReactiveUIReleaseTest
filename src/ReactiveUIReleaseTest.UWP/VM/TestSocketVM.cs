using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ReactiveUIReleaseTest.UWP.VM
{
    public class TestSocketVM : ReactiveObject
    {
        private string _hostName;
        private uint _port;
        private bool _isConnect = false;
        private readonly ApplicationDataContainer _localSettings;
        private string _textMessage = "";
        private IObservableCollection<ClientInComeMessage> _inComes;

        public TestSocketVM()
        {
            _localSettings = ApplicationData.Current.LocalSettings;
            _inComes = new ObservableCollectionExtended<ClientInComeMessage>();
            var canConnect = this.WhenAnyValue(vm => vm.HostName, vm => vm.Port, vm => vm.IsConnect,
                                               (hostname, port, isconnect) => !string.IsNullOrWhiteSpace(hostname) && port > 1000 && !isconnect);
            var canSendMessage = this.WhenAnyValue(vm => vm.TextMessage, vm => vm.IsConnect, (t, isconnect) => !string.IsNullOrWhiteSpace(t) && isconnect);
            Connect2Server = ReactiveCommand.CreateFromTask(_connect2Server, canConnect);
            Disconnect4Server = ReactiveCommand.Create(_disconnect4Server);
            SendMessage = ReactiveCommand.CreateFromTask(_sendMessage, canSendMessage);
            HostName = (string)_localSettings.Values["HostName"] ?? "";
            Port = uint.Parse(_localSettings.Values["Client.Port"]?.ToString() ?? "1000");
        }

        private void _disconnect4Server()
        {
            Debug.WriteLine("disconnect");
        }

        private async Task<bool> _connect2Server()
        {
            await Task.Delay(5000);
            return true;
        }
        private async Task<bool> _sendMessage()
        {
            await Task.Delay(300);
            TextMessage = string.Empty;
            return true;
        }
        public string HostName
        {
            get => _hostName; set
            {
                this.RaiseAndSetIfChanged(ref _hostName, value);
                _localSettings.Values["HostName"] = value;
            }
        }
        public string TextMessage
        {
            get => _textMessage; set => this.RaiseAndSetIfChanged(ref _textMessage, value);
        }
        public uint Port
        {
            get => _port; set
            {
                this.RaiseAndSetIfChanged(ref _port, value);
                _localSettings.Values["Client.Port"] = value;
            }
        }
        public ReactiveCommand<Unit, bool> Connect2Server { get; }
        public ReactiveCommand<Unit, Unit> Disconnect4Server { get; }
        public ReactiveCommand<Unit, bool> SendMessage { get; }
        public bool IsConnect { get => _isConnect; private set => this.RaiseAndSetIfChanged(ref _isConnect, value); }
        public IObservableCollection<ClientInComeMessage> InComes { get => _inComes; set => this.RaiseAndSetIfChanged(ref _inComes, value); }
    }
}
