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
using Visiblox.Charts;

namespace VotGESOrders.Views {

	public partial class CransPage : Page, INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyChanged(string propName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		public CranFilter CurrentFilter { get; set; }


		protected CranTaskInfo _currentTask;
		public CranTaskInfo CurrentTask {
			get { return _currentTask; }
			set {
				_currentTask = value;
				HasSelectedTask = _currentTask == null ? false : true;
				NotifyChanged("CurrentTask");
			}
		}
		public CranTaskInfo TempTask { get; set; }


		protected bool _hasSelectedTask;
		public bool HasSelectedTask {
			get { return _hasSelectedTask; }
			set {
				_hasSelectedTask = value;
				NotifyChanged("HasSelectedTask");
			}
		}


		public CransPage() {
			InitializeComponent();
			CransContext.init();
			grdRight.DataContext = this;
			init();
			initChart();
			newTask.Visibility = WebContext.Current.User.AllowCreateOrder ? Visibility.Visible : Visibility.Collapsed;
		}

		// Выполняется, когда пользователь переходит на эту страницу.
		protected override void OnNavigatedTo(NavigationEventArgs e) {

		}

		public void initChart() {
			CurrentChart.YAxis = new LinearAxis();
			CurrentChart.XAxis = new DateTimeAxis();
			CurrentChart.YAxis.AutoScaleToVisibleData = false;
			CurrentChart.YAxis.LabelFormatString="";
			IRange range=CurrentChart.YAxis.CreateRange();
			range.Minimum = 0;
			range.Maximum = 2.99;
			CurrentChart.YAxis.SetActualRange(range);
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
			else {
				if (CurrentTask != null && TempTask != null)
					copyTask(CurrentTask, TempTask);
			}
		}

		void Client_getCranTasksCompleted(object sender, CranService.getCranTasksCompletedEventArgs e) {
			CurrentFilter = e.Result as CranFilter;
			grdTasks.ItemsSource = CurrentFilter.Data;
			processCransData();
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
			TempTask = null;
			CurrentTask = new CranTaskInfo();
			CurrentTask.init = true;
			CurrentTask.canChange = true;
			CurrentTask.canCheck = false;
			CurrentTask.CranNumber = 1;
			CurrentTask.Comment = " ";
			CurrentTask.NeedStartDate = DateTime.Now.Date.AddDays(1);
			CurrentTask.NeedEndDate = DateTime.Now.Date.AddDays(2);
			pnlTask.DataContext = CurrentTask;
		}

		private void btnSendTask_Click(object sender, RoutedEventArgs e) {
			if (CurrentTask != null) {
				CransContext.Single.Client.CreateCranTaskAsync(CurrentTask);
			}
		}

		private void grdTasks_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			SelectTask(grdTasks.SelectedItem as CranTaskInfo);
		}

		private void SelectTask(CranTaskInfo task) {
			if (TempTask != null && CurrentTask != null) {
				copyTask(CurrentTask, TempTask);
			}
			CurrentTask = task;
			if (CurrentTask != null) {
				pnlTask.DataContext = CurrentTask;
				TempTask = new CranTaskInfo();
				copyTask(TempTask, CurrentTask);
			}
			try {
				if (grdTasks.SelectedItem != task)
					grdTasks.SelectedItem = task;
				foreach (LineSeries serie in CurrentChart.Series) {

					if (serie.Name == String.Format("order_{0}", task.Number)) {
						serie.LineStrokeThickness = 10;
					}
					else {
						int num = getTaskNumber(serie.Name);
						serie.LineStrokeThickness = 5;
					}
				}
			}
			catch { }
		}

		public void copyTask(CranTaskInfo copy, CranTaskInfo tsk) {
			try {
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
			}
			catch { }
		}


		public void processSingleCran(int cranNumber) {
			Dictionary<DateTime, CranTaskInfo> crans = new Dictionary<DateTime, CranTaskInfo>();	

			foreach (CranTaskInfo task in CurrentFilter.Data) {
				DateTime date = task.NeedStartDate;
				if (task.Allowed)
					date = task.AllowDateStart;
				if (task.CranNumber==cranNumber){
					while (crans.ContainsKey(date))
						date=date.AddMilliseconds(1);
					crans.Add(date,task);
				}
			}
			crans.OrderBy(task => task.Key);
					

			double diffA = 0;
			double diffD = 0.05;
			
			foreach (KeyValuePair<DateTime,CranTaskInfo> de in crans) {
				CranTaskInfo task = de.Value;
				LineSeries serie = new LineSeries();
				DataSeries<DateTime, double> Points = new DataSeries<DateTime, double>();
				serie.Name = String.Format("order_{0}", task.Number);
				serie.MouseLeftButtonUp += serie_MouseLeftButtonUp;
				serie.LineStrokeThickness = 5;
				serie.PointShape = Visiblox.Charts.Primitives.ShapeType.Rectangle;
				serie.PointSize = 15;
				serie.ShowPoints = true;

				if (task.Allowed) {
					Points.Add(new DataPoint<DateTime, double>(task.AllowDateStart, task.CranNumber+diffA));
					Points.Add(new DataPoint<DateTime, double>(task.AllowDateEnd, task.CranNumber + diffA));
					serie.LineStroke = new SolidColorBrush(Colors.Green);
					serie.PointFill = new SolidColorBrush(Colors.Green);
					diffA += 0.05;
				}

				else if (task.Denied) {
					Points.Add(new DataPoint<DateTime, double>(task.NeedStartDate, task.CranNumber - diffD));
					Points.Add(new DataPoint<DateTime, double>(task.NeedEndDate, task.CranNumber -  diffD));

					serie.LineStroke = new SolidColorBrush(Colors.Red);
					serie.PointFill = new SolidColorBrush(Colors.Red);
					diffD += 0.05;
				}

				else {
					Points.Add(new DataPoint<DateTime, double>(task.NeedStartDate, task.CranNumber - diffD));
					Points.Add(new DataPoint<DateTime, double>(task.NeedEndDate, task.CranNumber - diffD));

					serie.LineStroke = new SolidColorBrush(Colors.Gray);
					serie.PointFill = new SolidColorBrush(Colors.Gray);
					diffD += 0.05;
				}

				serie.DataSeries = Points;
				CurrentChart.Series.Add(serie);
			}
			
		}

		void serie_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			LineSeries serie = sender as LineSeries;
			CranTaskInfo task = getTaskBySerie(serie);
			SelectTask(task);
		}

		public void processCransData() {
			CurrentChart.Series.Clear();
					
			processSingleCran(1);
			processSingleCran(2);
		
			grdGraph.InvalidateArrange();
		}

		private int getTaskNumber(string name) {
			string[] parts = name.Split(new char[] { '_' });
			string num = parts[1];
			int number = Int32.Parse(num);
			return number;
		}

		private CranTaskInfo getTaskBySerie(LineSeries serie) {
			try {
				int number = getTaskNumber(serie.Name);
				foreach (CranTaskInfo task in CurrentFilter.Data) {
					if (number == task.Number) {
						return task;
					}
				}
				return null;
			}
			catch {
				return null;
			}
		}


	}

}


