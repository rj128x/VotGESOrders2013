﻿using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using VotGESOrders.Web.Models;

namespace VotGESOrders.Web {
	[ServiceContract(Namespace = "")]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class CranService {
		[OperationContract]
		public void DoWork() {
			// Добавьте здесь реализацию операции
			return;
		}

		[OperationContract]
		public CranFilter getCranTasks(CranFilter Filter) {
			return CranTaskInfo.LoadCranTasks(Filter);
		}

		[OperationContract]
		public ReturnMessage CreateCranTask(CranTaskInfo task) {
			return CranTaskInfo.CreateCranTask(task);
		}

		// Добавьте здесь дополнительные операции и отметьте их атрибутом [OperationContract]
	}
}