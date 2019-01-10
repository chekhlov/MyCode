using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Common.Tools;
using DevExpress.Web.Mvc;
using Marketing.Models;
using Marketing.ViewModels;

namespace Marketing.Controllers
{
	[Authorize]
	public partial class ReportController : BasePromoterController
	{
		//Перезагружаем диаграмму с гугла, чтобы отдать ее через https
		public ActionResult GetChartPng()
		{
			var request = WebRequest.Create(string.Format("http://chart.apis.google.com/chart{0}", Request.Url.Query));
			using (var response = request.GetResponse().GetResponseStream()) {
				var memory = new MemoryStream();
				response.CopyTo(memory);
				memory.Position = 0;
				return File(memory, "image/png");
			}
		}

		public ActionResult ProductMovementAggregatedGrid(ReportFilter filter)
		{
			filter.agColumnsIds = string.IsNullOrEmpty(filter.agColumnsIds)
				? ProductMovementItem.GetDefaultPropertyList()
				: filter.agColumnsIds;

			//Дополнительные параметры
			filter.agColumns = ProductMovementItem.PropertyAliasForAggrigationList(filter.agColumnsIds, filter);
			filter.Association = CurrentAssociation;
			filter.isAggregated = true;

			var model = new ProductMovementViewModel() {
				filter = filter,
				items = ProductMovementItem.GetProductMovementData(DbSession, filter)
			};
			return PartialView("partials/_ProductMovementAggregatedGrid", model);
		}

		public ActionResult ProductMovementWaybillsGrid(ReportFilter filter)
		{
			filter.Association = CurrentAssociation;

			var model = new ProductMovementViewModel() {
				filter = filter,
				items = ProductMovementItem.GetProductMovementData(DbSession, filter)
			};
			return PartialView("partials/_ProductMovementWaybillsGrid", model);
		}

		public ActionResult ProductMovementOrdersGrid(ReportFilter filter)
		{
			filter.Association = CurrentAssociation;
			var model = new ProductMovementViewModel() {
				filter = filter,
				items = ProductMovementItem.GetProductMovementData(DbSession, filter)
			};
			return PartialView("partials/_ProductMovementOrdersGrid", model);
		}

		[CheckPermission(Permission.Reports)]
		public ActionResult ProductMovementWaybills(ReportFilter filter = null)
		{
			filter = filter ?? new ReportFilter();
			filter.isWaybillReport = true;
			return ProductMovement(filter);
		}

		[CheckPermission(Permission.Reports)]
		public ActionResult ProductMovementOrders(ReportFilter filter = null)
		{
			filter = filter ?? new ReportFilter();
			filter.isWaybillReport = false;
			return ProductMovement(filter);
		}

		private ActionResult ProductMovement(ReportFilter filter)
		{
			filter.Association = CurrentAssociation;
			var reportWithoutData = false;

			if (filter.assortmentType != 0) {
				filter.eventId = null;
				filter.promotionIdList = "";
			}
			else {
				reportWithoutData = !filter.eventId.HasValue || filter.eventId == 0;
			}

			filter.dateBegin = filter.dateBegin ?? DateTime.Now.Date.AddDays(-(DateTime.Now.Day - 1));
			filter.dateEnd = filter.dateEnd ?? DateTime.Now.Date;

			var agColumns = new List<ViewModelListItem>();
			if (filter.isAggregated) {

				filter.agColumnsIds = string.IsNullOrEmpty(filter.agColumnsIds)
					? ProductMovementItem.GetDefaultPropertyList()
					: filter.agColumnsIds;

				//Дополнительные параметры
				agColumns = ProductMovementItem.PropertyAliasForAggrigationList(filter.agColumnsIds, filter);
			}

			filter.agColumns = agColumns;
			var model = new ProductMovementViewModel() {
				filter = filter,
				items = reportWithoutData
					? new List<ProductMovementItem>()
					: ProductMovementItem.GetProductMovementData(DbSession, filter)
			};

			// TODO: Переписать
			if (filter.exportToExcel.HasValue) {
				var stream = filter.ToExcel(DbSession, model.items);
				var reportName =
					filter.isAggregated
						? $"Cуммарные_данные_по_юр.лицам_{(filter.isWaybillReport ? "накладные" : "заявки")}.xlsx"
						: $"Движение_товара_по_{(filter.isWaybillReport ? "накладным" : "заявкам")}.xlsx";
				return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportName);
			}

			var totalSum = reportWithoutData
				? new ProductMovementItem.TotalNumberData()
				: ProductMovementItem.GetProductMovementDataTotalNumber(model.items);


			var chartData = reportWithoutData
				? new ProductMovementItem.ChartData[0]
				: ProductMovementItem.GetProductMovementSupplierRate(model.items);

			var additionalDataRaw = chartData;

			List<string> additionalDataRawString;

