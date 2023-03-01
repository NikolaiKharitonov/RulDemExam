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
using System.Windows.Navigation;
using System.Windows.Shapes;
using RulDemka.Model;
using RulDemka.Windows;


namespace RulDemka.Pages
{
    /// <summary>
    /// Логика взаимодействия для Client.xaml
    /// </summary>
    public partial class Client : Page
    {
        User user = new User(); //Создаем пустой объект пользователя
        public Client(User currentUser)
        {
            InitializeComponent();

            var product = Controller.Connect.GetContext().Product.ToList(); //Обращаемся к таблице товар
            LViewProduct.ItemsSource = product; //Передаем таблицу в лист
            DataContext = this; //Привязываем контекст данных к коду, чтобы обратиться к массивам

            txtAllAmount.Text = product.Count().ToString(); //передаем количество всех записей из таблицы

            user = currentUser;

            UpdateData(); //Вызываем метод
            User();
        }

        private void User()
        {
            if (user != null)
                txtFullname.Text = user.UserSurname.ToString() + user.UserName.ToString() + " " + user.UserPatronymic.ToString();
            else
                txtFullname.Text = "Гость";
        }

        private void UpdateData()
        {
            var result = Controller.Connect.GetContext().Product.ToList(); //Вводим переменную, которая принимает данные из таблицы товаров

            if (cmbSorting.SelectedIndex == 1)                                 //Реализация сортировки
                result = result.OrderBy(p => p.ProductCost).ToList();           //С помощью запросов на сортировку по возрастанию
            if (cmbSorting.SelectedIndex == 2)                                  //И убыванию цены
                result = result.OrderByDescending(p => p.ProductCost).ToList();

            if (cmbFilter.SelectedIndex == 1)                                 
                result = result.Where(p => p.ProductDiscountAmount >= 0 && p.ProductDiscountAmount < 10).ToList();
            if (cmbFilter.SelectedIndex == 2)
                result = result.Where(p => p.ProductDiscountAmount >= 10 && p.ProductDiscountAmount < 15).ToList();
            if (cmbFilter.SelectedIndex == 3)
                result = result.Where(p => p.ProductDiscountAmount >= 15).ToList();

            result = result.Where(p => p.ProductName.ToLower().Contains(txtSearch.Text.ToLower())).ToList(); //Реализация поиска
            LViewProduct.ItemsSource = result; //Передаем результат в ListView

            txtResultAmount.Text = result.Count().ToString(); //Передаем количество записей после применение поиска, сортировки и фильтрации
        }

        public string[] SortLingList { get; set; } =
        {
            "Без сортировки",
            "Стоимость по возрастанию",
            "Стоимость по убыванию",
        };

        public string[] FilterList { get; set; } =
{
            "Все диапазоны",
            "0%-9,99%",
            "10%-14,99%",
            "15% и более"
        };

        private void cmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateData();
        }

        private void cmbSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateData();
        }
        
        private void txtSearch_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateData();
        }

        List<Product> orderProducts = new List<Product>();
        private void btnAddProduct_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            orderProducts.Add(LViewProduct.SelectedItem as Product);
            if (orderProducts.Count > 0)
            {
                btnOrder.Visibility = Visibility.Visible;
            }
        }

        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {
            OrderWindow order = new OrderWindow(orderProducts, user);
            order.ShowDialog();
        }
    }
}
