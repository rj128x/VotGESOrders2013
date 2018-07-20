using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VotGESOrders.Web.ADONETEntities;
using VotGESOrders.Web.Logging;

namespace VotGESOrders.Web.Models
{

  public class CranFilter
  {
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public List<CranTaskInfo> Data { get; set; }
    public List<String> Managers { get; set; }
    public List<String> StropUsers { get; set; }
    public List<String> CranUsers { get; set; }
  }

  public class ReturnMessage
  {
    public string Message { get; set; }
    public bool Result { get; set; }

    public ReturnMessage() { }
    public ReturnMessage(bool result, string message) {
      Message = message;
      Result = result;
      Logger.info(String.Format("Возврат: {0} (1)", result, message), Logger.LoggerSource.server);
    }

  }

  public class CranTaskInfo
  {
    public static string PathFiles;
    public int CranNumber { get; set; }
    public string CranName { get; set; }
    public int Number { get; set; }
    public DateTime NeedStartDate { get; set; }
    public DateTime NeedEndDate { get; set; }
    public string Comment { get; set; }
    public string Author { get; set; }
    public string SelAuthor { get; set; }
    public string AuthorText { get; set; }
    public string AuthorAllow { get; set; }
    public string AuthorFinish { get; set; }
    public string Manager { get; set; }
    public string StropUser { get; set; }
    public string CranUser { get; set; }
    public string AgreeComments { get; set; }
    public DateTime AllowDateStart { get; set; }
    public DateTime AllowDateEnd { get; set; }
    public DateTime RealDateStart { get; set; }
    public DateTime RealDateEnd { get; set; }

    public bool Allowed { get; set; }
    public bool Denied { get; set; }
    public bool Cancelled { get; set; }
    public bool Finished { get; set; }
    public string State { get; set; }
    public bool init { get; set; }
    public bool change { get; set; }
    public bool check { get; set; }
    public bool changed { get; set; }
    public Dictionary<int, string> AgreeDict { get; set; }
    public string StateDB { get; set; }
    public DateTime DateCreate { get; set; }


    public bool canChange { get; set; }
    public bool canCheck { get; set; }
    public bool canComment { get; set; }
    public bool canCancel { get; set; }
    public bool canFinish { get; set; }

    public bool hasCrossTasks { get; set; }
    public string crossTasks { get; set; }

    public CranTaskInfo() {

    }

    public CranTaskInfo(CranTask tbl) {
      OrdersUser currentUser = OrdersUser.loadFromCache(HttpContext.Current.User.Identity.Name);
      CranNumber = tbl.CranNumber;
      Number = tbl.Number;
      NeedStartDate = tbl.NeedDateStart;
      NeedEndDate = tbl.NeedDateEnd;
      Comment = tbl.Comment;
      CranName = tbl.CranName;
      Author = OrdersUser.loadFromCache(tbl.Author).FullName;
      SelAuthor = tbl.SelAuthor;
      AuthorText = tbl.AuthorText;
      State = "Новая";
      StateDB = tbl.State;
      Allowed = tbl.Allowed;
      Denied = tbl.Denied;
      Cancelled = tbl.Cancelled;
      Finished = tbl.Finished;
      AgreeComments = tbl.AgreeComment;

      canChange = (!Cancelled) && (!Allowed) && (!Denied) && (!Finished) /*&& (tbl.Author.ToLower() == currentUser.Name.ToLower() || tbl.SelAuthor.ToLower()==currentUser.Name.ToLower())*/;
      canCancel = (!Cancelled) && (!Denied) && (!Finished) /*&& (tbl.Author.ToLower() == currentUser.Name.ToLower()|| tbl.SelAuthor.ToLower() == currentUser.Name.ToLower())*/;
      canFinish = Allowed && (/*tbl.Author.ToLower() == currentUser.Name.ToLower()|| tbl.SelAuthor.ToLower() == currentUser.Name.ToLower() || */currentUser.CanReviewCranTask || currentUser.CanFinishCranTask);

      canCheck = (currentUser.CanReviewCranTask) && (!Finished) && (!Cancelled);
      canComment = true;
      Manager = tbl.Manager;
      StropUser = tbl.StropUser;
      CranUser = tbl.CranUser;
      if (Denied) {
        State = "Отклонена";
        canChange = false;
        AuthorAllow = OrdersUser.loadFromCache(tbl.AuthorAllow).FullName;
      }
      if (tbl.Allowed) {
        AuthorAllow = OrdersUser.loadFromCache(tbl.AuthorAllow).FullName;
        AllowDateStart = tbl.AllowedDateStart.Value;
        AllowDateEnd = tbl.AllowedDateEnd.Value;
        RealDateStart = tbl.RealDateStart.Value;
        RealDateEnd = tbl.RealDateEnd.Value;
        canChange = false;
        State = "Разрешена";
      }
      if (tbl.Cancelled) {
        State = "Снята";
      }
      if (tbl.Finished) {
        State = "Закрыта";
        Finished = tbl.Finished;
        if (!string.IsNullOrEmpty(tbl.AuthorFinish))
          AuthorFinish = OrdersUser.loadFromCache(tbl.AuthorFinish).FullName;
      }
      DateCreate = tbl.DateCreate;
    }

