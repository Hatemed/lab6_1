using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

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
            try
            {
                VMGrid copyGrid = new VMGrid(New_grid.N, New_grid.Min, New_grid.Max, New_grid.VMf);
                int[] time = new int[3];
                double[] max_value = new double[3];
                vmd(copyGrid.N, copyGrid.Min, copyGrid.Max, (int)copyGrid.VMf, time, max_value);
                var new_time = new VMTime(copyGrid, time[0], time[1], time[2]);
                VMTimes.Add(new_time);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void AddVMAccuracy(VMGrid New_grid)
        {
            try
            {
                VMGrid copyGrid = new VMGrid(New_grid.N, New_grid.Min, New_grid.Max, New_grid.VMf);
                int[] time = new int[3];
                double[] max_value = new double[3];
                vmd(copyGrid.N, copyGrid.Min, copyGrid.Max, (int)copyGrid.VMf, time, max_value);
                var new_accur = new VMAccuracy(copyGrid, max_value[0], max_value[1], max_value[2]);
                VMAccuracies.Add(new_accur);

            }
            catch(Exception ex)
            {
                throw;
            }
        }
        public double Time_HA_base_min
        {
            get
            {
                var numDataItem = from i in VMTimes select i;
                if (numDataItem != null && numDataItem.Any())
                    return (numDataItem.Aggregate((i1, i2) => i1.Time_HA_base < i2.Time_HA_base ? i1 : i2)).Time_HA_base;
                else
                    return -1;
            }
        }
        public double Time_HA_base_max
        {
            get 
            {
                var numDataItem = from i in VMTimes select i;
                if (numDataItem != null && numDataItem.Any())
                    return (numDataItem.Aggregate((i1, i2) => i1.Time_HA_base > i2.Time_HA_base ? i1 : i2)).Time_HA_base;
                else
                    return -1;
            }
        }
    }
}