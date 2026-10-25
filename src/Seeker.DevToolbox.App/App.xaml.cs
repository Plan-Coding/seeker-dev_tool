using System.IO;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seeker.DevToolbox.App.Services;
using Seeker.DevToolbox.App.ViewModels;
using Seeker.DevToolbox.Core.Configuration;
using Seeker.DevToolbox.Tools;
using Seeker.DevToolbox.UI.Appearance;
using Seeker.DevToolbox.UI.Contracts;
using Seeker.DevToolbox.UI.Tools;
using Serilog;

namespace Seeker.DevToolbox.App;

public partial class App : Application
{
    private IHost? _host;
    private SingleInstance? _singleInstance;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var paths = AppPaths.Resolve();
        ConfigureLogging(paths);

        _singleInstance = new SingleInstance();
        if (!_singleInstance.IsFirstInstance)
        {
            _singleInstance.SendToPrimary(e.Args);
            Shutdown();
            return;
        }

        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        try
        {
            _host = BuildHost(paths, e.Args);
            _host.Start();

            var window = _host.Services.GetRequiredService<MainWindow>();
            MainWindow = window;
            window.Show();

            _singleInstance.ArgumentsReceived += args =>
                Dispatcher.Invoke(() => ActivateMainWindow(args));
            _singleInstance.StartListening();

            Log.Information(
                "Seeker DevToolbox started. Portable={Portable}, DataRoot={DataRoot}",
                paths.IsPortable,
                paths.DataRoot);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Startup failed");
            MessageBox.Show(
                $"启动失败:{ex.Message}\n\n日志目录:{paths.LogsDirectory}",
                "Seeker DevToolbox",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(-1);
        }
    }

    private static IHost BuildHost(AppPaths paths, string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices(services =>
            {
                services.AddSingleton(paths);
                services.AddSingleton<SettingsService>();
                services.AddSingleton<IThemeService, ThemeService>();
                services.AddSingleton<ToolRegistry>();

                foreach (var module in ToolCatalog.CreateModules())
                {
                    services.AddSingleton<IToolModule>(module);
                }

                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<MainWindow>();
            })
            .Build();
    }

    private static void ConfigureLogging(AppPaths paths)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Debug()
            .WriteTo.File(
                Path.Combine(paths.LogsDirectory, "app-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    private void ActivateMainWindow(string[] args)
    {
        if (MainWindow is null)
        {
            return;
        }

        if (MainWindow.WindowState == WindowState.Minimized)
        {
            MainWindow.WindowState = WindowState.Normal;
        }

        MainWindow.Show();
        MainWindow.Activate();

        // 让窗口真正前置一次,避免仅任务栏闪烁。
        MainWindow.Topmost = true;
        MainWindow.Topmost = false;
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Error(e.Exception, "Unhandled UI exception");
        MessageBox.Show(
            $"界面线程发生未处理异常:{e.Exception.Message}\n\n详细信息已写入日志。",
            "Seeker DevToolbox",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
        e.Handled = true;
    }

    private static void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Log.Fatal(e.ExceptionObject as Exception, "Unhandled domain exception");
        Log.CloseAndFlush();
    }

    private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        Log.Error(e.Exception, "Unobserved task exception");
        e.SetObserved();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        try
        {
            _host?.StopAsync(TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Host shutdown failed");
        }

        _host?.Dispose();
        _singleInstance?.Dispose();

        Log.Information("Seeker DevToolbox exited. Code={ExitCode}", e.ApplicationExitCode);
        Log.CloseAndFlush();

        base.OnExit(e);
    }
}
