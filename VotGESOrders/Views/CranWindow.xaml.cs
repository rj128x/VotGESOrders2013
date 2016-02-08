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
using VotGESOrders.Web.Models;

namespace VotGESOrders.Views {
	public partial class CranWindow : ChildWindow {
		public CranTaskInfo CurrentTask { get; set; }
		public List<String> Managers { get; set; }
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

		public void init(CranTaskInfo task,List<String>Managers) {
			this.Managers = Managers;
			CurrentTask = task;
			pnlTask.DataContext = CurrentTask;
			lstUsers.ItemsSource = from OrdersUser u in OrdersContext.Current.Context.OrdersUsers where u.CanAgreeCranTask select u;
			acbManager.ItemsSource = Managers;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e) {
			GlobalStatus.Current.IsBusy = true;
			CransContext.Single.Client.CreateCranTaskAsync(CurrentTask);
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
		}

		private void lstUsers_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			OrdersUser user = lstUsers.SelectedItem as OrdersUser;
			if (CurrentTask.AgreeUsersText == null ) {
				CurrentTask.AgreeUsersText = "";
				CurrentTask.AgreeUsersIDS = "";
				CurrentTask.AgreeDict = new Dictionary<int, string>();
			}
			if (user != null) {
				if (CurrentTask.AgreeDict.Keys.Contains(user.UserID)) {
					CurrentTask.AgreeDict.Remove(user.UserID);
				}
				else {
					CurrentTask.AgreeDict.Add(user.UserID, user.FullName);
				}
				CurrentTask.AgreeUsersText = string.Join("; ", from string name in CurrentTask.AgreeDict.Values select name);
				CurrentTask.AgreeUsersIDS = string.Join(";", from int key in CurrentTask.AgreeDict.Keys select key.ToString());
			}
		}
	}
}

