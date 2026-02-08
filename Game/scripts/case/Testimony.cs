using Godot;
using Lawfare.scripts.characters;

namespace Lawfare.scripts.@case;

[GlobalClass]
public partial class Testimony() : Resource
{
    [Export]
    public WitnessDef Witness { get; private set; }
    [Export]
    public Statement Statement { get; private set; }

    public Testimony(WitnessDef witness, Statement statement) : this()
    {
        Witness = witness;
        Statement = statement;
    }
}