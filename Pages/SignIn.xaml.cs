using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using System.Data;
using System.Data.SqlClient;
using HeartbeatApp.Classes;


namespace HeartbeatApp.Pages
{
    /// <summary>
    /// Interaction logic for SignIn.xaml
    /// </summary>
    public partial class SignIn : Page
    {
        private enum Process
        {
            None = 0,
            LogInClient = 1,
            RegisterClient = 2
        }

        public SignIn()
        {
            InitializeComponent();
            LoadScreen(Process.None);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string tempUsername = txtUsername.Text;
            string tempPassword = txtPassword.Text;
            using (Database.con)
            {
                Database.openConnection();
                Database.sql =
                    "Select * from ClientAuthenticate CA where CA.Username like '" + tempUsername + "' and CA.Password like '" + tempPassword + "'";
                Database.cmd.CommandType = CommandType.Text;
                Database.cmd.CommandText = Database.sql;

                Database.dr = Database.cmd.ExecuteReader();

                if (Database.dr.HasRows)
                {
                    while (Database.dr.Read())
                    {
                        string name = Database.dr["Username"].ToString();
                        int id = Database.dr.GetInt32(2);

                        if (name == tempUsername)
                        {
                            User u = new User() { userName = name, clientID = id};
                            MessageBox.Show("Login Successful", "Logged In", MessageBoxButton.OK, MessageBoxImage.Information);

                            ContinueForward(u);
                        }
                        else
                        {
                            MessageBox.Show("Username or Password is not Recognised", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                    MessageBox.Show("Username or Password is not Recognised", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);

                Database.dr.Close();
                Database.closeConnection();
            }
        }

        private void RegisterNewClient(object sender, RoutedEventArgs e)
        {
            int height = (int.Parse(txtHeight.Text));
            int weight = (int.Parse(txtWeight.Text));

            int newID = 1;

            using (Database.con)
            {
                Database.openConnection();
                Database.sql = "SELECT MAX(Clients.ID) AS LastID FROM Clients";
                Database.cmd.CommandType = CommandType.Text;
                Database.cmd.CommandText = Database.sql;

                Database.dr = Database.cmd.ExecuteReader();

                while (Database.dr.Read())
                    newID = Database.dr.GetInt32(0) + 1;

                Database.cmd.CommandText = "insert into Clients (ID, Forename, LastName, DateOfBirth, Height, Weight) values(" +
                    newID + ", '" + txtClientName.Text + "', '" + txtPassCode.Text + "', '" + BirthDate.SelectedDate + "'," + height + "," + weight + ")";

                MessageBox.Show("Register Successful", "Congrats", MessageBoxButton.OK, MessageBoxImage.Information);
                //ContinueForward();

                Database.dr.Close();
            }
        }

        private void ContinueForward(User user)
        {
            Diary diaryPage = new Diary();
            this.NavigationService.Navigate(diaryPage, user);
        }

        #region Text Events
        private void TxtUsername_MouseEnter(object sender, MouseEventArgs e)
        {
            if (txtUsername.Text.Equals(@"Username / Email"))
            {
                txtUsername.Text = "";
            }
        }

        private void TxtUsername_MouseLeave(object sender, MouseEventArgs e)
        {
            if (txtUsername.Text.Equals(""))
            {
                txtUsername.Text = @"Username / Email";
            }
        }

        #endregion

        #region Load Events
        private void LoadLogIn(object sender, RoutedEventArgs e)
        {
            LoadScreen(Process.LogInClient);
        }

        private void LoadSignUp(object sender, RoutedEventArgs e)
        {
            LoadScreen(Process.RegisterClient);
        }

        private void LoadScreen(Process pr)
        {
            switch (pr)
            {
                case Process.None:
                    MainGrid.Visibility = Visibility.Visible;
                    gridLogin.Visibility = Visibility.Hidden;
                    gridRegister.Visibility = Visibility.Hidden;
                    break;
                case Process.RegisterClient:
                    MainGrid.Visibility = Visibility.Hidden;
                    gridLogin.Visibility = Visibility.Hidden;
                    gridRegister.Visibility = Visibility.Visible;
                    break;
                case Process.LogInClient:
                    MainGrid.Visibility = Visibility.Hidden;
                    gridLogin.Visibility = Visibility.Visible;
                    gridRegister.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void LoadStart(object sender, RoutedEventArgs e)
        {
            LoadScreen(Process.None);
        }

        #endregion
    }
}
