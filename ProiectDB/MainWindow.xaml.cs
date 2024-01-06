using System;
using System.Windows;

namespace ProiectDB
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Inregistrare_Click(object sender, RoutedEventArgs e)
        {
            DeschideFereastraSecundara("Inregistrare");
        }

        private void Imprumut_Click(object sender, RoutedEventArgs e)
        {
           
            DeschideFereastraSecundara("Imprumut");
            
        }

        private void Returnare_Click(object sender, RoutedEventArgs e)
        {
            
            DeschideFereastraSecundara("Returnare");
            
        }

        private void CartiDisponibile_Click(object sender, RoutedEventArgs e)
        {
            DeschideFereastraSecundara("CartiDisponibile");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void DeschideFereastraSecundara(string numeFereastra)
        {
            // Aici creezi și deschizi fereastra secundară
            switch (numeFereastra)
            {
                case "Inregistrare":
                    InregistrareWindow inregistrareWindow = new InregistrareWindow();
                    inregistrareWindow.Show();
                    break;

                case "Imprumut":
                    ImprumutWindow imprumutWindow = new ImprumutWindow();
                    imprumutWindow.Show();
                    break;

                case "Returnare":
                    ReturnareWindow returnareWindow = new ReturnareWindow();
                    returnareWindow.Show();
                    break;

                case "CartiDisponibile":
                    CartiDisponibileWindow cartiDisponibileWindow = new CartiDisponibileWindow();
                    cartiDisponibileWindow.Show();
                    break;

                default:
                    break;
            }
        }
    }
}
