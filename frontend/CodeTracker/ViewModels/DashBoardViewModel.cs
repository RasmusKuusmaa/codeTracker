using CodeTracker.Models;
using CodeTracker.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeTracker.ViewModels
{
    public class DashBoardViewModel : BaseViewModel
    {
        private readonly DashBoardService _dashBoardService = new DashBoardService();

        private ObservableCollection<Session> _sessions;

        public ObservableCollection<Session> Sessions
        {
            get { return _sessions; }
            set { _sessions = value;
                OnPropertyChanged();
            }
        }


        public DashBoardViewModel()
        {
            Sessions = new ObservableCollection<Session>();
            _ = LoadSessionsAsync();
        }

        private async Task LoadSessionsAsync()
        {
            var sessions = await _dashBoardService.GetSessionsAsync();
            Sessions.Clear();
            foreach (var session in sessions)
            {
                Sessions.Add(session);
            }

        }
    }
}
