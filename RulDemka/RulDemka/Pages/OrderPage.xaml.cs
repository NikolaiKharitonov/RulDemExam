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



namespace RulDemka.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrderPage.xaml
    /// </summary>
    public partial class OrderPage : Page
    {
        List<Product> productList = new List<Product>(); //Создаем пустой лист, к которому можно будет обращаться во всех методах
        public OrderPage(List<Product> products, User user)
        {
            InitializeComponent();

            DataContext = this; // Привязываем контекст данных к коду
            productList = products; // Передаем список с товарми в пустой лист
            lViewOrder.ItemsSource = productList; // Выводим список выбранных товаров

            cmbPickupPoint.ItemsSource = Controller.Connect.GetContext().PickupPoint.ToList(); // Выводим в ComboBox список пунктов выдачи

            if (user != null)   // Добавляем проверку на пользователя, если пользователь есть в системе
                txtUser.Text = user.UserSurname.ToString() + user.UserName.ToString() + " " + user.UserPatronymic.ToString(); // Выводим в TextBox ФИО
        }

        public string Total
        {
            get
            {
                var total = productList.Sum(p => Convert.ToDouble(p.ProductCost) - Convert.ToDouble(p.ProductCost) * Convert.ToDouble(p.ProductDiscountAmount / 100.00));
                return total.ToString();
            }
        }


        private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить этот элемент?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                productList.Remove(lViewOrder.SelectedItem as Product);
        }

        private void btnOrderSave_Click(object sender, RoutedEventArgs e)
        {
            var productArticle = productList.Select(p => p.ProductArticleNumber).ToArray(); // Производим поиск товаров по артиклю, добавляя каждый отдельным элементам массива
            Random random = new Random(); // Добавляем рандом, для добавления случайного числа в поле "Код получения"
            var date = DateTime.Now; // Добавляем переменную, в которой хранится сегодняшня дата
            if (productList.Any(p => p.ProductQuantityInStock < 3))
                date = date.AddDays(6);
            else
                date = date.AddDays(3);

            if (cmbPickupPoint.SelectedItem == null) // Реализуем проверку на невозможность
            {
                MessageBox.Show("Выберите пункт выдачи!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information); // Добавление записи в БД
                return;
            }

             try
             {
                 Order newOrder = new Order()
                 {
                     OrderStatus = "Новый",
                     OrderDate = DateTime.Now,
                     OrderPickupPoint = cmbPickupPoint.SelectedIndex + 1,
                     OrderDeliveryDate = date,
                     ReceiptCode = random.Next(100, 1000),
                     ClientFullName = txtUser.Text,
                 };
                Controller.Connect.GetContext().Order.Add(newOrder); // Передаем добавленные данные в таблицу "заказ"

                  for (int i = 0; i < productArticle.Count(); i++) // Добавлеям счетчик, который будет добавлять записи до того момента, как не закончатся артикулы
                 {
                     OrderProduct newOrderProduct = new OrderProduct()
                     {
                         OrderID = newOrder.OrderID,
                         ProductArticleNumber = productArticle[i], // В пустой обьект добавляем данные
                         ProductCount = 1
                     };
                    Controller.Connect.GetContext().OrderProduct.Add(newOrderProduct);

                 }

                Controller.Connect.GetContext().SaveChanges(); // сохраняем добавленные в БД записи
                 MessageBox.Show("Заказ оформлен!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                 NavigationService.Navigate(new OrderTicketPage(newOrder, productList)); // переходим на страницу талона заказа
             }
             catch (Exception ex)
             {
                 MessageBox.Show(ex.Message.ToString()); // Если есть какие-то ошибки, выводим их
             }
        }
    }
}
