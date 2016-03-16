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
		public Dictionary<int, string> Crans;
		public CranWindow() {
			InitializeComponent();
			CransContext.Single.Client.CreateCranTaskCompleted += Client_CreateCranTaskCompleted;
			Crans = new Dictionary<int, string>();
			Crans.Add(0, "Выберите кран");
			Crans.Add(1, "Кран мостовой г/п 350/75/10 ст.№1 МЗ");
			Crans.Add(2, "Кран мостовой г/п 350/75/10 ст.№2 МЗ");
			Crans.Add(3, "Кран козловой г/п 2х20 ст.№1 СУС");
			Crans.Add(4, "Кран козловой г/п 2х20 ст.№2 СУС");
			Crans.Add(5, "Кран козловой г/п 2х63/2х5+16 ЩО НБ");
			Crans.Add(6, "Кран полукозловой г/п 2х150 ЩО ВБ");
			Crans.Add(7, "Кран мостовой г/п 50/10 Транс Башни");
			Crans.Add(8, "Кран козловой 2х125 ВСП");						
			
			Crans.Add(9, "Кран козловой г/п 63/10т Произ площ");
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
			cmbCranName.ItemsSource = Crans;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e) {
			if (GlobalStatus.Current.IsBusy)
				return;
			if (CurrentTask.CranNumber == 0) {
				MessageBox.Show("Выберите кран");
				return;
			}
			if (string.IsNullOrEmpty(CurrentTask.Manager)) {
				MessageBox.Show("Введите ответственного");
				return;
			}
			if (string.IsNullOrEmpty(CurrentTask.Comment)) {
				MessageBox.Show("Введите текст заявки");
				return;
			}
			if (CurrentTask.NeedEndDate <= CurrentTask.NeedStartDate) {
				MessageBox.Show("Время окончания меньше времени начала");
				return;
			}

			if (CurrentTask.NeedStartDate <= DateTime.Now) {
				MessageBox.Show("Время заявки меньше текущего");
				return;
			}

			CurrentTask.CranName = Crans[CurrentTask.CranNumber];
			GlobalStatus.Current.IsBusy = true;
			CransContext.Single.Client.CreateCranTaskAsync(CurrentTask);
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			if (GlobalStatus.Current.IsBusy)
				return;
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

