using Oracle.ManagedDataAccess.Client;
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
using System.Windows.Shapes;

namespace ProiectDB
{
    /// <summary>
    /// Interaction logic for VerificareUser.xaml
    /// </summary>
    public partial class VerificareUser : Window
    {
        public bool RezultatVerificare { get; private set; }
        public VerificareUser()
        {
            InitializeComponent();
        }

        private void Verifica_Click(object sender, RoutedEventArgs e)
        {
            string usernameVerificat = txtUsername.Text.Trim();

            if (string.IsNullOrWhiteSpace(usernameVerificat))
            {
                MessageBox.Show("Introduceți un username pentru verificare.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string connectionString = "User Id=bd013;Password=bd013;Data Source=81.180.214.85:1539/orcl;";

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM users WHERE username = :username";

                    using (OracleCommand cmd = new OracleCommand(query, connection))
                    {
                        cmd.Parameters.Add("username", OracleDbType.Varchar2).Value = usernameVerificat;

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                        {
                            RezultatVerificare = true; 
                            this.Close();
                        }
                        else
                        {
                            RezultatVerificare = false;
                            MessageBox.Show($"Username-ul '{usernameVerificat}' nu există în baza de date.", "Verificare eșuată", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }

                        txtUsername.Text= string.Empty; 
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la verificare: " + ex.Message);
                }
            }
        }

        private void Inapoi_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }


}
