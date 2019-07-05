using System.Collections.Generic;

namespace KeaneGames.AdvancedSceneSearch
{
    public class SettingsData
    {
        public List<SettingData> AllSettings;
        public SettingData CaseSensitive;
        public SettingData MatchWholeWord;
        public SettingData UseRegex;
        public SettingData IncludeDisabled;

        public SettingsData()
        {
            CaseSensitive = new SettingData("Match Case", "case_sens", false);
            MatchWholeWord = new SettingData("Match Whole Word", "match_whole", false);
            UseRegex = new SettingData("Use Regex", "use_regex", false);
            IncludeDisabled = new SettingData("Include Disabled", "inc_disabled", false);

            AllSettings = new List<SettingData>
            {
                CaseSensitive,
                MatchWholeWord,
                UseRegex,
                IncludeDisabled
            };
        }
    }
}