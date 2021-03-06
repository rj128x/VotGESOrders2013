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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VotGESOrders.Web.Services;
using VotGESOrders.Logging;
using VotGESOrders.Web.Models;
using VotGESOrders.Views;
using System.Windows.Data;
using System.Threading;
using System.Windows.Threading;
using System.ServiceModel.DomainServices.Client;
using System.Windows.Printing;
using System.ComponentModel;

namespace VotGESOrders
{
	public partial class Home : Page
	{
		DispatcherTimer timerExistChanges;
		PrintDocument multidoc;
		public Home() {			
			InitializeComponent();
			if (!MainPage.STARTING)
				finish();
		}


		private void Page_Loaded(object sender, RoutedEventArgs e) {
		}


    protected override void OnNavigatedTo(NavigationEventArgs e) {
      try {
        timerExistChanges.Start();
      } catch { }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e) {
      try {
        timerExistChanges.Stop();
      } catch { }
    }

    public void finish() {
			Logger.info("Начало загрузки главной страницы");
			pnlButtons.DataContext = WebContext.Current.User;

			OrdersContext.Current.View.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(View_CollectionChanged);
			OrdersContext.Current.Context.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(context_PropertyChanged);

			ordersGridControl.ordersGrid.ItemsSource = OrdersContext.Current.View;
			ordersGridControl.ordersGrid.MouseLeftButtonUp += new MouseButtonEventHandler(ordersGrid_MouseLeftButtonUp);

			timerExistChanges = new DispatcherTimer();
			timerExistChanges.Tick += new EventHandler(timerExistChanges_Tick);
			timerExistChanges.Interval = new TimeSpan(0, 0, 30);
      timerExistChanges.Start();

      cntrlOrder.Visibility = System.Windows.Visibility.Collapsed;
			cntrlFilter.DataContext = OrdersContext.Current.Filter;
			cmbFilterType.ItemsSource = OrderFilter.FilterTypes;
			cmbFilterType.DataContext = OrdersContext.Current.Filter;
			cntrlFilter.lstAllUsers.ItemsSource = OrdersContext.Current.Context.OrdersUsers;
			cntrlFilter.chooseObjectsWindow = new ChooseObjectsWindow();
			Logger.info("Главная страница загружена");
		}



		void View_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			if (OrderOperations.Current.CurrentOrder == null) {
				cntrlOrder.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		void timerExistChanges_Tick(object sender, EventArgs e) {
			if (OrdersContext.Current.Filter.FilterType != OrderFilterEnum.userFilter) {
				InvokeOperation<bool> oper=
					OrdersContext.Current.Context.ExistsChanges(OrdersContext.Current.SessionGUID);
				oper.Completed += new EventHandler(oper_Completed);
			}
		}

		void oper_Completed(object sender, EventArgs e) {
			InvokeOperation<bool> oper=sender as InvokeOperation<bool>;
			if (!oper.HasError) {
				GlobalStatus.Current.IsError = false;
				if (OrdersContext.Current.LastUpdate.AddMinutes(10) < DateTime.Now) {
					GlobalStatus.Current.NeedRefresh = true;
				}
				if (oper.Value || GlobalStatus.Current.NeedRefresh) {
					if (GlobalStatus.Current.CanRefresh) {
						OrdersContext.Current.RefreshOrders(true);
					} else {
						GlobalStatus.Current.NeedRefresh = true;
					}
				}
			} else {
				GlobalStatus.Current.IsError = true;
				oper.MarkErrorAsHandled();
			}

		}
		void context_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			OrdersContext.Current.View.Refresh();
			//ordersGrid.UpdateLayout();
		}
		private void btnCreateOrder_Click(object sender, RoutedEventArgs e) {
			OrderOperations.Current.initCreate();
		}
		private void btnRefresh_Click(object sender, RoutedEventArgs e) {
			//OrdersContext.Context.Load(OrdersContext.Context.LoadOrdersQuery(), System.ServiceModel.DomainServices.Client.LoadBehavior.RefreshCurrent, false);
			OrdersContext.Current.RefreshOrders(true);
			OrdersContext.Current.View.Refresh();
		}
		private void btnVisFilter_Click(object sender, RoutedEventArgs e) {
			if (cntrlFilter.Visibility == System.Windows.Visibility.Visible) {
				cntrlFilter.Visibility = System.Windows.Visibility.Collapsed;
			} else {
				cntrlFilter.Visibility = System.Windows.Visibility.Visible;
			}
		}
		private void ordersGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			Order order=ordersGridControl.ordersGrid.SelectedItem as Order;
			if (order != null) {
				cntrlOrder.DataContext = order;
				if (OrderOperations.Current.CurrentOrder == null) {
					cntrlOrder.Visibility = System.Windows.Visibility.Visible;
				}
				OrderOperations.Current.CurrentOrder = order;
			}
		}


