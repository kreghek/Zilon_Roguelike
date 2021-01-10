namespace Assets.Zilon.Scripts.Services
{
    class UiSettingService
    {
        public UiSettingService()
        {
            CurrentLanguage = Language.Russian;
            ShowTutorialOnStart = true;
        }

        public Language CurrentLanguage { get; set; }

        public bool ShowTutorialOnStart { get; set; }
    }
}
