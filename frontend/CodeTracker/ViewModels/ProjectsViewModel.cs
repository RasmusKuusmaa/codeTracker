using CodeTracker.Helpers;
using CodeTracker.Models;
using CodeTracker.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace CodeTracker.ViewModels
{
    public class ProjectsViewModel : BaseViewModel
    {
        private readonly ProjectService _projectService = new ProjectService();
        private readonly DashBoardService _dashBoardService = new DashBoardService();

        private ObservableCollection<Project> _projects;
        private ObservableCollection<Session> _projectSessions;
        private Project? _selectedProject;
        private bool _showProjectSessions;
        private ObservableCollection<ChartDataPoint> _chartData;

        public ObservableCollection<Project> Projects
        {
            get => _projects;
            set
            {
                _projects = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ChartDataPoint> ChartData
        {
            get => _chartData;
            set
            {
                _chartData = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Session> ProjectSessions
        {
            get => _projectSessions;
            set
            {
                _projectSessions = value;
                OnPropertyChanged();
            }
        }

        public Project? SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;
                OnPropertyChanged();
            }
        }

        public bool ShowProjectSessions
        {
            get => _showProjectSessions;
            set
            {
                _showProjectSessions = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadProjectsCommand { get; }
        public ICommand ViewProjectSessionsCommand { get; }
        public ICommand CloseSessionsViewCommand { get; }
        public ICommand DeleteProjectCommand { get; }

        public ProjectsViewModel()
        {
            _projects = new ObservableCollection<Project>();
            _projectSessions = new ObservableCollection<Session>();
            _chartData = new ObservableCollection<ChartDataPoint>();

            LoadProjectsCommand = new RelayCommand(async () => await ExecuteLoadProjectsAsync());
            ViewProjectSessionsCommand = new RelayCommand<Project>(async (project) => await ExecuteViewProjectSessionsAsync(project));
            CloseSessionsViewCommand = new RelayCommand(ExecuteCloseSessionsView);
            DeleteProjectCommand = new RelayCommand<Project>(async (project) => await ExecuteDeleteProjectAsync(project));

            _ = ExecuteLoadProjectsAsync();
        }

        private async System.Threading.Tasks.Task ExecuteLoadProjectsAsync()
        {
            try
            {
                var projects = await _projectService.GetProjectsAsync();
                Projects.Clear();
                foreach (var project in projects.OrderByDescending(p => p.TotalTimeMinutes))
                {
                    Projects.Add(project);
                }

                UpdateChartData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading projects: {ex.Message}");
            }
        }

        private void UpdateChartData()
        {
            var chartDataPoints = ChartColors.CreateChartData(
                Projects.ToDictionary(
                    p => p.Name,
                    p => (double)p.TotalTimeSeconds
                )
            );

            ChartData.Clear();
            foreach (var point in chartDataPoints)
            {
                ChartData.Add(point);
            }
        }

        private async System.Threading.Tasks.Task ExecuteViewProjectSessionsAsync(Project? project)
        {
            if (project == null) return;

            try
            {
                SelectedProject = project;
                var sessions = await _dashBoardService.GetSessionsByProjectIdAsync(project.ProjectId);
                ProjectSessions.Clear();
                foreach (var session in sessions)
                {
                    ProjectSessions.Add(session);
                }
                ShowProjectSessions = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading project sessions: {ex.Message}");
            }
        }

        private void ExecuteCloseSessionsView()
        {
            ShowProjectSessions = false;
            SelectedProject = null;
        }

        private async System.Threading.Tasks.Task ExecuteDeleteProjectAsync(Project? project)
        {
            if (project == null) return;

            try
            {
                await _projectService.DeleteProjectAsync(project.ProjectId);
                Projects.Remove(project);
                UpdateChartData();

                if (SelectedProject?.ProjectId == project.ProjectId)
                {
                    ExecuteCloseSessionsView();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting project: {ex.Message}");
            }
        }
    }
}
