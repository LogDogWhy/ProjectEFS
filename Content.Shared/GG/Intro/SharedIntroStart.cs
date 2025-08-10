using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.IntroSystem
{
    [RegisterComponent, NetworkedComponent]
    public sealed partial class IntroComponent : Component
    {
        /// <summary>
        /// Имя игрока, который видит интро.
        /// </summary>
        public string? PlayerName { get; set; }

        /// <summary>
        /// Роль игрока.
        /// </summary>
        public string? Role { get; set; }

        /// <summary>
        /// Название станции или команды.
        /// </summary>
        public string? Station { get; set; }
    }
}
