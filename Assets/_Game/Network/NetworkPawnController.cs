using UnityEngine;
using Fusion;
using Ruinborne.Systems.PawnAI;

namespace Ruinborne.Network
{
    public class NetworkPawnController : NetworkBehaviour
    {
        [Networked] public Vector3 NetworkedPosition { get; set; }
        [Networked] public Quaternion NetworkedRotation { get; set; }
        [Networked] public PawnController.PawnState NetworkedState { get; set; }

        private PawnController _pawnController;

        public override void Spawned()
        {
            _pawnController = GetComponent<PawnController>();
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority)
            {
                // 호스트에서 상태 업데이트
                NetworkedPosition = transform.position;
                NetworkedRotation = transform.rotation;
                if (_pawnController != null)
                    NetworkedState = _pawnController.State;
            }
            else
            {
                // 클라이언트에서 위치 동기화
                transform.position = Vector3.Lerp(
                    transform.position, NetworkedPosition, Runner.DeltaTime * 10f);
                transform.rotation = Quaternion.Lerp(
                    transform.rotation, NetworkedRotation, Runner.DeltaTime * 10f);
            }
        }
    }
}
