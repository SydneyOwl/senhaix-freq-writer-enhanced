using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Newtonsoft.Json.Linq;

namespace SenhaixFreqWriter.Views.Common;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            windows.Background = Brushes.BlanchedAlmond;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            linux.Background = Brushes.BlanchedAlmond;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            macos.Background = Brushes.BlanchedAlmond;
        }
    }

    private void RepoButton_OnClick(object? sender, RoutedEventArgs e)
    {
        OpenUrl("https://github.com/SydneyOwl/senhaix-freq-writer-enhanced");
    }
    private void OpenUrl(string url)
    {
        try
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    return;
                }
            }
        }
        catch
        {
            return;
        }
    }

    private async void UpdateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        string repoOwner = "SydneyOwl";
        string repoName = "senhaix-freq-writer-enhanced";
        string apiUrl = $"https://api.github.com/repos/{repoOwner}/{repoName}/releases/latest";

        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(5); 
        httpClient.DefaultRequestHeaders.Add("User-Agent", "agent"); 

        try
        {
            var response = await httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                JObject releaseJson = JObject.Parse(jsonContent);
                string tagName = ((string)releaseJson["tag_name"]).Replace("v","");
                string currentTag = Properties.VERSION.Version.Replace("v","");
                var result = tagName.CompareTo(currentTag);
                if (result > 0)
                    MessageBoxManager.GetMessageBoxStandard("注意", "有新版本可用~").ShowWindowDialogAsync(this);
                else
                    MessageBoxManager.GetMessageBoxStandard("注意", "您已是最新版~").ShowWindowDialogAsync(this);
            }
            else
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "无法获取最新版本信息~").ShowWindowDialogAsync(this);
            }
        }
        catch
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "无法获取最新版本信息~").ShowWindowDialogAsync(this);
        }
    }
}