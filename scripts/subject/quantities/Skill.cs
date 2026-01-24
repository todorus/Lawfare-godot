using Godot;

namespace Lawfare.scripts.subject.quantities;

[GlobalClass]
public partial class Skill : Property
{
    public static readonly Category SkillCategory = new()
    {
        Label = "Skill"
    };

    public override Category Category
    {
        get => SkillCategory;
        set { }
    }

    [Export] public Texture2D Background { get; private set; }

    [Export] public Texture2D PipFull { get; private set; }

    [Export] public Texture2D PipEmpty { get; private set; }

    [Export] public Texture2D Success { get; private set; }

    [Export] public Texture2D Failure { get; private set; }
}