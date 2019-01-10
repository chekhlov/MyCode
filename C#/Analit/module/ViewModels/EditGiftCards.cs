using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NHibernate;
using AnalitF.Net.Client.Models.Inventory;
using AnalitF.Net.Client.Helpers;
using AnalitF.Net.Client.Models;
using Caliburn.Micro;
using AnalitF.Net.Client.Models.Results;
using AnalitF.Net.Client.ViewModels.Parts;
using Common.Tools;
using ReactiveUI;
using AnalitF.Net.Client.Controls;
using static AnalitF.Net.Client.Models.Inventory.GiftCard;
using AnalitF.Net.Client.ViewModels.Dialogs;
using NHibernate.Linq;

namespace AnalitF.Net.Client.ViewModels.Inventory
{
	public class EditGiftCards : SlimScreen
	{
		private EditGiftCards()
		{
			AddressSelector = new AddressSelector(this, "Все адреса");
			Lines = new ReactiveCollection<GiftCard>();
		}

		public EditGiftCards(GiftCardsGroup doc) : this()
		{
			Doc.Value = doc;
			DisplayName = "Создание группы подарочных сертификатов/карт";
			IsCreate = true;
		}

		public EditGiftCards(Guid id) : this()
		{
			Doc.Value = new GiftCardsGroup() { Id = id };
			DisplayName = "Редактирование группы подарочных сертификатов/карт";
			IsCreate = false;
		}

		public NotifyValue<GiftCardsGroup> Doc { get; set; }
		public ReactiveCollection<GiftCard> Lines { get; set; }
		public NotifyValue<GiftCard> CurrentLine { get; set; }
		public NotifyValue<bool> CanPost { get; set; }
		public NotifyValue<bool> CanSaveAndClose { get; set; }

		public NotifyValue<List<string>> ValitityTypes { get; set; }
		public NotifyValue<string> CurrentValitityType { get; set; }

		public NotifyValue<bool> ValidDateVisible { get; set; }
		public NotifyValue<bool> ValidDaysVisible { get; set; }

		public NotifyValue<bool> IsNumberUse { get; set; }
		public NotifyValue<bool> IsBarcodeUse { get; set; }

		public NotifyValue<string> AddressList { get; set; }

		public AddressSelector AddressSelector { get; set; }

		public bool IsCreate { get; set; }

		public NotifyValue<bool> IsChanged { get; set; }

		public override async Task Load()
		{
			if (!IsCreate)
				Doc.Value = await Query(s => {
					var gc = s.Query<GiftCardsGroup>().Where(x => x.Id == Doc.Value.Id).Fetch(x => x.Cards).ToList().First();
					return gc;
				});

			IsChanged.Value = IsCreate;

			Lines.Clear();
			Lines.AddRange(Doc.Value.Cards);

			Doc.Value.PropertyChanged += (_, prop) =>
			{
				IsChanged.Value = true;
			};

			IsChanged.Throttle(TimeSpan.FromMilliseconds(30), Env.UiScheduler)
				.Subscribe(_ => {
					CanPost.Value = Doc.Value.Status == DocStatus.NotPosted;
					CanSaveAndClose.Value = IsChanged.Value && Doc.Value.Status == DocStatus.NotPosted;
				});

			AddressSelector.Init();
			AddressSelector.All.Value = true;
			AddressSelector.Addresses.Each(x => {
				x.IsSelected = !Doc.Value.Addresses.Any() || Doc.Value.Addresses.Contains(x.Item);
			});
			AddressSelector.FilterChanged.Subscribe(x => AddressList.Value = AddressSelector.GetActiveFilter().Select(a => a.Name).ToList().Implode("; "));
			AddressSelector.All.Subscribe(x => AddressList.Value = x ? AddressSelector.GetActiveFilter().Select(a => a.Name).ToList().Implode("; ") : Address.Name );

			ValitityTypes.Value = DescriptionHelper.GetDescriptions<GiftCardValitityType>().Select(x => x.Name).ToList();
			CurrentValitityType.Value = DescriptionHelper.GetDescriptions<GiftCardValitityType>().Where(s => s.Value == Doc.Value.ValitityType).Select(s => s.Name).First();
			CurrentValitityType.Subscribe(x => {
				Doc.Value.ValitityType = DescriptionHelper.GetDescriptions<GiftCardValitityType>().Where(s => s.Name == x).Select(s => s.Value).First();
				ValidDaysVisible.Value = Doc.Value.ValitityType == GiftCardValitityType.DaysAfterSale;
				ValidDateVisible.Value = Doc.Value.ValitityType == GiftCardValitityType.ToDate;
			});

			IsNumberUse.Value = !Doc.Value.UseBarcode;
			IsBarcodeUse.Value = Doc.Value.UseBarcode;

			IsNumberUse.Subscribe(x => {
				if (x) {
					Doc.Value.UseBarcode = false;
					IsBarcodeUse.Value = false;
				} 
			});
			IsBarcodeUse.Subscribe(x => {
				if (x) {
					Doc.Value.UseBarcode = true;
					IsNumberUse.Value = false;
				}
			});

		}

