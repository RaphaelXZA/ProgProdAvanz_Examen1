using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgProdAvanz_Examen1
{
    internal class Player : Character
    {
        public int MovementPoints { get; private set; }
        public int MaxMovementPoints { get; private set; }
        public int KillCount { get; private set; }

        public Player(string name, int health = 100, int minAttack = 15, int maxAttack = 22, int movementPoints = 3)
            : base(name, health, minAttack, maxAttack, 0, 0)
        {
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

        public void Rest()
        {
            int healAmount = MaxHealth / 5; //Se cura 20% de su vida maxima.
            Heal(healAmount);
        }

        public void IncreaseDamage(int minIncrease = 8, int maxIncrease = 10)
        {
            MinAttack += minIncrease;
            MaxAttack += maxIncrease;
        }

        public void AddKill()
        {
            KillCount++;
            IncreaseDamage();
        }

        public void ReceiveMovementPoint()
        {
            if (MovementPoints < MaxMovementPoints)
                MovementPoints++;
        }

        public override string GetDescription()
        {
            return $"{Name} | Vida: {Health}/{MaxHealth} | Ataque: {MinAttack}-{MaxAttack} | Enemigos derrotados: {KillCount}";
        }
    }
}
