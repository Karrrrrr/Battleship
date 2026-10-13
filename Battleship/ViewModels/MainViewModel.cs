using System.ComponentModel;
using Battleship.Models;

namespace Battleship.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Raise(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public GameState State { get; } = new GameState();

        public MainViewModel()
        {
            // Инициализация двух игроков по умолчанию
            State.Players.Add(new Player("Игрок 1"));
            State.Players.Add(new Player("Игрок 2"));
        }
    }
}
