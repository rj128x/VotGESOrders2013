using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VotGESOrders.Web.ADONETEntities;
using VotGESOrders.Web.Logging;

namespace VotGESOrders.Web.Models {

	public class CranFilter {
		public DateTime DateStart { get; set; }
		public DateTime DateEnd { get; set; }
		public List<CranTaskInfo> Data { get; set; }
		public List<String> Managers { get; set; }
	}

	public class ReturnMessage {
		public string Message { get; set; }
		public bool Result { get; set; }

		public ReturnMessage() { }
		public ReturnMessage(bool result, string message) {
			Message = message;
			Result = result;
			Logger.info(String.Format("Возврат: {0} (1)", result, message), Logger.LoggerSource.server);
		}

	}

	public class CranTaskInfo {
		public static List<string> Managers;
		public int CranNumber { get; set; }
		public int Number { get; set; }
		public DateTime NeedStartDate { get; set; }
		public DateTime NeedEndDate { get; set; }
		public string Comment { get; set; }
		public string Author { get; set; }
		public string AuthorAllow { get; set; }
		public string Manager { get; set; }
		public DateTime AllowDateStart { get; set; }
		public DateTime AllowDateEnd { get; set; }
		public bool Allowed { get; set; }
		public bool Denied { get; set; }
		public string State { get; set; }
		public bool init { get; set; }
		public bool change { get; set; }
		public bool check { get; set; }
		public bool changed { get; set; }
		public string AgreeUsersIDS { get; set; }
		public string AgreeUsersText { get; set; }
		public Dictionary<int, string> AgreeDict { get; set; }

		public bool canChange { get; set; }
		public bool canCheck { get; set; }

		public CranTaskInfo() {

		}

		public CranTaskInfo(CranTask tbl) {
			OrdersUser currentUser = OrdersUser.loadFromCache(HttpContext.Current.User.Identity.Name);
			CranNumber = tbl.CranNumber;
			Number = tbl.Number;
			NeedStartDate = tbl.NeedDateStart;
			NeedEndDate = tbl.NeedDateEnd;
			Comment = tbl.Comment;
			Author = OrdersUser.loadFromCache(tbl.Author).FullName;
			State = "Новая";
			Allowed = tbl.Allowed;
			Denied = tbl.Denied;
			canChange = (!Allowed) && (!Denied) && tbl.Author.ToLower() == currentUser.Name.ToLower();
			canCheck = currentUser.CanReviewCranTask;
			Manager = tbl.Manager;
			if (Denied) {
				State = "Отклонена";
				canChange = false;
				AuthorAllow = OrdersUser.loadFromCache(tbl.AuthorAllow).FullName;
			}
			if (tbl.Allowed) {
				AuthorAllow = OrdersUser.loadFromCache(tbl.AuthorAllow).FullName;
				AllowDateStart = tbl.AllowedDateStart.Value;
				AllowDateEnd = tbl.AllowedDateEnd.Value;
				canChange = false;
				State = "Разрешена";
			}
			AgreeUsersIDS = tbl.AgreeUsersIDS;
			AgreeDict = getAgreeUsers(AgreeUsersIDS);
			AgreeUsersText = string.Join(", ", AgreeDict.Values);
		}

		public static Dictionary<int,string> getAgreeUsers(string ids) {
			Dictionary<int, string> dict = new Dictionary<int, string>();
			try {
				string[] idArr = ids.Split(new char[] { ';' });

				foreach (string id in idArr) {
					try {
						OrdersUser user = OrdersUser.loadFromCache(Int32.Parse(id));
						dict.Add(user.UserID, user.FullName);
					}
					catch { }
				}
			}
			catch { }
			return dict;
		}

