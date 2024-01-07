using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using Oracle.ManagedDataAccess.Client;

namespace ProiectDB
{
    public partial class CartiDisponibileWindow : Window
    {  
        public ObservableCollection<Carte> CartiDisponibile { get; set; }

        public CartiDisponibileWindow()
        {
            InitializeComponent();
            CartiDisponibile = new ObservableCollection<Carte>();
            LoadCarti();
            lvCartiDisponibile.ItemsSource = CartiDisponibile;
        }
        private void LoadCarti()
        {
            string connectionString = "User Id=bd013;Password=bd013;Data Source=81.180.214.85:1539/orcl;";

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT titlu, autor, status FROM carti";

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

        private void Inapoi_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
