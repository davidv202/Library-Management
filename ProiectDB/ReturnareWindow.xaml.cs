using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ReturnareWindow.xaml
    /// </summary>
    public partial class ReturnareWindow : Window
    {
        public ObservableCollection<CarteImprumutata> CartiDetinute { get; set; }

        public ReturnareWindow()
        {
            InitializeComponent();
            CartiDetinute = new ObservableCollection<CarteImprumutata>();
            lvCartiDisponibile.ItemsSource = CartiDetinute;
        }

        private void LoadCartiDetinute()
        {
            string connectionString = "User Id=bd013;Password=bd013;Data Source=81.180.214.85:1539/orcl;";

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string username = tbUsername.Text;

                    if (string.IsNullOrWhiteSpace(username))
                    {
                        MessageBox.Show("Introduceți un username valid pentru verificare.");
                        return;
                    }

                    string getUserIdQuery = "SELECT id FROM users WHERE username = :username";
                    using (OracleCommand getUserIdCmd = new OracleCommand(getUserIdQuery, connection))
                    {
                        getUserIdCmd.Parameters.Add(new OracleParameter("username", username));

                        object userIdObj = getUserIdCmd.ExecuteScalar();

                        if (userIdObj != null && int.TryParse(userIdObj.ToString(), out int userId))
                        {
                            string query = "SELECT titlu, autor FROM imprumuturi WHERE user_id = :userId";
                            using (OracleCommand cmd = new OracleCommand(query, connection))
                            {
                                cmd.Parameters.Add(new OracleParameter("userId", userId));

                                using (OracleDataReader reader = cmd.ExecuteReader())
                                {
                                    CartiDetinute.Clear();

                                    while (reader.Read())
                                    {
                                        CartiDetinute.Add(new CarteImprumutata(reader["titlu"].ToString(), reader["autor"].ToString()));
                                    }
                                }
                            }

                            if (CartiDetinute.Count == 0)
                            {
                                MessageBox.Show("Nu exista carti imprumutate pentru username-ul introdus!");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Username-ul introdus nu există sau nu este valid.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la încărcarea cărților disponibile: " + ex.Message);
                }
            }
        }

        private void Returneaza_Click(object sender, RoutedEventArgs e)
        {
            string username = tbUsername.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Introduceți un username valid pentru verificare.");
                return;
            }

            if (lvCartiDisponibile.SelectedItems.Count == 0)
            {
                MessageBox.Show("Selectați cel puțin o carte pentru retur.");
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
                    string userIdQuery = "SELECT id, nr_imprumuturi FROM users WHERE username = :username";
                    using (OracleCommand userIdCmd = new OracleCommand(userIdQuery, connection))
                    {
                        userIdCmd.Parameters.Add(new OracleParameter("username", username));

                        using (OracleDataReader userReader = userIdCmd.ExecuteReader())
                        {
                            if (userReader.Read())
                            {
                                int userId = userReader.GetInt32(0);
                                int numarImprumuturi = userReader.GetInt32(1);

                                string updateUserQuery = "UPDATE users SET nr_imprumuturi = :numar_imprumuturi WHERE id = :user_id";
                                using (OracleCommand updateCmd = new OracleCommand(updateUserQuery, connection))
                                {
                                    updateCmd.Parameters.Add(new OracleParameter("numar_imprumuturi", numarImprumuturi + lvCartiDisponibile.SelectedItems.Count));
                                    updateCmd.Parameters.Add(new OracleParameter("user_id", userId));

                                    updateCmd.ExecuteNonQuery();

                                    string imprumutInsertQuery = "DELETE FROM imprumuturi WHERE user_id=:user_id AND titlu=:titlu AND autor=:autor";
                                    foreach (CarteImprumutata carteSelectata in lvCartiDisponibile.SelectedItems)
                                    {
                                        using (OracleCommand imprumutInsertCmd = new OracleCommand(imprumutInsertQuery, connection))
                                        {
                                            imprumutInsertCmd.Parameters.Add(new OracleParameter("user_id", userId));
                                            imprumutInsertCmd.Parameters.Add(new OracleParameter("titlu", carteSelectata.Titlu));
                                            imprumutInsertCmd.Parameters.Add(new OracleParameter("user_id", carteSelectata.Autor));

                                            imprumutInsertCmd.ExecuteNonQuery();
                                        }

                                        string updateCartiQuery = "UPDATE carti SET status = 'Disponibila' WHERE titlu = :titlu AND autor = :autor";
                                        using (OracleCommand updateCartiCmd = new OracleCommand(updateCartiQuery, connection))
                                        {
                                            updateCartiCmd.Parameters.Add(new OracleParameter("titlu", carteSelectata.Titlu));
                                            updateCartiCmd.Parameters.Add(new OracleParameter("autor", carteSelectata.Autor));

                                            updateCartiCmd.ExecuteNonQuery();
                                        }
                                    }
                                }

                                transaction.Commit();
                                MessageBox.Show("Cărțile au fost returnate cu succes!");
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Username-ul introdus nu există!");
                                transaction.Rollback();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la returnarea cărților: " + ex.Message);
                    transaction?.Rollback();
                }
                finally
                {
                    transaction?.Dispose();
                }
            }
        }

        private void Verifica_Click(object sender, RoutedEventArgs e)
        {
            LoadCartiDetinute();
        }

        private void Inapoi_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
