using UnityEngine;
using Ruinborne.Core;
using Ruinborne.Data;
using Ruinborne.Systems.Economy;

namespace Ruinborne.Systems.PawnAI.Actions
{
    public class EatAction : GoapAction
    {
        [SerializeField] private float eatDuration = 3f;
        private float _eatTimer = 0f;
        private bool _isEating = false;

        protected override void SetupConditions()
        {
            AddPrecondition("has_food", true);
            AddEffect("is_hungry", false);
        }

        public override bool CheckProceduralPrecondition()
        {
            var rm = ServiceLocator.Get<ResourceManager>();
            return rm != null && rm.GetAmount(ResourceType.RawFood) > 0;
        }

        public override bool Perform()
        {
            if (!_isEating)
            {
                _isEating = true;
                _eatTimer = eatDuration;
                _controller.SetState(PawnController.PawnState.Eating);
                Debug.Log($"[EatAction] {_controller.PawnName} 식사 시작");
            }

            _eatTimer -= Time.deltaTime;

            if (_eatTimer <= 0f)
            {
                var rm = ServiceLocator.Get<ResourceManager>();
                if (rm != null && rm.ConsumeResource(ResourceType.RawFood, 1))
                {
                    var needsDef = GetNeedsDef();
                    _needs?.FulfillNeed(NeedType.Food, needsDef?.fulfillAmountPerAction ?? 40f);
                }
                _controller.SetState(PawnController.PawnState.Idle);
                Debug.Log($"[EatAction] {_controller.PawnName} 식사 완료");
                return true;
            }

            return false;
        }

        public override void Reset()
        {
            _isEating = false;
            _eatTimer = 0f;
        }

        private Ruinborne.Definitions.NeedsDef GetNeedsDef()
        {
            // 추후 RaceDef에서 NeedsDef 참조로 교체
            return null;
        }
    }
}
