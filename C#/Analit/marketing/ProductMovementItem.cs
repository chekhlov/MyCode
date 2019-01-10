using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using Common.Tools;
using Dapper;
using DevExpress.Web.Mvc;
using log4net;
using Marketing.Helpers;
using Marketing.Models;
using NHibernate;
using NHibernate.Linq;
using NPOI.XSSF.UserModel;
using Newtonsoft.Json;

namespace Marketing.ViewModels
{
	public class ReportFilter
	{
		public class reactSelectItem
		{
			public uint Id { get; set; }
			public string Name { get; set; }
		}

		public ReportFilter()
		{
		}

		public ReportFilter(ReportFilter filter)
		{
			this.isWaybillReport = filter.isWaybillReport;
			this.dateBegin = filter.dateBegin;
			this.dateEnd = filter.dateEnd;
			this.addressIdList = filter.addressIdList;
			this.catalogIdList = filter.catalogIdList;
			this.producerIdList = filter.producerIdList;
			this.supplierIdList = filter.supplierIdList;
			this.clientIdList = filter.clientIdList;
			this.vitallyImportant = filter.vitallyImportant;
			this.isPku = filter.isPku;
			this.assortmentType = filter.assortmentType;
			this.eventId = filter.eventId;
			this.promotionIdList = filter.promotionIdList;
			this.Association = filter.Association;
			this.subscribeEventStatusId = filter.subscribeEventStatusId;
			this.regionIdList = filter.regionIdList;

			this.agColumns = filter.agColumns != null
				? new List<ViewModelListItem>(filter.agColumns)
				: new List<ViewModelListItem>();

			this.agColumnsIds = filter.agColumnsIds;
			this.memberCategoriesIdList = filter.memberCategoriesIdList;
		}

		public string SerializeJson()
		{
			return JsonConvert.SerializeObject(this);
		}

		public bool isWaybillReport { get; set; }
		public DateTime? dateBegin { get; set; }
		public DateTime? dateEnd { get; set; }
		public string regionIdList { get; set; }
		public string memberCategoriesIdList { get; set; }
		public string addressIdList { get; set; }
		public string catalogIdList { get; set; }
		public string producerIdList { get; set; }
		public string supplierIdList { get; set; }
		public string clientIdList { get; set; }
		public uint vitallyImportant { get; set; } = 0;
		public uint isPku { get; set; } = 0;
		public uint assortmentType { get; set; } = 0;
		public uint? eventId { get; set; }
		public uint subscribeEventStatusId { get; set; } = 1;
		public string promotionIdList { get; set; }
		public bool isAggregated { get; set; }

		[JsonIgnore]
		public bool? exportToExcel { get; set; }

		[JsonIgnore]
		public string agColumnsIds { get; set; }

		[JsonIgnore]
		public Association Association { get; set; }

		[JsonIgnore]
		public List<ViewModelListItem> agColumns { get; set; }

		public Stream ToExcel(ISession dbsession, List<ProductMovementItem> result)
		{
			var reportName =
				isAggregated
					? $"Cуммарные данные по юр.лицам ({(isWaybillReport ? "накладные" : "заявки")}), {Association.Name}"
					: $"Движение товара по {(isWaybillReport ? "накладным" : "заявкам")}, {Association.Name}";

			var workbook = new XSSFWorkbook();
			if (isAggregated) {
				workbook.CreateSheet("Лист");
				ProductMovementReportHelper.GetExcelHeader(workbook, reportName, dbsession, this, false);
				ProductMovementReportHelper.GetExcelBody(workbook, this, result);
			}
			else {
				var gridSettings = isWaybillReport
					? ProductMovementReportHelper.ExportWaybillGridViewSettings
					: ProductMovementReportHelper.ExportOrderGridViewSettings;
				var res = GridViewExtension.ExportToXlsx(gridSettings, result, true);
				workbook = new XSSFWorkbook(((FileStreamResult) res).FileStream);
				ProductMovementReportHelper.GetExcelHeader(workbook, reportName, dbsession, this, true);
			}

			var memory = new MemoryStream();
			workbook.Write(memory);
			//workbook.Write - закроет поток
			return new MemoryStream(memory.ToArray());
		}

		public bool IsEmpty()
		{
			return string.IsNullOrEmpty(supplierIdList) && string.IsNullOrEmpty(producerIdList) &&
			       string.IsNullOrEmpty(clientIdList) && string.IsNullOrEmpty(regionIdList) &&
			       string.IsNullOrEmpty(memberCategoriesIdList) &&
			       string.IsNullOrEmpty(catalogIdList) && string.IsNullOrEmpty(addressIdList) &&
			       eventId == null && string.IsNullOrEmpty(promotionIdList)
			       && exportToExcel == null && string.IsNullOrEmpty(agColumnsIds);
		}
	}
	public class ProductMovementViewModel
	{
		public ReportFilter filter = new ReportFilter();
		public List<ProductMovementItem> items = new List<ProductMovementItem>();
	}
	public class ProductMovementItem
	{
		private static ILog log = LogManager.GetLogger(typeof(ProductMovementItem));

		private const string simpleEncoding = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";


		public static readonly Dictionary<string, string> PropertyAliasForAggrigation = new Dictionary<string, string> {
			{nameof(LegalName), "Юр.лицо"},
			{nameof(INN), "ИНН"},
			{nameof(Region), "Регион"},
			{nameof(MemberCategory), "Статус клиента"},
			{nameof(Promotion), "Акция"},
			{nameof(Subscribe), "Подписка на акцию"},
			{nameof(Address), "Адрес аптеки"},
			{nameof(CatalogName), "Товар"},
			{nameof(ProducerName), "Производитель"},
			{nameof(SupplierName), "Поставщик"},
			{nameof(SupplierINN), "ИНН поставщика"},
			{nameof(ProviderDocumentId), "Номер документа"},
			{nameof(DocumentDate), "Дата документа"}
		};

