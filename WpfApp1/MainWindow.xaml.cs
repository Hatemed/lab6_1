using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using ClassLibrary;

namespace WpfApp1
{
    public class Converter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                string N = value[0].ToString();
                string Min = value[1].ToString();
                string Max = value[2].ToString();
                return N + ";" + Min + ";" + Max;
            }
            catch (Exception ex)
            {
                return "__";
            }
        }
        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                string str = value as string;
                string[] s = str.Split(';', StringSplitOptions.RemoveEmptyEntries);
                double min = double.Parse(s[1]);
                double max = double.Parse(s[2]);
                if (min > max)
                    throw (new Exception());
                return new object[] { Int32.Parse(s[0]), min, max };
            }
            catch
            {
                return new object[3];
            }
        }
    }
    public class DoubleConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                double d = (double)value;
                return Math.Round(d, 5).ToString();
            }
            catch (Exception ex)
            {
                return "__";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public partial class MainWindow : Window
    {
        public ViewData? ViewData;
        Microsoft.Win32.SaveFileDialog dlg;
        readonly string messageBoxText = "Do you want to save changes?";
        readonly string caption = "Save";
        readonly MessageBoxButton button = MessageBoxButton.YesNo;
        readonly MessageBoxImage icon = MessageBoxImage.Warning;
        public MainWindow()
        {
            dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Filename";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            InitializeComponent();
            VMBenchmark benchmark = new();
            ViewData = new ViewData(benchmark);
            this.DataContext = ViewData;
            comboBoxIn.ItemsSource = Enum.GetValues(typeof(VMf));
        }
        private void New_Click(object sender, RoutedEventArgs e)
        {
            if (ViewData.change == true)
            {
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                if (result == MessageBoxResult.Yes)
                {
                    if (dlg.ShowDialog() == true)
                    {
                        if (ViewData != null)
                            ViewData.Save(dlg.FileName);
                    }
                }
                ViewData.change = false;
            }
            ViewData.VMBenchmark.VMTimes.Clear();
            ViewData.VMBenchmark.VMAccuracies.Clear();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {

            if (dlg.ShowDialog() == true)
            {
                if (ViewData != null)
                    ViewData.Save(dlg.FileName);
            }
            ViewData.change = false;
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (ViewData.change == true)
            {
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                if (result == MessageBoxResult.Yes)
                {
                    Microsoft.Win32.SaveFileDialog dlg1 = new Microsoft.Win32.SaveFileDialog();
                    dlg1.FileName = "Filename";
                    dlg1.DefaultExt = ".txt";
                    dlg1.Filter = "Text documents (.txt)|*.txt";

                    if (dlg1.ShowDialog() == true)
                    {
                        if (ViewData != null)
                            ViewData.Save(dlg1.FileName);
                    }
                }
                ViewData.change = false;
            }
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Filename";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            if (dlg.ShowDialog() == true)
            {
                ViewData.Load(dlg.FileName);
            }
        }
        private void Add_VMTime(object sender, RoutedEventArgs e)
        {
            ViewData.change = true;
            ViewData.AddVMTime(ViewData.VMGrid);
        }
        private void Add_VMAccuracy(object sender, RoutedEventArgs e)
        {
            ViewData.change = true;
            ViewData.AddVMAccuracy(ViewData.VMGrid);
        }
        void WpfApp1_Closing(object sender, CancelEventArgs e)
        {
            if (ViewData.change == true)
            {
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                if (result == MessageBoxResult.Yes)
                {
                    if (dlg.ShowDialog() == true)
                    {
                        if (ViewData != null)
                            ViewData.Save(dlg.FileName);
                    }
                }
                ViewData.change = false;
            }
        }

    }
}
