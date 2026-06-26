using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Ruinborne.Systems.PawnAI
{
    public static class GoapPlanner
    {
        public static Queue<GoapAction> Plan(
            List<GoapAction> availableActions,
            Dictionary<string, bool> worldState,
            Dictionary<string, bool> goal)
        {
            // 목표를 달성할 수 있는 행동 시퀀스를 탐색
            List<GoapAction> usableActions = availableActions
                .Where(a => a.CheckProceduralPrecondition())
                .ToList();

            List<List<GoapAction>> leaves = new List<List<GoapAction>>();
            List<GoapAction> start = new List<GoapAction>();

            bool success = BuildGraph(start, leaves, usableActions, worldState, goal);

            if (!success)
            {
                Debug.Log("[GoapPlanner] 계획 실패 — 목표 달성 불가");
                return null;
            }

            // 가장 비용이 낮은 계획 선택
            List<GoapAction> cheapest = null;
            float cheapestCost = float.MaxValue;

            foreach (var leaf in leaves)
            {
                float cost = leaf.Sum(a => a.cost);
                if (cost < cheapestCost)
                {
                    cheapestCost = cost;
                    cheapest = leaf;
                }
            }

            if (cheapest == null) return null;

            Queue<GoapAction> result = new Queue<GoapAction>();
            foreach (var action in cheapest)
                result.Enqueue(action);

            Debug.Log($"[GoapPlanner] 계획 수립: {string.Join(" → ", cheapest.Select(a => a.actionName))} (비용: {cheapestCost})");
            return result;
        }

        private static bool BuildGraph(
            List<GoapAction> current,
            List<List<GoapAction>> leaves,
            List<GoapAction> usableActions,
            Dictionary<string, bool> state,
            Dictionary<string, bool> goal)
        {
            // 목표 달성 여부 확인
            if (GoalAchieved(goal, state))
            {
                leaves.Add(new List<GoapAction>(current));
                return true;
            }

            bool foundPath = false;

            foreach (var action in usableActions)
            {
                if (!PreconditionsMet(action.GetPreconditions(), state)) continue;

                // 이 행동의 효과를 적용한 새 상태 생성
                Dictionary<string, bool> newState = new Dictionary<string, bool>(state);
                foreach (var effect in action.GetEffects())
                    newState[effect.Key] = effect.Value;

                List<GoapAction> newCurrent = new List<GoapAction>(current) { action };
                List<GoapAction> remaining = usableActions.Where(a => a != action).ToList();

                if (BuildGraph(newCurrent, leaves, remaining, newState, goal))
                    foundPath = true;
            }

            return foundPath;
        }

        private static bool GoalAchieved(Dictionary<string, bool> goal, Dictionary<string, bool> state)
        {
            foreach (var g in goal)
            {
                if (!state.TryGetValue(g.Key, out bool val) || val != g.Value)
                    return false;
            }
            return true;
        }

        private static bool PreconditionsMet(Dictionary<string, bool> preconditions, Dictionary<string, bool> state)
        {
            foreach (var p in preconditions)
            {
                if (!state.TryGetValue(p.Key, out bool val) || val != p.Value)
                    return false;
            }
            return true;
        }
    }
}
