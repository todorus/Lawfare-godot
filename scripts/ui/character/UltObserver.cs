using Godot;
using Ult = Lawfare.scripts.characters.ult.Ult;

namespace Lawfare.scripts.ui.character;

public partial class UltObserver : Node
{
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
                OnUltChanged(_ult);
            }
            else 
            {
                EmitSignalUltActive(false);
            }
        }
    }

    private void OnUltChanged(Ult ult)
    {
        EmitSignalUltActive(ult.Active);
        EmitSignalUltProgress(ult.Progress);
    }
}