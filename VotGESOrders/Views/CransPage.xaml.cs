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

namespace VotGESOrders.Views {
	public partial class CransPage : Page {
		public CransPage() {
			InitializeComponent();
			CransContext.init();
		}

		// Выполняется, когда пользователь переходит на эту страницу.
		protected override void OnNavigatedTo(NavigationEventArgs e) {

		}

		public void init() {
			CransContext.Single.Client.getCranTasksCompleted += Client_getCranTasksCompleted;
			CransContext.Single.Client.CreateCranTaskCompleted += Client_CreateCranTaskCompleted;
		}

		public void deinit() {
			CransContext.Single.Client.getCranTasksCompleted -= Client_getCranTasksCompleted;
			CransContext.Single.Client.CreateCranTaskCompleted -= Client_CreateCranTaskCompleted;
		}

		void Client_CreateCranTaskCompleted(object sender, CranService.CreateCranTaskCompletedEventArgs e) {
			throw new NotImplementedException();
		}

		void Client_getCranTasksCompleted(object sender, CranService.getCranTasksCompletedEventArgs e) {
			throw new NotImplementedException();
		}





	}
}
