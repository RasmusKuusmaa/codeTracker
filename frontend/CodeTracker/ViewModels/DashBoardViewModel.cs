using CodeTracker.Helpers;
using CodeTracker.Models;
using CodeTracker.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace CodeTracker.ViewModels
{
    public class DashBoardViewModel : BaseViewModel
    {
        private readonly DashBoardService _dashBoardService = new DashBoardService();
        private readonly ProjectService _projectService = new ProjectService();
        private readonly LanguageService _languageService = new LanguageService();
        private readonly DispatcherTimer _timer;
        private readonly Stopwatch _stopwatch;

        private Session? _currentSession;
        private int _elapsedSeconds;
        private ObservableCollection<Project> _projects;
        private ObservableCollection<Language> _languages;
        private ObservableCollection<Language> _availableLanguages;
        private string _sessionTitle = "";
        private int? _selectedProjectId;
        private bool _isSessionActive;
        private bool _isSessionPaused;
        private bool _showNewSessionDialog;
        private bool _showNewProjectDialog;
        private string _newProjectName = "";
        private string _newProjectDescription = "";

        public Session? CurrentSession
        {
            get => _currentSession;
            set
            {
                _currentSession = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasActiveSession));
                OnPropertyChanged(nameof(CurrentSessionDisplay));
            }
        }

        public int ElapsedSeconds
        {
            get => _elapsedSeconds;
            set
            {
                _elapsedSeconds = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ElapsedTimeDisplay));
            }
        }

        public string ElapsedTimeDisplay
        {
            get
            {
                var time = TimeSpan.FromSeconds(ElapsedSeconds);
                return $"{(int)time.TotalHours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
            }
        }

        public bool HasActiveSession => CurrentSession != null && (IsSessionActive || IsSessionPaused);

        public string CurrentSessionDisplay =>
            CurrentSession != null
                ? $"{CurrentSession.Title} - {CurrentSession.ProjectName ?? "No Project"}"
                : "No active session";

        public ObservableCollection<Project> Projects
        {
            get => _projects;
            set
            {
                _projects = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Language> AvailableLanguages
        {
            get => _availableLanguages;
            set
            {
                _availableLanguages = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Language> SelectedLanguages
        {
            get => _languages;
            set
            {
                _languages = value;
                OnPropertyChanged();
            }
        }

        public string SessionTitle
        {
            get => _sessionTitle;
            set
            {
                _sessionTitle = value;
                OnPropertyChanged();
            }
        }

        public int? SelectedProjectId
        {
            get => _selectedProjectId;
            set
            {
                _selectedProjectId = value;
                OnPropertyChanged();
            }
        }

        public bool IsSessionActive
        {
            get => _isSessionActive;
            set
            {
                _isSessionActive = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasActiveSession));
                OnPropertyChanged(nameof(CanPause));
                OnPropertyChanged(nameof(CanResume));
                OnPropertyChanged(nameof(CanStop));
            }
        }

        public bool IsSessionPaused
        {
            get => _isSessionPaused;
            set
            {
                _isSessionPaused = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasActiveSession));
                OnPropertyChanged(nameof(CanPause));
                OnPropertyChanged(nameof(CanResume));
            }
        }

        public bool ShowNewSessionDialog
        {
            get => _showNewSessionDialog;
            set
            {
                _showNewSessionDialog = value;
                OnPropertyChanged();
            }
        }

        public bool ShowNewProjectDialog
        {
            get => _showNewProjectDialog;
            set
            {
                _showNewProjectDialog = value;
                OnPropertyChanged();
            }
        }

        public string NewProjectName
        {
            get => _newProjectName;
            set
            {
                _newProjectName = value;
                OnPropertyChanged();
            }
        }

        public string NewProjectDescription
        {
            get => _newProjectDescription;
            set
            {
                _newProjectDescription = value;
                OnPropertyChanged();
            }
        }

        public bool CanPause => IsSessionActive && !IsSessionPaused;
        public bool CanResume => IsSessionActive && IsSessionPaused;
        public bool CanStop => IsSessionActive;

        public ICommand ShowNewSessionDialogCommand { get; }
        public ICommand StartSessionCommand { get; }
        public ICommand PauseSessionCommand { get; }
        public ICommand ResumeSessionCommand { get; }
        public ICommand StopSessionCommand { get; }
        public ICommand ShowNewProjectDialogCommand { get; }
        public ICommand CreateProjectCommand { get; }
        public ICommand CancelProjectCommand { get; }
        public ICommand CancelSessionCommand { get; }
        public ICommand ToggleLanguageCommand { get; }

        public DashBoardViewModel()
        {
            _projects = new ObservableCollection<Project>();
            _languages = new ObservableCollection<Language>();
            _availableLanguages = new ObservableCollection<Language>();

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Timer_Tick;

            _stopwatch = new Stopwatch();

            ShowNewSessionDialogCommand = new RelayCommand(ExecuteShowNewSessionDialog, () => !HasActiveSession);
            StartSessionCommand = new RelayCommand(async () => await ExecuteStartSessionAsync(), CanStartSession);
            PauseSessionCommand = new RelayCommand(async () => await ExecutePauseSessionAsync(), () => CanPause);
            ResumeSessionCommand = new RelayCommand(async () => await ExecuteResumeSessionAsync(), () => CanResume);
            StopSessionCommand = new RelayCommand(async () => await ExecuteStopSessionAsync(), () => CanStop);
            ShowNewProjectDialogCommand = new RelayCommand(ExecuteShowNewProjectDialog);
            CreateProjectCommand = new RelayCommand(async () => await ExecuteCreateProjectAsync(), CanCreateProject);
            CancelProjectCommand = new RelayCommand(() => ShowNewProjectDialog = false);
            CancelSessionCommand = new RelayCommand(() => ShowNewSessionDialog = false);
            ToggleLanguageCommand = new RelayCommand<Language>(ExecuteToggleLanguage);

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await LoadProjectsAsync();
            await LoadLanguagesAsync();
            await LoadActiveOrLastSessionAsync();
        }

        private async Task LoadProjectsAsync()
        {
            try
            {
                var projects = await _projectService.GetProjectsAsync();
                Projects.Clear();
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

        private async Task LoadLanguagesAsync()
        {
            try
            {
                var languages = await _languageService.GetLanguagesAsync();
                AvailableLanguages.Clear();
                foreach (var language in languages)
                {
                    AvailableLanguages.Add(language);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading languages: {ex.Message}");
            }
        }

        private async Task LoadActiveOrLastSessionAsync()
        {
            try
            {
                var activeSession = await _dashBoardService.GetActiveSessionAsync();
                if (activeSession != null)
                {
                    CurrentSession = activeSession;
                    ElapsedSeconds = activeSession.DurationSeconds;

                    if (activeSession.Status == "running")
                    {
                        IsSessionActive = true;
                        IsSessionPaused = false;
                        _stopwatch.Start();
                        _timer.Start();
                    }
                    else if (activeSession.Status == "paused")
                    {
                        IsSessionActive = true;
                        IsSessionPaused = true;
                    }
                }
                else
                {
                    var lastSession = await _dashBoardService.GetLastSessionAsync();
                    CurrentSession = lastSession;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading session: {ex.Message}");
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            ElapsedSeconds++;
        }

        private void ExecuteShowNewSessionDialog()
        {
            SessionTitle = "";
            SelectedProjectId = null;
            SelectedLanguages.Clear();
            ShowNewSessionDialog = true;
        }

        private bool CanStartSession()
        {
            return !string.IsNullOrWhiteSpace(SessionTitle) && SelectedLanguages.Count > 0;
        }

        private async Task ExecuteStartSessionAsync()
        {
            try
            {
                var request = new CreateSessionRequest
                {
                    Title = SessionTitle,
                    ProjectId = SelectedProjectId,
                    LanguageIds = SelectedLanguages.Select(l => l.LanguageId).ToList()
                };

                var session = await _dashBoardService.CreateSessionAsync(request);
                CurrentSession = session;
                ElapsedSeconds = 0;
                IsSessionActive = true;
                IsSessionPaused = false;
                ShowNewSessionDialog = false;

                _stopwatch.Restart();
                _timer.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error starting session: {ex.Message}");
            }
        }

        private async Task ExecutePauseSessionAsync()
        {
            if (CurrentSession == null) return;

            try
            {
                _timer.Stop();
                _stopwatch.Stop();

                await _dashBoardService.PauseSessionAsync(CurrentSession.SessionId, ElapsedSeconds);
                IsSessionPaused = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error pausing session: {ex.Message}");
                _timer.Start();
                _stopwatch.Start();
            }
        }

        private async Task ExecuteResumeSessionAsync()
        {
            if (CurrentSession == null) return;

            try
            {
                await _dashBoardService.ResumeSessionAsync(CurrentSession.SessionId);
                IsSessionPaused = false;

                _stopwatch.Start();
                _timer.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error resuming session: {ex.Message}");
            }
        }

        private async Task ExecuteStopSessionAsync()
        {
            if (CurrentSession == null) return;

            try
            {
                _timer.Stop();
                _stopwatch.Stop();

                var stoppedSession = await _dashBoardService.StopSessionAsync(CurrentSession.SessionId, ElapsedSeconds);
                CurrentSession = stoppedSession;
                IsSessionActive = false;
                IsSessionPaused = false;
                ElapsedSeconds = 0;

                await LoadProjectsAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error stopping session: {ex.Message}");
            }
        }

        private void ExecuteShowNewProjectDialog()
        {
            NewProjectName = "";
            NewProjectDescription = "";
            ShowNewProjectDialog = true;
        }

        private bool CanCreateProject()
        {
            return !string.IsNullOrWhiteSpace(NewProjectName);
        }

        private async Task ExecuteCreateProjectAsync()
        {
            try
            {
                var request = new CreateProjectRequest
                {
                    Name = NewProjectName,
                    Description = NewProjectDescription
                };

                var project = await _projectService.CreateProjectAsync(request);
                Projects.Add(project);
                ShowNewProjectDialog = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating project: {ex.Message}");
            }
        }

        private void ExecuteToggleLanguage(Language? language)
        {
            if (language == null) return;

            var existing = SelectedLanguages.FirstOrDefault(l => l.LanguageId == language.LanguageId);
            if (existing != null)
            {
                SelectedLanguages.Remove(existing);
            }
            else
            {
                SelectedLanguages.Add(language);
            }
        }
    }
}
