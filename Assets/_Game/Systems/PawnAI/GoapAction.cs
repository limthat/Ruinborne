using UnityEngine;
using System.Collections.Generic;

namespace Ruinborne.Systems.PawnAI
{
    public abstract class GoapAction : MonoBehaviour
    {
        [Header("GOAP 설정")]
        public string actionName = "Action";
        public float cost = 1f;

        // 전제 조건 (이 행동을 수행하려면 충족되어야 하는 조건)
        protected Dictionary<string, bool> _preconditions = new Dictionary<string, bool>();
        // 효과 (이 행동 수행 후 변하는 상태)
        protected Dictionary<string, bool> _effects = new Dictionary<string, bool>();

        protected PawnController _controller;
        protected PawnNeeds _needs;

        protected virtual void Awake()
        {
            _controller = GetComponent<PawnController>();
            _needs = GetComponent<PawnNeeds>();
            SetupConditions();
        }

        // 하위 클래스에서 전제 조건과 효과를 정의
        protected abstract void SetupConditions();

        // 이 행동이 지금 수행 가능한지 확인
        public abstract bool CheckProceduralPrecondition();

        // 행동 수행 (매 프레임 호출, true 반환 시 완료)
        public abstract bool Perform();

        // 행동 초기화
        public virtual void Reset() { }

        public Dictionary<string, bool> GetPreconditions() => _preconditions;
        public Dictionary<string, bool> GetEffects() => _effects;

        protected void AddPrecondition(string key, bool value) => _preconditions[key] = value;
        protected void AddEffect(string key, bool value) => _effects[key] = value;
    }
}
