using UnityEditor;
using UnityEngine;

namespace KeaneGames.AdvancedSceneSearch
{
    public class SettingData
    {
        public string PrefsKey;
        public string SettingName;

        private const string SETTINGS_KEY = "KeaneSharedAdvancedSceneSearch";
        private bool _state;

        public SettingData(string settingName, string prefsKey, bool defaultValue)
        {
            SettingName = settingName;
            PrefsKey = prefsKey;
            _state = EditorPrefs.GetBool(SETTINGS_KEY + PrefsKey, defaultValue);
        }

        public bool State
        {
            get { return _state; }
            set
            {
                EditorPrefs.SetBool(SETTINGS_KEY + PrefsKey, value);
                _state = value;
            }
        }

        public void DrawButton()
        {
            State = GUILayout.Toggle(State, SettingName, EditorStyles.miniButton);
        }
    }
}