		private void btnVisDetails_Click(object sender, RoutedEventArgs e) {
			if (OrderOperations.Current.CurrentOrder == null) {
				cntrlOrder.Visibility = System.Windows.Visibility.Collapsed;
			} else {
				if ((cntrlOrder.Visibility == System.Windows.Visibility.Collapsed) && (ordersGridControl.ordersGrid.SelectedItem != null)) {
					cntrlOrder.Visibility = System.Windows.Visibility.Visible;
				} else {
					cntrlOrder.Visibility = System.Windows.Visibility.Collapsed;
				}
			}
		}


		private List<StackPanel> getPrintPages(double width, double height) {
			List<StackPanel> pages=new List<StackPanel>();
			bool finish=false;
			int index=0;
			while (!finish) {
				StackPanel host = new StackPanel();
				bool isFirst=true;
				while (index < OrdersContext.Current.View.Count) {
					OrderPrintBriefLandscapeControl cntrl = new OrderPrintBriefLandscapeControl();
					cntrl.DataContext = OrdersContext.Current.View.GetItemAt(index); ;
					//cntrl.UpdateLayout();
					if (isFirst) {
						cntrl.showHeader();
					}
					isFirst = false;
					host.Children.Add(cntrl);

					

					host.Measure(new Size(width, double.PositiveInfinity));

					//Logger.logMessage(cntrl.DesiredSize.Height+" "+ host.DesiredSize.Height + " " + height);
					if (host.DesiredSize.Height > height && host.Children.Count > 1) {
						host.Children.Remove(cntrl);
						break;
					}
					index++;
					finish = OrdersContext.Current.View.Count == index;
				}
				pages.Add(host);
			}
			return pages;
		}

		private Grid createGridLayout(double width, double height, out Grid grid, out TextBlock page) {
			grid=new Grid();
			Grid layout=new Grid();
			layout.VerticalAlignment = System.Windows.VerticalAlignment.Top;
			layout.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
			layout.Width = width;
			layout.Height = width;
			layout.Children.Add(grid);
			grid.RowDefinitions.Add(new RowDefinition());
			grid.RowDefinitions.Add(new RowDefinition());
			grid.RowDefinitions.Add(new RowDefinition());
			grid.RowDefinitions[0].Height = new GridLength(50, GridUnitType.Pixel);
			grid.RowDefinitions[2].Height = new GridLength(15, GridUnitType.Pixel);
			grid.RowDefinitions[1].Height = new GridLength(height - 65, GridUnitType.Pixel);


			StackPanel headerPanel=new StackPanel();
			headerPanel.Height = 50;
			headerPanel.Background = new SolidColorBrush(Colors.LightGray);
			TextBlock header=new TextBlock();
			//header.Text = String.Format("{0} на {1}", GlobalStatus.Current.HomeHeader,DateTime.Now.ToString("dd.MM.yy HH:mm"));
			header.Text = "";
			header.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
			header.FontSize = 13;
			headerPanel.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
			grid.Children.Add(headerPanel);
			headerPanel.Children.Add(header);
			headerPanel.SetValue(Grid.RowProperty, 0);

			//host.Measure(new Size(width, double.PositiveInfinity));


			Grid footerGrid=new Grid();
			footerGrid.Height = 15;
			footerGrid.Background = new SolidColorBrush(Colors.LightGray);
			footerGrid.ColumnDefinitions.Add(new ColumnDefinition());
			footerGrid.ColumnDefinitions.Add(new ColumnDefinition());
			footerGrid.ColumnDefinitions.Add(new ColumnDefinition());
			footerGrid.ColumnDefinitions[0].Width = GridLength.Auto;
			footerGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
			footerGrid.ColumnDefinitions[2].Width = GridLength.Auto;
			footerGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

			TextBlock footer=new TextBlock();			
			footer.Text = String.Format("{0} на {1} ", GlobalStatus.Current.HomeHeader, DateTime.Now.ToString("HH:mm")); ;
			footer.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
			footer.VerticalAlignment = System.Windows.VerticalAlignment.Center;
			footer.FontSize = 12;
			footerGrid.Children.Add(footer);
			footer.SetValue(Grid.ColumnProperty, 1);

			page=new TextBlock();
			page.VerticalAlignment = System.Windows.VerticalAlignment.Center;
			page.TextAlignment = TextAlignment.Left;
			page.FontSize = 12;
			footerGrid.Children.Add(page);
			page.SetValue(Grid.ColumnProperty, 0);

			TextBlock podp=new TextBlock();
			podp.Text = String.Format("{0}",  DateTime.Now.ToString("dd.MM.yy"));
			podp.TextAlignment = TextAlignment.Right;
			podp.VerticalAlignment = System.Windows.VerticalAlignment.Center;
			podp.FontSize = 12;
			footerGrid.Children.Add(podp);
			podp.SetValue(Grid.ColumnProperty, 2);

			grid.Children.Add(footerGrid);
			footerGrid.SetValue(Grid.RowProperty, 2);

			return layout;
		}

