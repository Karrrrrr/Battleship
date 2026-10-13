using System.Collections.Generic;

namespace Battleship.Models
{
    public enum GamePhase { Setup, Pass, Play, End }

    public class GameState
    {
        public List<Player> Players { get; } = new List<Player>();
        public int CurrentPlayerIndex { get; set; } = 0;
        public GamePhase Phase { get; set; } = GamePhase.Setup;

        public Player CurrentPlayer => Players.Count > 0 ? Players[CurrentPlayerIndex] : null;
        public Player OtherPlayer => Players.Count > 1 ? Players[1 - CurrentPlayerIndex] : null;
    }
}