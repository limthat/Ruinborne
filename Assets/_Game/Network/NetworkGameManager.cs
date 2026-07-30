using UnityEngine;
using Fusion;
using Ruinborne.Core;

namespace Ruinborne.Network
{
    public class NetworkGameManager : NetworkBehaviour
    {
        [Networked] public int PlayerCount { get; set; }
        [Networked] public bool GameStarted { get; set; }

        public override void Spawned()
        {
            ServiceLocator.Register<NetworkGameManager>(this);

            if (HasStateAuthority)
            {
                PlayerCount = 0;
                GameStarted = false;
                Debug.Log("[NetworkGameManager] 게임 매니저 스폰됨 (호스트)");
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            ServiceLocator.Unregister<NetworkGameManager>();
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority) return;
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_RequestJoin(PlayerRef player)
        {
            PlayerCount++;
            Debug.Log($"[NetworkGameManager] 플레이어 참가: {player}, 총 인원: {PlayerCount}");

            if (PlayerCount >= 1 && !GameStarted)
            {
                GameStarted = true;
                GameEventBus.Publish(new GameStartedEvent());
                Debug.Log("[NetworkGameManager] 게임 시작!");
            }
        }

        public static NetworkGameManager Instance =>
            ServiceLocator.TryGet<NetworkGameManager>(out var inst) ? inst : null;
    }
}
