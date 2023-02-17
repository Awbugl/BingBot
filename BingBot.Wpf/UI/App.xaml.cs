using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BingBot.Core.Common;
using BingBot.Wpf.Common;
using Newtonsoft.Json;
using MessageBox = System.Windows.MessageBox;
using Path = BingBot.Core.Common.Path;

#pragma warning disable CS4014

namespace BingBot.Wpf.UI;

internal partial class App
{
    private static Mutex? _mutex;

    protected override void OnStartup(StartupEventArgs e)
    {
        _mutex = new(true, "BingBotOnlyRunMutex");
        if (!_mutex.WaitOne(0, false) &&
            MessageBox.Show("已有在运行的BingBot，是否要打开新的实例？", "BingBot提示", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            Environment.Exit(0);

        Current.DispatcherUnhandledException += (_, args) =>
        {
            ExceptionLogger.Log(args.Exception);
            args.Handled = true;
        };

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            ExceptionLogger.Log(args.Exception.InnerException);
            args.SetObserved();
        };

        AppDomain.CurrentDomain.UnhandledException += (_, args) => ExceptionLogger.Log(args.ExceptionObject as Exception);

        if (!File.Exists(Path.Config)) File.WriteAllText(Path.Config, JsonConvert.SerializeObject(new BingBotConfig()));

        ExceptionLogger.OnExceptionRecorded += exception =>
        {
            Program.Add(Program.Exceptions, new() { Time = DateTime.Now, Exception = exception });
            if (Program.Exceptions.Count > 100) Program.RemoveFirst(Program.Exceptions);
        };

        FindResource("Taskbar");
        base.OnStartup(e);

        MainWindow = new MainWindow();
        MainWindow.Dispatcher.InvokeAsync(Program.ProgramInit);
        MainWindow.Show();
    }
}