		public async Task SaveAndClose()
		{
			if (!IsValid(Doc.Value)) return;

			await Save();
		}

		public async Task Save()
		{
			if (!IsValid(Doc.Value)) return;

			Doc.Value.Addresses.Clear();
			if (AddressSelector.AllVisible)
				Doc.Value.Addresses = AddressSelector.GetActiveFilter();
			else
				Doc.Value.Addresses = new List<Address>() { Address };

			await Session(s => Doc.Value.Save(s));
			IsChanged.Value = false;
			Bus.SendMessage(nameof(GiftCardsGroup), "db");
		}

		public async Task Post()
		{
			if (!IsValid(Doc.Value) || !Confirm("Провести документ?"))	return;

			await Session(s => Doc.Value.Post(s));
			IsChanged.Value = false; 

			Bus.SendMessage(nameof(GiftCardsGroup), "db");
		}

		public async Task Close()
		{
			if (Doc.Value.Status == DocStatus.NotPosted && IsChanged.Value
					&& Confirm("Сохранить изменения?"))
				await Save();

			TryClose();
		}

		public IEnumerable<IResult> Add()
		{
			if (Doc.Value.Status == DocStatus.Posted)
				throw new EndUserError("Документ проведен и не подлежит изменению.");

			while (true) {
				var dlg = new GiftCardNumber();
				yield return new DialogResult(dlg);

				if (dlg.WasCancelled) yield break;
				AddCard(dlg.Number);
			}
		}

		private async Task AddCard(string number)
		{
			var useBarcode = Doc.Value.UseBarcode;
			var exist = await Query(s => 	s.Query<GiftCard>()
				.Where(x => x.Barcode == number	&& x.GiftCardsGroup.UseBarcode == useBarcode
						&& (useBarcode || x.GiftCardsGroup.Id == Doc.Value.Id))
				.ToList().Any());

			exist |= Lines.Any(x => x.Barcode == number);

			if (exist)
			{
				Manager.Warning(Doc.Value.UseBarcode ? "Карта с таким штрих-кодом уже есть" : "Карта с таким номером уже есть");
				return;
			}

			var giftCard = new GiftCard(Doc.Value, number);
			Lines.Add(giftCard);
			Doc.Value.Cards.Add(giftCard);
			IsChanged.Value = true;
		}

		public async Task Delete()
		{
			if (!Confirm("Удалить выбранный штрих-код/номер?")) return;

			if (Doc.Value.Status == DocStatus.Posted)
				throw new EndUserError("Документ проведен и не подлежит изменению.");
			var giftCard = CurrentLine.Value;
			Lines.Remove(giftCard);

			if (giftCard.Id == Guid.Empty)
				Doc.Value.Cards.Remove(giftCard);
			else
				await Session(s => giftCard.Delete(s));
			IsChanged.Value = true;
		}
	}
}
