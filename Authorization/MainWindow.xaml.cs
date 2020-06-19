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
using System.Security.Cryptography;
using Microsoft.Win32;

namespace Authorization
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Dictionary<string, string>> users = new List<Dictionary<string, string>>(); 

        public MainWindow()
        {
            InitializeComponent();
            // создание и добавление в список двух пользователей
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["login"] = "admin";
            dict["password"] = "admin";
            using (MD5 md5Hash = MD5.Create())
            {
                dict["password"] = GetMd5Hash(md5Hash
                    , dict["password"]);
            }
            users.Add(dict);
            dict = new Dictionary<string, string>();
            dict["login"] = "guest";
            dict["password"] = "guest";
            using (MD5 md5Hash = MD5.Create())
            {
                dict["password"] = GetMd5Hash(md5Hash
                    , dict["password"]);
            }
            users.Add(dict);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey system = Registry
                                        .LocalMachine
                                        .OpenSubKey("HARDWARE")
                                        .OpenSubKey("DESCRIPTION")
                                        .OpenSubKey("System");
                string Identifier = system.GetValue("Identifier").ToString();                           // получение Identifier
                string SystemBiosDate = system.GetValue("SystemBiosDate").ToString();                   // получение SystemBiosDate
                string VideoBiosDate = system.GetValue("VideoBiosDate").ToString();                     // получение VideoBiosDate
                string rez = Identifier + SystemBiosDate + VideoBiosDate;
                //  Software\Authorization\Settings
                RegistryKey settings = Registry
                    .CurrentUser
                    .OpenSubKey("Software")
                    .OpenSubKey("Authorization")
                    .OpenSubKey("Settings");
                string key = settings.GetValue("Key").ToString();                                       // получение key
                if (rez != key)
                {
                    MessageBox.Show(
                        "Обратитесь к разработчику программы"
                        , "Error"
                        , MessageBoxButton.OK
                        , MessageBoxImage.Warning
                        );
                    Environment.Exit(0);
                }
            }
            catch
            {
                MessageBox.Show(
                        "Обратитесь к разработчику программы"
                        , "Error"
                        , MessageBoxButton.OK
                        , MessageBoxImage.Warning
                        );
                Environment.Exit(0);
            }
        }

        private void authorization_Click(object sender, RoutedEventArgs e)
        {
            string login_value = login.Text
                .ToString();
            string password_value = password.Password
                .ToString();
            if (login_value != "" && password_value != "")
            {
                bool check = false;
                foreach (Dictionary<string, string> dict in users)
                {
                    if (dict["login"] == login_value)
                        using (MD5 md5Hash = MD5.Create())
                        {
                            if (dict["password"] == GetMd5Hash(md5Hash, password_value))
                            {
                                check = true;
                                MessageBox.Show(
                                    "Вы успешно зарегистрировались!"
                                    ,"Success"
                                    ,MessageBoxButton.OK
                                    ,MessageBoxImage.Information);
                                // можно вставить переход на др окно
                            }
                            else
                            {
                                check = true;
                                MessageBox.Show(
                                    "Вы ввели неверный пароль!"
                                    , "Error"
                                    , MessageBoxButton.OK
                                    , MessageBoxImage.Error);
                            }
                        }
                }
                if (!check)
                    MessageBox.Show(
                                    "Пользователя с таким логином нет!"
                                    , "Error"
                                    , MessageBoxButton.OK
                                    , MessageBoxImage.Error);
            }
            else
                MessageBox.Show(
                    "Вы не заполнили одно или несколько полей!"
                    , "Error"
                    , MessageBoxButton.OK,
                    MessageBoxImage.Error);
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i]
                    .ToString("x2"));
            }
            // Return the hexadecimal string.
            return sBuilder
                .ToString();
        }
    }
}
