using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
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
    public class BoolConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if ((bool)value) return "Данные изменены"; else return "Данные не изменены";
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
    public class MaxConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                string str;
                if ((double)value == -1) str = "не определено";
                else str = "= " + value.ToString();
                return "Максимальное значение отношения " + str;
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
    public class MinConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                string str;
                if ((double)value == -1) str = "не определено";
                else str = "= " + value.ToString();
                return "Минимальное значение отношения " + str;
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
    public class ViewData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public VMGrid VMGrid { get; set; }
        public VMBenchmark VMBenchmark { get; set; }
        public ViewData(VMBenchmark VMBenchmark)
        {
            this.VMBenchmark = VMBenchmark;
            this.VMGrid = new VMGrid();
            VMBenchmark.VMTimes.CollectionChanged += Time_CollectionChanged;
            VMBenchmark.VMAccuracies.CollectionChanged += Accuracy_CollectionChanged;

        }
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        void Time_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("VMBenchmark.VMTimes");
        }
        void Accuracy_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("VMBenchmark.VMAccuracies");
        }
        public void AddVMTime(VMGrid New_grid)
        {
            try {
                VMBenchmark.AddVMTime(New_grid);
                OnPropertyChanged("Time_HA_base_max");
                OnPropertyChanged("Time_HA_base_min"); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void AddVMAccuracy(VMGrid New_grid)
        {
            try
            {
                VMBenchmark.AddVMAccuracy(New_grid);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public bool Save(string filename)
        {
            string jsonString = JsonSerializer.Serialize(VMBenchmark);
            try
            {
                using (StreamWriter writer = new StreamWriter(filename, false))
                {
                    writer.Write(jsonString);
                };
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        public bool Load(string filename)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    string jsonString;
                    jsonString = sr.ReadLine();
                    VMBenchmark VMBenchmark = JsonSerializer.Deserialize<VMBenchmark>(jsonString);
                    this.VMBenchmark.VMAccuracies.Clear();
                    for (int i = 0; i < VMBenchmark.VMAccuracies.Count; i++)
                    {
                        this.VMBenchmark.VMAccuracies.Add(VMBenchmark.VMAccuracies[i]);
                    }
                    this.VMBenchmark.VMTimes.Clear();
                    for (int i = 0; i < VMBenchmark.VMTimes.Count; i++)
                    {
                        this.VMBenchmark.VMTimes.Add(VMBenchmark.VMTimes[i]);
                    }
                    OnPropertyChanged("Time_HA_base_max");
                    OnPropertyChanged("Time_HA_base_min");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        private bool private_change = false;
        public bool change
        {
            get
            {
                return private_change;
            }
            set
            {
                private_change = value;
                OnPropertyChanged("change");
            }
        }
        public double Time_HA_base_max { get
            {
                return VMBenchmark.Time_HA_base_max;
            } }
        public double Time_HA_base_min { get
            {
                return VMBenchmark.Time_HA_base_min;
            } }
        public ViewData()
        {
            this.VMBenchmark = new VMBenchmark();
            this.VMGrid = new VMGrid();
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
