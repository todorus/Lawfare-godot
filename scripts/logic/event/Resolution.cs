using Godot;
using Lawfare.scripts.board.dice;
using Lawfare.scripts.logic.effects;

namespace Lawfare.scripts.logic.@event;

public partial class Resolution(IChange[] changes, DiceRoll[] diceRolls) : Resource
{
    public readonly IChange[] Changes = changes;
    public readonly DiceRoll[] DiceRolls = diceRolls;
}