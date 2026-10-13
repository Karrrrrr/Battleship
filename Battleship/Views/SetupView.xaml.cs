using System.Windows.Controls;
using System.Windows;
using Battleship.Models;
using Battleship.Services;

namespace Battleship.Views
{
    public partial class SetupView : UserControl
    {
        private GameController _controller;
        private Player _player;
        private bool _horizontal = true;

        public SetupView(GameController controller)
        {
            InitializeComponent();
            _controller = controller;
            _player = controller.State.CurrentPlayer;
            PlayerLabel.Text = $"Расстановка: {_player.Name}";
            BuildGrid();

            RotateButton.Click += (s, e) => { _horizontal = !_horizontal; };
            ReadyButton.Click += ReadyButton_Click;
        }

        private void ReadyButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, все ли корабли размещены
            bool allPlaced = true;
            foreach (var len in GameController.FleetTemplate)
            {
                bool exists = false;
                foreach (var s in _player.OwnBoard.Ships)
                {
                    if (s.Length == len && s.Cells.Count == len) { exists = true; break; }
                }
                if (!exists) { allPlaced = false; break; }
            }

            if (!allPlaced)
            {
                MessageBox.Show("Не все корабли размещены.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _controller.State.Phase = GamePhase.Pass;
            // Показать экран передачи
            var pass = new PassDeviceView(_controller);
            pass.Confirmed += Pass_Confirmed;
            var hostWindow = Window.GetWindow(sender as DependencyObject) ?? Window.GetWindow(this) ?? Application.Current?.MainWindow;
            if (hostWindow == null)
            {
                // Нечего отображать — безопасно выйти
                return;
            }
            hostWindow.Content = pass;
        }

        private void Pass_Confirmed(object sender, RoutedEventArgs e)
        {
            // Переходим к следующему игроку или к игре
            var hostWindow = Window.GetWindow(sender as DependencyObject) ?? Window.GetWindow(this) ?? Application.Current?.MainWindow;
            if (hostWindow == null)
            {
                // Нечего отображать — безопасно выйти
                return;
            }

            if (_controller.State.CurrentPlayerIndex == 0)
            {
                _controller.NextPlayer();
                _controller.PreparePlayerForSetup(_controller.State.CurrentPlayer);
                var setup2 = new SetupView(_controller);
                hostWindow.Content = setup2;
            }
            else
            {
                // Оба игрока разместили — начинаем игру
                // Установим стартового игрока (Игрок 1) перед началом фазы Play
                _controller.State.CurrentPlayerIndex = 0;
                _controller.StartPlayPhase();
                var game = new GameView(_controller);
                game.ShotMade += Game_ShotMade;
                hostWindow.Content = game;
            }
        }

        private void Game_ShotMade(object sender, RoutedEventArgs e)
        {
            var hostWindow = Window.GetWindow(sender as DependencyObject) ?? Window.GetWindow(this) ?? Application.Current?.MainWindow;
            if (hostWindow == null) return;

            // Если текущая фаза — Play, значит был попадание и игрок ходит ещё раз: просто обновляем представление
            if (_controller.State.Phase == GamePhase.Play)
            {
                var currentGame = hostWindow.Content as GameView;
                currentGame?.BuildGrids();
                return;
            }

            // В остальных случаях (Pass или End) показываем экран передачи
            var pass = new PassDeviceView(_controller);
            pass.Confirmed += (s, ev) =>
            {
                // Если игра завершилась, показываем результат и закрываем приложение
                if (_controller.State.Phase == GamePhase.End)
                {
                    var winner = _controller.State.CurrentPlayer;
                    MessageBox.Show($"Игра окончена. Победитель: {winner.Name}", "Конец игры", MessageBoxButton.OK, MessageBoxImage.Information);
                    hostWindow.Close();
                    return;
                }

                // Иначе переключаем игрока и продолжаем игру
                _controller.NextPlayer();
                // Возвращаем игровую фазу для следующего игрока
                _controller.StartPlayPhase();
                var nextGame = new GameView(_controller);
                nextGame.ShotMade += Game_ShotMade;
                hostWindow.Content = nextGame;
            };
            hostWindow.Content = pass;
        }

        private void BuildGrid()
        {
            BoardGrid.Children.Clear();
            for (int y = 0; y < Board.Size; y++)
            {
                for (int x = 0; x < Board.Size; x++)
                {
                    var btn = new Button();
                    btn.Tag = new System.Windows.Point(x, y);
                    btn.Margin = new Thickness(1);
                    btn.Click += Cell_Click;
                    UpdateButtonVisual(btn, _player.OwnBoard.Cells[x, y]);
                    BoardGrid.Children.Add(btn);
                }
            }
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var pt = (System.Windows.Point)btn.Tag;
            int x = (int)pt.X;
            int y = (int)pt.Y;

            // Находим следующий неразмещённый корабль из шаблона
            foreach (var len in GameController.FleetTemplate)
            {
                // если корабля такой длины ещё нет в списке
                bool exists = false;
                foreach (var s in _player.OwnBoard.Ships)
                {
                    if (s.Length == len && s.Cells.Count == len) { exists = true; break; }
                }
                if (!exists)
                {
                    if (_player.OwnBoard.CanPlaceShip(len, x, y, _horizontal))
                    {
                        var ship = new Ship(len);
                        _player.OwnBoard.PlaceShip(ship, x, y, _horizontal);
                        BuildGrid();
                    }
                    break;
                }
            }
        }

        private void UpdateButtonVisual(Button btn, Cell cell)
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
    }
}
