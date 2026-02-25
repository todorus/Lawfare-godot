using System.Collections.Generic;
using Godot;
using Lawfare.scripts.characters;
using Lawfare.scripts.characters.lawyers;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.logic.initiative.state;
using Lawfare.scripts.ui.character;

namespace Lawfare.scripts.ui.initiative;

public partial class InitiativeTrackDisplay : Control
{
    
    [Signal]
    public delegate void OnCurrentChangedEventHandler(Lawyer current);
    
    private void OnCurrent(IHasInitiative obj) => EmitSignalOnCurrentChanged(obj as Lawyer);
    
    [Export]
    private PackedScene _portraitScene;
    
    [Export]
    private PackedScene _slotBackgroundScene;
    
    [Export]
    private Control _slotContainer;
    
    [ExportGroup("Spacing")]
    [Export]
    private float _slotDistance = 100f;
    [Export]
    private float _stackDistance = 20f;
    
    [ExportGroup("Animation")]
    [Export] private float _moveDuration = 0.20f;
    [Export] private Tween.TransitionType _transition = Tween.TransitionType.Cubic;
    [Export] private Tween.EaseType _ease = Tween.EaseType.Out;
    
    private Dictionary<ICharacter, CharacterObserver> _portraitInstances = new();

    public void SeedFromContext(Context context)
    {
        _portraitInstances.Clear();
        foreach (var lawyer in context.Lawyers)
        {
            var instance = _portraitScene.Instantiate<CharacterObserver>();
            instance.Character = lawyer;
            _portraitInstances[lawyer] = instance;
            AddChild(instance);
        }

        context.InitiativeTrackChanged += OnTrackChanged;
        UpdateFromContext(context);
    }
    
    public void UpdateFromContext(Context context)
    {
        var slots = Initiative.ReadSlots(context.InitiativeTrack);
        OnSlotsChanged(slots);
    }
    
    private void OnTrackChanged(InitiativeTrackState track)
    {
        var slots = Initiative.ReadSlots(track);
        OnSlotsChanged(slots);
    }
    
    private void OnSlotsChanged(IReadOnlyList<InitiativeSlotState> slots)
    {
        var tween = CreateTween();
        tween.SetParallel(true);
        var portraitIndex = 0;
        
        _slotContainer.ClearChildren();
        
        for(int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            var entity = slot.Occupant;
            
            
            var deltaX = i * _slotDistance;
            var bg = _slotBackgroundScene.Instantiate<SlotDisplay>();
            bg.IsStaggered = slot.IsStaggered;
            // Position the portrait based on the slot index and stack 
            // var targetX = Size.X - deltaX - portrait.Size.X;
            var targetX = Size.X - deltaX - bg.Size.X;
            var targetPos = new Vector2(targetX, 0);
            
            bg.Position = targetPos;
            _slotContainer.AddChild(bg);
            
            if(entity != null && _portraitInstances.TryGetValue(entity as ICharacter, out var portrait))
            {
                    
                // Tween position for easing.
                tween.TweenProperty(portrait, "position", targetPos, _moveDuration)
                    .SetTrans(_transition)
                    .SetEase(_ease);

                portrait.ZIndex = portraitIndex;
                portraitIndex++;
            }
        }
    }

}