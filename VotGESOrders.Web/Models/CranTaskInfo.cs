﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VotGESOrders.Web.ADONETEntities;
using VotGESOrders.Web.Logging;

namespace VotGESOrders.Web.Models {

	public class CranFilter {
		public DateTime DateStart { get; set; }
		public DateTime DateEnd { get; set; }
		public IQueryable<CranTaskInfo> Data { get; set; }
	}

	public class ReturnMessage {
		public string Message { get; set; }
		public bool Result { get; set; }
		
		public ReturnMessage() { }
		public ReturnMessage(bool result, string message) {
			Message = message;
			Result = result;
			Logger.info(String.Format("Возврат: {0} (1)", result, message),Logger.LoggerSource.server);
		}

	}

	public class CranTaskInfo {
		public int CranNumber{get;set;}
		public int Number { get; set; }
		public DateTime NeedStartDate { get; set; }
		public DateTime NeedEndDate { get; set; }
		public string Comment { get; set; }
		public string Author { get; set; }
		public string AuthorAllow { get; set; }
		public DateTime AllowDateStart { get; set; }
		public DateTime AllowDateEnd { get; set; }
		public bool Allowed { get; set; }
		public bool Denied { get; set; }
		public bool init;

		public CranTaskInfo() {

		}

		public CranTaskInfo(CranTask tbl) {
			CranNumber = tbl.CranNumber;
			Number = tbl.Number;
			NeedStartDate = tbl.NeedDateStart;
			NeedEndDate = tbl.NeedDateEnd;
			Comment = tbl.Comment;
			Author = OrdersUser.loadFromCache(tbl.Author).FullName;
			Allowed = tbl.Allowed;
			Denied = tbl.Denied;
			if (tbl.Allowed) {
				AuthorAllow = OrdersUser.loadFromCache(tbl.AuthorAllow).FullName;
				AllowDateStart = tbl.AllowedDateStart.Value;
				AllowDateEnd = tbl.AllowedDateEnd.Value;
			}
		}

		public static ReturnMessage  CreateCranTask(CranTaskInfo task) {
			Logger.info("Создание/изменение заявки на работу крана",Logger.LoggerSource.server);
			try {
				string result="";
				OrdersUser currentUser=OrdersUser.loadFromCache(HttpContext.Current.User.Identity.Name);
				VotGESOrdersEntities eni = new VotGESOrdersEntities();
				CranTask tbl = new CranTask();
				if (task.init) {
					Logger.info("Определение номера заявки на кран", Logger.LoggerSource.server);
					CranTask tsk = (from t in eni.CranTask orderby t.Number descending select t).FirstOrDefault();
					if (tsk != null) {
						task.Number = tsk.Number+1;
					}
					else {
						tsk.Number = 1;
					}
					tbl.Allowed = false;
					tbl.Denied = false;
					tbl.Author = currentUser.Name;
					result="Заявка на кран успешно создана";
				}
				else {
					CranTask tsk = (from t in eni.CranTask where t.Number==task.Number select t).FirstOrDefault();
					if (tsk == null) {
						return new ReturnMessage(false,"Заявка не найдена");
					}
					tbl = tsk;
					result = "Заявка на кран успешно создана";
				}
				tbl.NeedDateStart = task.NeedStartDate;
				tbl.NeedDateEnd = task.NeedEndDate;
				tbl.Comment = task.Comment;
				tbl.CranNumber = task.CranNumber;
				if (task.Allowed) {
					tbl.AllowedDateStart = task.AllowDateStart;
					tbl.AllowedDateEnd = task.AllowDateEnd;
					tbl.Denied = false;
					result = "Заявка на кран разрешена";
				}

				if (task.Denied) {
					tbl.AllowedDateStart = null;
					tbl.AllowedDateEnd = null;
					tbl.Allowed = false;
					result = "Заявка на кран отклонена";
				}

				eni.CranTask.AddObject(tbl);
				eni.SaveChanges();
				return new ReturnMessage(true,result);
			}
			catch (Exception e) {
				Logger.info("Ошибка при создании/изменении заявки на работу крана", Logger.LoggerSource.server);
				return new ReturnMessage(false,"Ошибка при создании/изменении заявки на работу крана");
			}
		}

		public static CranFilter LoadCranTasks (CranFilter Filter=null){
			if (Filter==null){
				Filter=new CranFilter();
				Filter.DateStart = DateTime.Now.Date.AddDays(-5);
				Filter.DateEnd = DateTime.Now.AddDays(5);				
			}
			VotGESOrdersEntities eni = new VotGESOrdersEntities();
			List<CranTaskInfo> result = new List<CranTaskInfo>();
			IQueryable<CranTask>data=from t in eni.CranTask where t.NeedDateStart>Filter.DateStart && t.NeedDateStart<Filter.DateEnd select t;
			foreach (CranTask tbl in data) {
				result.Add(new CranTaskInfo(tbl));
			}
			Filter.Data = result.AsQueryable();
			return Filter;
		}


	}
}