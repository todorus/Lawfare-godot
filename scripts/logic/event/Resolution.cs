using System.Linq;
using Godot;
using Lawfare.scripts.board.dice;
using Lawfare.scripts.logic.effects;

namespace Lawfare.scripts.logic.@event;

public partial class Resolution(IDiff[] changes, DiceRoll[] diceRolls) : Resource
{
    public readonly IDiff[] Changes = changes;
    public readonly DiceRoll[] DiceRolls = diceRolls;
}