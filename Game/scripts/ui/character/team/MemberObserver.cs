using Godot;

namespace Lawfare.scripts.ui.character.team;

public partial class MemberObserver : CharacterObserver
{
    [Signal]
    public delegate void OnLayoutDirectionChangedEventHandler(Window.LayoutDirection layoutDirection);
    
    [Signal]
    public delegate void OnOppositeLayoutDirectionChangedEventHandler(Window.LayoutDirection layoutDirection);
    
    public override bool Mirror
    {
        set
        {
            base.Mirror = value;
            EmitSignalOnLayoutDirectionChanged(value ? Window.LayoutDirection.Rtl : Window.LayoutDirection.Ltr);
            EmitSignalOnOppositeLayoutDirectionChanged(value ? Window.LayoutDirection.Ltr : Window.LayoutDirection.Rtl);
        }
    }
}