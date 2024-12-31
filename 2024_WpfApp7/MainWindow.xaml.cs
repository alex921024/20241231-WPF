using LiveCharts;
using LiveCharts.Wpf;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace _2024_WpfApp7
{
    public partial class MainWindow : Window
    {
        // 預設的 URL，用於抓取空氣品質資料
        string defaultURL = "https://data.moenv.gov.tw/api/v2/aqx_p_432?api_key=e8dd42e6-9b8b-43f8-991e-b3dee723a52d&limit=1000&sort=ImportDate%20desc&format=JSON";

        // 用於儲存 AQI 資料的實體
        AQIData aqiData = new AQIData();
        List<Field> fields = new List<Field>();
        List<Record> records = new List<Record>();
        List<Record> selectedRecords = new List<Record>();

        // 用於儲存圖表的系列資料
        SeriesCollection seriesCollection = new SeriesCollection();

        public MainWindow()
        {
            InitializeComponent();
            UrlTextBox.Text = defaultURL;
        }

        // 當按下 "Get AQI" 按鈕時，進行資料抓取
        private async void GetAQIButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTextBox.Text = "抓取資料中...";

            // 非同步抓取資料
            string data = await FetchContentAsync(defaultURL);
            ContentTextBox.Text = data;

            // 反序列化資料成 AQIData 物件
            aqiData = JsonSerializer.Deserialize<AQIData>(data);
            fields = aqiData.fields.ToList();
            records = aqiData.records.ToList();
            selectedRecords = records;

            statusTextBlock.Text = $"共有 {records.Count} 筆資料";

            // 顯示 AQI 資料
            DisplayAQIData();
        }

        // 顯示 AQI 資料在 DataGrid 中
        private void DisplayAQIData()
        {
            RecordDataGrid.ItemsSource = records;

            // 取得第一筆資料
            Record record = records[0];

            // 為每個欄位建立 CheckBox
            foreach (Field field in fields)
            {
                var propertyInfo = record.GetType().GetProperty(field.id);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(record) as string;
                    if (double.TryParse(value, out double v))
                    {
                        CheckBox cb = new CheckBox
                        {
                            Content = field.info.label,
                            Tag = field.id,
                            Margin = new Thickness(3),
                            FontSize = 16,
                            FontWeight = FontWeights.Bold,
                            Width = 150
                        };
                        cb.Checked += UpdateChart;
                        cb.Unchecked += UpdateChart;
                        DataWrapPanel.Children.Add(cb);
                    }
                }
            }
        }

        // 更新圖表
        private void UpdateChart(object sender, RoutedEventArgs e)
        {
            // 清除現有的系列
            seriesCollection.Clear();

            // 根據 CheckBox 狀態更新圖表資料
            foreach (CheckBox cb in DataWrapPanel.Children)
            {
                if (cb.IsChecked == true)
                {
                    List<string> labels = new List<string>();
                    string tag = cb.Tag as string;
                    ColumnSeries columnSeries = new ColumnSeries();
                    ChartValues<double> values = new ChartValues<double>();

                    // 為選中的紀錄新增資料點
                    foreach (Record r in selectedRecords)
                    {
                        var propertyInfo = r.GetType().GetProperty(tag);
                        if (propertyInfo != null)
                        {
                            var value = propertyInfo.GetValue(r) as string;
                            if (double.TryParse(value, out double v))
                            {
                                labels.Add(r.sitename);
                                values.Add(v);
                            }
                        }
                    }
                    columnSeries.Values = values;
                    columnSeries.Title = tag;
                    columnSeries.LabelPoint = point => $"{labels[(int)point.X]}:{point.Y.ToString()}";
                    seriesCollection.Add(columnSeries);
                }
            }
            AQIChart.Series = seriesCollection;
        }

        // 非同步抓取資料內容
        private async Task<string> FetchContentAsync(string url)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(100);
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                catch (HttpRequestException e)
                {
                    MessageBox.Show($"Request exception: {e.Message}");
                    return null;
                }
            }
        }

        // 在 DataGrid 加載行時顯示行號
        private void RecordDataGrid_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        // 當 DataGrid 的選擇變更時，更新選擇的紀錄和圖表
        private void RecordDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedRecords = RecordDataGrid.SelectedItems.Cast<Record>().ToList();
            statusTextBlock.Text = $"共選擇 {selectedRecords.Count} 筆資料";
            UpdateChart(null, null);
        }
    }
}
