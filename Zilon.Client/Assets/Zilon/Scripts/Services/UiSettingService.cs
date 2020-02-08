namespace Assets.Zilon.Scripts.Services
{
    class UiSettingService
    {
        public UiSettingService()
        {
            CurrentLanguage = Language.Russian;
        }

        public Language CurrentLanguage { get; set; }

        public Language TargetLanguage { get; set; }
    }
}
