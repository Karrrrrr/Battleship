namespace Battleship.Models
{
    public class Player
    {
        public string Name { get; set; }
        public Board OwnBoard { get; } = new Board();
        public Board TrackingBoard { get; } = new Board(); // для отметок выстрелов по противнику

        public Player(string name)
        {
            Name = name;
        }
    }
}