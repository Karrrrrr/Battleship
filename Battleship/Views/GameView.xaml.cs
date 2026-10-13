using System.Windows.Controls;
using System.Windows;
using Battleship.Services;
using Battleship.Models;

namespace Battleship.Views
{
    public partial class GameView : UserControl
    {
        private GameController _controller;

        public event RoutedEventHandler ShotMade;

        public GameView(GameController controller)
        {
            InitializeComponent();
            _controller = controller;
            BuildGrids();
        }

        public void BuildGrids()
        {
            OwnGrid.Children.Clear();
            EnemyGrid.Children.Clear();
            var player = _controller.State.CurrentPlayer;
            var enemy = _controller.State.OtherPlayer;

            // Обновляем подпись: кто сейчас ходит
            if (CurrentPlayerText != null)
                CurrentPlayerText.Text = $"Ход: {player.Name}";

            for (int y = 0; y < Board.Size; y++)
            {
                for (int x = 0; x < Board.Size; x++)
                {
                    var btn = new Button();
                    btn.Tag = new Point(x, y);
                    btn.Margin = new Thickness(1);
                    var cell = player.OwnBoard.Cells[x, y];
                    UpdateOwnButtonVisual(btn, cell);
                    OwnGrid.Children.Add(btn);
                }
            }

            for (int y = 0; y < Board.Size; y++)
            {
                for (int x = 0; x < Board.Size; x++)
                {
                    var btn = new Button();
                    btn.Tag = new Point(x, y);
                    btn.Margin = new Thickness(1);
                    btn.Click += EnemyCell_Click;
                    var tracking = player.TrackingBoard.Cells[x, y];
                    UpdateEnemyButtonVisual(btn, tracking);
                    EnemyGrid.Children.Add(btn);
                }
            }
        }

        private void EnemyCell_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var pt = (Point)btn.Tag;
            int x = (int)pt.X;
            int y = (int)pt.Y;

            var res = _controller.Shoot(x, y);

            if (res.AlreadyShot)
            {
                MessageBox.Show("По этой клетке уже стреляли.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (res.Hit && !res.Sunk)
                {
                    MessageBox.Show("Попадание! Сделайте ещё ход.", "Попадание", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                if (res.Sunk && res.SunkShip != null)
                {
                    MessageBox.Show($"Вы потопили корабль длины {res.SunkShip.Length}.", "Корабль потоплен", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                if (res.GameWon && res.Winner != null)
                {
                    MessageBox.Show($"Игра окончена. Победитель: {res.Winner.Name}", "Победа", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            // обновляем представление полей
            BuildGrids();
            ShotMade?.Invoke(this, new RoutedEventArgs());
        }

        private void UpdateOwnButtonVisual(Button btn, Cell cell)
        {
            if (cell.State == CellState.Ship)
                btn.Background = System.Windows.Media.Brushes.Gray;
            else if (cell.State == CellState.Empty)
                btn.Background = System.Windows.Media.Brushes.LightBlue;
            else if (cell.State == CellState.Miss)
                btn.Background = System.Windows.Media.Brushes.White;
            else if (cell.State == CellState.Hit)
                btn.Background = System.Windows.Media.Brushes.Red;
        }

        private void UpdateEnemyButtonVisual(Button btn, Cell cell)
        {
            if (cell.State == CellState.Hit)
                btn.Background = System.Windows.Media.Brushes.Red;
            else if (cell.State == CellState.Miss)
                btn.Background = System.Windows.Media.Brushes.White;
            else
                btn.Background = System.Windows.Media.Brushes.DarkBlue; // скрытая клетка
        }
    }
}
