using System.Windows.Controls;
using System.Windows;
using Battleship.Services;

namespace Battleship.Views
{
    public partial class PassDeviceView : UserControl
    {
        private GameController _controller;

        public event RoutedEventHandler Confirmed;

        public PassDeviceView(GameController controller)
        {
            InitializeComponent();
            _controller = controller;
            // Показать имя того игрока, которому нужно передать устройство (обычно OtherPlayer)
            var target = _controller.State.OtherPlayer ?? _controller.State.CurrentPlayer;
            MessageText.Text = $"Передайте устройство игроку: {target.Name}";
            ReadyButton.Click += (s, e) => Confirmed?.Invoke(this, new RoutedEventArgs());
        }
    }
}
