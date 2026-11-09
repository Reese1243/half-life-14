using Robust.Shared.GameStates;
using Content.Shared.Chat;
using Content.Shared.Chat.Prototypes;

namespace Content.Shared.Emoting;

public abstract class SharedAnimatedEmotesSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AnimatedEmotesComponent, EmoteEvent>(OnEmote);
    }

    private void OnEmote(Entity<AnimatedEmotesComponent> ent, ref EmoteEvent args)
    {
        if (args.Handled)
            return;

        if (args.Emote.Event != null)
        {
            RaiseLocalEvent(ent, args.Emote.Event);
        }
    }
}