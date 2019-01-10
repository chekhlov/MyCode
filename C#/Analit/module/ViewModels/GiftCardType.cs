using AnalitF.Net.Client.Models;
using AnalitF.Net.Client.Models.Results;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AnalitF.Net.Client.Helpers;
using System.Collections.Generic;
using System.Linq;
using AnalitF.Net.Client.Models.Contract;
using Dapper;
using AnalitF.Net.Client.ViewModels.Inventory;
using AnalitF.Net.Client.Config.Caliburn;
using AnalitF.Net.Client.Models.Inventory;

namespace AnalitF.Net.Client.ViewModels.Dialogs
{
	public class GiftCardType : SlimScreen, ICancelable
	{

		public GiftCardType()
		{
			DisplayName = "Выберите подарочный сертификат/карту";
		}

		public GiftCardType(List<GiftCard> cards) : this()
		{
			Cards.Value = cards;
			CurrentCard.Value = cards?.Count > 0 ? cards[0] : null ;
			WasCancelled = true;
		}

		public NotifyValue<List<GiftCard>> Cards { get; set; }
		public NotifyValue<GiftCard> CurrentCard { get; set; }

		public bool WasCancelled { get; private set; }

		public async Task OK()
		{
		
			WasCancelled = false;
			TryClose();
		}
	}
}
