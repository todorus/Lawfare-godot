using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.board.dice;

public struct DieRoll(Skill skill, int value, bool isSuccess, bool garanteed)
{
    public readonly Skill Skill = skill;
    public readonly int Value = value;
    public readonly bool IsSuccess = isSuccess;
    public readonly bool Garanteed = garanteed;
}