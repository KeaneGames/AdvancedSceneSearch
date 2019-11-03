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
        public SettingData IncludeSubClasses;

        public SettingsData()
        {
            CaseSensitive = new SettingData("Match Case", "case_sens", false, new []{typeof(ASS_SearchFilter_Name), typeof(ASS_SearchFilter_MeshName)});
            MatchWholeWord = new SettingData("Match Whole Word", "match_whole", false, new []{typeof(ASS_SearchFilter_Name), typeof(ASS_SearchFilter_MeshName)});
            UseRegex = new SettingData("Use Regex", "use_regex", false, new []{ typeof(ASS_SearchFilter_Name), typeof(ASS_SearchFilter_MeshName)});
            IncludeDisabled = new SettingData("Include Disabled", "inc_disabled", false);
            IncludeSubClasses = new SettingData("Include Subclasses", "inc_subclasses", true, new []{typeof(ASS_SearchFilter_Components)});

            AllSettings = new List<SettingData>
            {
                CaseSensitive,
                MatchWholeWord,
                UseRegex,
                IncludeDisabled,
                IncludeSubClasses
            };
        }
    }
}