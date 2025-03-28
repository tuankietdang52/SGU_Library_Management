using DotNetEnv;
using SGULibraryManagement.Config;
using SGULibraryManagement.Helper;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace SGULibraryManagement;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static App? Instance { get; private set; }
    
    public App()
    {
        Instance ??= this;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        LoadEnv();

        this.ConfigureSize()
            .ConfigureMySql();
    }

    private void LoadEnv()
    {
        string current = AppDomain.CurrentDomain.BaseDirectory;
        var root = Directory.GetParent(current)!.Parent!.Parent!.Parent;

        Env.Load($"{root?.FullName}/Resources/.env");
    }
}