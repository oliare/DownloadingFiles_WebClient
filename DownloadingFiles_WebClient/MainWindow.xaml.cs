using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace DownloadingFiles_WebClient
{

    public partial class MainWindow : Window
    {
        /// <summary>
        /// https://letsenhance.io/static/8f5e523ee6b2479e26ecc91b9c25261e/1015f/MainAfter.jpg
        /// https://ash-speed.hetzner.com/100MB.bin
        /// </summary>

        private ObservableCollection<DownloadElement> downloads = new ObservableCollection<DownloadElement>();
        private WebClient client = new WebClient();

        public MainWindow()
        {
            InitializeComponent();
            listBox.ItemsSource = downloads;
            client.DownloadFileCompleted += ClientDownloadFileCompleted;
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            string url = urlTextBox.Text;
            string dest = destTextBox.Text;

            if (string.IsNullOrEmpty(url) && string.IsNullOrEmpty(dest))
            {
                MessageBox.Show("Please fill empty fields");
                return;
            }
            try
            {
                DownloadFileAsync(url, dest);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Oops..{ex.Message}");
            }

        }

        private bool isDownloading = false;
        private async Task DownloadFileAsync(string url, string dest)
        {
            if (isDownloading)
            {
                MessageBox.Show("Download is already in progress"); return;
            }

            isDownloading = true;
            var item = new DownloadElement
            {
                Name = Path.GetFileName(url),
                Url = url,
                Destination = dest,
                Progress = 0
            };

            client.DownloadProgressChanged += ClientDownloadProgressChanged;

            string file = Path.GetFileName(url);

            await client.DownloadFileTaskAsync(url, Path.Combine(dest, file));
            downloads.Add(item);
            isDownloading = false;
        }

        private void ClientDownloadProgressChanged(object? sender, DownloadProgressChangedEventArgs e)
        {
            percent.Value = e.ProgressPercentage;
            progressTextBox.Text = $"{e.ProgressPercentage}%";
        }
        private void ClientDownloadFileCompleted(object? sender, AsyncCompletedEventArgs e)
        {
            MessageBox.Show("Download successful!", "Notification");
            percent.Value = 0;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog();
            if (dialog.ShowDialog() == true)
            {
                string dir = Path.GetFullPath(dialog.FolderName);
                destTextBox.Text = dir;
            }
        }

        private void CancelDownloadingBtn_Click(object sender, RoutedEventArgs e)
        {
            client.DownloadProgressChanged -= ClientDownloadProgressChanged;
            client.DownloadFileCompleted -= ClientDownloadFileCompleted;

            if (!isDownloading)
            {
                MessageBox.Show("No download in progress to cancel");
                return;
            }

            client.CancelAsync();
            percent.Value = 0;
            progressTextBox.Text = "0%";
            MessageBox.Show("Download cancelled");
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                string dir = Path.GetDirectoryName(path)!;
                Process.Start(path, dir);
            }
        }
    }


    public class DownloadElement
    {
        public string Name { get; set; }
        public string? Url { get; set; }
        public string? Destination { get; set; }
        public bool IsDownloading { get; set; }
        public int Progress { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }

}



