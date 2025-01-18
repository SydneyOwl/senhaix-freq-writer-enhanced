using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using MsBox.Avalonia;
using SenhaixFreqWriter.Constants.Common;
using SenhaixFreqWriter.Utils.Other;
using Version = SenhaixFreqWriter.Properties.Version;

namespace SenhaixFreqWriter.Views.Common;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) windows.Background = Brushes.BurlyWood;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) linux.Background = Brushes.BurlyWood;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) macos.Background = Brushes.BurlyWood;
        MVersionTag.Content = Version.VersionTag == "@TAG_NAME@"
            ? Language.GetString("internal_version")
            : Version.VersionTag;
        MGitCommitHash.Content = Version.GitCommitHash == "@COMMIT_HASH@"
            ? Language.GetString("internal_version")
            : Version.GitCommitHash;
        MBuildTime.Content = Version.BuildTime == "@BUILD_TIME@"
            ? Language.GetString("internal_version")
            : Version.BuildTime;
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
                    // Linux无法在sudo下打开firefox
                    // Process.Start("xdg-open", url);
                    MessageBoxManager.GetMessageBoxStandard("Repo URL", url).ShowWindowDialogAsync(this);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
            }
        }
        catch
        {
            // ignored
        }
    }

    private void AckButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var thanks = $@"感谢以下友台对软件开发的大力支持：
{Thankslist.ToThankUString()}
名单可能多有遗漏之处，请见谅，并请及时告知！";
        MessageBoxManager.GetMessageBoxStandard("注意", thanks).ShowWindowDialogAsync(this);
    }
}