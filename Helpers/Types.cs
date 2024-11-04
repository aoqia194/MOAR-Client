namespace MOAR
{
    public class GetPresetsListResponse
    {
        public Preset[] data;
    }

    public class Preset
    {
        public string Name { get; set; }
        public string Label { get; set; }
    }

    public class SetPresetRequest
    {
        public string Preset { get; set; }
    }

    public class ConfigSettings
    {
        public double pmcDifficulty { get; set; }
        public double scavDifficulty { get; set; }
        public double defaultScavStartWaveRatio { get; set; }
        public double defaultScavWaveMultiplier { get; set; }
        public bool startingPmcs { get; set; }
        public double defaultPmcStartWaveRatio { get; set; }
        public double defaultPmcWaveMultiplier { get; set; }
        public int defaultMaxBotCap { get; set; }
        public int defaultMaxBotPerZone { get; set; }
        public bool moreScavGroups { get; set; }
        public bool morePmcGroups { get; set; }
        public int defaultGroupMaxPMC { get; set; }
        public int defaultGroupMaxScav { get; set; }
        public bool sniperBuddies { get; set; }
        public bool noZoneDelay { get; set; }
        public bool reducedZoneDelay { get; set; }
        public bool bossOpenZones { get; set; }
        public bool randomRaiderGroup { get; set; }
        public int randomRaiderGroupChance { get; set; }
        public bool randomRogueGroup { get; set; }
        public int randomRogueGroupChance { get; set; }
        public bool disableBosses { get; set; }
        public int mainBossChanceBuff { get; set; }
        public bool bossInvasion { get; set; }
        public int bossInvasionSpawnChance { get; set; }
        public bool gradualBossInvasion { get; set; }
    }
}
