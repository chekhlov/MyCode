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

	public class GiftCard : BaseNotify
	{
		public enum GiftCardState
		{
			[Description("Непроведена")] Created,
			[Description("")] Free,
			[Description("Продана")] Sales,
			[Description("Использована")] Used
		}

		public virtual Guid Id { get; set; }

		public virtual GiftCardsGroup GiftCardsGroup { get; set; }

		public virtual string Barcode { get; set; }

		public virtual bool Valid(DateTime? now = null)
		{
			if (!now.HasValue) now = DateTime.Now.Date;

			return (GiftCardsGroup.ValitityType == GiftCardValitityType.Unlimited)
				|| (GiftCardsGroup.ValitityType == GiftCardValitityType.ToDate && now <= GiftCardsGroup.ValidDate)
				|| (GiftCardsGroup.ValitityType == GiftCardValitityType.DaysAfterSale && SaleDate.HasValue && now <= SaleDate.Value.AddDays(GiftCardsGroup.ValidDays));

		}

		[Ignore, Style("Exp", "Period", Description = "Истекший срок действия карты")]
		public virtual bool IsOverdue => !Valid();

		public virtual Guid? SalesCheckGuidId { get; set; }
		public virtual string SaleCheck { get; set; }
		public virtual DateTime? SaleDate { get; set; }

		public virtual Guid? PaymentCheckGuidId { get; set; }
		public virtual string PaymentCheck { get; set; }
		public virtual DateTime? PaymentDate { get; set; }
		public virtual decimal PaymentSum { get; set; }
		public virtual bool IsNew { get; set; }
		public virtual bool Hidden { get; set; }

		[Ignore, Style(Context = "GiftCard", Description = "Сертификат использован")]
		public virtual bool IsUsed => State == GiftCardState.Used;

		[Ignore, Style(Context = "GiftCard", Description = "Сертификат продан")]
		public virtual bool IsSaled => State == GiftCardState.Sales;

		[Ignore]
		public virtual GiftCardState State {
			get {
				return PaymentCheckGuidId.HasValue
					? GiftCardState.Used
					: SalesCheckGuidId.HasValue
						? GiftCardState.Sales
							: (GiftCardsGroup.Status == DocStatus.Posted)
								? GiftCardState.Free
								: GiftCardState.Created;
			}
		}

		public GiftCard()
		{
		}

		public GiftCard(GiftCardsGroup doc, string barcode)
		{
			IsNew = true;
			GiftCardsGroup = doc;
			Barcode = barcode;
		}
		public virtual void Save(ISession session)
		{
			if (GiftCardsGroup == null)
				throw new EndUserError("Неуказана группа подарочных сертификатов/карт");
			IsNew = true;
			if (Id == Guid.Empty) {
				if (!Hidden) session.Save(this);
			}
			else
				session.Update(this);
		}

		public virtual void Delete(ISession session)
		{
			Hidden = true;
		}

		public virtual void Sale(ISession session, Check check)
		{
			SalesCheckGuidId = check.Id;
			SaleDate = check.Date;
			SaleCheck = check.NumberDoc + " от " + check.Date.ToShortDateString();
			Save(session);
		}
		public virtual void Apply(ISession session, Check check)
		{
			PaymentCheckGuidId = check.Id;
			PaymentDate = check.Date;
			PaymentCheck = check.NumberDoc + " от " + check.Date.ToShortDateString();
			PaymentSum = check.GiftCardSum;
			Save(session);
		}
	}
}
