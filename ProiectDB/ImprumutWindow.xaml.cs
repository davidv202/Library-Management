using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace ProiectDB
{
    public partial class ImprumutWindow : Window
    {
        public ObservableCollection<Carte> CartiDisponibile { get; set; }

        public ImprumutWindow()
        {
            InitializeComponent();
            CartiDisponibile = new ObservableCollection<Carte>();
            LoadCartiDisponibile();
            lvCartiDisponibile.ItemsSource = CartiDisponibile;
        }

        private void LoadCartiDisponibile()
        {
            string connectionString = "User Id=bd013;Password=bd013;Data Source=81.180.214.85:1539/orcl;";

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT titlu, autor, status FROM carti WHERE status='Disponibila'";

                    using (OracleCommand cmd = new OracleCommand(query, connection))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CartiDisponibile.Add(new Carte(reader["titlu"].ToString(), reader["autor"].ToString(), reader["status"].ToString()));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la încărcarea cărților disponibile: " + ex.Message);
                }
            }
        }

        private void Imprumuta_Click(object sender, RoutedEventArgs e)
        {
          
        }

        private void Inapoi_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
