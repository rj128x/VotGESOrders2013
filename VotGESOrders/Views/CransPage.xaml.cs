using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using VotGESOrders.CranService;
using System.ComponentModel;

namespace VotGESOrders.Views {
	public partial class CransPage : Page , INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyChanged(string propName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		public CranFilter CurrentFilter {get;set;}


		protected CranTaskInfo _currentTask;
		public CranTaskInfo CurrentTask { get {return _currentTask; }
			set {
				_currentTask = value;
				HasSelectedTask = _currentTask==null?false:true;
				NotifyChanged("CurrentTask");
			}
		}
		public CranTaskInfo TempTask { get; set; }


		protected  bool _hasSelectedTask;
		public bool HasSelectedTask {
			get { return _hasSelectedTask;}
			set{_hasSelectedTask=value;
			NotifyChanged("HasSelectedTask");
			}
		}


		public CransPage() {
			InitializeComponent();
			CransContext.init();
			grdRight.DataContext = this;
			init();
		}

		// Выполняется, когда пользователь переходит на эту страницу.
		protected override void OnNavigatedTo(NavigationEventArgs e) {

		}

		public void init() {
			CransContext.Single.Client.getCranTasksCompleted += Client_getCranTasksCompleted;
			CransContext.Single.Client.CreateCranTaskCompleted += Client_CreateCranTaskCompleted;
			CransContext.Single.Client.getCranTasksAsync(null);
		}

		public void deinit() {
			CransContext.Single.Client.getCranTasksCompleted -= Client_getCranTasksCompleted;
			CransContext.Single.Client.CreateCranTaskCompleted -= Client_CreateCranTaskCompleted;
		}

		void Client_CreateCranTaskCompleted(object sender, CranService.CreateCranTaskCompletedEventArgs e) {
			ReturnMessage ret = e.Result as ReturnMessage;
			MessageBox.Show(ret.Message);
			if (ret.Result) {
				CurrentTask = null;
				CransContext.Single.Client.getCranTasksAsync(CurrentFilter);
			}
		}

		void Client_getCranTasksCompleted(object sender, CranService.getCranTasksCompletedEventArgs e) {
			CurrentFilter = e.Result as CranFilter;
			grdTasks.ItemsSource = CurrentFilter.Data;
			CurrentTask = null;
		}

		private void btnAllow_Click(object sender, RoutedEventArgs e) {
			CurrentTask.Allowed = true;
			CurrentTask.AllowDateStart = CurrentTask.NeedStartDate;
			CurrentTask.AllowDateEnd = CurrentTask.NeedEndDate;
			CurrentTask.Denied = false;
		}

		private void btnDenie_Click(object sender, RoutedEventArgs e) {
			CurrentTask.Denied = true;
			CurrentTask.Allowed = false;
		}

		private void newTask_Click(object sender, RoutedEventArgs e) {
			CurrentTask = new CranTaskInfo();
			CurrentTask.init = true;
			CurrentTask.canChange = true;
			CurrentTask.canCheck = false;
			CurrentTask.NeedStartDate = DateTime.Now.Date.AddDays(1);
			CurrentTask.NeedEndDate = DateTime.Now.Date.AddDays(2);
			pnlTask.DataContext = CurrentTask;
		}

		private void btnSendTask_Click(object sender, RoutedEventArgs e) {
			CransContext.Single.Client.CreateCranTaskAsync(CurrentTask);
			CurrentTask = null;
		}

		private void grdTasks_SelectionChanged(object sender, SelectionChangedEventArgs e) {			
			TempTask = grdTasks.SelectedItem as CranTaskInfo;
			if (TempTask != null) {
				CurrentTask = copyTask(TempTask);
				pnlTask.DataContext = CurrentTask;
			}
			else {
				CurrentTask = null;
			}
		}

		public CranTaskInfo copyTask(CranTaskInfo tsk) {
			CranTaskInfo copy = new CranTaskInfo();
			copy.Number = tsk.Number;
			copy.CranNumber = tsk.CranNumber;
			copy.NeedStartDate = tsk.NeedStartDate;
			copy.NeedEndDate = tsk.NeedEndDate;
			copy.AllowDateStart = tsk.AllowDateStart;
			copy.AllowDateEnd = tsk.AllowDateEnd;
			copy.Allowed = tsk.Allowed;
			copy.Denied = tsk.Denied;
			copy.State = tsk.State;
			copy.canChange = tsk.canChange;
			copy.canCheck = tsk.canCheck;
			copy.Author = tsk.Author;
			copy.AuthorAllow = tsk.AuthorAllow;
			copy.Comment = tsk.Comment;
			return copy;
		}




	}
}
