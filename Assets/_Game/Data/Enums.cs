namespace Ruinborne.Data
{
    public enum RaceType
    {
        Dwarf, Human, Elf, Undead, Orc, Demon, Beastkin, Operator
    }

    public enum NeedType
    {
        Food, Sleep, Social, Comfort, Recreation, Blood, Rust
    }

    public enum ReproductionType
    {
        Natural, Egg, Conversion, Craft, None
    }

    public enum RespawnType
    {
        ItemRequired, TechRequired, BellCost, Default, None
    }

    public enum WorkType
    {
        ChopWood, Mine, Haul, Build, Deconstruct, Cook, Research,
        Doctor, Clean, Hunt, Farm, Craft, Smithing, Art, Social,
        Warden, Handle, Recruit, FireFighting
    }

    public enum SkillType
    {
        Shooting, Melee, Construction, Mining, Plants, Animals,
        Crafting, Cooking, Medicine, Social, Art, Research,
        Intellectual, Leadership, Philosophy, Magic, Engineering,
        Stealth, Survival
    }

    public enum PassionLevel
    {
        None, Passion, BurningPassion
    }

    public enum MentalBreakType
    {
        Lethargy, Wander, Berserk, Flee, Eating, Crying
    }

    public enum MentalBreakSeverity
    {
        Minor, Major, Extreme
    }

    public enum ThoughtCategory
    {
        Social, Environment, Physical, Special
    }

    public enum TraitFlag
    {
        None, Pyromaniac, Psychopath, Masochist, Bloodlust
    }

    public enum TileType
    {
        Grass, Forest, Mountain, Water, Desert, Snow, Lava, Sand, Soil
    }

    public enum BiomeType
    {
        Plains, Forest, Mountain, Desert, Tundra, Swamp, Volcano
    }

    public enum ResourceType
    {
        RawWood, RawStone, RawOre, RawFood, Leather, Bone,
        MagicStone, Poison, Feather,
        Wood, Stone, Metal, Food, ProcessedMaterial,
        BuildingMaterial, Alloy, ArcanePart, ArcaneCore,
        ProcessedFood, Bell
    }

    public enum Season { Spring, Summer, Fall, Winter }

    public enum WeatherType
    {
        Clear, Cloudy, Rain, Thunderstorm, Snow, Blizzard, HeatWave
    }

    public enum DamageType
    {
        Sharp, Blunt, Flame, Electric, Cold, Psychic, Arcane
    }

    public enum StatType
    {
        MaxHealth, MoveSpeed, WorkSpeed, ShootingAccuracy,
        MeleeSkill, Consciousness, Manipulation, Moving,
        Sight, Hearing, ArmorRating
    }

    public enum BodyPartType
    {
        Head, Torso, LeftArm, RightArm, LeftLeg, RightLeg
    }

    public enum ViewMode
    {
        FirstPerson, TopDown
    }
}
