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
    /// Логика взаимодействия для Autorization.xaml
    /// </summary>
    public partial class Autorization : Page
    {
        private int countUnsuccessful = 0; //Количество неверных попыток входа
        public Autorization()
        {
            InitializeComponent();

            txtCaptcha.Visibility = Visibility.Hidden; //Скрытие надписи
            textBlockCaptcha.Visibility = Visibility.Hidden; //Скрытие текствого поля
        }

        private void btnEnterGuest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Client(null)); //переход на страницу клиенты (как гость)
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim(); //объявление переменной, в которую будут записываться значение с textBox
            string password = txtPassword.Text.Trim(); //объявление переменной, в которую будут записываться значение с textBox

            User user = new User(); //Создаем пустой объект пользователя

            user = Controller.Connect.GetContext().User.Where(p => p.UserLogin == login && p.UserPassword == password).FirstOrDefault(); //Условие на нахождение пользователя с паролем и логином
            int userCount = Controller.Connect.GetContext().User.Where(p => p.UserLogin == login && p.UserPassword == password).Count(); //Находим количество пользователей

            if (countUnsuccessful <1)
            {
                if (userCount > 0)
                {
                    MessageBox.Show("вы вошли под: " + user.Role.RoleName.ToString());
                    LoadForm(user.Role.RoleName.ToString(), user);
                }
                else
                {
                    MessageBox.Show("Вы ввели неврно логин или пароль!");
                    countUnsuccessful++;
                    if (countUnsuccessful == 1) // Если неверных попыток равно 1
                        GenerateCaptcha(); // Генерируем капчу
                }
            }
            else
            {
                if (userCount > 0 && textBlockCaptcha.Text == txtCaptcha.Text)
                {
                    MessageBox.Show("Вы вошли под: " + user.Role.RoleName.ToString());
                    LoadForm(user.Role.RoleName.ToString(), user);
                }
                else
                {
                    MessageBox.Show("Введите данные заново!");
                }
            }
        }

        private void GenerateCaptcha()
        {
            txtCaptcha.Visibility = Visibility.Visible;
            textBlockCaptcha.Visibility = Visibility.Visible;
            Random random = new Random();
            int randmNum = random.Next(0, 3);

            switch (randmNum)
            {
                case 1:
                    textBlockCaptcha.Text = "Ju2s";
                    textBlockCaptcha.TextDecorations = TextDecorations.Strikethrough;
                    break;
                case 2:
                    textBlockCaptcha.Text = "INK2";
                    textBlockCaptcha.TextDecorations = TextDecorations.Strikethrough;
                    break;
                case 3:
                    textBlockCaptcha.Text = "Gul2";
                    textBlockCaptcha.TextDecorations = TextDecorations.Strikethrough;
                    break;
            }
        }


        private void LoadForm(string _role, User user)
        {
            switch (_role)
            {
                case "Клиент":
                    NavigationService.Navigate(new Client(user));
                    break;
                case "Менеджер":
                    NavigationService.Navigate(new Client(user));
                    break;
                case "Администратор":
                    NavigationService.Navigate(new Admin(user));
                    break;
            }
        }
    }
}
