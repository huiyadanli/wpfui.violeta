﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Wpf.Ui.Controls;
using Wpf.Ui.Violeta.Controls;

namespace Wpf.Ui.Test;

[ObservableObject]
public partial class MainWindow : FluentWindow
{
    public MainWindow()
    {
        DataContext = this;
        InitializeComponent();

        Thread.Sleep(600);
        Splash.CloseOnLoaded(this, minimumMilliseconds: 1800);
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        if (WindowBackdrop.IsSupported(WindowBackdropType.Mica))
        {
            Background = new SolidColorBrush(Colors.Transparent);
            WindowBackdrop.ApplyBackdrop(this, WindowBackdropType.Mica);
        }
    }

    [RelayCommand]
    private void ShowToast(Button self)
    {
        string message = "This is a toast message";
        ToastLocation toastLocation = (ToastLocation)Enum.Parse(typeof(ToastLocation), self.Content.ToString()!);

        if (self.Tag.ToString() == "Information")
        {
            Toast.Information(message, toastLocation);
        }
        else if (self.Tag.ToString() == "Error")
        {
            Toast.Error(message, toastLocation);
        }
        else if (self.Tag.ToString() == "Success")
        {
            Toast.Success(message, toastLocation);
        }
        else if (self.Tag.ToString() == "Warning")
        {
            Toast.Warning(message, toastLocation);
        }
        else if (self.Tag.ToString() == "Question")
        {
            Toast.Question(message, toastLocation);
        }
        else if (self.Tag.ToString() == "None")
        {
            Toast.Show(null!, message, new ToastConfig()
            {
                Location = toastLocation,
            });
        }
    }

    [RelayCommand]
    private void ShowFlyoutInline()
    {
        Toast.Success("The cake is a lie!");
    }

    [RelayCommand]
    private async Task ShowContentDialogAsync()
    {
        ContentDialog dialog =
            new()
            {
                Title = "My sample dialog",
                Content = "Content of the dialog",
                CloseButtonText = "Close button",
                PrimaryButtonText = "Primary button",
                SecondaryButtonText = "Secondary button"
            };

        // Setting the dialog container
        dialog.DialogHost = ContentDialogHostService.ContentPresenterForDialogs;

        // Showing the dialog
        _ = await dialog.ShowAsync(CancellationToken.None);
    }

    [RelayCommand]
    private void ShowMessageBox(Button self)
    {
        if (self.Content.ToString() == "Information")
        {
            _ = MessageBox.Information("This is a information message");
        }
        else if (self.Content.ToString() == "Warning")
        {
            _ = MessageBox.Warning("This is a warning message");
        }
        else if (self.Content.ToString() == "Question")
        {
            _ = MessageBox.Question("This is a question and do you want to click OK?");
        }
        else if (self.Content.ToString() == "Error")
        {
            _ = MessageBox.Error(
                """
                Dummy exception from Violeta:
                   at Violeta.View.MainWindow.OnNotifyIconLeftDoubleClick(NotifyIcon sender, RoutedEventArgs e) in D:\GitHub\Violeta\View\MainWindow.xaml.cs:line 53
                   at System.RuntimeMethodHandle.InvokeMethod(Object target, Void** arguments, Signature sig, Boolean isConstructor)
                   at System.Reflection.MethodBaseInvoker.InvokeDirectByRefWithFewArgs(Object obj, Span`1 copyOfArgs, BindingFlags invokeAttr)
                """
            );
        }
    }

    [RelayCommand]
    private async Task AsyncShowMessageBoxAsync(Button self)
    {
        if (self.Content.ToString() == "Information")
        {
            _ = await MessageBox.InformationAsync("This is a information message");
        }
        else if (self.Content.ToString() == "Warning")
        {
            _ = await MessageBox.WarningAsync("This is a warning message");
        }
        else if (self.Content.ToString() == "Question")
        {
            _ = await MessageBox.QuestionAsync("This is a question and do you want to click OK?");
        }
        else if (self.Content.ToString() == "Error")
        {
            _ = await MessageBox.ErrorAsync("This is a error message");
        }
    }

    [ObservableProperty]
    private RegistryModel treeModel = new();
}

public partial class RegistryModel : ITreeModel
{
    public IEnumerable GetChildren(object parent)
    {
        if (parent == null)
        {
            yield return Registry.ClassesRoot;
            yield return Registry.CurrentUser;
            yield return Registry.LocalMachine;
            yield return Registry.Users;
            yield return Registry.CurrentConfig;
        }
        else if (parent is RegistryKey key)
        {
            foreach (var name in key.GetSubKeyNames())
            {
                RegistryKey? subKey = null;
                try
                {
                    subKey = key.OpenSubKey(name);
                }
                catch
                {
                }

                if (subKey != null)
                {
                    yield return subKey;
                }
            }

            foreach (var name in key.GetValueNames())
            {
                yield return new RegValue()
                {
                    Name = name,
                    Data = key.GetValue(name),
                    Kind = key.GetValueKind(name)
                };
            }
        }
    }

    public bool HasChildren(object parent)
    {
        return parent is RegistryKey;
    }
}

public struct RegValue
{
    public string Name { get; set; }
    public object? Data { get; set; }
    public RegistryValueKind Kind { get; set; }
}
