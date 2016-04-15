﻿using System;
using System.Windows;
using System.Windows.Controls;
using VotGESOrders.CranService;

namespace VotGESOrders.Views
{
	public partial class CranReviewWindow : ChildWindow {
		public CranTaskInfo CurrentTask { get; set; }
		public CranReviewWindow() {
			InitializeComponent();
			CransContext.Single.Client.CreateCranTaskCompleted += Client_CreateCranTaskCompleted;
		}

		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			CransContext.Single.Client.CreateCranTaskCompleted -= Client_CreateCranTaskCompleted;
		}

		void Client_CreateCranTaskCompleted(object sender, CranService.CreateCranTaskCompletedEventArgs e) {
			GlobalStatus.Current.IsBusy = false;
			ReturnMessage ret = e.Result as ReturnMessage;
			MessageBox.Show(ret.Message);
			if (ret.Result) {
				this.DialogResult = true;
			}
		}

		public void init(CranTaskInfo task) {
			CurrentTask = task;
			pnlTask.DataContext = CurrentTask;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e) {
			if (GlobalStatus.Current.IsBusy)
				return;
			if (CurrentTask.Allowed && CurrentTask.AllowDateEnd < CurrentTask.AllowDateStart) {
				MessageBox.Show("Время окончания меньше времени начала");
				return;
			}
			/*if (CurrentTask.Allowed && CurrentTask.AllowDateStart<DateTime.Now) {
				MessageBox.Show("Время заявки меньше текущего");
				return;
			}*/
			GlobalStatus.Current.IsBusy = true;
			CransContext.Single.Client.CreateCranTaskAsync(CurrentTask);
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
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
	}
}

