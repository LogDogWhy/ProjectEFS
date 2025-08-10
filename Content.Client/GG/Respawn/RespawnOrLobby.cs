using System.Numerics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using static Robust.Client.UserInterface.Controls.BoxContainer;

namespace Content.Client.GG.EscapeTile;

public sealed class RespawnMenu : DefaultWindow
{
    public readonly Button AcceptButton;
    public readonly Button CancelButton;

    public RespawnMenu()
    {
        Title = Loc.GetString("respawn-menu-title");

        Contents.AddChild(new BoxContainer
        {
            Orientation = LayoutOrientation.Vertical,
            Children =
            {
                new BoxContainer
                {
                    Orientation = LayoutOrientation.Vertical,
                    Children =
                    {
                        new Label()
                        {
                            Text = Loc.GetString("respawn-menu-text")
                        },
                        new BoxContainer
                        {
                            Orientation = LayoutOrientation.Horizontal,
                            Align = AlignMode.Center,
                            Children =
                            {
                                (AcceptButton = new Button
                                {
                                    Text = Loc.GetString("respawn-menu-accept-button"),
                                }),

                                new Control
                                {
                                    MinSize = new Vector2(20, 0)
                                },

                                (CancelButton = new Button
                                {
                                    Text = Loc.GetString("respawn-menu-cancel-button"),
                                })
                            }
                        },
                    }
                },
            }
        });
    }
}
