using System.Linq;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.characters;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;
using Enumerable = System.Linq.Enumerable;

namespace Lawfare.scripts.dice;

public static class DicePoolExt
{
    public static DicePool GetDicePools(this GameEvent gameEvent, Faction faction)
    {
        if(gameEvent.Action == null)
        {
            return new DicePool(faction, []);
        }
        
        var skills = gameEvent.Action.DicePools;
        var pools = gameEvent.Subjects
            .Where(subject => subject.CanHaveFaction && subject.Allegiances.Contains(faction))
            .SelectMany(subject => subject.Pools)
            .Where(pool => skills.Contains(pool.Skill))
            .ToArray();
        return new DicePool(faction, pools);
    }
}