		public DateTime DocumentDate { get; set; }
		public string ProviderDocumentId { get; set; }
		public string Region { get; set; }
		public string SupplierName { get; set; }
		public string SupplierINN { get; set; }
		public string MemberCategory { get; set; }
		public string Address { get; set; }
		public string CatalogName { get; set; }
		public string ProducerName { get; set; }
		public string LegalName { get; set; }
		public string INN { get; set; }
		public double SupplierCost { get; set; }
		public double Quantity { get; set; }
		public double Sum { get; set; }
		public uint hProducerId { get; set; }
		public uint hCatalogId { get; set; }
		public uint hAddressId { get; set; }
		public uint hFirmCode { get; set; }
		public string Subscribe { get; set; }
		public string Promotion { get; set; }

		public static string GetDefaultPropertyList(string fieldsString = null)
		{
			var indexList = new List<int>();
			var fields = !String.IsNullOrEmpty(fieldsString)
				? fieldsString.Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries)
				: DefaultAggregats();

			foreach (var item in fields) {
				indexList.Add(PropertyAliasForAggrigation.IndexOf(PropertyAliasForAggrigation.First(s => s.Key == item)));
			}

			return string.Join(",", indexList);
		}

		public static string[] DefaultAggregats()
		{
			return new[] {
				nameof(LegalName),
				nameof(INN),
				nameof(Address),
				nameof(SupplierName),
				nameof(SupplierINN),
				nameof(CatalogName),
				nameof(ProducerName)
			};
		}

		public static List<ViewModelListItem> PropertyAliasForAggrigationList(string currentValues = "",
			ReportFilter filter = null)
		{
			var agColumnsIds = !string.IsNullOrEmpty(currentValues)
				? currentValues
				: GetDefaultPropertyList();

			var currentValuesList = GetUIntListForString(agColumnsIds);

			var listRaw = new List<ViewModelListItem>();
			for (var i = 0; i < PropertyAliasForAggrigation.Count; i++) {
				var el = PropertyAliasForAggrigation.ElementAt(i);

				if (filter != null && (filter.assortmentType == 1 || !filter.eventId.HasValue || filter.eventId == 0) 
					&& (nameof(Promotion) == el.Key || nameof(Subscribe) == el.Key)) continue;

				listRaw.Add(new ViewModelListItem {
					Value = (ulong) i,
					Text = el.Value,
					ReservedText = el.Key,
					Selected = currentValuesList.Any(s => s == i)
				});
			}
			return listRaw;
		}

		public static string PropertyAliasForAggrigationJson()
		{
			return Json.Encode(PropertyAliasForAggrigationList().Select(s => new {@Key = s.Value, @Value = s.Text}).ToList());
		}

