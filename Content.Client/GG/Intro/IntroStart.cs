using Content.Shared.IntroSystem;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameStates;
using Timer = Robust.Shared.Timing.Timer;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client.IntroSystem
{
    public sealed class IntroSystem : EntitySystem
    {
        [Dependency] private readonly IUserInterfaceManager _uiManager = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<IntroComponent, ComponentHandleState>(OnIntroHandleState);
            SubscribeLocalEvent<IntroComponent, ComponentStartup>(OnIntroStartup);
        }

        private void OnIntroStartup(EntityUid uid, IntroComponent component, ComponentStartup args)
        {
            ShowIntro(component.PlayerName, component.Role, component.Station);
            Timer.Spawn(TimeSpan.FromSeconds(5), () => RemoveComponent(uid, component));
        }

        private void ShowIntro(string? playerName, string? role, string? station)
        {
            var window = new DefaultWindow
            {
                Title = "",

            };

            var container = new BoxContainer
            {
                Orientation = BoxContainer.LayoutOrientation.Vertical
            };

            container.AddChild(new Label
            {
                Text = $"Welcome, {playerName ?? "Unknown"}",
                Align = Label.AlignMode.Center
            });

            container.AddChild(new Label
            {
                Text = $"Role: {role ?? "Unassigned"}",
                Align = Label.AlignMode.Center
            });

            container.AddChild(new Label
            {
                Text = $"Station: {station ?? "Unknown"}",
                Align = Label.AlignMode.Center
            });

            window.Contents.AddChild(container);
            _uiManager.StateRoot.AddChild(window);

            Timer.Spawn(TimeSpan.FromSeconds(5), () =>
            {
                window.Dispose();
            });
        }

        private void RemoveComponent(EntityUid uid, IntroComponent component)
        {
            if (Deleted(uid))
                return;

            RemComp<IntroComponent>(uid);
        }

        private void OnIntroHandleState(EntityUid uid, IntroComponent component, ref ComponentHandleState args)
        {
            // Убедитесь, что состояние синхронизировано.
        }
    }
}
