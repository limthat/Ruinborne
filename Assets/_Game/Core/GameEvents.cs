using Ruinborne.Data;
using UnityEngine;

namespace Ruinborne.Core
{
    // 자원 관련
    public struct ResourceChangedEvent
    {
        public ResourceType Type;
        public int Amount;
        public int Delta;
    }

    public struct ResourceHarvestedEvent
    {
        public ResourceType Type;
        public int Amount;
        public Vector3 Position;
    }

    // 건물 관련
    public struct BuildingPlacedEvent
    {
        public Vector3 Position;
        public string BuildingName;
    }

    public struct BuildingRemovedEvent
    {
        public Vector3 Position;
    }

    // 폰 관련
    public struct PawnSpawnedEvent
    {
        public string PawnName;
        public Vector3 Position;
    }

    public struct PawnDiedEvent
    {
        public string PawnName;
        public Vector3 Position;
    }

    public struct PawnMeltdownEvent
    {
        public string PawnName;
        public MentalBreakSeverity Severity;
    }

    public struct PawnNeedCriticalEvent
    {
        public string PawnName;
        public NeedType NeedType;
        public float CurrentValue;
    }

    // 지휘관 관련
    public struct ViewModeChangedEvent
    {
        public ViewMode NewMode;
        public Vector3 CommandStationPosition;
    }

    public struct CommandStationEnteredEvent
    {
        public string CommanderName;
    }

    public struct CommandStationExitedEvent
    {
        public string CommanderName;
    }

    // 게임 상태
    public struct GameStartedEvent { }
    public struct GamePausedEvent { public bool IsPaused; }
    public struct NavMeshBakedEvent { }

    // 맵 관련
    public struct MapGeneratedEvent
    {
        public int Seed;
    }

    // 건축 관련
    public struct BlockPlacedEvent
    {
        public Vector3 Position;
        public Ruinborne.Systems.Building.BlockType BlockType;
    }

    public struct BlockRemovedEvent
    {
        public Vector3 Position;
    }

    // 지휘관 관련 (스포너)
    public struct CommanderAssignedEvent
    {
        public string PawnName;
        public int PawnIndex;
    }

    // 상호작용 관련
    public struct PawnInteractedEvent
    {
        public string PawnName;
        public Vector3 Position;
    }

    public struct PawnCommandIssuedEvent
    {
        public string PawnName;
        public string CommandType;
        public Vector3 TargetPosition;
    }

    // 폰 명령 관련
    public struct PawnGroupSelectedEvent
    {
        public int GroupIndex;
    }

    public struct PawnCommandEvent
    {
        public Ruinborne.UI.PawnCommand Command;
        public int GroupIndex;
        public Vector3 TargetPosition;
    }
}
