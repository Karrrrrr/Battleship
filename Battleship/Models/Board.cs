using System;
using System.Collections.Generic;

namespace Battleship.Models
{
    public class Board
    {
        public const int Size = 10;
        public Cell[,] Cells { get; } = new Cell[Size, Size];
        public List<Ship> Ships { get; } = new List<Ship>();

        public Board()
        {
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    Cells[x, y] = new Cell(x, y);
        }

        public bool CanPlaceShip(int length, int startX, int startY, bool horizontal)
        {
            int dx = horizontal ? 1 : 0;
            int dy = horizontal ? 0 : 1;

            int endX = startX + dx * (length - 1);
            int endY = startY + dy * (length - 1);
            if (endX < 0 || endX >= Size || endY < 0 || endY >= Size) return false;

            for (int i = 0; i < length; i++)
            {
                int x = startX + dx * i;
                int y = startY + dy * i;
                if (Cells[x, y].State == CellState.Ship)
                    return false;
            }
            return true;
        }

        public void Clear()
        {
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                {
                    Cells[x, y].State = CellState.Empty;
                }
            Ships.Clear();
        }

        public bool PlaceShip(Ship ship, int startX, int startY, bool horizontal)
        {
            if (ship == null) return false;
            int dx = horizontal ? 1 : 0;
            int dy = horizontal ? 0 : 1;

            // Проверка выхода за границы
            int endX = startX + dx * (ship.Length - 1);
            int endY = startY + dy * (ship.Length - 1);
            if (endX < 0 || endX >= Size || endY < 0 || endY >= Size) return false;

            // Разрешено касание, поэтому не проверяем соседние клетки
            for (int i = 0; i < ship.Length; i++)
            {
                int x = startX + dx * i;
                int y = startY + dy * i;
                if (Cells[x, y].State == CellState.Ship)
                    return false; // нельзя накладывать на уже существующий корабль
            }

            for (int i = 0; i < ship.Length; i++)
            {
                int x = startX + dx * i;
                int y = startY + dy * i;
                Cells[x, y].State = CellState.Ship;
                ship.Cells.Add(Cells[x, y]);
            }

            Ships.Add(ship);
            return true;
        }

        public (bool hit, Ship sunk) ApplyShot(int x, int y)
        {
            if (x < 0 || x >= Size || y < 0 || y >= Size) throw new ArgumentOutOfRangeException();
            var cell = Cells[x, y];
            if (cell.State == CellState.Hit || cell.State == CellState.Miss)
                return (false, null); // уже стреляли

            if (cell.State == CellState.Ship)
            {
                cell.State = CellState.Hit;
                var ship = FindShipContaining(cell);
                if (ship != null && ship.IsSunk)
                    return (true, ship);
                return (true, null);
            }
            else
            {
                cell.State = CellState.Miss;
                return (false, null);
            }
        }

        private Ship FindShipContaining(Cell cell)
        {
            foreach (var ship in Ships)
            {
                if (ship.Cells.Contains(cell)) return ship;
            }
            return null;
        }

        public Ship GetShipAt(int x, int y)
        {
            if (x < 0 || x >= Size || y < 0 || y >= Size) return null;
            var cell = Cells[x, y];
            return FindShipContaining(cell);
        }

        public bool AllShipsSunk() => Ships.Count > 0 && Ships.TrueForAll(s => s.IsSunk);
    }
}