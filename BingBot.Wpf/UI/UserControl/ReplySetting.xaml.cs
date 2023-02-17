using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using BingBot.Core.Common;
using Newtonsoft.Json;
using Path = BingBot.Core.Common.Path;

namespace BingBot.Wpf.UI.UserControl;

internal partial class ReplySetting
{
    internal ReplySetting()
    {
        InitializeComponent();
        ComboBox.ItemsSource = typeof(RobotReply).GetProperties();
    }

    private PropertyInfo? CurrentProperty { get; set; }

    private void OnSaveButtonClick(object sender, RoutedEventArgs e)
    {
        if (CurrentProperty is null) return;

        CurrentProperty.SetValue(GlobalConfig.RobotReply, TextBox.Text);
        File.WriteAllText(Path.RobotReply, JsonConvert.SerializeObject(GlobalConfig.RobotReply, Formatting.Indented));
    }

    private void ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (((ComboBox)sender).SelectedItem is PropertyInfo info)
        {
            CurrentProperty = info;
            TextBox.Text = CurrentProperty.GetValue(GlobalConfig.RobotReply) as string;
        }
    }
}