  	public static string PrepareFilters(ReportFilter filter, ref Dictionary<string, object> parameters)
		{

			var filters = String.Empty;

			if (!string.IsNullOrEmpty(filter.regionIdList)) {
				filters += "AND rc.RegionCode IN @regionIdList ";
				parameters.Add("regionIdList", Parse(filter.regionIdList));
			}

			if (!string.IsNullOrEmpty(filter.memberCategoriesIdList)) {
				filters += "AND mc.Id IN @memberCategoriesIdList ";
				parameters.Add("memberCategoriesIdList", Parse(filter.memberCategoriesIdList));
			}

			if (!string.IsNullOrEmpty(filter.addressIdList)) {
				filters += "AND a.Id IN @addressIdList ";
				parameters.Add("addressIdList", Parse(filter.addressIdList));
			}

			if (!string.IsNullOrEmpty(filter.producerIdList)) {
				filters += "AND pr.Id IN @producerIdList ";
				parameters.Add("producerIdList", Parse(filter.producerIdList));
			}

			if(!string.IsNullOrEmpty(filter.supplierIdList))
			{
				var parse = Parse(filter.supplierIdList);
				var addFilter = "";
				// Если выбрано - все разрешенные поставщики (код 0)
				if(filter.assortmentType == 0 && !String.IsNullOrEmpty(filter.promotionIdList)
						 && parse.Any() && parse[0] == 0)
				{
					addFilter = @"
					OR d.SuppliersType = 0
					OR (d.SuppliersType = 1
							AND s.Id IN (SELECT ps.SupplierId
									FROM PromotionSuppliers ps
									WHERE ps.PromotionId = d.PromotionId))
					OR (d.SuppliersType = 2
							AND s.Id NOT IN (SELECT ps.SupplierId
									FROM PromotionSuppliers ps
									WHERE ps.PromotionId = d.PromotionId))";
				}

				filters += $"AND (s.Id IN @supplierIdList {addFilter}) ";
				parameters.Add("supplierIdList", parse);
			}

			if(!string.IsNullOrEmpty(filter.catalogIdList)) {
				filters += "AND p.Id IN @catalogIdList ";
				parameters.Add("catalogIdList", Parse(filter.catalogIdList));
			}

			// 0 - неважно, 1 - да, 2 - нет
			switch (filter.vitallyImportant) {
				case 1: filters += "AND c.VitallyImportant = 1 "; break;
				case 2: filters += "AND c.VitallyImportant = 0 "; break;
				default: break;
			}

			// 0 - неважно, 1 - да, 2 - нет
			switch (filter.isPku) {
				case 1: filters += "AND (c.Narcotic = 1 OR c.Toxic = 1 OR c.Combined = 1 OR c.Other = 1) "; break;
				case 2: filters += "AND c.Narcotic = 0 AND c.Toxic = 0 AND c.Combined = 0 AND c.Other = 0 "; break;
				default: break;
			}

			if(filter.assortmentType == 0 && filter.eventId.HasValue && filter.eventId != 0)
			{
				// Фильтр подписки на акцию: 0 все, 1 - подписанные, 2 - подписанные Производителем и Ассоциацией,
				// 3 - подписанные Производителем, Ассоциацией и Аптекой, 4 - неподписанные
				switch(filter.subscribeEventStatusId)
				{
					case 1: filters += @"AND EXISTS (SELECT DISTINCT 1 
						FROM customers.promotionsubscribes ps 
						WHERE ps.MemberId = pm.Id 
							AND ps.PromotionId = d.PromotionId 
							AND ps.byAssociation = 1 
							AND ps.byAdmin = 1 
							AND ps.byOwner = 1 
							AND ps.byClient = 1) "; break;
					case 2: filters += @"AND EXISTS (SELECT DISTINCT 1 
						FROM customers.promotionsubscribes ps 
						WHERE ps.MemberId = pm.Id 
							AND ps.PromotionId = d.PromotionId 
							AND ps.byAssociation = 1 
							AND ps.byOwner = 1) "; break;
					case 3: filters += @"AND EXISTS (SELECT DISTINCT 1 
						FROM customers.promotionsubscribes ps 
						WHERE ps.MemberId = pm.Id 
							AND ps.PromotionId = d.PromotionId 
							AND ps.byAssociation = 1 
							AND ps.byOwner = 1 
							AND ps.byClient = 1) "; break;
					case 4: filters += @"AND NOT EXISTS (SELECT DISTINCT 1 
						FROM customers.promotionsubscribes ps 
						WHERE ps.MemberId = pm.Id 
							AND ps.PromotionId = d.PromotionId 
							AND (ps.byAssociation = 1 
								OR ps.byOwner = 1 
								OR ps.byClient = 1
								OR ps.byAdmin = 1)) "; break;
					default: break;
				}
			}
			return filters;
		}


		public static List<ProductMovementItem> GetProductMovementData(ISession dbSession, ReportFilter filter)
		{
			var beginDate = filter.dateBegin ?? DateTime.Now.Date.AddDays(-(DateTime.Now.Day - 1));
			var endDate = filter.dateEnd ?? DateTime.Now;
			var parameters = new Dictionary<string, object> {
				{"beginDate", beginDate.Date},
				{"endDate", endDate.Date.AddDays(1)},
				{"association", filter.Association.Id}
			};

			var filtersSql = PrepareFilters(filter, ref parameters);

			var promotionSql = String.Empty;
			var joinSql = String.Empty;
			var additionColumnsSql = String.Empty;

			if (filter.assortmentType == 0) {
				var promotionFilters = string.Empty;
				if (!String.IsNullOrEmpty(filter.promotionIdList)) {
					promotionFilters = "AND pp.Id  IN @promotionIdList ";
					parameters.Add("promotionIdList", Parse(filter.promotionIdList));
				}
				parameters.Add("eventId", filter.eventId);

				promotionSql = $@"DROP TEMPORARY TABLE IF EXISTS ProductsForReport;
				CREATE TEMPORARY TABLE ProductsForReport(
					ProductId int unsigned not null,
					ProducerId int unsigned not null,
					PromotionId int unsigned not null,
					Promotion varchar(255),
					SuppliersType tinyint(4),
					INDEX idx_ProductsForReport (ProductId, ProducerId)
				) engine = memory;

				INSERT INTO ProductsForReport(ProducerId, ProductId, PromotionId, Promotion, SuppliersType)
				SELECT DISTINCT mep.ProducerId, ppt.ProductId, pp.Id, pp.Name, pp.SuppliersType
				FROM customers.PromotionProducts AS ppt
				INNER JOIN customers.ProducerPromotions AS pp ON pp.Id = ppt.PromotionId
				INNER JOIN customers.promoterproducers AS mep ON mep.MarketingEventId = pp.MarketingEventId
				WHERE mep.MarketingEventId = @eventId
					AND pp.SelectMethod = 0
					{promotionFilters}
				UNION
				SELECT DISTINCT ref.CodeFirmCr as ProducerId, ref.ProductId, pp.Id, pp.Name, pp.SuppliersType
				FROM customers.ProducerPromotions AS pp
				JOIN Farm.Core0 c on c.PriceCode = pp.PriceId
				JOIN Customers.MarketingEvents me on me.Id = pp.MarketingEventId
				JOIN Customers.Associations a on a.Id = me.AssociationId
				JOIN Usersettings.PricesData pd on pd.FirmCode = a.SupplierId
				JOIN Farm.Core0 ref on ref.PriceCode = pd.PriceCode
				WHERE pp.MarketingEventId = @eventId
					AND pp.PriceId is not null
					AND pp.SelectMethod = 1
					AND ref.ProductId is not null
					AND ref.CodeFirmCr is not null
					AND ref.Code <> ''
					AND ref.Code = c.Code
					{promotionFilters}
				GROUP BY ProducerId, ProductId, Id, Name, SuppliersType;";

				additionColumnsSql = @"
				,IFNULL((SELECT DISTINCT
						CASE
							WHEN ps.byAssociation = 1 AND ps.byAdmin = 1 AND ps.byOwner = 1 AND ps.byClient = 1 THEN 'Подписан'
							WHEN ps.byAssociation = 1 AND ps.byOwner = 1 AND ps.byClient = 1 THEN 'Подписан Производителем, Ассоциацией и Аптекой'
							WHEN ps.byAssociation = 1 AND ps.byAdmin = 1 AND ps.byOwner = 1 AND ps.byClient = 1 THEN 'Подписан Производителем и Ассоциацией'
						ELSE 'Неподписан'
						END 'Subscribe'
					FROM customers.promotionsubscribes ps 
					WHERE ps.MemberId = pm.Id
					AND ps.PromotionId = d.PromotionId), 'Неподписан') AS 'Subscribe'
				,d.Promotion AS 'Promotion' ";

				joinSql = @"JOIN ProductsForReport AS d ON d.ProductId = p.Id AND d.ProducerId = pr.Id";
			}

			var sql = String.Empty;
			if (filter.isWaybillReport) {
				sql = $@"SELECT DISTINCT
					 dh.DocumentDate AS 'DocumentDate'
					,dh.ProviderDocumentId AS 'ProviderDocumentId'
					,db.SupplierCost AS 'SupplierCost'
					,db.Quantity AS 'Quantity'
					,(db.SupplierCost * db.Quantity) AS 'Sum'
					,CONCAT(s.Name, ' (', rs.Region, ') ') AS 'SupplierName'
					,pa.INN AS 'SupplierINN'
					,ass.Name AS 'Association'
					,rc.Region AS 'Region'
					,a.address AS 'Address'
					,mc.Name AS 'MemberCategory'
					,CONCAT_WS(' ', c.Name, p.Properties) AS 'CatalogName'
					,pr.name AS 'ProducerName'
					,c.id AS 'hCatalogId'
					,le.Name AS 'LegalName'
					,le.INN AS 'INN'
					,s.Id AS 'SupplierId'
					,p.Id AS 'hProducerId'
					,a.Id AS 'hAddressId'
					,s.Id AS 'hFirmCode'
					{additionColumnsSql}
				FROM documents.documentheaders dh
				JOIN documents.documentbodies db ON db.DocumentId = dh.Id
				JOIN ClientsForReport cs ON cs.ClientId = dh.ClientCode
				JOIN customers.promotionmembers pm ON cs.ClientId = pm.ClientId
				JOIN customers.associations ass ON pm.AssociationId = ass.Id
				JOIN customers.membercategories mc ON mc.Id = pm.MemberCategoryId
				JOIN catalogs.Producers Pr ON pr.id = db.ProducerId
				JOIN customers.Suppliers s ON s.id = dh.FirmCode
				LEFT JOIN billing.Payers pa ON s.Payer = pa.PayerID
				JOIN catalogs.Products P ON p.id = db.ProductId
				JOIN catalogs.catalog c	ON c.id = p.catalogid
				JOIN customers.Addresses a ON a.id = dh.AddressId
				JOIN billing.LegalEntities le ON le.Id = a.LegalEntityId
				JOIN farm.Regions rs ON s.HomeRegion = rs.RegionCode
				JOIN farm.Regions rc ON cs.RegionCode = rc.RegionCode
				{joinSql}
				WHERE ass.Id = @association
				AND dh.DocumentDate >= @beginDate AND dh.DocumentDate < @endDate
				{filtersSql}
				ORDER BY 1, 2";
		}
			else {
				sql = $@"SELECT DISTINCT 
					dh.WriteTime AS 'DocumentDate'
					,CAST(dh.RowID AS CHAR) AS 'ProviderDocumentId'
					,db.Cost AS 'SupplierCost'
					,db.Quantity AS 'Quantity'
					,(db.Cost * db.Quantity) AS 'Sum'
					,CONCAT(s.Name, ' (', rs.Region, ') ') AS 'SupplierName'
					,pa.INN AS 'SupplierINN'
					, ass.Name AS 'Association'
					,rc.Region AS 'Region'
					,a.address AS 'Address'
					,mc.Name AS 'MemberCategory'
					,CONCAT_WS(' ', c.Name, p.Properties) AS 'CatalogName'
					,pr.name AS 'ProducerName'
					,c.id AS 'hCatalogId'
					,le.Name AS 'LegalName'
					,le.INN AS 'INN'
					,s.Id AS 'SupplierId'
					,p.Id AS 'hProducerId'
					,a.Id AS 'hAddressId'
					,s.Id AS 'hFirmCode'
					{additionColumnsSql}
				FROM Orders.OrdersHead dh
				JOIN Orders.OrdersList db ON db.OrderID = dh.RowId
				JOIN ClientsForReport cs ON cs.ClientId = dh.ClientCode
				JOIN customers.promotionmembers pm ON cs.ClientId = pm.ClientId
				JOIN customers.associations ass ON pm.AssociationId = ass.Id
				JOIN customers.membercategories mc ON mc.Id = pm.MemberCategoryId
				JOIN catalogs.Producers Pr ON pr.id = db.CodeFirmCr
				JOIN usersettings.pricesdata pd ON dh.PriceCode = pd.PriceCode
				JOIN customers.Suppliers s ON s.id = pd.FirmCode
				LEFT JOIN billing.Payers pa ON s.Payer = pa.PayerID
				JOIN catalogs.Products P ON p.id = db.ProductId
				JOIN catalogs.catalog c ON c.id = p.catalogid
				JOIN customers.Addresses a ON a.id = dh.AddressId
				JOIN billing.LegalEntities le ON le.Id = a.LegalEntityId
				JOIN farm.Regions	rs ON s.HomeRegion = rs.RegionCode
				JOIN farm.Regions	rc ON dh.RegionCode = rc.RegionCode
				{joinSql}
				WHERE ass.Id = @association
				AND dh.Processed = 1 and dh.WriteTime >= @beginDate and dh.WriteTime < @endDate
				{filtersSql}
				ORDER BY 1, 2";
			}

			if (filter.isAggregated) {
				var aggregates = filter.agColumns.Where(x => x.Selected).Implode(x => x.ReservedText);
				if (aggregates.Any()) {
					sql = $@"SELECT {aggregates}, SUM(Quantity) as Quantity, SUM(SupplierCost*Quantity) as Sum
		  		 			 FROM ( {sql} ) AS f
		      			 GROUP BY {aggregates}";
				}
			}


			if (filter.assortmentType == 0) {
				sql = $@"{promotionSql}
					    {sql};
					    DROP TEMPORARY TABLE ProductsForReport";
			}

			// Используем временную таблицу - существенно ускоряется выполнение запроса.
			if(!string.IsNullOrEmpty(filter.clientIdList))
				parameters.Add("clientIds", Parse(filter.clientIdList));
			else
				parameters.Add("clientIds", GetClients(dbSession, filter).Select(s => s.Id).ToArray());

			sql = $@"
				DROP TEMPORARY TABLE IF EXISTS ClientsForReport;
				CREATE TEMPORARY TABLE ClientsForReport(ClientId int UNSIGNED, RegionCode BIGINT(20)) engine=memory;
				INSERT INTO ClientsForReport SELECT DISTINCT Id, RegionCode FROM customers.clients WHERE id IN @clientIds;
			  {sql};
			  DROP TEMPORARY TABLE ClientsForReport;";

			if(log.IsDebugEnabled) {
				log.Debug(sql);
				log.Debug(parameters.Implode(x => {
					if (x.Value is IList list)
						return $"{x.Key} = " + String.Join(", ", list);
					return $"{x.Key} = {x.Value}";
				}));
			}

			return dbSession.Connection.Query<ProductMovementItem>(sql, parameters).ToList();
		}

		private static uint?[] Parse(string promotionIdList)
		{
			var ids = promotionIdList.Split(',').Select(x => NullableConvert.ToUInt32(x.Trim()))
				.Where(x => x != null).OrderBy(x => x).ToArray();
			return ids;
		}

		public static TotalNumberData GetProductMovementDataTotalNumber(List<ProductMovementItem> data)
		{
			if(!data.Any()) return new TotalNumberData();
			return new TotalNumberData {DoubleNumber = data.Sum(s => s.Sum), UIntNumber = (uint) data.Sum(s => s.Quantity)};
		}

		public static ChartData[] GetProductMovementSupplierRate(List<ProductMovementItem> data)
		{
			if (!data.Any()) return new ChartData[0];

			var totalCount = data.Sum(s => s.Sum);

			if (totalCount == 0) return new ChartData[0];
			
			var group = data.GroupBy(s => s.SupplierName);
			return group.Select(g => new ChartData {
				Sum = g.Sum(s => s.Sum),
				Part = g.Sum(s => s.Sum) / totalCount * 100,
				SupplierName = g.Key,
				SupplierId = g.First().hFirmCode
			}).OrderByDescending(c=>c.Sum).ToArray();
		}

		private static string NormalizeTerm(string term)
		{
			return $@"%{
					term.Replace(" ", "%").Replace(",", "%").Replace(".", "%").Replace("-", "%")
						.Replace("(", "%").Replace(")", "%").Replace("[", "%").Replace("]", "%")
						.Replace("/", "%").Replace("\\", "%")
				}%";
		}

		public static List<uint> GetUIntListForString(string selectedList)
		{
			var tempList = selectedList?.Split(',').Select(s => {
				uint val = 0;
				if (uint.TryParse(s, out val))
					return val;
				return (uint?) null;
			}).ToList() ?? new List<uint?>();
			return tempList.Where(s => s != null).Select(s => s.Value).ToList();
		}

		// Преобразует величину процента в простую кодировку GoogleChartAPI
		private static char EncodePercent(double percent)
		{
			return simpleEncoding[Convert.ToInt16(Math.Round(percent / 100 * (simpleEncoding.Length - 1)))];
		}

		// На основе данных для отчета "Доли поставщиков" строит URL для GoogleChartAPI (ссылка на рисунок диаграммы)
		// Массив ChartData должен быть отсортирован по убыванию.
		public static string GetGooglePieChartUrl(ChartData[] reportItems, double accuracy = 5, bool showPercents = false,
			int width = 500, int height = 200)
		{
			var data = string.Empty;
			var titles = string.Empty;
			double FirstSupPartsSum = 0;

			var length = Math.Min(3, reportItems.Count());

			for (var i = 0; i < length; i++) {
				FirstSupPartsSum += reportItems[i].Part;
				data += EncodePercent(reportItems[i].Part);
				titles += reportItems[i].SupplierName + (showPercents ? $" - {Math.Round(reportItems[i].Part, 2)}%" : "") + '|';
			}

			if (reportItems.Length == 3)
				titles = titles.TrimEnd('|');
			else if (reportItems.Length == 4) {
				data += EncodePercent(reportItems[3].Part);
				titles += reportItems[3].SupplierName + (showPercents ? $" - {Math.Round(reportItems[3].Part, 2)}%" : "");
			}
			else {
				var i = 3;
				while (i < reportItems.Length && reportItems[i].Part >= accuracy) {
					FirstSupPartsSum += reportItems[i].Part;
					data += EncodePercent(reportItems[i].Part);
					titles += reportItems[i].SupplierName + (showPercents ? $" - {Math.Round(reportItems[i].Part, 2)}%" : "") + '|';
					i++;
				}

				if (reportItems.Length > i) {
					data += EncodePercent(100 - FirstSupPartsSum);
					titles += "Другие" + (showPercents ? $" - {Math.Round(100 - FirstSupPartsSum, 2)}%" : "");
				}
				else
					titles = titles.TrimEnd('|');
			}

			var result =
				Uri.EscapeUriString(
					string.Format(
						"GetChartPng?cht=p&chd=s:{0}&chs={2}x{3}&chl={1}", data, titles, width, height));

			return result;
		}

		public class TotalNumberData
		{
			public double DoubleNumber;
			public uint UIntNumber;
		}

		//Entity данных отчета "Доли поставщиков"
		public class ChartData
		{
			public double Part;
			public double Sum;
			public uint SupplierId;
			public string SupplierName;
		}

		public static List<MemberCategory> GetMemberCategories(ISession dbSession, string memberCategoriesIdList = "")
		{
			var sql = $@"SELECT mc.Id, mc.Name
				FROM customers.membercategories as mc
				{(!String.IsNullOrEmpty(memberCategoriesIdList) ? $"WHERE mc.Id IN ({memberCategoriesIdList}) " : "")} 
				ORDER BY Name";
			return dbSession.Connection.Query<MemberCategory>(sql).ToList();
		}

		public static List<Region> GetRegions(ISession dbSession, Association association, string regionIdList = "")
		{
		    var sql = $@"SELECT r.RegionCode As Id, r.Region As Name
					FROM farm.regions r
					LEFT OUTER JOIN customers.associationregions ar ON ar.RegionId = r.RegionCode and ar.AssociationId = @id
					WHERE r.RegionCode NOT IN (0, {Models.Region.INFOROOM_CODE})
					{(!String.IsNullOrEmpty(regionIdList) ? $"AND r.RegionCode IN ({regionIdList}) " : "")} 
					ORDER BY Name";
				return dbSession.Connection.Query<Region>(sql, new { id = association.Id }).ToList();
		}

		public static List<Client> GetClients(ISession dbSession, ReportFilter filter)
		{

			var addFilter = String.Empty;
			var parameters = new Dictionary<string, object>() {
				{"id", filter.Association.Id}
			};

			var promotionIdList = filter.promotionIdList;

			if(filter.assortmentType == 0 && filter.eventId.HasValue && filter.eventId != 0 && !String.IsNullOrEmpty(filter.promotionIdList))
			{

				// Фильтр подписки на акцию: 0 - все, 1 - подписанные, 2 - подписанные Производителем и Ассоциацией,
				// 3 - подписанные Производителем, Ассоциацией и Аптекой, 4 - неподписанные
				switch(filter.subscribeEventStatusId)
				{
					case 0:
						promotionIdList = String.Empty;
						break;
					case 1:
						addFilter = @"AND ps.byAssociation = 1 
							AND ps.byAdmin = 1 
							AND ps.byOwner = 1 
							AND ps.byClient = 1 ";
						break;
					case 2:
						addFilter = @"AND ps.byAssociation = 1 
							AND ps.byOwner = 1 ";
						break;
					case 3:
						addFilter = @"AND ps.byAssociation = 1 
							AND ps.byOwner = 1 
							AND ps.byClient = 1 ";
						break;
					case 4:
						addFilter = @"OR ps.Id IS NULL
							OR (ps.byAssociation = 0
							AND ps.byOwner = 0
							AND ps.byClient = 0
							AND ps.byAdmin = 0) ";
						break;
					default: break;
				}

				if (!String.IsNullOrEmpty(promotionIdList)) {
					addFilter = $"AND (ps.PromotionId IN @promotionIdList {addFilter}) ";
					parameters.Add("promotionIdList", Parse(promotionIdList)); 
				}
				else
					addFilter = $"AND (1 = 1 {addFilter}) ";
			}

			if(!string.IsNullOrEmpty(filter.regionIdList))
			{
				addFilter += "AND cs.RegionCode IN @regionIdList ";
				parameters.Add("regionIdList", Parse(filter.regionIdList));
			}

			if(!string.IsNullOrEmpty(filter.memberCategoriesIdList))
			{
				addFilter += "AND mc.Id IN @memberCategoriesIdList ";
				parameters.Add("memberCategoriesIdList", Parse(filter.memberCategoriesIdList));
			}

			if(!string.IsNullOrEmpty(filter.clientIdList))
			{
				addFilter += "AND cs.Id IN @clientIdList ";
				parameters.Add("clientIdList", Parse(filter.clientIdList));
			}

			var sql = $@"SELECT cs.Id, cs.Name, cs.FullName
				FROM customers.clients cs
				JOIN customers.promotionmembers pm ON cs.Id = pm.ClientId
				JOIN customers.associations ass ON pm.AssociationId = ass.Id 
				JOIN customers.membercategories mc ON mc.Id = pm.MemberCategoryId
				LEFT JOIN customers.promotionsubscribes ps ON pm.id = ps.MemberId 
				WHERE ass.Id = @id
				{addFilter}
				GROUP BY cs.Id, cs.Name, cs.FullName
				ORDER BY Name";
			return dbSession.Connection.Query<Client>(sql, parameters).ToList();
		}

		public static List<ReportFilter.reactSelectItem> GetEvents(ISession dbSession, ReportFilter filter)
		{
			var sql = $@"SELECT me.Id as 'Id', me.Name as 'Name'
				FROM customers.Associations AS a
				JOIN customers.AssociationRegions AS ar ON ar.AssociationId = a.Id
				JOIN customers.MarketingEvents AS me ON me.AssociationId = a.Id
				WHERE a.Id = @associationId
				{(filter.eventId.HasValue && filter.eventId != 0 ? "AND me.Id = @eventId" : "")}
				GROUP BY Id
				ORDER BY Name";
			var result = dbSession.Connection.Query<ReportFilter.reactSelectItem>(sql, new { associationId = filter.Association.Id, eventId = filter.eventId }).ToList();
			return result;
		}

		public static List<ReportFilter.reactSelectItem> GetPromotions(ISession dbSession, uint? eventId, string promotionIdList = "")
		{
			if(!eventId.HasValue || eventId == 0) return new List<ReportFilter.reactSelectItem>();

			var sql = $@" SELECT pp.Id as 'Id', pp.Name as 'Name'
				FROM customers.ProducerPromotions AS pp
				LEFT JOIN customers.MarketingEvents AS me ON me.Id = pp.MarketingEventId
				WHERE me.Id = @eventId
				{(!String.IsNullOrEmpty(promotionIdList) ? $"OR pp.Id IN ({promotionIdList}) " : "")}
  			GROUP BY Id
				ORDER BY Name;";

			return dbSession.Connection.Query<ReportFilter.reactSelectItem>(sql, new { eventId }).ToList();
		}
		public static List<ReportFilter.reactSelectItem> GetProductsByAccion(ISession dbSession, ReportFilter filter, string value = "")
		{
			value = value?.Trim() ?? "";
			value = NormalizeTerm(value);
			// первая часть запроса выборка товаров участвующих в акции
			// вторая часть запроса - выборка товаров из прайс-листов
			var sql = $@"SELECT pr.Id as 'Id', concat_ws(' ', c.Name, pr.Properties) as 'Name'
  			FROM customers.PromotionProducts AS ppt
				JOIN catalogs.Products pr ON pr.Id = ppt.ProductId
				JOIN catalogs.catalog AS c ON c.Id = pr.CatalogId
				JOIN customers.ProducerPromotions AS pp ON pp.Id = ppt.PromotionId
				INNER JOIN customers.MarketingEvents AS me ON me.Id = pp.MarketingEventId
				INNER JOIN customers.promoterproducers AS mep ON mep.MarketingEventId = pp.MarketingEventId
				INNER JOIN catalogs.assortment AS ast ON ast.CatalogId = c.Id AND ast.ProducerId = mep.ProducerId
				WHERE 1 = 1
				AND pp.SelectMethod = 0
				{(!String.IsNullOrEmpty(filter.promotionIdList) ? $@"AND pp.Id in ({filter.promotionIdList})" : "")}
				AND me.Id = @eventId
				AND c.Hidden = 0
				AND pr.Hidden = 0
				AND pr.Custom = 0
				AND (c.Name LIKE @value {(!String.IsNullOrEmpty(filter.catalogIdList) ? $@"OR pr.Id in ({filter.catalogIdList})" : "")})
  			GROUP BY Id

				UNION

				SELECT pr.Id, concat_ws('', c0.Code, '; ', c.Name, ' (', cn.Name, ' ', cf.Form , ')') Name
				FROM Farm.Core0 c0
				JOIN Catalogs.Products pr on c0.ProductId = pr.Id
				JOIN Catalogs.Catalog c on pr.CatalogId = c.Id
				JOIN Catalogs.CatalogNames cn on c.NameId = cn.Id
				JOIN Catalogs.CatalogForms cf on c.FormId = cf.Id
				LEFT JOIN Catalogs.Producers ppr on c0.CodeFirmCr = ppr.Id
				LEFT JOIN Farm.SynonymFirmCr sfc on sfc.SynonymFirmCrCode = c0.SynonymFirmCrCode
				JOIN customers.ProducerPromotions AS pp 
				WHERE c0.PriceCode = pp.PriceId
				AND pp.MarketingEventId = @eventId
				{(!String.IsNullOrEmpty(filter.promotionIdList) ? $@"AND pp.Id in ({filter.promotionIdList})" : "")}
				AND pp.SelectMethod = 1
				AND pp.priceId IS NOT NULL
				AND c.Hidden = 0
				AND pr.Hidden = 0
				AND pr.Custom = 0
				AND (c.Name LIKE @value
				OR cn.Name LiKE @value
				OR c0.Code LIKE @value
				{(!String.IsNullOrEmpty(filter.catalogIdList) ? $@"OR pr.Id in ({filter.catalogIdList})" : "")})
				GROUP BY pr.Id

				ORDER BY Name";
			return dbSession.Connection.Query<ReportFilter.reactSelectItem>(sql, new { eventId = filter.eventId, value }).ToList();
		}

		public static List<ReportFilter.reactSelectItem> GetProducts(ISession dbSession, string productIdList,
			string value = "", uint limit = 500)
		{
			value = value?.Trim() ?? "";
			value = NormalizeTerm(value);
			var sql = String.Empty;
			if (!String.IsNullOrEmpty(productIdList)) {
				sql = $@"SELECT p.Id as 'Id', concat_ws(' ', ct.Name, p.Properties) as 'Name'
					FROM Catalogs.Products p
					JOIN catalogs.Catalog as ct ON ct.Id = p.CatalogId
					WHERE ct.Hidden = 0
					  AND p.Hidden = 0
					  AND p.Custom = 0
					  AND  p.Id in ({productIdList})
 				    GROUP BY Id
					UNION";
			}

			sql = $@"{sql}
				SELECT s.Id, s.Name
				FROM (SELECT p.Id as 'Id', concat_ws(' ', ct.Name, p.Properties) as 'Name'
					FROM Catalogs.Products p
					JOIN catalogs.Catalog as ct ON ct.Id = p.CatalogId
					WHERE ct.Hidden = 0
					AND p.Hidden = 0
					AND p.Custom = 0
					AND ct.Name LIKE @value
      				GROUP BY Id
					LIMIT {limit}) s
				ORDER BY Name";
			return dbSession.Connection.Query<ReportFilter.reactSelectItem>(sql, new {value}).ToList();
		}

		public static List<ReportFilter.reactSelectItem> GetAddresses(ISession dbSession, ReportFilter filter)
		{
			var sql = $@"SELECT ad.Id AS Id, CONCAT(ad.Address, ' (', cs.Name, ') ')  AS Name
				FROM customers.Addresses ad
				JOIN customers.clients cs ON ad.ClientId = cs.Id
				WHERE ad.ClientId IN ({(String.IsNullOrEmpty(filter.clientIdList) ? "0" : filter.clientIdList)})
				{(!String.IsNullOrEmpty(filter.addressIdList) ? $"AND ad.Id IN ({filter.addressIdList}) " : "")} 
				ORDER BY Name";
			return dbSession.Connection.Query<ReportFilter.reactSelectItem>(sql).ToList();
		}

		public static List<ReportFilter.reactSelectItem> GetSuppliers(ISession dbSession, ReportFilter filter)
		{
			var sql = $@"SELECT s.Id as 'Id', s.Name as 'Name'
 				FROM Customers.Clients cl
				JOIN farm.Regions regs ON (cl.RegionCode & regs.RegionCode) > 0
				JOIN Customers.Suppliers s ON (s.RegionMask & regs.RegionCode) > 0
				WHERE cl.Id IN ({(String.IsNullOrEmpty(filter.clientIdList) ? "0" : filter.clientIdList)})
				AND s.Disabled = 0
				AND s.IsVirtual = 0
				GROUP BY Id
				ORDER BY Name";
			return dbSession.Connection.Query<ReportFilter.reactSelectItem>(sql).ToList();
		}


		public static List<ReportFilter.reactSelectItem> GetProducersByAction(ISession dbSession, ReportFilter filter, string value = "")
		{
			value = value?.Trim() ?? "";
			value = NormalizeTerm(value);

			// первая часть запроса - выборка произодителей акции
			// вторая часть - выборка поставщиков из прайса
			var sql = $@"
				SELECT pr.Id as 'Id', pr.Name as 'Name'
				FROM catalogs.Producers as pr
				INNER JOIN customers.promoterproducers AS mep ON mep.ProducerId = pr.Id
				INNER JOIN customers.MarketingEvents as me  ON me.Id = mep.MarketingEventId
				JOIN customers.ProducerPromotions AS pp 
				WHERE 1 = 1 
				AND me.Id = @eventId
				AND me.Id = pp.MarketingEventId
				{(!String.IsNullOrEmpty(filter.promotionIdList) ? $@"AND pp.Id in ({filter.promotionIdList})" : "")}
				AND (pr.Name LIKE @value
				  {(!String.IsNullOrEmpty(filter.producerIdList) ? $@"OR pr.Id IN ({filter.producerIdList})" : "")}
				)
				AND pp.SelectMethod = 0
				GROUP BY Id

				UNION

				SELECT DISTINCT ppr.id, CONCAT_WS('', sfc.Synonym, ' (', ppr.Name, ')') Name       
				FROM Farm.Core0 c0
				JOIN Catalogs.Products pr on c0.ProductId = pr.Id
				JOIN Catalogs.Catalog c on pr.CatalogId = c.Id
				JOIN Catalogs.CatalogNames cn on c.NameId = cn.Id
				JOIN Catalogs.CatalogForms cf on c.FormId = cf.Id
				LEFT JOIN Catalogs.Producers ppr on c0.CodeFirmCr = ppr.Id
				LEFT JOIN Farm.SynonymFirmCr sfc on sfc.SynonymFirmCrCode = c0.SynonymFirmCrCode
				JOIN customers.ProducerPromotions AS pp 
				WHERE c0.PriceCode = pp.PriceId
				AND pp.MarketingEventId = @eventId
				{(!String.IsNullOrEmpty(filter.promotionIdList) ? $@"AND pp.Id IN ({filter.promotionIdList})" : "")}
				AND (ppr.Name LIKE @value
				  OR sfc.Synonym LIKE @value
				  {(!String.IsNullOrEmpty(filter.producerIdList) ? $@"OR ppr.Id IN ({filter.producerIdList})" : "")}
				)
				AND pp.SelectMethod = 1
				AND pp.priceId IS NOT NULL
				AND c.Hidden = 0
				AND pr.Hidden = 0
				AND pr.Custom = 0
				AND ppr.id IS NOT NULL
				GROUP BY Id
				ORDER BY Name";
			return dbSession.Connection.Query<ReportFilter.reactSelectItem>(sql, new { eventId = filter.eventId, value }).ToList();
		}

		public static List<ReportFilter.reactSelectItem> GetProducers(ISession dbSession, string value = "", uint limit = 300)
		{
			value = value?.Trim() ?? "";
			value = NormalizeTerm(value);

			var sql = $@"SELECT s.Id, s.Name
				FROM (SELECT pr.Id as 'Id', pr.Name as 'Name'
					FROM catalogs.Producers as pr
					WHERE pr.Name LIKE @value
					LIMIT {limit}) s
				ORDER BY Name";

			return dbSession.Connection.Query<ReportFilter.reactSelectItem>(sql, new { value }).ToList();
		}
	}
}