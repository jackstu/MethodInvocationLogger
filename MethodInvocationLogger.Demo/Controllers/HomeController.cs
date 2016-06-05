using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MethodInvocationLogger.Demo.Core;
using Newtonsoft.Json;

namespace MethodInvocationLogger.Demo.Controllers
{
	public class HomeController : Controller
	{
		private readonly IUserContext _userContext;
		private readonly BusinessEventsList _businessEventsList;
		private readonly ISomeApiClient _someApiClient;
		private readonly CyclicApiCaller _cycliApiCaller;
		private readonly PerformanceItemsList _performanceItemsList;

		public HomeController(IUserContext userContext, BusinessEventsList businessEventsList, ISomeApiClient someApiClient, CyclicApiCaller cycliApiCaller,
			PerformanceItemsList performanceItemsList)
		{
			_userContext = userContext;
			_businessEventsList = businessEventsList;
			_someApiClient = someApiClient;
			_cycliApiCaller = cycliApiCaller;
			_performanceItemsList = performanceItemsList;
		}

		public ActionResult Index()
		{
			return View();
		}

		public virtual ActionResult EnableCyclicApiCaller()
		{
			_cycliApiCaller.Enabled = true;
			return new EmptyResult();
		}

		public virtual ActionResult DisableCyclicApiCaller()
		{
			_cycliApiCaller.Enabled = false;
			return new EmptyResult();
		}

		public virtual ActionResult SendSomeForm(FormData formData)
		{
			return Json(new { }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult PutDataToApi(string data)
		{
			_someApiClient.PutSomeData(data, new Data() {Data1=data, Data2 = data});
			return Json(new { }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult PerformanceLogChartData(DateTime min, DateTime max)
		{
			var items = _performanceItemsList.GetItems(min, max);

			if (items.Any())
			{
				return Json(items.Select(i => new KeyValuePair<DateTime, int>((DateTime)i["BeginTime"], (int)((TimeSpan)i["Duration"]).TotalMilliseconds)), JsonRequestBehavior.AllowGet);
			}
			else
			{
				List<KeyValuePair<DateTime, int>> list = new List<KeyValuePair<DateTime, int>>();
				list.Add(new KeyValuePair<DateTime, int>(DateTime.Now, 0));
					
				return Json(list, JsonRequestBehavior.AllowGet);
			}

			
		}

		public ActionResult PerformanceLog()
		{
			return View(_performanceItemsList.GetItems().OrderByDescending(i=>(DateTime)i["BeginTime"]).Select(JsonConvert.SerializeObject));
		}

		public ActionResult BusinessLog()
		{
			return View(_businessEventsList.GetEvents());
		}
	}
}