using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using WpfTodoListApp.Models;

namespace WpfTodoListApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        HttpClient client = new HttpClient();
        // ObservableCollection을 무조건 사용할 필요는 없다
        TodoItemsCollection todoItems = new TodoItemsCollection();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var comboPairs = new List<KeyValuePair<string, int>> {
                new KeyValuePair<string, int>("완료", 1),
                new KeyValuePair<string, int>("미완료", 0),
            };
            CboIsComplete.ItemsSource = comboPairs;

            // RestAPI 호출 준비
            client.BaseAddress = new System.Uri("http://localhost:5108/"); // API 서버 URL
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            GetDatas();
        }

        private async Task GetDatas()
        {
            // /api/TodoItems GET 메서드
            GrdTodoItems.ItemsSource = todoItems;
            try
            {
                // http://localhost:5108/api/TodoItems
                HttpResponseMessage? response = await client.GetAsync("/api/TodoItems");
                response.EnsureSuccessStatusCode(); // 상태코드 확인

                var items = await response.Content.ReadAsAsync<IEnumerable<TodoItem>>();
                todoItems.CopyFrom(items); // ObservableCollection으로 형변환

                await this.ShowMessageAsync("API호출", "로드 성공");

            }
            catch (Exception ex)
            { 
                await this.ShowMessageAsync("API호출 에러", ex.Message);
            }
        }

        private void InitControls()
        {
            TxtId.Text = string.Empty;
            TxtTitle.Text = string.Empty;
            DtpTodoDate.Text = string.Empty;
            CboIsComplete.Text = string.Empty;
        }

        private async void BtnInsert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var todoItem = new TodoItem
                {
                    Id = 0,
                    Title = TxtTitle.Text,
                    TodoDate = Convert.ToDateTime(DtpTodoDate.SelectedDate).ToString("yyyyMMdd"),
                    IsComplete = Convert.ToBoolean(CboIsComplete.SelectedValue),
                };
                var response = await client.PostAsJsonAsync("/api/TodoItems", todoItem);
                response.EnsureSuccessStatusCode();

                GetDatas();
                InitControls();
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("API호출 에러", ex.Message);
            }
        }

        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var todoItem = new TodoItem
                {
                    Id = Convert.ToInt32(TxtId.Text),
                    Title = TxtTitle.Text,
                    TodoDate = Convert.ToDateTime(DtpTodoDate.SelectedDate).ToString("yyyyMMdd"),
                    IsComplete = Convert.ToBoolean(CboIsComplete.SelectedValue),
                };
                var response = await client.PutAsJsonAsync($"/api/TodoItems/{todoItem.Id}", todoItem);
                response.EnsureSuccessStatusCode();

                GetDatas();
                InitControls();
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("API호출 에러", ex.Message);
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var Id = Convert.ToInt32(TxtId.Text);
                var response = await client.DeleteAsync($"/api/TodoItems/{Id}");
                response.EnsureSuccessStatusCode();

                GetDatas();
                InitControls();
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("API호출 에러", ex.Message);
            }
        }

        private async void GrdTodoItems_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                var Id = (GrdTodoItems.SelectedItem as TodoItem)?.Id; 
                if (Id == null) return;

                var response = await client.GetAsync($"/api/TodoItems/{Id}");
                response.EnsureSuccessStatusCode();

                var item = await response.Content.ReadAsAsync<TodoItem>();
                TxtId.Text = item.Id.ToString();
                TxtTitle.Text = item.Title.ToString();
                DtpTodoDate.SelectedDate = DateTime.Parse(item.TodoDate.Insert(4, "-").Insert(7, "-"));
                CboIsComplete.SelectedValue = item.IsComplete;

            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("API호출 에러", ex.Message);

            }
        }
    }
}