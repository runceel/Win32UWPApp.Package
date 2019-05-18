using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace Win32UWPApp.Win32
{
    class Program
    {
        private static readonly TaskCompletionSource<int> _source = new TaskCompletionSource<int>();
        static async Task Main(string[] args)
        {
            var conn = new AppServiceConnection
            {
                PackageFamilyName = Package.Current.Id.FamilyName,
                AppServiceName = "InProcAppService",
            };

            conn.RequestReceived += Conn_RequestReceived;
            conn.ServiceClosed += Conn_ServiceClosed;
            await conn.OpenAsync();

            Console.WriteLine("Connected");
            _ = Task.Run(async () =>
            {
                var t = _source.Task;
                while(!t.IsCompleted)
                {
                    await Task.Delay(1000 * 5);
                    var res = await conn.SendMessageAsync(new ValueSet
                    {
                        ["text"] = $"{DateTime.Now} Message from Win32 app",
                    });

                    res.Message.TryGetValue("text", out var text);
                    Console.WriteLine($"{res.Status}: {text}");
                }
            });

            await _source.Task;
        }

        private static void Conn_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            _source.SetResult(0);
        }

        private static async void Conn_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var d = args.GetDeferral();
            args.Request.Message.TryGetValue("text", out var text);
            Console.WriteLine($"\"{text}\" を受信しました。");
            await args.Request.SendResponseAsync(new ValueSet
            {
                ["text"] = $"{DateTime.Now}: UWP から受け付けたメッセージ {text} を処理しました。",
            });

            d.Complete();
        }
    }
}
