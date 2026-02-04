using Godot;
using Ult = Lawfare.scripts.characters.ult.Ult;

namespace Lawfare.scripts.ui.character;

public partial class UltObserver : Node
{
    [Signal]
    public delegate void UltVisibleEventHandler(bool visible);
    
    [Signal] 
    public delegate void UltActiveEventHandler(bool active);
    
    [Signal]
    public delegate void UltProgressEventHandler(float progress);
    
    public void SetUlt(Ult ult)
    {
        Ult = ult;
    }

    private Ult _ult;
    private Ult Ult
    {
        get => _ult;
        set 
        {
            if (_ult != null)
            {
                _ult.OnChange -= OnUltChanged;   
            }
            _ult = value;
            if (_ult != null)
            {
                _ult.OnChange += OnUltChanged;
            }
            
            OnUltChanged(_ult);
        }
    }

    private void OnUltChanged(Ult ult)
    {
        EmitSignalUltVisible(ult != null);
        EmitSignalUltActive(ult?.Active == true);
        EmitSignalUltProgress(ult?.Progress ?? 0f);
    }
}