    public static Dictionary<int, string> getAgreeUsers(string ids) {
      Dictionary<int, string> dict = new Dictionary<int, string>();
      try {
        string[] idArr = ids.Split(new char[] { ';' });

        foreach (string id in idArr) {
          try {
            OrdersUser user = OrdersUser.loadFromCache(Int32.Parse(id));
            dict.Add(user.UserID, user.FullName);
          } catch { }
        }
      } catch { }
      return dict;
    }

    public static ReturnMessage CreateCranTask(CranTaskInfo task) {
      Logger.info("Создание/изменение заявки на работу крана", Logger.LoggerSource.server);
      try {
        string result = "";
        string message = String.Format("Заявка на работу крана \"{0}\" №", task.CranName);
        OrdersUser currentUser = OrdersUser.loadFromCache(HttpContext.Current.User.Identity.Name);
        VotGESOrdersEntities eni = new VotGESOrdersEntities();
        CranTask tbl = new CranTask();
        if (task.init) {
          Logger.info("Определение номера заявки на кран", Logger.LoggerSource.server);
          tbl.State = "new";
          CranTask tsk = (from t in eni.CranTask orderby t.Number descending select t).FirstOrDefault();
          task.DateCreate = DateTime.Now;
          if (tsk != null) {
            task.Number = tsk.Number + 1;
          } else {
            task.Number = 1;
          }
          tbl.Allowed = false;
          tbl.Denied = false;
          tbl.Author = currentUser.Name;
          tbl.DateCreate = task.DateCreate;
          task.Author = currentUser.FullName;
          eni.CranTask.AddObject(tbl);
          result = "Заявка на кран успешно создана";
        } else {
          CranTask tsk = (from t in eni.CranTask where t.Number == task.Number select t).FirstOrDefault();
          if (tsk == null) {
            return new ReturnMessage(false, "Заявка не найдена");
          }
          tbl = tsk;
          result = "Заявка на кран успешно изменена";
        }
        message += task.Number + ". ";

        if ((task.NeedEndDate <= task.NeedStartDate) || (task.Allowed && (task.AllowDateEnd <= task.AllowDateStart))) {
          return new ReturnMessage(false, "Дата окончания заявки больше чем дата начала");
        }


        tbl.Number = task.Number;
        tbl.CranName = task.CranName;
        tbl.AgreeComment = task.AgreeComments;
        tbl.NeedDateStart = task.NeedStartDate;
        tbl.NeedDateEnd = task.NeedEndDate;
        tbl.Comment = task.Comment;
        tbl.Manager = task.Manager;
        tbl.CranNumber = task.CranNumber;
        tbl.SelAuthor = task.SelAuthor;
        tbl.StropUser = task.StropUser;
        tbl.AuthorText = String.Format("{0} [{1}]", currentUser.FullName, OrdersUser.loadFromCache(tbl.SelAuthor).FullName);
        if (task.AgreeDict != null)
          tbl.AgreeUsersIDS = string.Join(";", task.AgreeDict.Keys);

        if (task.Finished) {
          tbl.State = "finished";
          result = "Заявка на кран завершена";
          tbl.RealDateStart = task.RealDateStart;
          tbl.RealDateEnd = task.RealDateEnd;
          tbl.AuthorFinish = currentUser.Name;
          tbl.Finished = true;
          message += " Заявка завершена";
        } else if (task.Allowed) {
          tbl.AllowedDateStart = task.AllowDateStart;
          tbl.AllowedDateEnd = task.AllowDateEnd;
          tbl.RealDateStart = task.AllowDateStart;
          tbl.RealDateEnd = task.AllowDateEnd;
          tbl.CranUser = task.CranUser;
          tbl.Denied = false;
          tbl.Allowed = true;
          tbl.Cancelled = false;
          tbl.State = "allowed";
          task.AuthorAllow = currentUser.FullName;
          result = "Заявка на кран разрешена";
          message += " Заявка разрешена";
        } else if (task.Denied) {
          tbl.AllowedDateStart = null;
          tbl.AllowedDateEnd = null;
          tbl.RealDateEnd = null;
          tbl.RealDateStart = null;
          tbl.Allowed = false;
          tbl.Denied = true;
          tbl.Cancelled = false;
          tbl.State = "denied";
          tbl.CranUser = "";
          task.AuthorAllow = currentUser.FullName;
          result = "Заявка на кран отклонена";
          message += " Заявка отклонена";
        } else if (task.Cancelled) {
          tbl.State = "cancelled";
          tbl.Denied = false;
          tbl.Allowed = false;
          tbl.Cancelled = true;
          tbl.AuthorAllow = null;
          tbl.AllowedDateStart = null;
          tbl.AllowedDateEnd = null;
          tbl.RealDateEnd = null;
          tbl.RealDateStart = null;
          result = "Заявка на кран снята";
          message += " Заявка снята";
        } else if (!task.init) {
          message += " Заявка изменена";
        }

        if (task.Allowed || task.Denied) {
          tbl.AuthorAllow = currentUser.Name;
        }

        eni.SaveChanges();
        MailContext.sendCranTask(message, new CranTaskInfo(tbl));
        return new ReturnMessage(true, result);
      } catch (Exception e) {
        Logger.info("Ошибка при создании/изменении заявки на работу крана " + e.ToString(), Logger.LoggerSource.server);
        return new ReturnMessage(false, "Ошибка при создании/изменении заявки на работу крана");
      }
    }

