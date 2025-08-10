using Content.Client.Eui;
using Content.Shared.GG.Escapetile;
using JetBrains.Annotations;
using Robust.Client.Graphics;

namespace Content.Client.GG.EscapeTile
{
    [UsedImplicitly]
    public sealed class RespawnEui : BaseEui
    {
        private readonly RespawnMenu _menu;

        public RespawnEui()
        {
            _menu = new RespawnMenu();

            _menu.CancelButton.OnPressed += _ =>
            {

                SendMessage(new RespawnChoiceMessage(false));
                _menu.Close();
            };

            _menu.AcceptButton.OnPressed += _ =>
            {
                SendMessage(new RespawnChoiceMessage(true));
                _menu.Close();
            };

        }

        public override void Opened()
        {
            IoCManager.Resolve<IClyde>().RequestWindowAttention();
            _menu.OpenCentered();
        }

        public override void Closed()
        {
            base.Closed();

            // В случае закрытия окна без выбора, считаем, что игрок не хочет возрождаться (False)
            SendMessage(new RespawnChoiceMessage(false));
            _menu.Close();
        }
    }
}
