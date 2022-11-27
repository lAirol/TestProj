using System; 
using System.Collections.Generic;
using System.Linq;  
using System.Windows;
using System.Windows.Controls; 
using System.IO;
using Newtonsoft.Json; 
using ASPControllerPC.Models;
using System.Windows.Forms.DataVisualization.Charting;
using Series = System.Windows.Forms.DataVisualization.Charting.Series; 
using log4net;

namespace ASPControllerPC
{
    public partial class MainWindow : Window
    {        
        static DateTime date = DateTime.Today;
        static DateTime age = new DateTime(2021, 07, 08);
        static DateTime datemin = date.AddYears(-5);
        static DateTime datemax = date.AddYears(5);
        public MainWindow()
        {
            try
            {            
                InitializeComponent();
                log.Info("Приложение инициализированно");
                log4net.Config.XmlConfigurator.Configure();
                Load();                
                GraphStuff.ChartAreas.Add(new ChartArea("Main"));
                var currentSeries = new Series("Dependence")
                {
                    IsValueShownAsLabel = false
                };
               
                GraphStuff.Series.Add(currentSeries);
                ComboChartTypes.ItemsSource = Enum.GetValues(typeof(SeriesChartType));
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString()); 
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            log.Info("Кнопка получить нажата");
            try
            {
                var firstdatedt = dp1.SelectedDate.Value;
                var lastdatedt = dp2.SelectedDate.Value;
                var firstdate = dp1.SelectedDate.Value.ToString("yyyy-MM-dd");
                var lastdate = dp2.SelectedDate.Value.ToString("yyyy-MM-dd");
                Rate value = new Rate();
                Series currentSeries = GraphStuff.Series.FirstOrDefault();
                currentSeries.Points.Clear();
                List<Rate> GraphList = new List<Rate>();
                List<Rate> GraphList1 = new List<Rate>();
                List<string> OutRange = new List<string>();
                if (datemin > Convert.ToDateTime(dp1.Text) | datemax < Convert.ToDateTime(dp1.Text) | Convert.ToDateTime(dp1.Text) > Convert.ToDateTime(dp2.Text))
                {
                    log.Info("Указана неверная дата в dp1");
                    label1.Content = "Введена неверная дата";
                    label1.Foreground = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    label1.Content = "Начальная дата";
                    label1.Foreground = System.Windows.Media.Brushes.Black;
                }
                if (datemax < Convert.ToDateTime(dp2.Text) | datemin > Convert.ToDateTime(dp2.Text) | Convert.ToDateTime(dp1.Text) > Convert.ToDateTime(dp2.Text))
                {
                    log.Info("Указана неверная дата в dp2");
                    label2.Content = "Введена неверная дата";
                    label2.Foreground = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    label2.Content = "Конечная дата";
                    label2.Foreground = System.Windows.Media.Brushes.Black;

                    try
                    {
                        switch (ComboBox1.SelectedIndex)
                        {


                            case 0:  //USD
                                log.Info("Выбрана валюта USD");
                                if (Convert.ToDateTime(dp2.Text) <= age)
                                {
                                    Graph("USD.json", "145", firstdate, lastdate, firstdatedt, lastdatedt, value);
                                }
                                else
                                {
                                    Graph("USD.json", "431", firstdate, lastdate, firstdatedt, lastdatedt, value);
                                }
                                break;
                            case 1:  //EUR
                                log.Info("Выбрана валюта EUR");
                                if (Convert.ToDateTime(dp2.Text) <= age)
                                {
                                    Graph("EUR.json", "19", firstdate, lastdate, firstdatedt, lastdatedt, value);
                                }
                                else
                                {
                                    Graph("EUR.json", "451", firstdate, lastdate, firstdatedt, lastdatedt, value);
                                }
                                break;
                            case 2: //RUB
                                log.Info("Выбрана валюта RUB");
                                if (Convert.ToDateTime(dp2.Text) <= age)
                                {
                                    Graph("RUB.json", "190", firstdate, lastdate, firstdatedt, lastdatedt, value);
                                }
                                else
                                {
                                    Graph("RUB.json", "456", firstdate, lastdate, firstdatedt, lastdatedt, value);
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void Graph(string str,string numb, string firstdate, string lastdate, DateTime firstdatedt, DateTime lastdatedt, Rate value)
        {
            log.Info("Десериализация из файла");
            var jread = File.ReadAllText(str);
            var storeddata = JsonConvert.DeserializeObject<List<Rate>>(jread); 
            if (storeddata != null)
            {
                List<Rate> SortedList = storeddata.OrderBy(o => o.Date).ToList();
                foreach (var rate3 in SortedList)
                {
                    if (DateTime.Compare(rate3.Date, firstdatedt) < 0)
                    {

                    }
                    else
                    if (DateTime.Compare(rate3.Date, firstdatedt) == 0)
                    {
                        firstdatedt = firstdatedt.AddDays(+1);
                    }
                    else
                    {
                        DateTime tempdt;
                        if (DateTime.Compare(rate3.Date, lastdatedt) <= 0)
                        {

                            tempdt = rate3.Date.AddDays(-1);

                        }
                        else
                        {
                            tempdt = lastdatedt;
                        }
                        log.Info("Десериализация с сайта");
                        List<Rate> UnexsistedDataFromSite;
                        try
                        {
                            string temp1 = value.Format($"https://www.nbrb.by/API/ExRates/Rates/Dynamics/" + numb + "?startDate=" + firstdatedt.ToString("yyyy-MM-dd") + "&endDate=" + tempdt.ToString("yyyy-MM-dd"));
                            UnexsistedDataFromSite = JsonConvert.DeserializeObject<List<Rate>>(temp1);
                            foreach (var unexsist in UnexsistedDataFromSite)
                            {
                                storeddata.Add(unexsist);
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.ToString()); }
                        TimeSpan TS = tempdt - firstdatedt;
                        firstdatedt = tempdt.AddDays(TS.TotalDays + 1);
                    }
                    if (DateTime.Compare(firstdatedt, lastdatedt) > 0)
                    {
                        break;
                    }
                }
                try
                {
                    SortedList = storeddata.OrderBy(o => o.Date).ToList();
                    log.Info("Запись в файл");
                    File.WriteAllText(str, JsonConvert.SerializeObject(SortedList));
                    firstdatedt = dp1.SelectedDate.Value;
                }
                catch (Exception ex) { MessageBox.Show(ex.ToString()); }
                foreach (var rate5 in SortedList)
                {
                    if (DateTime.Compare(rate5.Date, lastdatedt) <= 0 && DateTime.Compare(rate5.Date, firstdatedt) >= 0)
                    {
                        Series currentSeries = GraphStuff.Series.FirstOrDefault();
                        currentSeries.Points.AddXY(rate5.Date.ToString("d"), rate5.Cur_OfficialRate);
                    }
                }
            }
            else
            {
                try
                {
                    log.Info("Десериализация с сайта");
                    var temp = value.Format($"https://www.nbrb.by/API/ExRates/Rates/Dynamics/" + numb + "?startDate=" + firstdate + "&endDate=" + lastdate);
                    var datafromsite = JsonConvert.DeserializeObject<List<Rate>>(temp);
                    log.Info("Запись в файл");
                    File.WriteAllText(str, JsonConvert.SerializeObject(datafromsite));
                    foreach (var rate in datafromsite)
                    {
                        Series currentSeries = GraphStuff.Series.FirstOrDefault();
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            }

        }
        private void UpateChart(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ComboChartTypes.SelectedItem is SeriesChartType currentType)
                {
                    Series currentSeries = GraphStuff.Series.FirstOrDefault();
                    currentSeries.ChartType = currentType;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void dp2_Initialized(object sender, EventArgs e)
        { 
            dp2.SelectedDate = date;
            dp2.DisplayDateEnd = datemax;
            dp2.DisplayDateStart = datemin;
            dp2.BlackoutDates.Add(new CalendarDateRange(new DateTime(1, 1, 1),datemin));
            dp2.BlackoutDates.Add(new CalendarDateRange(datemax, new DateTime(9999, 12, 31)));
        }

        private void dp1_Initialized(object sender, EventArgs e)
        {
            dp1.SelectedDate = date;
            dp1.DisplayDateEnd = datemax;
            dp1.DisplayDateStart = datemin;
            dp1.BlackoutDates.Add(new CalendarDateRange(new DateTime(1, 1, 1), datemin));
            dp1.BlackoutDates.Add(new CalendarDateRange(datemax, new DateTime(9999, 12, 31)));
        }
        private void Load()
        {
            try
            {
                if (File.Exists("USD.json") == false)
                    File.Create("USD.json");
                if (File.Exists("EUR.json") == false)
                    File.Create("EUR.json");
                if (File.Exists("RUB.json") == false)
                    File.Create("RUB.json");
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
    
}
