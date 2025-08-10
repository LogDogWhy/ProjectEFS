// using Content.Server.GameTicking;
// using Robust.Shared.GameObjects;
// using Robust.Shared.IoC;
// using Robust.Shared.Player;
// using Robust.Shared.Serialization;
// using Robust.Server.Player;
// using Robust.Shared.Network;
// using Content.Shared.GG.Loadout;
// using Content.Server.GameTicking;
// using Content.Server.GameTicking;
// using Content.Shared.GG.Loadout;
// using Robust.Server.GameObjects;
// using Robust.Server.Player;
// using Robust.Shared.GameObjects;
// using Robust.Shared.IoC;
// using Robust.Shared.Player;
// using Robust.Shared.Network;

// using Content.Server.GameTicking;
// using Robust.Shared.GameObjects;
// using Robust.Shared.IoC;
// using Robust.Shared.Player;
// using Content.Shared.GG.Loadout;
// using Robust.Server.Player;

// namespace Content.Server.GG.Loadout
// {
//     public sealed class LoadoutPointsSystem : EntitySystem
//     {
//         [Dependency] private readonly IPlayerManager _playerManager = default!;

//         // Словарь для хранения очков пользователя
//         private readonly Dictionary<NetUserId, int> _playerPoints = new();

//         public override void Initialize()
//         {
//             base.Initialize();
//             SubscribeLocalEvent<PlayerJoinedLobbyEvent>(OnPlayerJoined);
//             SubscribeNetworkEvent<MsgRequestLoadoutPoints>(OnRequestLoadoutPoints);
//         }

//         private void OnPlayerJoined(PlayerJoinedLobbyEvent args)
//         {
//             // Начальные очки при входе
//             if (!_playerPoints.ContainsKey(args.PlayerSession.UserId))
//             {
//                 _playerPoints[args.PlayerSession.UserId] = 10; // Например, начальные очки равны 10
//             }
//         }

//         private void OnRequestLoadoutPoints(MsgRequestLoadoutPoints msg, EntitySessionEventArgs args)
//         {
//             if (!_playerPoints.TryGetValue(args.SenderSession.UserId, out var points))
//             {
//                 points = 0; // Если игрок не найден, вернем 0
//             }

//             var response = new MsgLoadoutPointsResponse
//             {
//                 Points = points
//             };

//             // Проверка, что у сессии есть привязанное сущностное представление
//             if (args.SenderSession.AttachedEntity is { } attachedEntity)
//             {
//                 RaiseNetworkEvent(response, args.SenderSession.ConnectedClient);
//             }
//         }

//         public void AddPoints(NetUserId userId, int points)
//         {
//             if (_playerPoints.ContainsKey(userId))
//             {
//                 _playerPoints[userId] += points;
//             }
//             else
//             {
//                 _playerPoints[userId] = points;
//             }
//         }

//         public void SetPoints(NetUserId userId, int points)
//         {
//             _playerPoints[userId] = points;
//         }
//     }
// }
