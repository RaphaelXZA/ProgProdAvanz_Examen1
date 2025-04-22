using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgProdAvanz_Examen1
{
    internal class Boss : Enemy
    {
        public Boss(string name, int health = 200, int minAttack = 15, int maxAttack = 25, int row = 0, int column = 0)
        : base(name, health, minAttack, maxAttack, row, column, 0)
        {
            Type = EnemyType.Boss;
        }

        public override (int newRow, int newCol) DecideNextMove(char[,] mapa, int playerRow, int playerCol)
        {
            //El jefe no se mueve, así que siempre devuelve su posición actual
            return (Row, Column);
        }

        public override string GetDescription()
        {
            return $"JEFE: {Name} | Vida: {Health}/{MaxHealth} | Ataque: {MinAttack}-{MaxAttack}";
        }
    }
}
