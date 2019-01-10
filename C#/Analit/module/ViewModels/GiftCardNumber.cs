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

namespace AnalitF.Net.Client.ViewModels.Dialogs
{
	public class GiftCardNumber : SlimScreen, ICancelable, IBarcodeAware
	{
		public GiftCardNumber()
		{
			WasCancelled = true;
		}

		public string Number { get; set; }
		public bool WasCancelled { get; private set; }

		void IBarcodeAware.BarcodeScanned(string barcode)
		{
			ViewModelHelper.ProcessResult(BarcodeScanned(barcode));
		}

		public override async Task Load()
		{
			if (Shell != null)
				Shell.BarcodeDialog = this;
		}

		public async Task BarcodeScanned(string barcode)
		{
			Number = barcode;
			await OK();
		}

		public override void TryClose()
		{
			if (Shell != null)
				Shell.BarcodeDialog = null;
			base.TryClose();
		}

		public async Task OK()
		{
			if (string.IsNullOrWhiteSpace(Number))	{
				Manager.Error("Не указан номер/штрих-код сертификата/карты");
				return;
			}

			if (!Number.All(Char.IsDigit)) {
				Manager.Error("Поле 'Номер карты' может содержать только цифры");
				return;
			}

			WasCancelled = false;
			TryClose();
		}
	}
}
