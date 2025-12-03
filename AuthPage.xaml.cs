using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Murzayanov41
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private Random random = new Random();

        private string CaptchaText = "";

        private int[] CaptchaIntArray = new int[62];
        private void FillArray()
        {
            int j = 0;

            for (int i=48;i<=57;i++)
            {
                CaptchaIntArray[j] = i;
                j++;
            }
            for (int i = 65; i<=90; i++)
            {
                CaptchaIntArray[j] = i;
                j++;
            }
            for (int i = 97; i<=122; i++)
            {
                CaptchaIntArray[j] = i;
                j++;
            }
        }

        private string DoCaptcha()
        {
            CaptchaOne.Text = Convert.ToChar(CaptchaIntArray[random.Next(62)]) + "";
            CaptchaTwo.Text = Convert.ToChar(CaptchaIntArray[random.Next(62)]) + "";
            CaptchaThree.Text = Convert.ToChar(CaptchaIntArray[random.Next(62)]) + "";
            CaptchaFour.Text = Convert.ToChar(CaptchaIntArray[random.Next(62)]) + "";

            return CaptchaOne.Text + CaptchaTwo.Text + CaptchaThree.Text + CaptchaFour.Text;
        }

        public AuthPage()
        {
            InitializeComponent();
            FillArray();
        }

        private void LoginGuestButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new ProductPage(null));
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            string login = LoginTBox.Text;
            string password = PasswordTBox.Text;

            if (login == "" || password == "")
            {
                MessageBox.Show("Есть пустые поля");
                return;
            }

            User user = MurzayanovEntities.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);

            if (CaptchaText != "")
            {
                if (CaptchaAnswer.Text != CaptchaText)
                {
                    errors.AppendLine("Неверно введена Captcha!");

                    CaptchaText = DoCaptcha();

                    LoginButton.IsEnabled = false;
                }
            }

            if (user != null)
            {
                if (CaptchaText != "")
                {
                    if (CaptchaAnswer.Text == CaptchaText)
                    {
                        Manager.MainFrame.Navigate(new ProductPage(user));
                        LoginTBox.Text = "";
                        PasswordTBox.Text = "";
                        CaptchaText = "";

                        CaptchaPanel.Visibility = Visibility.Hidden;
                    }
                }
                else
                {
                    Manager.MainFrame.Navigate(new ProductPage(user));
                    LoginTBox.Text = "";
                    PasswordTBox.Text = "";
                }
            }
            else
            {
                errors.AppendLine("Введены неверные данные");
                
                CaptchaPanel.Visibility = Visibility.Visible;

                if (CaptchaText == "")
                {
                    CaptchaText = DoCaptcha();
                }
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());

                if (LoginButton.IsEnabled)
                {
                    await Task.Delay(10000);
                    LoginButton.IsEnabled = true;
                }
            }
        }
    }
}
