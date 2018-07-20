﻿using System;
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
using System.Windows.Data;

namespace VotGESOrders.Views {

	public partial class CransPage : Page, INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyChanged(string propName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		public CranFilter CurrentFilter { get; set; }
		public List<String> Managers { get; set; }
        public List<String> CranUsers { get; set; }
        public List<String> StropUsers { get; set; }


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

		public void initChart(Chart CurrentChart) {
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
			CurrentChart.XAxis.LabelFormatString = "dd.MM HH";
			CurrentChart.XAxis.Element.Height = 20;
			CurrentChart.XAxis.Element.MaxHeight = 20;
			(CurrentChart.XAxis as DateTimeAxis).ShowMinorTicks = false;
			

		}


		public void init() {
			CransContext.init();
			grdRight.DataContext = this;
			CransContext.Single.Client.getCranTasksCompleted += Client_getCranTasksCompleted;
			CransContext.Single.Client.CommentCranTaskCompleted+=Client_CommentCranTaskCompleted;
			CransContext.Single.Client.CancelCranTaskCompleted += Client_CancelCranTaskCompleted;
			CransContext.Single.Client.FinishCranTaskCompleted += Client_FinishCranTaskCompleted;

			GlobalStatus.Current.IsBusy = true;
			CransContext.Single.Client.getCranTasksAsync(null);
			initChart(ChartMZ);
			initChart(ChartSUS);
			initChart(ChartNBVB);
			initChart(ChartTransVSP);
			initChart(ChartPromPlosh);
			newTask.Visibility = WebContext.Current.User.AllowCreateCranTask ? Visibility.Visible : Visibility.Collapsed;
		}

		





		public void deinit() {
			CransContext.Single.Client.getCranTasksCompleted -= Client_getCranTasksCompleted;
			CransContext.Single.Client.CommentCranTaskCompleted -= Client_CommentCranTaskCompleted;
			CransContext.Single.Client.CancelCranTaskCompleted -= Client_CancelCranTaskCompleted;
			CransContext.Single.Client.FinishCranTaskCompleted -= Client_FinishCranTaskCompleted;
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e) {
			base.OnNavigatedFrom(e);
			deinit();
		}

		

		void Client_getCranTasksCompleted(object sender, CranService.getCranTasksCompletedEventArgs e) {
			GlobalStatus.Current.IsBusy = false;
			CurrentFilter = e.Result as CranFilter;
			Managers = CurrentFilter.Managers.ToList();
            StropUsers = CurrentFilter.StropUsers.ToList();
            CranUsers = CurrentFilter.CranUsers.ToList();
            CurrentFilter.Managers = null;
            CurrentFilter.StropUsers = null;
            CurrentFilter.CranUsers = null;
			pnlFilter.DataContext = CurrentFilter;
			//grdTasks.ItemsSource = CurrentFilter.Data;
			PagedCollectionView pcv = new PagedCollectionView(CurrentFilter.Data);
			pcv.GroupDescriptions.Add(new PropertyGroupDescription("CranName"));
			grdTasks.ItemsSource = pcv;
			processCransData(ChartMZ,(new int[]{1,2}).ToList());
			processCransData(ChartSUS, (new int[] { 3, 4 }).ToList());
			processCransData(ChartNBVB, (new int[] { 5, 6 }).ToList());
			processCransData(ChartTransVSP, (new int[] { 7, 8 }).ToList());
			processCransData(ChartPromPlosh, (new int[] { -1, 9 }).ToList());
			CurrentTask = null;
		}

		

		private void newTask_Click(object sender, RoutedEventArgs e) {
			CranTaskInfo newTask  = new CranTaskInfo();
			newTask.init = true;
			newTask.canChange = true;
			newTask.canCheck = false;
			newTask.CranNumber = 0;
			newTask.Comment = "";
			newTask.NeedStartDate = DateTime.Now.Date.AddDays(1);
			newTask.NeedEndDate = DateTime.Now.Date.AddDays(2);
            newTask.SelAuthor = WebContext.Current.User.Name;
            CranWindow taskWindow = new CranWindow();
			taskWindow.init(newTask,Managers,StropUsers);
			taskWindow.Closed += taskWindow_Closed;
			taskWindow.Show();
		}
		
		private void btnChange_Click(object sender, RoutedEventArgs e) {
			if (CurrentTask == null)
				return;
			CranWindow taskWindow = new CranWindow();
			taskWindow.init(CurrentTask,Managers,StropUsers);
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
			reviewWin.init(CurrentTask,CranUsers);
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
			if (task == null)
				return;
			if (CurrentTask != null) {
				pnlTask.DataContext = CurrentTask;
			}
			Chart CurrentChart = null;

			switch (task.CranNumber){
				case 1:
				case 2:
					CurrentChart = ChartMZ;
					tCntrl.SelectedIndex = 0;
					break;
				case 3:
				case 4:
					CurrentChart = ChartSUS;
					tCntrl.SelectedIndex = 1;
					break;
				case 5:
				case 6:
					CurrentChart = ChartNBVB;
					tCntrl.SelectedIndex = 2;
					break;
				case 7:
				case 8:
					CurrentChart = ChartTransVSP;
					tCntrl.SelectedIndex = 3;
					break;
				case 9:
					CurrentChart = ChartPromPlosh;
					tCntrl.SelectedIndex = 4;
					break;
				default:
					return;
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

		public void processSingleCran(int cranNumber,Chart CurrentChart,int cranVal) {
			Dictionary<DateTime, CranTaskInfo> crans = new Dictionary<DateTime, CranTaskInfo>();	

			foreach (CranTaskInfo task in CurrentFilter.Data) {
				DateTime date = task.NeedEndDate;
				if (task.Allowed)
					date = task.AllowDateEnd;
				if (task.CranNumber==cranNumber){
					while (crans.ContainsKey(date))
						date=date.AddMilliseconds(1);
					crans.Add(date,task);
				}
			}
			IEnumerable<KeyValuePair<DateTime,CranTaskInfo>> sorted= crans.OrderBy(task => task.Key);
					

			double diffA = 0;
			double diffD = 0.05;

			CranTaskInfo prevTaskA = null;
			CranTaskInfo prevTaskD = null;

			foreach (KeyValuePair<DateTime,CranTaskInfo> de in sorted) {
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
					if (prevTaskA != null && task.AllowDateStart > prevTaskA.AllowDateEnd) {
							diffA = 0;
					}
					if (!task.Finished) {
						Points.Add(new DataPoint<DateTime, double>(task.AllowDateStart, cranVal + diffA));
						Points.Add(new DataPoint<DateTime, double>(task.AllowDateEnd, cranVal + diffA));
					}
					else {
						Points.Add(new DataPoint<DateTime, double>(task.RealDateStart, cranVal + diffA));
						Points.Add(new DataPoint<DateTime, double>(task.RealDateEnd, cranVal + diffA));
					}
					serie.LineStroke = new SolidColorBrush(!task.Finished?Colors.Green:Colors.Blue);
					serie.PointFill = new SolidColorBrush(!task.Finished ? Colors.Green : Colors.Blue);
					diffA += 0.05;
					prevTaskA = task;
				}

				else if (task.Denied) {
					if (prevTaskD != null && task.NeedStartDate > prevTaskD.NeedEndDate) {
						diffD = 0.05;
					}
					Points.Add(new DataPoint<DateTime, double>(task.NeedStartDate, cranVal - diffD));
					Points.Add(new DataPoint<DateTime, double>(task.NeedEndDate, cranVal - diffD));

					serie.LineStroke = new SolidColorBrush(Colors.Red);
					serie.PointFill = new SolidColorBrush(Colors.Red);
					diffD += 0.05;
					prevTaskD = task;
				}

				else if (task.Cancelled) {
					if (prevTaskD != null && task.NeedStartDate > prevTaskD.NeedEndDate) {
						diffD = 0.05;
					}
					Points.Add(new DataPoint<DateTime, double>(task.NeedStartDate, cranVal - diffD));
					Points.Add(new DataPoint<DateTime, double>(task.NeedEndDate, cranVal - diffD));

					serie.LineStroke = new SolidColorBrush(Colors.LightGray);
					serie.PointFill = new SolidColorBrush(Colors.LightGray);
					diffD += 0.05;
					prevTaskD = task;
				}

				else if (task.Finished) {
					if (prevTaskA != null && task.AllowDateStart > prevTaskA.AllowDateEnd) {
						diffA = 0;
					}
					Points.Add(new DataPoint<DateTime, double>(task.AllowDateStart, cranVal + diffA));
					Points.Add(new DataPoint<DateTime, double>(task.AllowDateEnd, cranVal + diffA));
					serie.LineStroke = new SolidColorBrush(Colors.Blue);
					serie.PointFill = new SolidColorBrush(Colors.Blue);
					diffA += 0.05;
					prevTaskA = task;
				}

				else  {
					if (prevTaskD != null && task.NeedStartDate > prevTaskD.NeedEndDate) {
						diffD = 0.05;
					}
					Points.Add(new DataPoint<DateTime, double>(task.NeedStartDate, cranVal - diffD));
					Points.Add(new DataPoint<DateTime, double>(task.NeedEndDate, cranVal - diffD));

					serie.LineStroke = new SolidColorBrush(Colors.Gray);
					serie.PointFill = new SolidColorBrush(Colors.Gray);
					diffD += 0.05;
					prevTaskD = task;
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

		public void processCransData(Chart CurrentChart, List<int>Crans) {
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

			int i = 1;
			foreach (int cran in Crans){
				processSingleCran(cran,CurrentChart,i++);				
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

		private void btnRefresh_Click(object sender, RoutedEventArgs e) {
			GlobalStatus.Current.IsBusy = true;
			CurrentTask = null;
			CransContext.Single.Client.getCranTasksAsync(CurrentFilter);
		}

		private void CurrentChart_MouseWheel(object sender, MouseWheelEventArgs e) {
			try {
				Chart CurrentChart = sender as Chart;
				ScrollViewer scrChart = CurrentChart.Parent as ScrollViewer;
				double newSize = CurrentChart.ActualWidth + e.Delta/3;
				if (newSize > scrChart.ActualWidth * 0.98 && newSize < scrChart.ActualWidth * 3)
					CurrentChart.Width = newSize;
			}catch{}
		}

		private void btnComment_Click(object sender, RoutedEventArgs e) {
			if (CurrentTask == null)
				return;
			if (String.IsNullOrEmpty(txtComment.Text)) {
				MessageBox.Show("Введите комментарий");
				return;
			}
			GlobalStatus.Current.IsBusy = true;
			CransContext.Single.Client.CommentCranTaskAsync(CurrentTask, txtComment.Text);
				
		}


		private void btnAgree_Click(object sender, RoutedEventArgs e) {
			txtComment.Text = "Согласовано";
		}

		private void btnNotAgree_Click(object sender, RoutedEventArgs e) {
			txtComment.Text = "Не согласовано";
		}

		private void Client_CommentCranTaskCompleted(object sender, CommentCranTaskCompletedEventArgs e) {
			GlobalStatus.Current.IsBusy = false;
			if (e.Result.Result) {
				GlobalStatus.Current.IsBusy = true;
				CransContext.Single.Client.getCranTasksAsync(CurrentFilter);
			}
			else {
				MessageBox.Show(e.Result.Message);
			}
		}

		private void btnPrint_Click(object sender, RoutedEventArgs e) {
			FloatWindow.OpenWindow(String.Format("/Home/PrintCranTasks?year1={0}&month1={1}&day1={2}&year2={3}&month2={4}&day2={5}",
				CurrentFilter.DateStart.Year, CurrentFilter.DateStart.Month, CurrentFilter.DateStart.Day,
				CurrentFilter.DateEnd.Year, CurrentFilter.DateEnd.Month, CurrentFilter.DateEnd.Day));
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e) {
			if (CurrentTask == null)
				return;
			if (MessageBox.Show("Вы уверены что хотите снять заявку?", "Отмена заявки", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
				GlobalStatus.Current.IsBusy = true;
				CurrentTask.Cancelled = true;
				CurrentTask.Allowed = false;
				CurrentTask.Denied = false;
				CransContext.Single.Client.CancelCranTaskAsync(CurrentTask);
			}
		}

		void Client_CancelCranTaskCompleted(object sender, CranService.CancelCranTaskCompletedEventArgs e) {
			GlobalStatus.Current.IsBusy = false;
			ReturnMessage ret = e.Result as ReturnMessage;
			MessageBox.Show(ret.Message);
			GlobalStatus.Current.IsBusy = true;
			CransContext.Single.Client.getCranTasksAsync(CurrentFilter);
		}

		private void btnFinish_Click(object sender, RoutedEventArgs e) {
			if (CurrentTask == null)
				return;
			if (MessageBox.Show("Вы уверены что хотите закрыть заявку?", "Закрытие заявки", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
				GlobalStatus.Current.IsBusy = true;
				CurrentTask.Finished = true;
				CransContext.Single.Client.FinishCranTaskAsync(CurrentTask);
			}
		}

		void Client_FinishCranTaskCompleted(object sender, FinishCranTaskCompletedEventArgs e) {

			GlobalStatus.Current.IsBusy = false;
			ReturnMessage ret = e.Result as ReturnMessage;
			MessageBox.Show(ret.Message);
			GlobalStatus.Current.IsBusy = true;
			CransContext.Single.Client.getCranTasksAsync(CurrentFilter);
		}

	}

}


