using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgProdAvanz_Examen1
{
    internal interface ICombatEntity
    {
        string Name { get; }
        int Health { get; }
        int MaxHealth { get; }
        int MinAttack { get; }
        int MaxAttack { get; }
        bool IsAlive { get; }

        int Attack();
        void TakeDamage(int damage);
        void Heal(int amount);
    }
}
