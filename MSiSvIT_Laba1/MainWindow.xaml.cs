using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace JavaCodeAnalyzer
{
    public partial class MainWindow
    {
        private static string codeForMetrics;
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog {CheckFileExists = true, Multiselect = false};

            if (openDialog.ShowDialog() == true) {
                this.ShowNewPathToFile(openDialog.FileName);
                codeForMetrics = File.ReadAllText(openDialog.FileName);
                this.PerformActionsWithButtons();
            }
        }
        private void ShowNewPathToFile(string path)
        {
            this.LblPath.Content = path;
        }
        private void PerformActionsWithButtons()
        {
            this.MetricsPanel.IsEnabled = true;
        }

        private void btnChapin_Click(object sender, RoutedEventArgs e)
        {
            Modul javaCode = new Modul(codeForMetrics);
            this.TxtboxToOutMetricResult.Text = javaCode.GetChapinReport();
        }

        private void btnHolsted_Click(object sender, RoutedEventArgs e)
        {
            Modul javaCode = new Modul(codeForMetrics);
            this.TxtboxToOutMetricResult.Text = javaCode.GetHolstedReport();
        }

        private void BtnMcCabe_Click(object sender, RoutedEventArgs e)
        {
            this.TxtboxToOutMetricResult.Text = McCabeAnalizer.GetMcCabeReport(codeForMetrics);
        }

        private void BtnSloc_Click(object sender, RoutedEventArgs e)
        {
            this.TxtboxToOutMetricResult.Text = SlocAnalizer.GetSlocReport(codeForMetrics);
        }

    }
}
