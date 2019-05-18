using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace Win32UWPApp.UWP
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private string _latestReceivedMessage;
        public string LatestReceivedMessage
        {
            get => _latestReceivedMessage;
            set => this.SetProperty(ref _latestReceivedMessage, value, PropertyChanged);
        }

        private string _sendingMessage;
        public string SendingMessage
        {
            get => _sendingMessage;
            set => this.SetProperty(ref _sendingMessage, value, PropertyChanged);
        }

        public MainPage()
        {
            this.InitializeComponent();
            AsyncMessageBroker.Default.Subscribe<ReceivedMessage>(MessageReceivedAsync);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async Task MessageReceivedAsync(ReceivedMessage arg)
        {
            void messageReceived()
            {
                LatestReceivedMessage = arg.Text;
                arg.ReplyText = $"{DateTime.Now}: UWP でメッセージ {arg.Text} を受信しました。";
            }

            if (!Dispatcher.HasThreadAccess)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, messageReceived);
            }
            else
            {
                messageReceived();
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var message = new SendingMessage(SendingMessage);
            SendingMessage = "";
            await AsyncMessageBroker.Default.PublishAsync(message);
            LatestReceivedMessage = message.ReplyText;
        }
    }

    public static class INPEx
    {
        public static bool SetProperty<T>(this INotifyPropertyChanged self, ref T field, T value, PropertyChangedEventHandler h, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            h?.Invoke(self, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
