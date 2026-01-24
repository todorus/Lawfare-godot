using System.Linq;
using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.property.amounts.dice;

[GlobalClass]
public partial class HostSuccesses : AmountProvider
{
    protected override int Count(GameEvent gameEvent, ISubject subject)
    {
        if (gameEvent.DiceRolls == null || gameEvent.DiceRolls.Length == 0) return 0;

        return gameEvent.DiceRolls
            .Where(roll => roll.Faction == gameEvent.Faction)
            .Sum(roll => roll.Successes);
    }
}