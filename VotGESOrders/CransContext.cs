using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using VotGESOrders.CranService;

namespace VotGESOrders {
	public class CransContext {
		public static CransContext Single { get; set; }
		public CranServiceClient Client;
		protected CransContext() {

		}
		static CransContext() {
			Single = new CransContext();
		}
		public static void init() {
			Single.Client = new CranServiceClient();
		}


	}
}
