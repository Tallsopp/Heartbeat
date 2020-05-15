using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using System.Data;
using System.Data.SqlClient;
using HeartbeatApp.Classes;
using System.CodeDom;
using System.Windows.Navigation;

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

        User u = new User();

        public SignIn()
        {
            InitializeComponent();
            LoadScreen(Process.None);
        }

        // Login to the App and connect to the server
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string tempUsername = txtUsername.Text;
            string tempPassword = txtPassword.Text;

            try
            {
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
                                u.LogIn(tempUsername, tempPassword);

                                Database.dr.Close();
                                Database.closeConnection();

                                ContinueForward();
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
            catch (Exception ex)
            {
                Console.WriteLine("Could not connect to the SQL Server! :" + ex.Message);
            }
        }

        // Register a new profile and insert into the server
        private void RegisterNewClient(object sender, RoutedEventArgs e)
        {
            try
            {
                // Makes a new User, assigns values to be passed into Stored Procedure
                u.Register(txtUser.Text, txtPassCode.Text, txtFirst.Text, txtLast.Text, (DateTime)BirthDate.SelectedDate, txtEmail.Text);

                using (Database.con)
                {
                    Database.openConnection();

                    Database.sql =
                        "EXEC RegisterNewClient @Forename= '" + u.Forename + "', @Surname= '" + u.LastName +
                        "', @DateOfBirth= '" + u.DateOfBirth.ToString("yyyy/MM/dd") + "', @EmailAddress= '" + u.EmailAddress +
                        "', @Username= '" + u.userName + "', @Password= '" + u.Password + "'";

                    Database.cmd.CommandType = CommandType.Text;
                    Database.cmd.CommandText = Database.sql;

                    Database.dr = Database.cmd.ExecuteReader();
                    Database.dr.Close();
                    Database.closeConnection();

                    ContinueForward();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not connect to the SQL Server! :" + ex.Message);
            }
        }

        // Connected, now entering the app via navigation
        private void ContinueForward()
        {
            Diary diaryPage = new Diary(u);
            NavigationService.Navigate(diaryPage, u);
        }

        //// Loads User data when Page has finished Loading
        private void NavigationService_LoadCompleted(object sender, NavigationEventArgs e)
        {
            u = (User)e.ExtraData;
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
