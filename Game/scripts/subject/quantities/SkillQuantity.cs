using Godot;

namespace Lawfare.scripts.subject.quantities;

[GlobalClass]
public partial class SkillQuantity : Resource, IQuantity
{
    [Export] public Skill Skill { get; private set; }

    public Property Property => Skill;

    [Export] public int Amount { get; set; }
}