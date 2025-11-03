using CodeTracker.Helpers;
using CodeTracker.Models;
using CodeTracker.Service;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace CodeTracker.ViewModels
{
    public class LanguagesViewModel : BaseViewModel
    {
        private readonly LanguageService _languageService = new LanguageService();
        private readonly DashBoardService _dashBoardService = new DashBoardService();

        private ObservableCollection<LanguageStats> _languageStats;
        private ObservableCollection<Session> _languageSessions;
        private LanguageStats? _selectedLanguage;
        private bool _showLanguageSessions;

        public ObservableCollection<LanguageStats> LanguageStats
        {
            get => _languageStats;
            set
            {
                _languageStats = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Session> LanguageSessions
        {
            get => _languageSessions;
            set
            {
                _languageSessions = value;
                OnPropertyChanged();
            }
        }

        public LanguageStats? SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                _selectedLanguage = value;
                OnPropertyChanged();
            }
        }

        public bool ShowLanguageSessions
        {
            get => _showLanguageSessions;
            set
            {
                _showLanguageSessions = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadLanguageStatsCommand { get; }
        public ICommand ViewLanguageSessionsCommand { get; }
        public ICommand CloseSessionsViewCommand { get; }

        public LanguagesViewModel()
        {
            _languageStats = new ObservableCollection<LanguageStats>();
            _languageSessions = new ObservableCollection<Session>();

            LoadLanguageStatsCommand = new RelayCommand(async () => await ExecuteLoadLanguageStatsAsync());
            ViewLanguageSessionsCommand = new RelayCommand<LanguageStats>(async (lang) => await ExecuteViewLanguageSessionsAsync(lang));
            CloseSessionsViewCommand = new RelayCommand(ExecuteCloseSessionsView);

            _ = ExecuteLoadLanguageStatsAsync();
        }

        private async System.Threading.Tasks.Task ExecuteLoadLanguageStatsAsync()
        {
            try
            {
                var stats = await _languageService.GetLanguageStatsAsync();
                LanguageStats.Clear();
                foreach (var stat in stats.OrderByDescending(s => s.TotalSeconds))
                {
                    LanguageStats.Add(stat);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading language stats: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task ExecuteViewLanguageSessionsAsync(LanguageStats? language)
        {
            if (language == null) return;

            try
            {
                SelectedLanguage = language;
                var sessions = await _dashBoardService.GetSessionsByLanguageIdAsync(language.LanguageId);
                LanguageSessions.Clear();
                foreach (var session in sessions)
                {
                    LanguageSessions.Add(session);
                }
                ShowLanguageSessions = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading language sessions: {ex.Message}");
            }
        }

        private void ExecuteCloseSessionsView()
        {
            ShowLanguageSessions = false;
            SelectedLanguage = null;
        }
    }
}
