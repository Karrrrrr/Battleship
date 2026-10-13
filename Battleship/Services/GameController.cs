using Battleship.Models;

namespace Battleship.Services
{
    public class GameController
    {
        public GameState State { get; } = new GameState();

        public GameController(Player p1, Player p2)
        {
            State.Players.Add(p1);
            State.Players.Add(p2);
            State.CurrentPlayerIndex = 0;
            State.Phase = GamePhase.Setup;
        }

        public void NextPlayer()
        {
            State.CurrentPlayerIndex = 1 - State.CurrentPlayerIndex;
        }

        public void StartPlayPhase()
        {
            State.Phase = GamePhase.Play;
        }

        public bool CheckWin(out Player winner)
        {
            if (State.OtherPlayer != null && State.OtherPlayer.OwnBoard.AllShipsSunk())
            {
                winner = State.CurrentPlayer;
                State.Phase = GamePhase.End;
                return true;
            }
            winner = null;
            return false;
        }

        // Шаблон флота: 1x4, 2x3, 3x2, 4x1
        public static readonly int[] FleetTemplate = new[] { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };

        public void PreparePlayerForSetup(Player player)
        {
            player.OwnBoard.Clear();
            player.TrackingBoard.Clear();
            player.OwnBoard.Ships.Clear();
            // корабли будут создаваться при размещении игроком; можно заранее подготовить объекты при необходимости
        }

        public void ResetGame()
        {
            foreach (var p in State.Players)
            {
                p.OwnBoard.Clear();
                p.TrackingBoard.Clear();
                p.OwnBoard.Ships.Clear();
            }
            State.Phase = GamePhase.Setup;
            State.CurrentPlayerIndex = 0;
        }

        public ShotResult Shoot(int x, int y)
        {
            var target = State.OtherPlayer;
            var shooter = State.CurrentPlayer;
            var result = new ShotResult();
            // Проверка: уже стреляли по этой клетке на доске противника
            var targetCell = target.OwnBoard.Cells[x, y];
            if (targetCell.State == CellState.Hit || targetCell.State == CellState.Miss)
            {
                result.AlreadyShot = true;
                return result;
            }

            var apply = target.OwnBoard.ApplyShot(x, y);
            // apply.hit, apply.sunk

            var trackingCell = shooter.TrackingBoard.Cells[x, y];

            if (apply.hit)
            {
                trackingCell.State = CellState.Hit;
                result.Hit = true;
                if (apply.sunk != null)
                {
                    result.Sunk = true;
                    result.SunkShip = apply.sunk;
                }
            }
            else
            {
                trackingCell.State = CellState.Miss;
                result.Hit = false;
            }

            // Если промах — переходим в фазу Pass, чтобы показать экран передачи устройства.
            if (!result.Hit)
            {
                State.Phase = GamePhase.Pass;
            }

            // Проверить победу
            if (target.OwnBoard.AllShipsSunk())
            {
                result.GameWon = true;
                result.Winner = shooter;
                State.Phase = GamePhase.End;
            }

            return result;
        }
    }
}
