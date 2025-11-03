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
    public class SessionsViewModel : BaseViewModel
    {
        private readonly DashBoardService _dashBoardService = new DashBoardService();
        private readonly ProjectService _projectService = new ProjectService();
        private readonly LanguageService _languageService = new LanguageService();

        private ObservableCollection<Session> _allSessions;
        private ObservableCollection<Session> _filteredSessions;
        private ObservableCollection<Project> _projects;
        private ObservableCollection<Language> _languages;
        private int? _filterProjectId;
        private int? _filterLanguageId;
        private string _searchText = "";

        public ObservableCollection<Session> AllSessions
        {
            get => _allSessions;
            set
            {
                _allSessions = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Session> FilteredSessions
        {
            get => _filteredSessions;
            set
            {
                _filteredSessions = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Project> Projects
        {
            get => _projects;
            set
            {
                _projects = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Language> Languages
        {
            get => _languages;
            set
            {
                _languages = value;
                OnPropertyChanged();
            }
        }

        public int? FilterProjectId
        {
            get => _filterProjectId;
            set
            {
                _filterProjectId = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public int? FilterLanguageId
        {
            get => _filterLanguageId;
            set
            {
                _filterLanguageId = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public ICommand LoadSessionsCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand DeleteSessionCommand { get; }

        public SessionsViewModel()
        {
            _allSessions = new ObservableCollection<Session>();
            _filteredSessions = new ObservableCollection<Session>();
            _projects = new ObservableCollection<Project>();
            _languages = new ObservableCollection<Language>();

            LoadSessionsCommand = new RelayCommand(async () => await ExecuteLoadSessionsAsync());
            ClearFiltersCommand = new RelayCommand(ExecuteClearFilters);
            DeleteSessionCommand = new RelayCommand<Session>(async (session) => await ExecuteDeleteSessionAsync(session));

            _ = InitializeAsync();
        }

        private async System.Threading.Tasks.Task InitializeAsync()
        {
            await ExecuteLoadProjectsAsync();
            await ExecuteLoadLanguagesAsync();
            await ExecuteLoadSessionsAsync();
        }

        private async System.Threading.Tasks.Task ExecuteLoadSessionsAsync()
        {
            try
            {
                var sessions = await _dashBoardService.GetSessionsAsync();
                AllSessions.Clear();
                foreach (var session in sessions)
                {
                    AllSessions.Add(session);
                }
                ApplyFilters();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading sessions: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task ExecuteLoadProjectsAsync()
        {
            try
            {
                var projects = await _projectService.GetProjectsAsync();
                Projects.Clear();
                Projects.Add(new Project { ProjectId = 0, Name = "All Projects" });
                foreach (var project in projects)
                {
                    Projects.Add(project);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading projects: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task ExecuteLoadLanguagesAsync()
        {
            try
            {
                var languages = await _languageService.GetLanguagesAsync();
                Languages.Clear();
                Languages.Add(new Language { LanguageId = 0, Name = "All Languages" });
                foreach (var language in languages)
                {
                    Languages.Add(language);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading languages: {ex.Message}");
            }
        }

        private void ApplyFilters()
        {
            var filtered = AllSessions.AsEnumerable();

            if (FilterProjectId.HasValue && FilterProjectId.Value > 0)
            {
                filtered = filtered.Where(s => s.ProjectId == FilterProjectId.Value);
            }

            if (FilterLanguageId.HasValue && FilterLanguageId.Value > 0)
            {
                filtered = filtered.Where(s => s.Languages != null &&
                                             s.Languages.Any(l => l.LanguageId == FilterLanguageId.Value));
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(s => s.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            FilteredSessions.Clear();
            foreach (var session in filtered)
            {
                FilteredSessions.Add(session);
            }
        }

        private void ExecuteClearFilters()
        {
            FilterProjectId = null;
            FilterLanguageId = null;
            SearchText = "";
        }

        private async System.Threading.Tasks.Task ExecuteDeleteSessionAsync(Session? session)
        {
            if (session == null) return;

            try
            {
                AllSessions.Remove(session);
                ApplyFilters();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting session: {ex.Message}");
            }
        }
    }
}