		public static ReturnMessage CreateCranTask(CranTaskInfo task) {
			Logger.info("Создание/изменение заявки на работу крана", Logger.LoggerSource.server);
			try {
				string result = "";
				string message = String.Format("Заявка на работу крана {0} №", task.CranNumber);
				OrdersUser currentUser = OrdersUser.loadFromCache(HttpContext.Current.User.Identity.Name);
				VotGESOrdersEntities eni = new VotGESOrdersEntities();
				CranTask tbl = new CranTask();
				if (task.init) {
					Logger.info("Определение номера заявки на кран", Logger.LoggerSource.server);
					CranTask tsk = (from t in eni.CranTask orderby t.Number descending select t).FirstOrDefault();
					if (tsk != null) {
						task.Number = tsk.Number + 1;
					}
					else {
						task.Number = 1;
					}
					tbl.Allowed = false;
					tbl.Denied = false;
					tbl.Author = currentUser.Name;
					task.Author = currentUser.FullName;
					eni.CranTask.AddObject(tbl);
					result = "Заявка на кран успешно создана";
				}
				else {
					CranTask tsk = (from t in eni.CranTask where t.Number == task.Number select t).FirstOrDefault();
					if (tsk == null) {
						return new ReturnMessage(false, "Заявка не найдена");
					}
					tbl = tsk;
					result = "Заявка на кран успешно создана";
				}
				message += task.Number + ". ";

				if ((task.NeedEndDate <= task.NeedStartDate) || (task.Allowed && (task.AllowDateEnd <= task.AllowDateStart))) {
					return new ReturnMessage(false, "Дата окончания заявки больше чем дата начала");
				}
				if (task.CranNumber < 1 || task.CranNumber > 2) {
					return new ReturnMessage(false, "Проверьте номер крана");
				}

				tbl.Number = task.Number;
				tbl.NeedDateStart = task.NeedStartDate;
				tbl.NeedDateEnd = task.NeedEndDate;
				tbl.Comment = task.Comment;
				tbl.Manager = task.Manager;
				tbl.CranNumber = task.CranNumber;
				if (task.AgreeDict != null)
					tbl.AgreeUsersIDS = string.Join(";", task.AgreeDict.Keys);
				if (task.Allowed) {
					tbl.AllowedDateStart = task.AllowDateStart;
					tbl.AllowedDateEnd = task.AllowDateEnd;
					tbl.Denied = false;
					tbl.Allowed = true;
					task.AuthorAllow = currentUser.FullName;
					result = "Заявка на кран разрешена";
					message += " Заявка разрешена";
				}
				else if (task.Denied) {
					tbl.AllowedDateStart = null;
					tbl.AllowedDateEnd = null;
					tbl.Allowed = false;
					tbl.Denied = true;
					task.AuthorAllow = currentUser.FullName;
					result = "Заявка на кран отклонена";
					message += " Заявка отклонена";
				}
				else if (!task.init) {
					message += " Заявка изменена";
				}

				if (task.Allowed || task.Denied) {
					tbl.AuthorAllow = currentUser.Name;
				}



				eni.SaveChanges();
				MailContext.sendCranTask(message, task);
				if (Managers != null) {
					if (!Managers.Contains(task.Manager)) {
						Managers.Add(task.Manager);
					}
				}
				else
					ReadManagers();
				return new ReturnMessage(true, result);
			}
			catch (Exception e) {
				Logger.info("Ошибка при создании/изменении заявки на работу крана " + e.ToString(), Logger.LoggerSource.server);
				return new ReturnMessage(false, "Ошибка при создании/изменении заявки на работу крана");
			}
		}

		public static CranFilter LoadCranTasks(CranFilter Filter = null) {
			Logger.info("Получение списка заявок на кран", Logger.LoggerSource.server);
			if (Managers == null)
				ReadManagers();
			if (Filter == null) {
				Filter = new CranFilter();
				Filter.DateStart = DateTime.Now.Date;
				Filter.DateEnd = DateTime.Now.Date.AddDays(10);
			}
			Filter.Managers = Managers;
			VotGESOrdersEntities eni = new VotGESOrdersEntities();
			List<CranTaskInfo> result = new List<CranTaskInfo>();
			IQueryable<CranTask> data = from t in eni.CranTask
																	where
																		t.NeedDateStart > Filter.DateStart && t.NeedDateStart < Filter.DateEnd ||
																		t.NeedDateEnd > Filter.DateStart && t.NeedDateEnd < Filter.DateEnd ||
																		t.NeedDateStart < Filter.DateStart && t.NeedDateEnd > Filter.DateEnd ||
																		(t.Allowed &&
																		t.AllowedDateStart > Filter.DateStart && t.AllowedDateStart < Filter.DateEnd ||
																		t.AllowedDateEnd > Filter.DateStart && t.AllowedDateEnd < Filter.DateEnd ||
																		t.AllowedDateStart < Filter.DateStart && t.AllowedDateEnd > Filter.DateEnd)
																	select t;
			foreach (CranTask tbl in data) {
				result.Add(new CranTaskInfo(tbl));
			}
			Filter.Data = result;
			return Filter;
		}

		public static void ReadManagers() {
			Logger.info("Получение списка ответственных", Logger.LoggerSource.server);
			Managers=new List<string>();
			try {
				VotGESOrdersEntities eni = new VotGESOrdersEntities();
				IQueryable<string> data = (from t in eni.CranTask select t.Manager).Distinct();
				foreach (string name in data) {
					if (!(Managers.Contains(name))) {
						Managers.Add(name);
					}
				}
			}
			catch (Exception e) {
				Logger.info("Ошибка при чтении доступных ответственных " + e.ToString(),Logger.LoggerSource.server);
			}
		}

	}
}