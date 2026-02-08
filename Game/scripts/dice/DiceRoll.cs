using System.Linq;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.board.dice;

public class DiceRoll(Faction faction, Skill skill, int[] results, int guaranteedSuccesses)
{
    public readonly Faction Faction = faction;

    /**
     * Guaranteed successes
     */
    public readonly int GuaranteedSuccesses = guaranteedSuccesses;

    /**
     * Results of D6 dice rolls
     */
    public readonly int[] Results = results;

    public readonly Skill Skill = skill;

    public int Successes => GuaranteedSuccesses + GetRolls().Count(roll => roll.IsSuccess);

    public DieRoll[] GetRolls()
    {
        var guaranteedResults = new DieRoll[GuaranteedSuccesses]
            .Select(_ => new DieRoll(Skill, 6, true, true));
        var rolledResult = Results.Select(result => new DieRoll(
            Skill,
            result,
            result >= 4,
            false
        ));
        return guaranteedResults
            .Concat(rolledResult)
            .ToArray();
    }
}