using System;
using System.Data;
using System.Windows;
using Oracle.ManagedDataAccess.Client;

namespace ProiectDB
{

    public partial class InregistrareWindow : Window
    {
        public InregistrareWindow()
        {
            InitializeComponent();
        }

        private void InregistrareUtilizator_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNume.Text) ||
                string.IsNullOrWhiteSpace(txtPrenume.Text) ||
                string.IsNullOrWhiteSpace(txtUsername.Text) ||
                !dpDataNasterii.SelectedDate.HasValue ||
                string.IsNullOrWhiteSpace(txtCNP.Text) ||
                string.IsNullOrWhiteSpace(txtNrTel.Text))
            {
                MessageBox.Show("Completați toate câmpurile înainte de a înregistra utilizatorul.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string connectionString = "User Id=bd013;Password=bd013;Data Source=81.180.214.85:1539/orcl;";

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "INSERT INTO users (nume, prenume, username, data_nasterii, cnp, nr_tel) VALUES (:nume, :prenume, :username, :dataNasterii, :cnp, :nrTel)";

                    using (OracleCommand cmd = new OracleCommand(query, connection))
                    {
                        cmd.Parameters.Add("nume", OracleDbType.Varchar2).Value = txtNume.Text;
                        cmd.Parameters.Add("prenume", OracleDbType.Varchar2).Value = txtPrenume.Text;
                        cmd.Parameters.Add("username", OracleDbType.Varchar2).Value = txtUsername.Text;
                        cmd.Parameters.Add("dataNasterii", OracleDbType.Date).Value = dpDataNasterii.SelectedDate ?? DateTime.MinValue;
                        cmd.Parameters.Add("cnp", OracleDbType.Varchar2).Value = txtCNP.Text;
                        cmd.Parameters.Add("nrTel", OracleDbType.Varchar2).Value = txtNrTel.Text;

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Utilizator înregistrat cu succes!");

                        txtNume.Text = string.Empty;
                        txtPrenume.Text = string.Empty;
                        txtUsername.Text = string.Empty;
                        dpDataNasterii.SelectedDate = null;
                        txtCNP.Text = string.Empty;
                        txtNrTel.Text = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la înregistrare: " + ex.Message);
                }
            }
        }

        private void InregistrareIesire_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
