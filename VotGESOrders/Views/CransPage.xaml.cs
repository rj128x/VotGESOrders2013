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
			if (!MainPage.STARTING)
				init();
		}

		// Выполняется, когда пользователь переходит на эту страницу.
		protected override void OnNavigatedTo(NavigationEventArgs e) {

		}

		public void initChart() {
			CurrentChart.YAxis = new LinearAxis();
			CurrentChart.XAxis = new DateTimeAxis();
			CurrentChart.YAxis.Element.Width = 0;
			CurrentChart.YAxis.LabelFormatString="";
			CurrentChart.YAxis.AutoScaleToVisibleData = false;
			CurrentChart.YAxis.IsAutoMarginEnabled = false;
			IRange range=CurrentChart.YAxis.CreateRange();
			range.Minimum = 0;
			range.Maximum = 3;
			CurrentChart.YAxis.SetActualRange(range);
			CurrentChart.XAxis.IsAutoMarginEnabled = false;
		}


		public void init() {
			CransContext.init();
			grdRight.DataContext = this;
			CransContext.Single.Client.getCranTasksCompleted += Client_getCranTasksCompleted;
			
			GlobalStatus.Current.IsBusy = true;
			CransContext.Single.Client.getCranTasksAsync(null);
			initChart();
			newTask.Visibility = WebContext.Current.User.AllowCreateOrder ? Visibility.Visible : Visibility.Collapsed;
		}

		public void deinit() {
			CransContext.Single.Client.getCranTasksCompleted -= Client_getCranTasksCompleted;
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e) {
			base.OnNavigatedFrom(e);
			deinit();
		}

		

		void Client_getCranTasksCompleted(object sender, CranService.getCranTasksCompletedEventArgs e) {
			GlobalStatus.Current.IsBusy = false;
			CurrentFilter = e.Result as CranFilter;
			pnlFilter.DataContext = CurrentFilter;
			grdTasks.ItemsSource = CurrentFilter.Data;
			processCransData();
			CurrentTask = null;
		}

		

		private void newTask_Click(object sender, RoutedEventArgs e) {
			CranTaskInfo newTask  = new CranTaskInfo();
			newTask.init = true;
			newTask.canChange = true;
			newTask.canCheck = false;
			newTask.CranNumber = 1;
			newTask.Comment = " ";
			newTask.NeedStartDate = DateTime.Now.Date.AddDays(1);
			newTask.NeedEndDate = DateTime.Now.Date.AddDays(2);
			CranWindow taskWindow = new CranWindow();
			taskWindow.init(newTask);
			taskWindow.Closed += taskWindow_Closed;
			taskWindow.Show();
		}
		
		private void btnChange_Click(object sender, RoutedEventArgs e) {
			if (CurrentTask == null)
				return;
			CranWindow taskWindow = new CranWindow();
			taskWindow.init(CurrentTask);
			taskWindow.Closed += taskWindow_Closed;
			taskWindow.Show();
		}

		void taskWindow_Closed(object sender, EventArgs e) {
			CranWindow win = sender as CranWindow;
			GlobalStatus.Current.IsBusy = true;
			CurrentTask = null;
			CransContext.Single.Client.getCranTasksAsync(CurrentFilter);
		}

		private void btnCheck_Click(object sender, RoutedEventArgs e) {
			if (CurrentTask == null)
				return;
			CranReviewWindow reviewWin = new CranReviewWindow();
			reviewWin.init(CurrentTask);
			reviewWin.Closed += reviewWin_Closed;
			reviewWin.Show();
		}

		void reviewWin_Closed(object sender, EventArgs e) {
			CranReviewWindow win = sender as CranReviewWindow;
			GlobalStatus.Current.IsBusy = true;
			CurrentTask = null;
			CransContext.Single.Client.getCranTasksAsync(CurrentFilter);
		}


		private void btnSendTask_Click(object sender, RoutedEventArgs e) {
			if (CurrentTask != null) {
				GlobalStatus.Current.IsBusy = true;
				CransContext.Single.Client.CreateCranTaskAsync(CurrentTask);
			}
		}

		private void grdTasks_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			SelectTask(grdTasks.SelectedItem as CranTaskInfo);
		}

		private void SelectTask(CranTaskInfo task) {
			CurrentTask = task;
			if (CurrentTask != null) {
				pnlTask.DataContext = CurrentTask;
			}
			try {
				if (grdTasks.SelectedItem != task)
					grdTasks.SelectedItem = task;
				foreach (LineSeries serie in CurrentChart.Series) {
					try {
						if (serie.Name == String.Format("order_{0}", task.Number)) {
							serie.LineStrokeThickness = 10;
						}
						else {
							int num = getTaskNumber(serie.Name);
							serie.LineStrokeThickness = 5;
						}
					}
					catch { }
				}
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

			DateTime min = DateTime.MaxValue;
			DateTime max = DateTime.MinValue;

			foreach (CranTaskInfo task in CurrentFilter.Data) {
				if (!task.Allowed) {
					if (task.NeedStartDate < min)
						min = task.NeedStartDate;
					if (task.NeedEndDate > max)
						max = task.NeedEndDate;
				}
				if (task.Allowed) {
					if (task.AllowDateStart < min)
						min = task.AllowDateStart;
					if (task.AllowDateEnd > max)
						max = task.AllowDateEnd;
				}
			}

			LineSeries nulSer = new LineSeries();
			DataSeries<DateTime, double> nulP = new DataSeries<DateTime, double>();
			nulP.Add(new DataPoint<DateTime, double>(min, 0.5));
			nulP.Add(new DataPoint<DateTime, double>(max, 0.5));

			nulSer.DataSeries = nulP;

			LineSeries ser15 = new LineSeries();
			DataSeries<DateTime, double> p15 = new DataSeries<DateTime, double>();
			p15.Add(new DataPoint<DateTime, double>(min, 1.5));
			p15.Add(new DataPoint<DateTime, double>(max, 1.5));
			ser15.DataSeries = p15;

			LineSeries ser3 = new LineSeries();
			DataSeries<DateTime, double> p3 = new DataSeries<DateTime, double>();
			p3.Add(new DataPoint<DateTime, double>(min, 2.5));
			p3.Add(new DataPoint<DateTime, double>(max, 2.5));
			ser3.DataSeries = p3;

			CurrentChart.Series.Add(nulSer);
			CurrentChart.Series.Add(ser15);
			CurrentChart.Series.Add(ser3);

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

		private void btnRefresh_Click(object sender, RoutedEventArgs e) {
			GlobalStatus.Current.IsBusy = true;
			CurrentTask = null;
			CransContext.Single.Client.getCranTasksAsync(CurrentFilter);
		}

		



	}

}


