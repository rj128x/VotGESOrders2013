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
using System.Windows.Controls.DataVisualization.Charting;

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
		}

		// Выполняется, когда пользователь переходит на эту страницу.
		protected override void OnNavigatedTo(NavigationEventArgs e) {

		}

		protected DateTimeAxis xAxis;
		protected LinearAxis yAxis;
		protected Style slGreen;
		protected Style spGreen;
		protected Style slRed;
		protected Style spRed;
		protected Style slGray;
		protected Style spGray;
		protected Style slSel;
		protected Style spSel;
		protected Dictionary<int, Style> StylesPoly;
		protected Dictionary<int, Style> StylesPoint;

		public void init() {
			xAxis = new DateTimeAxis();
			yAxis = new LinearAxis();
			xAxis.Orientation = AxisOrientation.X;
			xAxis.Location = AxisLocation.Bottom;
			xAxis.IntervalType = DateTimeIntervalType.Hours;
			xAxis.Interval = 6;
			xAxis.ShowGridLines = true;
			yAxis.Orientation = AxisOrientation.Y;
			yAxis.Location = AxisLocation.Left;
			yAxis.Minimum = 0;
			yAxis.Maximum = 2.9;
			yAxis.Interval = 1;
			yAxis.ShowGridLines = true;
			chtTasks.Axes.Add(xAxis);
			chtTasks.Axes.Add(yAxis);

			slGreen = new Style(typeof(Polyline));
			slGreen.Setters.Add(new Setter(Polyline.StrokeProperty, new SolidColorBrush(Colors.Green)));
			slGreen.Setters.Add(new Setter(Polyline.StrokeThicknessProperty, 10));
			slGreen.Setters.Add(new Setter(Polyline.OpacityProperty, 0.8));
			spGreen = new Style(typeof(DataPoint));
			spGreen.Setters.Add(new Setter(DataPoint.WidthProperty, 20));
			spGreen.Setters.Add(new Setter(DataPoint.HeightProperty, 20));
			spGreen.Setters.Add(new Setter(DataPoint.BackgroundProperty, new SolidColorBrush(Colors.Green)));


			slRed = new Style(typeof(Polyline));
			slRed.Setters.Add(new Setter(Polyline.StrokeProperty, new SolidColorBrush(Colors.Red)));
			slRed.Setters.Add(new Setter(Polyline.StrokeThicknessProperty, 10));
			slRed.Setters.Add(new Setter(Polyline.OpacityProperty, 0.8));
			spRed = new Style(typeof(DataPoint));
			spRed.Setters.Add(new Setter(DataPoint.WidthProperty, 15));
			spRed.Setters.Add(new Setter(DataPoint.HeightProperty, 15));
			spRed.Setters.Add(new Setter(DataPoint.BackgroundProperty, new SolidColorBrush(Colors.Red)));


			slGray = new Style(typeof(Polyline));
			slGray.Setters.Add(new Setter(Polyline.StrokeProperty, new SolidColorBrush(Colors.Gray)));
			slGray.Setters.Add(new Setter(Polyline.StrokeThicknessProperty, 10));
			slGray.Setters.Add(new Setter(Polyline.OpacityProperty, 0.8));
			spGray = new Style(typeof(DataPoint));
			spGray.Setters.Add(new Setter(DataPoint.WidthProperty, 15));
			spGray.Setters.Add(new Setter(DataPoint.HeightProperty, 15));
			spGray.Setters.Add(new Setter(DataPoint.BackgroundProperty, new SolidColorBrush(Colors.Gray)));

			slSel = new Style(typeof(Polyline));
			slSel.Setters.Add(new Setter(Polyline.StrokeProperty, new SolidColorBrush(Colors.Blue)));
			slSel.Setters.Add(new Setter(Polyline.StrokeThicknessProperty, 10));
			slSel.Setters.Add(new Setter(Polyline.OpacityProperty, 0.8));
			spSel = new Style(typeof(DataPoint));
			spSel.Setters.Add(new Setter(DataPoint.WidthProperty, 15));
			spSel.Setters.Add(new Setter(DataPoint.HeightProperty, 15));
			spSel.Setters.Add(new Setter(DataPoint.BackgroundProperty, new SolidColorBrush(Colors.Blue)));

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
			CurrentTask = new CranTaskInfo();
			CurrentTask.init = true;
			CurrentTask.canChange = true;
			CurrentTask.canCheck = false;
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
				foreach (LineSeries serie in chtTasks.Series) {					
					if (serie.Name == String.Format("order_{0}", task.Number)) {
						serie.PolylineStyle = slSel;
						serie.DataPointStyle = spSel;
						
					}
					else {
						int num = getTaskNumber(serie.Name);
						serie.PolylineStyle = StylesPoly[num];
						serie.DataPointStyle = StylesPoint[num];
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

		public void processCransData() {

			chtTasks.Series.Clear();

			Style legendStyle = new Style(typeof(FrameworkElement));
			legendStyle.Setters.Add(new Setter(FrameworkElement.WidthProperty, 0));
			chtTasks.LegendStyle = legendStyle;

			Style titleStyle = new Style(typeof(FrameworkElement));
			titleStyle.Setters.Add(new Setter(FrameworkElement.HeightProperty, 0));
			chtTasks.TitleStyle = titleStyle;

			StylesPoly = new Dictionary<int, Style>();
			StylesPoint = new Dictionary<int, Style>();

			foreach (CranTaskInfo task in CurrentFilter.Data) {
				LineSeries serie = new LineSeries();
				List<TaskInfo> Points = new List<TaskInfo>();
				serie.DependentValuePath = "Cran";
				serie.IndependentValuePath = "Date";
				serie.ItemsSource = Points;
				serie.Title = String.Format("Заявка №{0} ({1})", task.Number, task.State);
				serie.Name = String.Format("order_{0}", task.Number);
				serie.IsSelectionEnabled = false;
				serie.MouseLeftButtonUp += serie_MouseLeftButtonUp;

				if (task.Allowed) {
					Points.Add(new TaskInfo(task.AllowDateStart, task.CranNumber));
					Points.Add(new TaskInfo(task.AllowDateEnd, task.CranNumber));
					serie.DataPointStyle = spGreen;
					serie.PolylineStyle = slGreen;

					StylesPoly.Add(task.Number, slGreen);
					StylesPoint.Add(task.Number, spGreen);
				}

				else if (task.Denied) {
					Points.Add(new TaskInfo(task.NeedStartDate, task.CranNumber - 0.3));
					Points.Add(new TaskInfo(task.NeedEndDate, task.CranNumber - 0.3));
					serie.DataPointStyle = spRed;
					serie.PolylineStyle = slRed;

					StylesPoly.Add(task.Number, slRed);
					StylesPoint.Add(task.Number, spRed);
				}

				else {
					Points.Add(new TaskInfo(task.NeedStartDate, task.CranNumber - 0.3));
					Points.Add(new TaskInfo(task.NeedEndDate, task.CranNumber - 0.3));
					serie.DataPointStyle = spGray;
					serie.PolylineStyle = slGray;

					StylesPoly.Add(task.Number, slGray);
					StylesPoint.Add(task.Number, spGray);
				}

				chtTasks.Series.Add(serie);
				serie.IndependentAxis = xAxis;
				serie.DependentRangeAxis = yAxis;
			}

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

		void serie_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {			
			LineSeries serie = sender as LineSeries;
			CranTaskInfo task = getTaskBySerie(serie);
			SelectTask(task);
		}
	}

	public class TaskInfo {
		public DateTime Date { get; set; }
		public double Cran { get; set; }
		public TaskInfo(DateTime date, double cran) {
			Cran = cran;
			Date = date;
		}
	}
}


