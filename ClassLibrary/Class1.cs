using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace ClassLibrary
{
    public enum VMf
        {
            vmdTan,
            vmdErfInv
        }
    public class VMGrid
    {
        public int N { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public VMGrid(int N = 2, double Min = 0, double Max = 0, VMf vmf = VMf.vmdTan)
        {
            this.N = N;
            this.Min = Min;
            this.Max = Max;
            this.VMf = vmf;
        }
        public double Step
        {
            get { return (Max - Min) / (N - 1); }
        }
        public VMf VMf { get; set; }
    }
    public struct VMTime
    {
        public VMGrid Grid { get; set; }
        public int Time_HA { get; set; }
        public int Time_EP { get; set; }
        public int Time_base { get; set; }
        public VMTime (VMGrid Grid, int Time_HA, int Time_EP, int Time_base)
        {
            this.Grid = Grid;
            this.Time_HA = Time_HA;
            this.Time_EP = Time_EP;
            this.Time_base = Time_base;
        }
        public double Time_EP_base
        {
            get
            {
                if (Time_base == 0)
                    return 0;
                else
                    return (double)Time_EP / Time_base;
            }
        }
        public double Time_HA_base
        {
            get { if (Time_base == 0) 
                return 0; 
            else
                    return (double)Time_HA / Time_base; }
        }
        public override string ToString()
        {
            return $"N = {Grid.N}, Min = {Grid.Min}, Max = {Grid.Max}, " +
                $"Step = {Grid.Step}, func = {Grid.VMf}, Time_HA = {Time_HA}, " +
                $"Time_EP = {Time_EP}, Time_base = {Time_base}, Time_HA / Time_base = {Time_HA_base}, " +
                $"Time_EP / Time_base = {Time_EP_base}";
        }
    }
    public struct VMAccuracy
    {
        public VMGrid Grid { get; set; }
        public double Max_sub_value_HA { get; set; }
        public double Max_sub_value_EP { get; set; }
        public double Max_sub_arg { get; set; }
        public VMAccuracy(VMGrid Grid, double Max_sub_value_HA, double Max_sub_value_EP, double Max_sub_arg)
        {
            this.Grid = Grid;
            this.Max_sub_value_HA = Max_sub_value_HA;
            this.Max_sub_value_EP = Max_sub_value_EP;
            this.Max_sub_arg = Max_sub_arg;
        }
        public double Max_sub_value
        {
            get { return Math.Abs(Max_sub_value_EP - Max_sub_value_HA); }
        }
        public override string ToString()
        {
            return $"N = {Grid.N}, Min = {Grid.Min}, Max = {Grid.Max}, " +
                $"Step = {Grid.Step}, func = {Grid.VMf}, Max_sub_value = {Max_sub_value}, " +
                $"Max_sub_arg = {Max_sub_arg}, Value VML_HA = {Max_sub_value_HA}, Value VML_EP = {Max_sub_value_EP}";
        }
    }
    public class VMBenchmark
    {
        [DllImport(@"C:\Users\User\source\repos\lab6_1\x64\Debug\Dll.dll")]
        static extern void vmd(int n, double min, double max, int num_func, int[] time, double[] max_value);
        public ObservableCollection<VMTime> VMTimes { get; set; }
        public ObservableCollection<VMAccuracy> VMAccuracies { get; set; }
        public VMBenchmark()
        {
            VMTimes = new ObservableCollection<VMTime>();
            VMAccuracies = new ObservableCollection<VMAccuracy>();
        }
        public void AddVMTime(VMGrid New_grid)
        {
            VMGrid copyGrid = new VMGrid(New_grid.N, New_grid.Min, New_grid.Max, New_grid.VMf);
            int[] time = new int[3];
            double[] max_value = new double[3];
            vmd(copyGrid.N, copyGrid.Min, copyGrid.Max, (int)copyGrid.VMf, time, max_value);
            var new_time = new VMTime(copyGrid, time[0], time[1], time[2]);
            VMTimes.Add(new_time);
        }
        public void AddVMAccuracy(VMGrid New_grid)
        {
            VMGrid copyGrid = new VMGrid(New_grid.N, New_grid.Min, New_grid.Max, New_grid.VMf);
            int[] time = new int[3];
            double[] max_value = new double[3];
            vmd(copyGrid.N, copyGrid.Min, copyGrid.Max, (int)copyGrid.VMf, time, max_value);
            var new_accur = new VMAccuracy(copyGrid, max_value[0], max_value[1], max_value[2]);
            VMAccuracies.Add(new_accur);
        }
        public double Time_HA_base
        {
            get
            {
                double max = -1;
                for (int i = 0; i < VMTimes.Count; i++)
                {
                    if (max < VMTimes[i].Time_HA_base)
                        max = VMTimes[i].Time_HA_base;
                }
                return max;
            }
        }
        public double Time_EP_base
        {
            get
            {
                double max = -1;
                for (int i = 0; i < VMTimes.Count; i++)
                {
                    if (max < VMTimes[i].Time_EP_base)
                        max = VMTimes[i].Time_EP_base;
                }
                return max;
            }
        }
    }
    public class ViewData: INotifyPropertyChanged
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
            VMBenchmark.AddVMTime(New_grid);
        }
        public void AddVMAccuracy(VMGrid New_grid)
        {
            VMBenchmark.AddVMAccuracy(New_grid);
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
            catch(Exception e)
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
                    for(int i = 0; i< VMBenchmark.VMTimes.Count; i++)
                    {
                        this.VMBenchmark.VMTimes.Add(VMBenchmark.VMTimes[i]);
                    }
                }

            }
            catch(Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        private bool private_change = false;
        private string str_change = "Данные не изменены";
        public string public_str_change { get { return str_change; } set { str_change = value; OnPropertyChanged("public_str_change");} }
        public bool change
        {
            get
            {
                return private_change;
            }
            set
            {
                private_change = value;
                if (change) { public_str_change = "Данные изменены"; }
                else { public_str_change = "Данные не изменены"; }
            }
        }
        public ViewData()
        {
            this.VMBenchmark = new VMBenchmark();
            this.VMGrid = new VMGrid();
        }
    }

}