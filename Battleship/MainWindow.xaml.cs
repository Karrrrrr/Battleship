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

namespace Battleship
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Services.GameController _controller;
        public MainWindow()
        {
            InitializeComponent();
            StartButton.Click += StartButton_Click;
            ExitButton.Click += (s, e) => Close();
            // Инициализация контроллера с двумя игроками
            var p1 = new Models.Player("Игрок 1");
            var p2 = new Models.Player("Игрок 2");
            _controller = new Services.GameController(p1, p2);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Переходим к расстановке первого игрока
            var setup = new Views.SetupView(_controller);
            setup.HorizontalAlignment = HorizontalAlignment.Stretch;
            setup.VerticalAlignment = VerticalAlignment.Stretch;
            // Подменяем содержимое окна
            this.Content = setup;
        }
    }
}
