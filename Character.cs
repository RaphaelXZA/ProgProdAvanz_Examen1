using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgProdAvanz_Examen1
{
    internal abstract class Character : ICombatEntity
    {
        public string Name { get; protected set; }
        public int Health { get; protected set; }
        public int MaxHealth { get; protected set; }
        public int MinAttack { get; protected set; }
        public int MaxAttack { get; protected set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public bool IsAlive => Health > 0;

        protected readonly Random random = new Random();

        protected Character(string name, int health, int minAttack, int maxAttack, int row, int column)
        {
            Name = name;
            Health = health;
            MaxHealth = health;
            MinAttack = minAttack;
            MaxAttack = maxAttack;
            Row = row;
            Column = column;
        }

        public virtual int Attack()
        {
            return random.Next(MinAttack, MaxAttack + 1);
        }

        public virtual void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health < 0) Health = 0;
        }

        public virtual void Heal(int amount)
        {
            Health += amount;
            if (Health > MaxHealth) Health = MaxHealth;
        }

        public abstract string GetDescription();
    }
}
