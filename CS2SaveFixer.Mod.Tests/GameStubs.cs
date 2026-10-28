using System;
using System.Collections.Generic;

namespace Game.Modding
{
    public interface IMod
    {
        void OnLoad(UpdateSystem updateSystem);
        void OnDispose();
    }
}

namespace Game
{
    public class UpdateSystem { }
}

namespace Game.SceneFlow
{
    public class ExecutableAsset
    {
        public string path { get; set; } = string.Empty;
    }

    public class ModManager
    {
        public bool TryGetExecutableAsset(object mod, out ExecutableAsset asset)
        {
            asset = new ExecutableAsset();
            return false;
        }
    }

    public class LocalizationManager
    {
        public void AddSource(string locale, object source) { }
    }

    public class GameManager
    {
        public static GameManager instance { get; } = new GameManager();
        public ModManager modManager { get; } = new ModManager();
        public LocalizationManager localizationManager { get; } = new LocalizationManager();
    }
}

namespace Colossal.Logging
{
    public interface ILog
    {
        ILog SetShowsErrorsInUI(bool value);
        void Info(string message);
        void Error(string message);
        void Warn(string message);
    }

    public class DummyLog : ILog
    {
        public ILog SetShowsErrorsInUI(bool value) => this;
        public void Info(string message) => Console.WriteLine($"[INFO] {message}");
        public void Error(string message) => Console.WriteLine($"[ERROR] {message}");
        public void Warn(string message) => Console.WriteLine($"[WARN] {message}");
    }

    public static class LogManager
    {
        public static ILog GetLogger(string name) => new DummyLog();
    }
}

namespace Colossal.IO.AssetDatabase
{
    public class AssetDatabase
    {
        public static AssetDatabase global { get; } = new AssetDatabase();
        public void LoadSettings(string name, object setting, object defaultSetting) { }
    }
}

namespace Colossal
{
    public class FileLocationAttribute : Attribute
    {
        public FileLocationAttribute(string location) { }
    }

    public interface IDictionarySource
    {
        IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts);
        void Unload();
    }

    public interface IDictionaryEntryError { }
}

namespace Game.Settings
{
    public class SettingsUIGroupOrderAttribute : Attribute
    {
        public SettingsUIGroupOrderAttribute(params string[] groups) { }
    }

    public class SettingsUIShowGroupNameAttribute : Attribute
    {
        public SettingsUIShowGroupNameAttribute(params string[] groups) { }
    }

    public class SettingsUISectionAttribute : Attribute
    {
        public SettingsUISectionAttribute(string section, string group) { }
    }

    public class SettingsUIButtonAttribute : Attribute { }

    public class SettingsUIConfirmationAttribute : Attribute { }

    public class SettingsUISliderAttribute : Attribute
    {
        public int min { get; set; }
        public int max { get; set; }
        public int step { get; set; }
        public int scalarMultiplier { get; set; }
        public object unit { get; set; }
    }

    public class SettingsUIDropdownAttribute : Attribute
    {
        public SettingsUIDropdownAttribute(Type type, string method) { }
    }

    public class ModSetting
    {
        public ModSetting(Game.Modding.IMod mod) { }
        public virtual void SetDefaults() { }
        public void RegisterInOptionsUI() { }
        public void UnregisterInOptionsUI() { }
        public string GetSettingsLocaleID() => "SettingsLocaleID";
        public string GetOptionTabLocaleID(string section) => "OptionTabLocaleID";
        public string GetOptionGroupLocaleID(string group) => "OptionGroupLocaleID";
        public string GetOptionLabelLocaleID(string option) => "OptionLabelLocaleID";
        public string GetOptionDescLocaleID(string option) => "OptionDescLocaleID";
        public string GetOptionWarningLocaleID(string option) => "OptionWarningLocaleID";
        public string GetEnumValueLocaleID(object enumValue) => "EnumValueLocaleID";
    }
}

namespace Game.UI.Widgets
{
    public class DropdownItem<T>
    {
        public T value { get; set; }
        public string displayName { get; set; }
    }
}

namespace Game.UI
{
    public enum Unit
    {
        kDataMegabytes
    }
}
