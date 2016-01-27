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
using VotGESOrders.CranService;

namespace VotGESOrders.Views {
	public partial class CranWindow : ChildWindow {
		public CranTaskInfo CurrentTask { get; set; }
		public CranWindow() {
			InitializeComponent();
			CransContext.Single.Client.CreateCranTaskCompleted += Client_CreateCranTaskCompleted;
		}

		void Client_CreateCranTaskCompleted(object sender, CranService.CreateCranTaskCompletedEventArgs e) {
			GlobalStatus.Current.IsBusy = false;
			ReturnMessage ret = e.Result as ReturnMessage;
			MessageBox.Show(ret.Message);
			if (ret.Result) {
				this.DialogResult = true;
			}
		}

		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			CransContext.Single.Client.CreateCranTaskCompleted -= Client_CreateCranTaskCompleted;
		}

		public void init(CranTaskInfo task) {
			CurrentTask = task;
			pnlTask.DataContext = CurrentTask;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e) {
			GlobalStatus.Current.IsBusy = true;
			CransContext.Single.Client.CreateCranTaskAsync(CurrentTask);
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
		}
	}
}

