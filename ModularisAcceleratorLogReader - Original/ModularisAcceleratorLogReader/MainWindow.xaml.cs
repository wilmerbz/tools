using Microsoft.Win32;
using ModularisAcceleratorLogReader.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModularisAcceleratorLogReader
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string FileTitleFormat = "File: {0}";
        private const string WindowTitleFormat = "Modularis Log Reader - {0}";
        private string _fileName = string.Empty;
        private OpenFileDialog _openFileDialog;

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();


        public MainWindow()
        {
            InitializeComponent();
            _openFileDialog = new OpenFileDialog();
            _openFileDialog.Filter = "Log Files (.log)|*.log";

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            LoadLogEntries();
        }

        private void browseFile_Click(object sender, RoutedEventArgs e)
        {
            _fileName = string.Empty;
            var result = _openFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                _fileName = _openFileDialog.FileName;
                var fileInfo = new System.IO.FileInfo(_fileName);
                this.uxTitleLabel.Content = string.Format(FileTitleFormat, _fileName);
                this.Title = string.Format(WindowTitleFormat, fileInfo.Name);
                LoadLogEntries();
            }
        }

        private void LoadLogEntries()
        {
            if (string.IsNullOrEmpty(_fileName)) return;
            LogReader.ReadLogFile(_fileName);
            //uxLogItems.ItemsSource = logEntries;
            FilterLogEntries();
        }

        private void uxRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadLogEntries();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.F5) return;
            LoadLogEntries();
        }
        private string _seachText = string.Empty;

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            _seachText = uxSearchTextBox.Text.ToLower();

            FilterLogEntries();

        }

        private void FilterLogEntries()
        {
            List<LogEntry> logEntries = LogReader.LogEntries;

            if (logEntries != null && !string.IsNullOrEmpty(_seachText))
            {
                logEntries = logEntries.Where(l => l.FullContent.ToLower().Contains(_seachText)).ToList();
            }

            uxLogItems.ItemsSource = logEntries;
        }

        private void LogEntry_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            // Some operations with this row
            var logEntry = row.Item as LogEntry;
            var logEntryDetailsWindow = new LogEntryDetails(logEntry.FullContent);
            //logEntryDetailsWindow.ShowDialog();
            logEntryDetailsWindow.Show();
        }

        private void uxClearFileButton_Click(object sender, RoutedEventArgs e)
        {
            var confirm = System.Windows.MessageBox.Show("Are you sure you want to clear the content of the log file?", "Clear Log File", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm == MessageBoxResult.No) return;

            LogReader.ClearLogFile(this._fileName);
            LoadLogEntries();
        }

        private void uxAutoRefreshCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_fileName))
                dispatcherTimer.Start();
        }

        private void uxAutoRefreshCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_fileName))
                dispatcherTimer.Stop();
        }
    }
}
