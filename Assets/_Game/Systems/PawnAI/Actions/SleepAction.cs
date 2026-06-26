using UnityEngine;
using Ruinborne.Data;

namespace Ruinborne.Systems.PawnAI.Actions
{
    public class SleepAction : GoapAction
    {
        [SerializeField] private float sleepDuration = 8f;
        private float _sleepTimer = 0f;
        private bool _isSleeping = false;

        protected override void SetupConditions()
        {
            AddPrecondition("has_sleep_spot", true);
            AddEffect("is_tired", false);
        }

        public override bool CheckProceduralPrecondition()
        {
            // 추후 SleepSpotDef 연동으로 교체
            return true;
        }

        public override bool Perform()
        {
            if (!_isSleeping)
            {
                _isSleeping = true;
                _sleepTimer = sleepDuration;
                _controller.SetState(PawnController.PawnState.Sleeping);
                Debug.Log($"[SleepAction] {_controller.PawnName} 수면 시작");
            }

            _sleepTimer -= Time.deltaTime;
            _needs?.FulfillNeed(NeedType.Sleep, 25f * Time.deltaTime);

            if (_sleepTimer <= 0f)
            {
                _controller.SetState(PawnController.PawnState.Idle);
                Debug.Log($"[SleepAction] {_controller.PawnName} 수면 완료");
                return true;
            }

            return false;
        }

        public override void Reset()
        {
            _isSleeping = false;
            _sleepTimer = 0f;
        }
    }
}
