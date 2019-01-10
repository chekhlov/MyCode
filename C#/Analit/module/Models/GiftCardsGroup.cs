using System;
using System.Collections.Generic;
using System.Linq;
using AnalitF.Net.Client.Helpers;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using AnalitF.Net.Client.Config.NHibernate;
using Common.Tools;
using NHibernate;
using NHibernate.Linq;
using AnalitF.Net.Client.Models.Print;
using System.Globalization;
using AnalitF.Net.Client.Models.Contract;

namespace AnalitF.Net.Client.Models.Inventory
{

	public enum GiftCardValitityType
	{
		[Description("Дней после продажи")]  DaysAfterSale = 0,
		[Description("До даты")] ToDate,
		[Description("Без ограничения по сроку действия")] Unlimited
	}

	public class GiftCardsGroupAddresses
	{
	}

	public class GiftCardsGroup : PropertyChangedBase, IDataErrorInfo2
	{
		public GiftCardsGroup()
		{
			IsNew = true;
			Status = DocStatus.NotPosted;
			CreateDate = DateTime.Now;
			Cards = new List<GiftCard>();
			Addresses = new List<Address>();
			ValitityType = GiftCardValitityType.Unlimited;
		}

		public virtual DocStatus Status { get; set; }

		[Ignore]
		[Style(Description = "Непроведенный документ")]
		public virtual bool IsNotPosted => Status == DocStatus.NotPosted;

		public virtual Guid Id { get; set; }

		public virtual string Name { get; set; }
		public virtual DateTime CreateDate { get; set; }
		public virtual GiftCardValitityType ValitityType { get; set; }
		public virtual DateTime? ValidDate { get; set; }
		public virtual long ValidDays { get; set; }
		public virtual decimal Nominal { get; set; }
		public virtual bool UseBarcode { get; set; }

		public virtual IList<Address> Addresses { get; set; }
		[NoMagic]
		public virtual bool IsNew { get; set; }
		[NoMagic]
		public virtual bool Hidden { get; set; }

		public virtual string AddressName => Addresses?.Select(x => x.Name).Implode(", ") ?? "";

		public virtual string ValidTo {
			get {
				switch (ValitityType) {
					case GiftCardValitityType.DaysAfterSale: return $"{ValidDays} дней после продажи";
					case GiftCardValitityType.ToDate: return $"Действительно до {ValidDate}";
					case GiftCardValitityType.Unlimited: return $"Без ограничения по сроку действия";
				}
				return "";
			}
		}
		
		public virtual IList<GiftCard> Cards { get; set; }

		public virtual string[] FieldsForValidate => new[] { nameof(Name), nameof(Nominal) };
		public virtual string Error { get; }
		public virtual string this[string columnName]
		{
			get
			{
				if (columnName == nameof(Name) && Name == null)
					return "Не указано наименование сертификата/карты";
				if (columnName == nameof(Nominal) && Nominal == 0)
					return "Не указан номинал сертификата/карты";
			
				return null;
			}
		}
		public virtual void Save(ISession session)
		{
			IsNew = true;
			if (Id == Guid.Empty) session.Save(this);

			foreach (var line in Cards) 
				line.Save(session);
			
			session.Update(this);
		}

		public virtual void Delete(ISession session)
		{
			Hidden = true;
			if (Id == Guid.Empty) return;

			foreach (var line in Cards) 
				line.Delete(session);

			session.Update(this);
		}

		public virtual void Post(ISession session)
		{
			if (!Cards.Any())
				throw new EndUserError("Пустой документ не может быть проведен");

			Status = DocStatus.Posted;
			Save(session);
			session.Flush();
		}
	}
}
