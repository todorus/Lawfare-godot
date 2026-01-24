using System.Linq;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;
using Enumerable = System.Linq.Enumerable;

namespace Lawfare.scripts.dice;

public static class DicePoolExt
{
    public static DicePool GetDicePools(this GameEvent gameEvent, Faction faction)
    {
        if (gameEvent.Action == null) return new DicePool(faction, []);

        var skills = gameEvent.Action.DicePools;
        var pools = Enumerable.ToArray(Enumerable.Where(
            Enumerable.SelectMany(
                Enumerable.Where(
                    Enumerable.Where(gameEvent.Subjects,
                        subject => subject.CanHaveFaction && subject.Allegiances.Contains(faction)),
                    subject => subject is ICharacter), pawn => pawn.Pools), pool => skills.Contains(pool.Skill)));
        return new DicePool(faction, pools);
    }
}