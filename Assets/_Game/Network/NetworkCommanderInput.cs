using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.InputSystem;

namespace Ruinborne.Network
{
    public struct CommanderInputData : INetworkInput
    {
        public Vector2 MoveDirection;
        public Vector2 LookDelta;
        public NetworkBool Jump;
        public NetworkBool Sprint;
        public NetworkBool Interact;
        public NetworkBool Attack;
    }

    public class NetworkCommanderInput : MonoBehaviour, INetworkRunnerCallbacks
    {
        private NetworkRunner _runner;

        private void Start()
        {
            _runner = FindAnyObjectByType<NetworkRunner>();
            if (_runner != null)
                _runner.AddCallbacks(this);
        }

        private void OnDestroy()
        {
            if (_runner != null)
                _runner.RemoveCallbacks(this);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard == null || mouse == null) return;

            var data = new CommanderInputData();

            // 이동
            float h = 0f, v = 0f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) h = -1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) h = 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) v = -1f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) v = 1f;
            data.MoveDirection = new Vector2(h, v);

            // 마우스
            data.LookDelta = mouse.delta.ReadValue() * 0.1f;

            // 버튼
            data.Jump = keyboard.spaceKey.isPressed;
            data.Sprint = keyboard.leftShiftKey.isPressed;
            data.Interact = keyboard.eKey.isPressed;
            data.Attack = mouse.leftButton.isPressed;

            input.Set(data);
        }

        // 나머지 콜백은 빈 구현
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, System.Collections.Generic.List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, System.Collections.Generic.Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    }
}
