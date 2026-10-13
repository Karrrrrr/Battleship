namespace Battleship.Models
{
    public class ShotResult
    {
        public bool Hit { get; set; }
        public bool Sunk { get; set; }
        public Ship SunkShip { get; set; }
        public bool AlreadyShot { get; set; }
        public bool GameWon { get; set; }
        public Player Winner { get; set; }
    }
}
