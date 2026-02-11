using Godot;
using Lawfare.scripts.characters.lawyers;

namespace Lawfare.scripts.ui.initiative;

public partial class Move : GodotObject
{
    public Lawyer Lawyer;
    public int Initiative;
}