using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgProdAvanz_Examen1
{
    internal class Enemy : Character
    {
        public int MovementPoints { get; private set; }
        public int MaxMovementPoints { get; private set; }
        public EnemyType Type { get; protected set; }

        public Enemy(string name, int health, int minAttack, int maxAttack, int row, int column, int movementPoints = 2)
            : base(name, health, minAttack, maxAttack, row, column)
        {
            Type = EnemyType.Normal;
            MaxMovementPoints = movementPoints;
            MovementPoints = movementPoints;
        }

        public void ResetMovementPoints()
        {
            MovementPoints = MaxMovementPoints;
        }

        public bool CanMove()
        {
            return MovementPoints > 0;
        }

        public void Move()
        {
            if (MovementPoints > 0)
                MovementPoints--;
        }

        public virtual (int newRow, int newCol) DecideNextMove(char[,] mapa, int playerRow, int playerCol)
        {
            int newRow = Row;
            int newCol = Column;

            //Si está adyacente al jugador, no se mueve sino que ataca
            if ((Math.Abs(Row - playerRow) == 1 && Column == playerCol) ||
                (Math.Abs(Column - playerCol) == 1 && Row == playerRow))
            {
                return (Row, Column);
            }


            if (CanMove())
            {
                //Movimiento en el eje horizontal o vertical (el que esté más lejos)
                if (Math.Abs(Row - playerRow) > Math.Abs(Column - playerCol))
                {
                    //Movimiento vertical
                    newRow = Row < playerRow ? Row + 1 : Row - 1;

                    //Verifica si la nueva posición es válida
                    if (newRow >= 0 && newRow < mapa.GetLength(0) && mapa[newRow, Column] == '.')
                    {
                        Move();
                        return (newRow, Column);
                    }
                }
                else
                {
                    //Movimiento horizontal
                    newCol = Column < playerCol ? Column + 1 : Column - 1;

                    //Verifica si la nueva posición es válida
                    if (newCol >= 0 && newCol < mapa.GetLength(1) && mapa[Row, newCol] == '.')
                    {
                        Move();
                        return (Row, newCol);
                    }
                }

                //Si el movimiento elegido no es válido, intenta el otro eje
                if (newRow == Row && newCol == Column)
                {
                    if (Math.Abs(Row - playerRow) > Math.Abs(Column - playerCol))
                    {
                        //Intentar moverse horizontalmente
                        newCol = Column < playerCol ? Column + 1 : Column - 1;
                        if (newCol >= 0 && newCol < mapa.GetLength(1) && mapa[Row, newCol] == '.')
                        {
                            Move();
                            return (Row, newCol);
                        }
                    }
                    else
                    {
                        //Intentar moverse verticalmente
                        newRow = Row < playerRow ? Row + 1 : Row - 1;
                        if (newRow >= 0 && newRow < mapa.GetLength(0) && mapa[newRow, Column] == '.')
                        {
                            Move();
                            return (newRow, Column);
                        }
                    }
                }
            }

            //Si no se puede mover, se queda en la misma casilla
            return (Row, Column);
        }

        public override string GetDescription()
        {
            return $"{Name} | Vida: {Health}/{MaxHealth} | Ataque: {MinAttack}-{MaxAttack} | Tipo: {Type}";
        }
    }
}
