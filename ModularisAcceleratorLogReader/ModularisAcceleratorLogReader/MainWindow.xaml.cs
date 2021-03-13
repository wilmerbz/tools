using Microsoft.Win32;
using ModularisAcceleratorLogReader.Core;
using ModularisAcceleratorLogReader.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        #region Constants
        private const string ClearLogConfirmationMessage = "Are you sure you want to clear the content of the log file?";
        private const string ClearLogConfirmationTitle = "Clear Log File";

        private const string NoneGroupName = "None";
        private const string EventIDGroupName = "EventID";
        private const string ThreadIDGroupName = "ThreadID";
        private const string TraceIDGroupName = "TraceID";

        private const string FileTitleFormat = "File: {0}";
        private const string WindowTitleFormat = "Modularis Log Reader - {0}";

        #endregion

        private string _fileName = string.Empty;
        private OpenFileDialog _openFileDialog;

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();


        public MainWindow()
        {
            InitializeComponent();

            InitializeFileDialog();
            InitializeAutorefreshTimer();
            InitializeGroupType();
        }

        #region Initialization

        private void InitializeAutorefreshTimer()
        {
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        }

        private void InitializeFileDialog()
        {
            _openFileDialog = new OpenFileDialog();
            _openFileDialog.Filter = "Log Files (.log)|*.log";
        }

        private void InitializeGroupType()
        {
            List<GroupType> groupTypes = new List<GroupType>();
            groupTypes.Add(new GroupType { Name = NoneGroupName });
            groupTypes.Add(new GroupType { Name = EventIDGroupName });
            groupTypes.Add(new GroupType { Name = ThreadIDGroupName });
            groupTypes.Add(new GroupType { Name = TraceIDGroupName });

            uxGroupTypesComboBox.ItemsSource = groupTypes;
            uxGroupTypesComboBox.SelectedValue = NoneGroupName;
        }

        #endregion

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

        private const string logEntriesCollectionResourceName = "logEntriesCollection";

        private LogEntries LogEntriesCollection
        {
            get
            {
                return (LogEntries)this.Resources[logEntriesCollectionResourceName];
            }
        }

        private ICollectionView DefaultCollectionView
        {
            get
            {
                return CollectionViewSource.GetDefaultView(uxLogItemsDataGrid.ItemsSource);
            }
        }

        private void LoadLogEntries()
        {
            if (string.IsNullOrEmpty(_fileName)) return;

            LogReader.ReadLogFile(_fileName);
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
            _seachText = uxSearchTextBox.Text;
            FilterLogEntries();
        }

        private void FilterLogEntries()
        {
            var logEntries = LogReader.FilterLogEntries(_seachText);
            uxLogItemsDataGrid.ItemsSource = logEntries;

            if (SelectedGroupType != NoneGroupName)
            {
                GroupLogEntries();
            }
        }

        private void LogEntry_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            var logEntry = row.Item as LogEntry;
            var logEntryDetailsWindow = new LogEntryDetails(logEntry.FullContent);
            logEntryDetailsWindow.Show();
        }

        private void uxClearFileButton_Click(object sender, RoutedEventArgs e)
        {
            var confirm = System.Windows.MessageBox.Show(ClearLogConfirmationMessage, ClearLogConfirmationTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);

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

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            var logEntry = e.Item as LogEntry;

            if (string.IsNullOrEmpty(_seachText) || logEntry.FullContent.ToLower().IndexOf(_seachText) > -1)
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        private string SelectedGroupType => (string)uxGroupTypesComboBox.SelectedValue;

        private void uxGroupTypesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GroupLogEntries();
        }

        private void GroupLogEntries()
        {
            ICollectionView collectionView = DefaultCollectionView;
            if (collectionView == null || collectionView.CanGroup == false) return;

            var selectectedGroup = SelectedGroupType;
            collectionView.GroupDescriptions.Clear();

            switch (selectectedGroup)
            {
                case NoneGroupName:
                    break;
                case EventIDGroupName:
                case ThreadIDGroupName:
                case TraceIDGroupName:
                    collectionView.GroupDescriptions.Add(new PropertyGroupDescription(selectectedGroup));
                    break;
                default:
                    break;
            }
        }
    }
}
