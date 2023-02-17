using System.Collections.Specialized;
using System.Windows.Controls;
using BingBot.Wpf.Common;

namespace BingBot.Wpf.UI.UserControl;

internal partial class ExceptionLog
{
    public ExceptionLog()
    {
        InitializeComponent();

        List.ItemsSource = Program.Exceptions;
        Program.Exceptions.CollectionChanged += OnExceptionsChanged;
    }

    private void OnExceptionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var b = List.GetBindingExpression(ItemsControl.ItemsSourceProperty);
        b?.UpdateTarget();
    }
}
