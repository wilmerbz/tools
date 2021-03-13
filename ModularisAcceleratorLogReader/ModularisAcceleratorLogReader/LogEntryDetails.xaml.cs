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
using System.Windows.Shapes;

namespace ModularisAcceleratorLogReader
{
    /// <summary>
    /// Interaction logic for LogEntryDetails.xaml
    /// </summary>
    public partial class LogEntryDetails : Window
    {
        public LogEntryDetails()
        {
            InitializeComponent();
        }

        public LogEntryDetails(string content)
        {
            InitializeComponent();

            uxLogContent.Text = content;
        }
    }
}
