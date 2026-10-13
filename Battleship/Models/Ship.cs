using System.Collections.Generic;
using System.Linq;

namespace Battleship.Models
{
    public class Ship
    {
        public int Length { get; }
        public List<Cell> Cells { get; } = new List<Cell>();

        public bool IsSunk => Cells.Count > 0 && Cells.All(c => c.State == CellState.Hit);

        public Ship(int length)
        {
            Length = length;
        }
    }
}