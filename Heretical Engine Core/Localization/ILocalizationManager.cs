namespace HereticalSolutions.Localization
{
    public interface ILocalizationManager
    {
        void SwitchLanguage(LanguageSettings settings);
        
        LocalizedString Translate(string key);
        
        LocalizedString Translate(string key, PlaceholderOptions options);
    }
}