using System;

using UnityEngine;

namespace Assets.Zilon.Scripts.Services
{
    class UiSettingService
    {
        private Language _currentLanguage;

        public UiSettingService()
        {
            // Warinig!!! Use _currentLanguage to avoid event raising.
            _currentLanguage = GetInitialCurrentLanguage();

            ShowTutorialOnStart = true;
        }

        private static Language GetInitialCurrentLanguage()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.English:
                    return Language.English;

                case SystemLanguage.Russian:
                    return Language.Russian;

                default:
                    return Language.English;
            }
        }

        public Language CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                var raiseEvent = false;
                if (_currentLanguage != value)
                {
                    raiseEvent = true;
                }

                _currentLanguage = value;

                if (raiseEvent)
                {
                    DoCurrentLanguageChanged();
                }
            }
        }

        private void DoCurrentLanguageChanged()
        {
            CurrentLanguageChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool ShowTutorialOnStart { get; set; }

        public event EventHandler<EventArgs> CurrentLanguageChanged;
    }
}
