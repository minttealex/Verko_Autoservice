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

namespace Verko_Autoservice
{
    /// <summary>
    /// Логика взаимодействия для SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        private Service _currentService = new Service();

        public SignUpPage(Service SelectedService)
        {
            InitializeComponent();
            if (SelectedService != null)
                this._currentService = SelectedService;

            DataContext = _currentService;

            var _currentCLient = VerkoAutoserviceEntities.GetContext().Client.ToList();
            ComboClient.ItemsSource = _currentCLient;
        }

        private ClientService _currentClientService = new ClientService();

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (ComboClient.SelectedItem == null)
                errors.AppendLine("Укажите ФИО клиента");

            if (StartDate.Text == "")
                errors.AppendLine("Укажите дату услуги");

            if (TBStart.Text == "")
                errors.AppendLine("Укажите время начала услуги");

            if (TBEnd.Text == "")
                errors.AppendLine("Укажите время окончания услуги");

            if (hasErrors)
                errors.AppendLine("Исправьте ошибки во вводе времени");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            _currentClientService.ClientID = ComboClient.SelectedIndex + 1;
            _currentClientService.ServiceID = _currentService.ID;
            _currentClientService.StartTime = Convert.ToDateTime(StartDate.Text + " " + TBStart.Text);

            if (_currentClientService.ID == 0)
                VerkoAutoserviceEntities.GetContext().ClientService.Add(_currentClientService);

            try
            {
                VerkoAutoserviceEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private bool hasErrors = true;

        private void TBStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = TBStart.Text;
            hasErrors = true;

            if (s.Length == 2)
            {
                if (!int.TryParse(s, out int startHour) || startHour < 0 || startHour > 23)
                {
                    TBEnd.Text = "";
                    return;
                }

                int startMin = 0;

                int sum = (startHour * 60) + startMin + _currentService.Duration;
                int endHour = sum / 60;
                int endMin = sum % 60;

                endHour %= 24;

                TBEnd.Text = $"{endHour:D2}:{endMin:D2}";
                hasErrors = false;
            }
            else if (s.Length == 5 && s.Contains(':') && s.IndexOf(':') == 2)
            {
                string[] start = s.Split(':');

                if (!int.TryParse(start[0], out int startHour) || !int.TryParse(start[1], out int startMin))
                {
                    TBEnd.Text = "";
                    return;
                }

                if (startHour < 0 || startHour > 23 || startMin < 0 || startMin > 59)
                {
                    TBEnd.Text = "";
                    return;
                }

                int sum = (startHour * 60) + startMin + _currentService.Duration;
                int endHour = sum / 60;
                int endMin = sum % 60;

                endHour %= 24;

                TBEnd.Text = $"{endHour:D2}:{endMin:D2}"; // чтобы отображался 0 перед однозначным числом
                hasErrors = false;

                /*s = endHour.ToString() + ":" + endMin.ToString();
                TBEnd.Text = s;*/
            }
            else { TBEnd.Text = ""; }
        }
    }
}
