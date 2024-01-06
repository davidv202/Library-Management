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
            string username = tbUsername.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Introduceți un username valid pentru verificare.");
                return;
            }

            if (lvCartiDisponibile.SelectedItems.Count == 0)
            {
                MessageBox.Show("Selectați cel puțin o carte pentru împrumut.");
                return;
            }

            string connectionString = "User Id=bd013;Password=bd013;Data Source=81.180.214.85:1539/orcl;";

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleTransaction transaction = null;

                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    // Verificăm dacă username-ul există în tabela users și are împrumuturi disponibile
                    string userIdQuery = "SELECT id, nr_imprumuturi FROM users WHERE username = :username AND nr_imprumuturi >= :numar_carti FOR UPDATE";
                    using (OracleCommand userIdCmd = new OracleCommand(userIdQuery, connection))
                    {
                        userIdCmd.Parameters.Add(new OracleParameter("username", username));
                        userIdCmd.Parameters.Add(new OracleParameter("numar_carti", lvCartiDisponibile.SelectedItems.Count));

                        using (OracleDataReader userReader = userIdCmd.ExecuteReader())
                        {
                            if (userReader.Read())
                            {
                                int userId = userReader.GetInt32(0);
                                int numarImprumuturi = userReader.GetInt32(1);

                                if (numarImprumuturi < lvCartiDisponibile.SelectedItems.Count)
                                {
                                    MessageBox.Show("Nu aveți împrumuturi disponibile suficiente pentru toate cărțile selectate.");
                                    transaction.Rollback();
                                    return;
                                }

                                // Actualizăm numărul de împrumuturi în tabela users
                                string updateUserQuery = "UPDATE users SET nr_imprumuturi = :numar_imprumuturi WHERE id = :user_id";
                                using (OracleCommand updateCmd = new OracleCommand(updateUserQuery, connection))
                                {
                                    updateCmd.Parameters.Add(new OracleParameter("numar_imprumuturi", numarImprumuturi - lvCartiDisponibile.SelectedItems.Count));
                                    updateCmd.Parameters.Add(new OracleParameter("user_id", userId));

                                    updateCmd.ExecuteNonQuery();

                                    // Inserăm datele în tabela imprumuturi utilizând secvența pentru id
                                    string imprumutInsertQuery = "INSERT INTO imprumuturi (imprumut_id, titlu, autor, user_id) VALUES (imprumuturi_seq.NEXTVAL, :titlu, :autor, :id_user)";
                                    foreach (Carte carteSelectata in lvCartiDisponibile.SelectedItems)
                                    {
                                        using (OracleCommand imprumutInsertCmd = new OracleCommand(imprumutInsertQuery, connection))
                                        {
                                            imprumutInsertCmd.Parameters.Add(new OracleParameter("titlu", carteSelectata.Titlu));
                                            imprumutInsertCmd.Parameters.Add(new OracleParameter("autor", carteSelectata.Autor));
                                            imprumutInsertCmd.Parameters.Add(new OracleParameter("id_user", userId));

                                            imprumutInsertCmd.ExecuteNonQuery();
                                        }

                                        string updateCartiQuery = "UPDATE carti SET status = 'Indisponibila' WHERE titlu = :titlu AND autor = :autor";
                                        using (OracleCommand updateCartiCmd = new OracleCommand(updateCartiQuery, connection))
                                        {
                                            updateCartiCmd.Parameters.Add(new OracleParameter("titlu", carteSelectata.Titlu));
                                            updateCartiCmd.Parameters.Add(new OracleParameter("autor", carteSelectata.Autor));

                                            updateCartiCmd.ExecuteNonQuery();
                                        }
                                    }
                                }

                                // Commit tranzacția după ce s-au efectuat toate împrumuturile
                                transaction.Commit();
                                MessageBox.Show("Cărțile au fost împrumutate cu succes.");
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Username-ul introdus nu există sau nu are împrumuturi disponibile suficiente.");
                                transaction.Rollback();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la împrumutarea cărților: " + ex.Message);
                    transaction?.Rollback();
                }
                finally
                {
                    transaction?.Dispose();
                }
            }
        }


        private void Inapoi_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
