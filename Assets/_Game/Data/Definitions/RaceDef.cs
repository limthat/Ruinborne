using UnityEngine;
using Ruinborne.Data;

namespace Ruinborne.Definitions
{
    [CreateAssetMenu(fileName = "RaceDef_", menuName = "Ruinborne/Definitions/RaceDef")]
    public class RaceDef : ScriptableObject
    {
        [Header("기본 정보")]
        public string raceName;
        public RaceType raceType;

        [Header("욕구 설정")]
        public NeedType[] allowedNeeds;
        public bool hasNoNeeds;
        public bool hasNoMood;

        [Header("특수 플래그")]
        public bool hasBlood = true;
        public bool canLearnMagic = true;
        public bool canLearnTech = true;
        public bool sunDebuff = false;
        public bool rustOnWater = false;

        [Header("수명 & 번식")]
        [Tooltip("-1이면 불사")] public float lifeExpectancy = 80f;
        public ReproductionType reproductionType = ReproductionType.Natural;
        public float reproductionSpeed = 1f;

        [Header("부활")]
        public RespawnType deathRespawnType = RespawnType.ItemRequired;

        [Header("그래픽")]
        public Color[] colorPalette;
    }
}