    public static ReturnMessage AddComment(CranTaskInfo task, string comment) {
      Logger.info("Добавление комментария к заявке на работу крана", Logger.LoggerSource.server);
      try {
        string result = "";
        OrdersUser currentUser = OrdersUser.loadFromCache(HttpContext.Current.User.Identity.Name);
        VotGESOrdersEntities eni = new VotGESOrdersEntities();
        CranTask tbl = new CranTask();

        CranTask tsk = (from t in eni.CranTask where t.Number == task.Number select t).FirstOrDefault();
        if (tsk == null) {
          return new ReturnMessage(false, "Заявка не найдена");
        }
        tbl = tsk;

        if (!string.IsNullOrEmpty(task.AgreeComments))
          task.AgreeComments += "\r\n";
        task.AgreeComments += String.Format("{2} {0}:\r\n   {1}", currentUser.FullName, comment, DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
        tbl.AgreeComment = task.AgreeComments;


        eni.SaveChanges();
        string message = String.Format("Заявка на работу крана \"{0}\" №{1}. Комментарий", task.CranName, task.CranNumber);
        MailContext.sendCranTask(message, new CranTaskInfo(tbl));
        return new ReturnMessage(true, "Комментарий добавлен");
      } catch (Exception e) {
        Logger.info("Ошибка при создании/изменении заявки на работу крана " + e.ToString(), Logger.LoggerSource.server);
        return new ReturnMessage(false, "ошибка при добавлении комментария");
      }
    }

    protected static bool DatesCross(DateTime start1, DateTime end1, DateTime start2, DateTime end2, bool first = true) {
      //Logger.info(String.Format("DatesCross {0} {1} {2} {3} {4}", start1, end1, start2, end2, first), Logger.LoggerSource.server);
      bool cross =
          (start1 >= start2 && start1 < end2 ||
          end1 > start2 && end1 <= end2 ||
          start1 >= start2 && end1 <= end2 ||
          start1 <= start2 && end1 >= end2);
      if (!cross && first) {
        cross = DatesCross(start2, end2, start1, end1, false);
      }
      return cross;

    }

    public static CranFilter LoadCranTasks(CranFilter Filter = null) {
      Logger.info("Получение списка заявок на кран", Logger.LoggerSource.server);

      if (Filter == null) {
        Filter = new CranFilter();
        Filter.DateStart = DateTime.Now.Date;
        Filter.DateEnd = DateTime.Now.Date.AddDays(10);

      }
      Filter.Managers = ReadTextFile("Managers.txt");
      Filter.CranUsers = ReadTextFile("CranUsers.txt");
      Filter.StropUsers = ReadTextFile("StropUsers.txt");
      //Filter.Managers = Managers;
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
                                  orderby t.CranNumber, t.NeedDateStart
                                  select t;
      foreach (CranTask tbl in data) {
        result.Add(new CranTaskInfo(tbl));
      }
      foreach (CranTaskInfo task in result) {
        task.hasCrossTasks = false;
        task.crossTasks = "";
        foreach (CranTaskInfo crossTask in result) {
          if (crossTask.Number == task.Number)
            continue;
          if (crossTask.CranNumber != task.CranNumber)
            continue;
          if (task.Denied || crossTask.Denied || task.Cancelled || crossTask.Cancelled || task.Finished || crossTask.Finished)
            continue;
          bool crossed = false;

          //Logger.info(String.Format("{0} - {1}",task.Number,crossTask.Number), Logger.LoggerSource.server);
          if (task.Allowed && crossTask.Allowed) {
            if (DatesCross(task.AllowDateStart, task.AllowDateEnd, crossTask.AllowDateStart, crossTask.AllowDateEnd)) {
              crossed = true;
            }

          }
          if (!task.Allowed && crossTask.Allowed) {
            if (DatesCross(task.NeedStartDate, task.NeedEndDate, crossTask.AllowDateStart, crossTask.AllowDateEnd)) {
              crossed = true;
            }
          }
          if (task.Allowed && !crossTask.Allowed) {
            if (DatesCross(task.AllowDateStart, task.AllowDateEnd, crossTask.NeedStartDate, crossTask.NeedEndDate)) {
              crossed = true;
            }
          }
          if (!task.Allowed && !crossTask.Allowed) {
            if (DatesCross(task.NeedStartDate, task.NeedEndDate, crossTask.NeedStartDate, crossTask.NeedEndDate)) {
              crossed = true;
            }
          }
          if (crossed) {
            task.hasCrossTasks = true;
            task.crossTasks += string.IsNullOrEmpty(task.crossTasks) ? crossTask.Number.ToString() : "," + crossTask.Number.ToString();
          }
        }
      }
      Filter.Data = result;
      return Filter;
    }

    public static List<string> ReadTextFile(string fileName) {
      Logger.info("read file" + fileName, Logger.LoggerSource.server);
      string[] lines = System.IO.File.ReadAllLines(PathFiles + fileName);
      Logger.info(lines.Count().ToString(), Logger.LoggerSource.server);
      return lines.ToList();
      //return new List<string>();
    }

    /*public static void ReadManagers() {
			Logger.info("Получение списка ответственных", Logger.LoggerSource.server);
			Managers = new List<string>();
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
				Logger.info("Ошибка при чтении доступных ответственных " + e.ToString(), Logger.LoggerSource.server);
			}
		}*/

    public static string getTashHTML(CranTaskInfo order, bool showStyle = true) {
      try {
        string style = showStyle ? "<Style>table {border-collapse: collapse;} td{text-align:center;} td.comments{text-align:left;} td, th {border-width: 1px;	border-style: solid;	border-color: #BBBBFF;	padding-left: 3px;	padding-right: 3px;}</Style>" : "";
        string htmlNumber = String.Format("Заявка на работу крана №{0} ", order.Number);
        string htmlState = String.Format("Состояние: {0}", order.State);
        string htmlFirstTRTable = String.Format("<table width='100%'><tr><th>{0}</th><th>{1}</th></tr></table>", htmlNumber, htmlState);
        string htmlInfoTable = String.Format("<table width='100%'><tr><th colspan='3'>Информация о заявке</th></tr><tr><th width='30%'>Оборудование</th><th  width='30%'>Текст заявки</th><th width='30%'>Согласовано</th></tr><tr><td width='30%'>{0}</td><td width='30%'>{1}</td><td width='30%'>{2}</td></tr></table>",
          order.CranName, String.Format("{0}<br/><b>Ответственный: </b>{1}", order.Comment, order.Manager), "");


        string htmlDatesTable =
          String.Format("<table width='100%'><tr><th colspan='4'>Сроки заявки</th></tr><tr><th>&nbsp;</th><th>Начало</th><th>Окончание</th><th>&nbsp;</th></tr><tr><th>Заявка</th><td>{0}</td><td>{1}</td><td>{2}</td></tr><tr><th>Разрешение</th><td>{3}</td><td>{4}</td><td>{5}</td></tr><tr><th>Факт</th><td>{6}</td><td>{7}</td><td>{8}</td></tr></table>",
          order.NeedStartDate.ToString("dd.MM.yy HH:mm"), order.NeedEndDate.ToString("dd.MM.yy HH:mm"), order.Author,
          order.Allowed ? order.AllowDateStart.ToString("dd.MM.yy HH:mm") : order.Denied ? "Отклонено" : order.Cancelled ? "Снята" : "&nbsp;",
          order.Allowed ? order.AllowDateEnd.ToString("dd.MM.yy HH:mm") : order.Denied ? "Отклонено" : order.Cancelled ? "Снята" : "&nbsp;",
          order.Allowed || order.Denied ? order.AuthorAllow : "-",
          order.Finished ? order.RealDateStart.ToString("dd.MM.yy HH:mm") : "-",
          order.Finished ? order.RealDateEnd.ToString("dd.MM.yy HH:mm") : "-", !string.IsNullOrEmpty(order.AuthorFinish) ? order.AuthorFinish : "-");


        string aComments = string.IsNullOrEmpty(order.AgreeComments) ? "" : order.AgreeComments.Replace("\r\n", "<br/>");
        string fullTable = String.Format("<table width='100%'><tr><td colspan='2'>{0}</td></tr><tr><td colspan='2'>{1}</td></tr><tr><td width='50%'>{2}</td><td width='50%'>{3}</td></tr></table>",
          htmlFirstTRTable, htmlInfoTable, htmlDatesTable, aComments);
        return style + fullTable;
      } catch (Exception e) {
        Logger.info("Ошибка при формировании html представления " + e.ToString(), Logger.LoggerSource.server);
        return "";
      }
    }

    public static string getTaskPrintHTML(CranTaskInfo order, bool showStyle = true) {
      try {
        string style = showStyle ? "<Style>table {border-collapse: collapse;} th.solid, td.solid {border-width: 1px;	border-style: solid;	border-color: #000000;} td.under {border-bottom-width: 1px;	border-bottom-style: solid;	border-bottom-color: #000000;} </Style>" : "";
        string body = String.Format(@"
<table >
	<tr >
		<th  colspan='7' align='right'>Приложение 1 <br/>к Регламенту по предоставлению подъемных сооружений Воткинской ГЭС<br/>(Для статистического учета работ ПС ГЭС)</th>	
	</tr>
	<tr>	
		<th  colspan='7' align='center'><h2>ЗАЯВКА №{0}</h2></th>
	</tr>	
	<tr>	
		<th  colspan='7' align='center'><h3>{1}</h3></th>
	</tr>	
	<tr><td  bordercolor='white' colspan='6' >&nbsp;</td></tr>
	<tr>	
		<th/>
		<th/>
		<th colspan='3' align='center' class='under'>&nbsp;</th>
		<th/>
	</tr>
	<tr>	
		<td colspan='7' align='center'><i>Наименование организации</i></td>
	</tr>
	<tr><td  bordercolor='white' colspan='6' >&nbsp;</td></tr>
	<tr >
		<th align='center' valign='top' width='50' class='solid' rowspan='2' >№ п/п</th>
		<th align='center' valign='top' width='150' class='solid' rowspan='2' >Наименование ПС</th>
		<th align='center' valign='top' width='250' class='solid' rowspan='2' >Краткое содержание работ</th>

		<th align='center' valign='top' width='300' class='solid' colspan='2' >Ответственный стропальщик</th>
		<th align='center' valign='top' width='220' class='solid' colspan='2'>Период использования</th>
		</tr>
	<tr>	
		
		<th align='center' valign='center' width='230' class='solid' >Ф.И.О.</th>
		<th align='center' valign='center' width='70' class='solid' >№ удостоверения</th>
		<th align='center' valign='center' width='110' class='solid' >Начало</th>
		<th align='center' valign='center' width='110' class='solid' >Окончание</th>
		
	</tr>
	<tr>
		<td align='center' valign='top' class='solid' >{2}</td>
		<td align='center' valign='top' class='solid' >{3}</td>
		<td align='center' valign='top' class='solid' >{4}</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >{5}</td>
		<td align='center' valign='top' class='solid' >{6}</td>
	</tr>

	<tr>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
	</tr>

	<tr>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
	</tr>

	<tr>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
	</tr>

	<tr>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
	</tr>

	<tr><td  bordercolor='white' colspan='6' >&nbsp;</td></tr>
	<tr><td  bordercolor='white' colspan='6' >&nbsp;</td></tr>
	<tr>
		<td bordercolor='white'  colspan='4'>Специалист, ответственный за безопасное производство работ с применением ПС «Заказчика»</td>
		<td bordercolor='white' class='under' align='right'>/</td>
		<td  bordercolor='white' class='under' colspan='2' >{7}</td>
	</tr>


	<tr><td  bordercolor='white' colspan='6' >&nbsp;</td></tr>
	<tr><th colspan='7' bordercolor='white' colspan='2'>СОГЛАСОВАНО </th></tr>

	<tr><td  bordercolor='white' colspan='6' >&nbsp;</td></tr>
	<tr>
		<td  bordercolor='white' colspan='4'>Представитель Дирекции по реализации ПКМ (только для кранов МЗ) </td>
		<td  bordercolor='white' class='under' align='right'>/</td>
		<td  bordercolor='white' class='under' colspan='2'>&nbsp;</td>
	</tr>

	<tr><td  bordercolor='white' colspan='7' >&nbsp;</td></tr>
	<tr>
		<td  bordercolor='white' colspan='4'>Специалист, ответственный за осуществление производственного контроля при эксплуатации ПС (при необходимости)</td>
		<td  bordercolor='white' class='under' align='right'>/</td>
		<td  bordercolor='white' class='under' colspan='2'>&nbsp;</td>
	</tr>	

	<tr><td  bordercolor='white' colspan='7' >&nbsp;</td></tr>
	<tr><th colspan='6' bordercolor='white' colspan='2'>УТВЕРЖДЕНО </th></tr>

	<tr><td  bordercolor='white' colspan='7' >&nbsp;</td></tr>
	<tr>
		<td  bordercolor='white' colspan='4'>{8}</td>
		<td  bordercolor='white' class='under' align='right'>/</td>
		<td  bordercolor='white' class='under' colspan='2'>{9}</td>
	</tr>	


</table>
", order.Number, order.DateCreate.ToString("dd.MM.yyyy"), 1, order.CranName, order.Comment,
  order.Allowed ? order.AllowDateStart.ToString("dd.MM.yy HH:mm") : order.NeedStartDate.ToString("dd.MM.yy HH:mm"),
  order.Allowed ? order.AllowDateEnd.ToString("dd.MM.yy HH:mm") : order.NeedEndDate.ToString("dd.MM.yy HH:mm"),
  order.CranNumber <= 2 ? "Представитель группы ТиГМО ПТС (только для кранов МЗ)" : "Исполнитель заявки", order.Manager, !String.IsNullOrEmpty(order.AuthorAllow) ? order.AuthorAllow : " ");
        return style + body;
      } catch (Exception e) {
        Logger.info("Ошибка при формировании html представления " + e.ToString(), Logger.LoggerSource.server);
        return "";
      }
    }

  }
}