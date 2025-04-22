using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgProdAvanz_Examen1
{
    internal class MovementHistory
    {
        private struct MovementState
        {
            public int Row { get; }
            public int Column { get; }
            public int RemainingMoves { get; }

            public MovementState(int row, int column, int remainingMoves)
            {
                Row = row;
                Column = column;
                RemainingMoves = remainingMoves;
            }
        }

        private Stack<MovementState> movements;
        private readonly int maxUndos;

        public MovementHistory(int maxUndos = 10)
        {
            movements = new Stack<MovementState>();
            this.maxUndos = maxUndos;
        }

        public void RecordMovement(int row, int column, int remainingMoves)
        {
            movements.Push(new MovementState(row, column, remainingMoves));
        }

        public bool CanUndo()
        {
            return movements.Count > 0;
        }

        public (int Row, int Column, int RemainingMoves) UndoMovement()
        {
            if (!CanUndo())
                throw new InvalidOperationException("No hay movimientos para deshacer.");

            MovementState previousState = movements.Pop();
            return (previousState.Row, previousState.Column, previousState.RemainingMoves);
        }

        public void ClearHistory()
        {
            movements.Clear();
        }
    }
}