			if (filter.isWaybillReport)
				additionalDataRawString = additionalDataRaw.Where(s => !chartData.Any(c => c.SupplierId == s.SupplierId))
					.Select(s => $"<span class='item'>{s.SupplierName}</span>")
					.ToList();
			else
				additionalDataRawString = chartData.Where(s => !additionalDataRaw.Any(c => c.SupplierId == s.SupplierId))
					.Select(s => $"<span class='item'>{s.SupplierName}</span>")
					.ToList();

			if (chartData.Length > 0)
				ViewBag.ChartUrl = ProductMovementItem.GetGooglePieChartUrl(chartData, 1, true, 800);
			var additionalData = additionalDataRawString.Count > 0
				? string.Join(", ", additionalDataRawString)
				: "";
			if (!string.IsNullOrEmpty(additionalData))
				ViewBag.AdditionalData = additionalData;

			ViewBag.TotalQuantity = totalSum?.UIntNumber ?? 0;
			ViewBag.TotalSum = (totalSum?.DoubleNumber ?? 0).ToString("##.00");

			if (filter.isWaybillReport) {
				return View("ProductMovementWaybills", model);
			}

			return View("ProductMovementOrders", model);
		}

		[HttpPost]
		public ActionResult GetColumnList(ReportFilter filter, string filterName, string currentValues = "")
		{
			return PartialView("partials/_ProductMovementAggregatedColumnsLogic",
				new ReportFilterDataViewModel(filterName,
					ProductMovementItem.PropertyAliasForAggrigationList(currentValues, filter)));
		}

		[HttpPost]
		public ActionResult GetListMemberCategories()
		{
			return Json(ProductMovementItem.GetMemberCategories(DbSession));
		}

		[HttpPost]
		public ActionResult GetListRegion()
		{
			return Json(ProductMovementItem.GetRegions(DbSession, CurrentAssociation));
		}

		[HttpPost]
		public ActionResult GetListClients(ReportFilter filter)
		{
			filter.clientIdList = "";
			filter.Association = CurrentAssociation;
			return Json(ProductMovementItem.GetClients(DbSession, filter)
				.Select(m => new {m.Id, m.Name}).ToList());
		}

		[HttpPost]
		public ActionResult GetListEvents(ReportFilter filter)
		{
			filter.Association = CurrentAssociation;
			filter.eventId = null;

			var data = (filter.assortmentType != 0)
				? new List<ReportFilter.reactSelectItem>()
				: ProductMovementItem.GetEvents(DbSession, filter);
			return Json(data);
		}

		[HttpPost]
		public ActionResult GetListPromotions(uint? eventId)
		{
			var data = (!eventId.HasValue || eventId == 0)
				? new List<ReportFilter.reactSelectItem>()
			  : ProductMovementItem.GetPromotions(DbSession, eventId);
			return Json(data);
		}

		[HttpPost]
		public ActionResult GetListProducts(ReportFilter filter, string value = "")
		{
			var product = new List<ReportFilter.reactSelectItem>();
			if (filter.assortmentType == 0) {
				if (filter.eventId.HasValue && filter.eventId != 0)
					product = ProductMovementItem.GetProductsByAccion(DbSession, filter, value);
			}
			else {
				product = ProductMovementItem.GetProducts(DbSession, filter.catalogIdList, value);
			}

			return Json(product);
		}

		[HttpPost]
		public ActionResult GetListAddresses(ReportFilter filter)
		{
			filter.addressIdList = "";
			filter.Association = CurrentAssociation;
			if (string.IsNullOrEmpty(filter.clientIdList))
				filter.clientIdList = string.Join(",", ProductMovementItem.GetClients(DbSession, filter).Select(s => s.Id).ToList());

			var data = ProductMovementItem.GetAddresses(DbSession, filter);
			return Json(data);
		}

		[HttpPost]
		public ActionResult GetListSuppliers(ReportFilter filter)
		{
			filter.supplierIdList = "";
			filter.Association = CurrentAssociation;
			if (string.IsNullOrEmpty(filter.clientIdList))
				filter.clientIdList = string.Join(",", ProductMovementItem.GetClients(DbSession, filter).Select(s => s.Id).ToList());

			var data = ProductMovementItem.GetSuppliers(DbSession, filter);

			if (filter.assortmentType == 0 && filter.eventId.HasValue && filter.eventId != 0 && !string.IsNullOrEmpty(filter.promotionIdList))
				data.Insert(0, new ReportFilter.reactSelectItem(){ Id = 0, Name = "Только разрешенные"});

			return Json(data);
		}

		[HttpPost]
		public ActionResult GetListProducers(ReportFilter filter, string value = "")
		{
			filter.Association = CurrentAssociation;
			var producers = new List<ReportFilter.reactSelectItem>();
			if (filter.assortmentType == 0 && filter.eventId.HasValue && filter.eventId != 0)
				producers = ProductMovementItem.GetProducersByAction(DbSession, filter, value);
			else
				producers = ProductMovementItem.GetProducers(DbSession, value);
			return Json(producers);
		}
	}
}