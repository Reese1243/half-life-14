using Content.Shared.ActionBlocker;
using Content.Shared.Movement.Events;
using Content.Shared.StepTrigger.Systems;
using Content.Shared.Weapons.Misc;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using Robust.Shared.Prototypes; // need for EntityPrototype

namespace Content.Shared.Chasm;

public sealed class ChasmSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly ActionBlockerSystem _blocker = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedGrapplingGunSystem _grapple = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ChasmComponent, StepTriggeredOffEvent>(OnStepTriggered);
        SubscribeLocalEvent<ChasmComponent, StepTriggerAttemptEvent>(OnStepTriggerAttempt);
        SubscribeLocalEvent<ChasmFallingComponent, UpdateCanMoveEvent>(OnUpdateCanMove);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (_net.IsClient)
            return;

        var query = EntityQueryEnumerator<ChasmFallingComponent>();
        while (query.MoveNext(out var uid, out var chasm))
        {
            if (_timing.CurTime < chasm.NextDeletionTime)
                continue;

            QueueDel(uid);
        }
    }

    private void OnStepTriggered(EntityUid uid, ChasmComponent component, ref StepTriggeredOffEvent args)
    {
        if (HasComp<ChasmFallingComponent>(args.Tripper))
            return;

        StartFalling(uid, component, args.Tripper, playSound: true);
    }

    /// <summary>
    /// Starts the falling process for an entity that has stepped into a chasm.
    /// </summary>
    public void StartFalling(EntityUid chasm, ChasmComponent component, EntityUid tripper, bool playSound = true)
    {
        var falling = AddComp<ChasmFallingComponent>(tripper);

        falling.NextDeletionTime = _timing.CurTime + falling.DeletionTime;
        _blocker.UpdateCanMove(tripper);

        if (!playSound)
            return;

        var metaQuery = GetEntityQuery<MetaDataComponent>();


        if (!metaQuery.TryGetComponent(tripper, out var meta))
            return;


        var prototype = meta.EntityPrototype;
        
        if (prototype is null)
            return; 

        string prototypeId = prototype.ID;

        string[] allowedPrototypes = { 
            "MobHuman"
            // "AlienMob",
            // "MonkeyMob"
        };

        bool isAllowed = false;

        foreach (var proto in allowedPrototypes)
        {
            // Check if the prototype ID matches any of the allowed prototypes (case-insensitive)
            if (string.Equals(prototypeId, proto, StringComparison.OrdinalIgnoreCase))
            {
                isAllowed = true;
                break;
            }
        }

        if (!isAllowed)
            return;

        _audio.PlayPredicted(component.FallingSound, chasm, tripper);
    }

    private void OnStepTriggerAttempt(EntityUid uid, ChasmComponent component, ref StepTriggerAttemptEvent args)
    {
        if (_grapple.IsEntityHooked(args.Tripper))
        {
            args.Cancelled = true;
            return;
        }

        args.Continue = true;
    }

    private void OnUpdateCanMove(EntityUid uid, ChasmFallingComponent component, UpdateCanMoveEvent args)
    {
        args.Cancel();
    }
}