		private void btnPrint_Click(object sender, RoutedEventArgs e) {
			printDocument();
			
		}

		protected void printDocument() {
			multidoc = new PrintDocument();
			int index = 0;
			List<StackPanel> pages=null;

			StackPanel host=null;
			Grid layout=null;
			Grid grid=null;
			TextBlock page=null;
			bool isFirstPage=true;
			double width=0;
			double height=0;
			bool rotate=false;

			
			multidoc.PrintPage += (s, arg) => {
				if (isFirstPage) {
					width = arg.PrintableArea.Width;
					height = arg.PrintableArea.Height;
					rotate = false;


					if (width < height) {
						double temp=width;
						width = height;
						height = temp;
						rotate = true;
					}

					pages = getPrintPages(width, height - 65);

					layout = createGridLayout(width, height, out grid, out page);
					isFirstPage = false;
				}
								
				if (index < pages.Count) {
					int pageIndex=index + 1;
					//GlobalStatus.Current.Status = String.Format("Печать страницы №{0} из {1}", pageIndex, pages.Count);
					
					grid.Children.Remove(host);
					host = pages[index];
					page.Text = String.Format("Cтраница {0} из {1}", pageIndex, pages.Count);

					grid.Children.Add(host);
					host.SetValue(Grid.RowProperty, 1);
					if (rotate) {
						CompositeTransform transform=new CompositeTransform() {
							Rotation = 90,
							TranslateX = height

						};
						grid.RenderTransform = transform;
					}					
					arg.PageVisual = layout;
				}
				index++;
				arg.HasMorePages = index < pages.Count;
			};

			multidoc.BeginPrint += (s, arg) => {
				GlobalStatus.Current.Status = "Печать списка заявок";
				GlobalStatus.Current.IsBusy = true;
			};

			multidoc.EndPrint += (s, arg) => {
				GlobalStatus.Current.Status = "Готово";
				GlobalStatus.Current.IsBusy = false;
			};

			multidoc.Print("Список заявок");
		}



		void doc_PrintPage(object sender, PrintPageEventArgs e) {
			OrderControl cntrl=new OrderControl();
			cntrl.DataContext = OrderOperations.Current.CurrentOrder;
			e.PageVisual = cntrl;
		}

		private void btnFullScreen_Click(object sender, RoutedEventArgs e) {
			Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
		}

		private void cmbFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			btnVisFilter.Visibility = OrdersContext.Current.Filter.FilterType == OrderFilterEnum.userFilter ?
				System.Windows.Visibility.Visible :
				System.Windows.Visibility.Collapsed;
			cntrlFilter.Visibility = OrdersContext.Current.Filter.FilterType == OrderFilterEnum.userFilter ?
				System.Windows.Visibility.Visible :
				System.Windows.Visibility.Collapsed;
		}

		private void btnMail_Click(object sender, RoutedEventArgs e) {
			OrdersContext.Current.SendMail(true);
		}

		private void btnCreateBaseOrder_Click(object sender, RoutedEventArgs e) {
			if (OrderOperations.Current.CurrentOrder != null) {
				OrderOperations.Current.initCreateBase(OrderOperations.Current.CurrentOrder);
			}
		}
	}
}