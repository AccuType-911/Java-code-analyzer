using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
using Microsoft.Win32;

namespace MSiSvIT_Laba1
{
    public partial class MainWindow : Window
    {
        private static string textForMetric;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.CheckFileExists = true;
            openDialog.Multiselect = false;

            if (openDialog.ShowDialog() == true) {
                ShowNewPathToFile(openDialog.FileName);
                textForMetric = File.ReadAllText(openDialog.FileName);
                PerformActionsWithButtons();
            }
        }

        private void ShowNewPathToFile(string path)
        {
            lblPath.Content = path;
        }
        private void PerformActionsWithButtons()
        {
            metricsPanel.IsEnabled = true;
        }

        private void btnChapin_Click(object sender, RoutedEventArgs e)
        {
            cmbClass.Visibility = Visibility.Visible;
            cmbMethod.Visibility = Visibility.Visible;

            ChapinResult chapinResult = ChapinJavaAnalizer.Analize(textForMetric);
        }

        private void btnHolsted_Click(object sender, RoutedEventArgs e)
        {
            cmbClass.Visibility = Visibility.Hidden;
            cmbMethod.Visibility = Visibility.Hidden;
        }

        private void cmbClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
