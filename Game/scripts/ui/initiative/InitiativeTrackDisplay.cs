using System.Collections.Generic;
using Godot;
using Lawfare.scripts.characters;
using Lawfare.scripts.characters.lawyers;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.logic.initiative.state;
using Lawfare.scripts.ui.character.portrait;

namespace Lawfare.scripts.ui.initiative;

public partial class InitiativeTrackDisplay : Control
{
    
    [Signal]
    public delegate void OnCurrentChangedEventHandler(Lawyer current);
    
    private void OnCurrent(IHasInitiative obj) => EmitSignalOnCurrentChanged(obj as Lawyer);
    
    [Export]
    private PackedScene _portraitScene;
    
    [Export]
    private float _slotDistance = 100f;
    [Export]
    private float _stackDistance = 20f;
    
    private Dictionary<ICharacter, PortraitDisplay> _portraitInstances = new();

    public void SeedFromContext(Context context)
    {
        _portraitInstances.Clear();
        foreach (var lawyer in context.Lawyers)
        {
            var instance = _portraitScene.Instantiate<PortraitDisplay>();
            instance.CharacterObserver.Character = lawyer;
            _portraitInstances[lawyer] = instance;
            AddChild(instance);
        }

        var slots = Initiative.ReadSlots(context.InitiativeTrack);
        OnSlotsChanged(slots);
    }
    
    private void OnSlotsChanged(IReadOnlyList<InitiativeSlotState> slots)
    {
        for(int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            for(int j = 0; j < slot.Row.Length; j++)
            {
                var entity = slot.Row[j];
                if(_portraitInstances.TryGetValue(entity as ICharacter, out var portrait))
                {
                    // Position the portrait based on the slot index and stack 
                    var x = Size.X - (i * _slotDistance + j * _stackDistance) - portrait.Size.X;
                    portrait.Position = new Vector2(x, 0);
                }
            }
        }
    }

}