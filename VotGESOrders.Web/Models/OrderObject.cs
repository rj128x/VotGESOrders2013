﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using VotGESOrders.Web.ADONETEntities;
using VotGESOrders.Web.Logging;
using System.ServiceModel.DomainServices.Server;

namespace VotGESOrders.Web.Models
{
	public class OrderObject
	{

		public string ObjectName { get; set; }

		private string fullName;

		public string FullName {
			get { return fullName; }
			set { fullName = value; }
		}

		[Key]
		public int ObjectID { get; set; }
		public int ParentObjectID { get; set; }
		public bool ShowInFullName { get; set; }


		private OrderObject parentObject;
		[Include]
		[Association("Order_OrderObject1", "ParentObjectID", "ObjectID")]
		public OrderObject ParentObject {
			get {
				return parentObject;
			}
			set {
				parentObject = value;
				ParentObjectID = value == null ? -1 : value.ObjectID;
			}
		}

		[Include]
		[Association("Order_OrderObject2", "ObjectID", "ParentObjectID")]
		public List<OrderObject> ChildObjects { get; set; }



		protected static VotGESOrdersEntities context;
		protected static Dictionary<int,OrderObject> allObjects;

		public static void init() {
			Logger.info("Чтение списка объектов из БД", Logger.LoggerSource.server);
			allObjects = new Dictionary<int, OrderObject>();
			context = new VotGESOrdersEntities();

			VotGESOrdersEntities ctx = new VotGESOrdersEntities();
			IQueryable<OrderObjects> dbObjects=from oo in ctx.OrderObjects orderby oo.objectName select oo;
			foreach (OrderObjects dbObject in dbObjects) {
				allObjects.Add(dbObject.objectID, getFromDB(dbObject));
			}
			createNames();
			Logger.info("Чтение списка объектов из БД завершено", Logger.LoggerSource.server);
		}

		static OrderObject() {

		}


		public string getFullName() {
			OrderObject parent=getByID(ParentObjectID);
			List<string> names=new List<string>();
			names.Add(ObjectName);
			while (parent != null) {
				if (parent.ShowInFullName) {
					names.Add(parent.ObjectName);
				}
				parent = getByID(parent.ParentObjectID);
			}
			names.Reverse();
			return String.Join(" => ", names);
		}

		protected static void createNames() {
			foreach (KeyValuePair<int,OrderObject> de in allObjects) {
				OrderObject obj=de.Value;

				obj.FullName = obj.getFullName();
			}
		}

		public static List<int> getObjectIDSByFullName(string fullName) {
			return new List<int>(from OrderObject o in allObjects.Values where o.FullName.Contains(fullName) select o.ObjectID);
		}

		public void appendObjectIDSChildIDS(List<int> ObjectIDS) {
			IEnumerable<OrderObject> children=from OrderObject o in allObjects.Values where o.ParentObjectID==ObjectID select o;
			foreach (OrderObject obj in children) {
				if (!ObjectIDS.Contains(obj.ObjectID)) {
					ObjectIDS.Add(obj.ObjectID);
					obj.appendObjectIDSChildIDS(ObjectIDS);
				}
			}
		}

		public static IQueryable<OrderObject> getAllObjects() {
			return allObjects.Values.AsQueryable();
		}


		public static OrderObject getFromDB(OrderObjects objectDB) {
			try {
				OrderObject obj=new OrderObject();
				obj.ObjectName = objectDB.objectName;
				obj.ObjectID = objectDB.objectID;
				obj.ParentObjectID = objectDB.parentID;
				obj.ShowInFullName = objectDB.showInFullName;
				return obj;
			} catch (Exception e) {
				Logger.error(String.Format("Ошибка при получении информации об оборудовании: {0}", e), Logger.LoggerSource.server);
			}
			return null;
		}

		public static OrderObject getByID(int id) {
			if (allObjects.ContainsKey(id)) {
				return allObjects[id];
			} else {
				return null;
			}
		}

		public override string ToString() {
			return string.Format("ID: {0}, ParentID: {1}, Name: {2}",
				ObjectID, ParentObjectID, ObjectName);
		}
	}
}