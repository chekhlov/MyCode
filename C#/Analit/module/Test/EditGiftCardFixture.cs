using System.Linq;
using AnalitF.Net.Client.Models;
using AnalitF.Net.Client.Test.TestHelpers;
using NUnit.Framework;
using AnalitF.Net.Client.ViewModels.Inventory;
using AnalitF.Net.Client.Models.Results;
using System.IO;
using AnalitF.Net.Client.ViewModels.Dialogs;
using AnalitF.Net.Client.Models.Inventory;
using NHibernate.Linq;
using System.Collections.Generic;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using AnalitF.Net.Client.Helpers;
using AnalitF.Net.Client.ViewModels;
using Caliburn.Micro;
using Common.NHibernate;
using Common.Tools;
using Common.Tools.Calendar;
using Dapper;
using ReactiveUI.Testing;

namespace AnalitF.Net.Client.Test.Integration.View
{
	[TestFixture]
	public class EditGiftCardsFixture : ViewModelFixture
	{
		private EditGiftCards model;

		[SetUp]
		public void Setup()
		{
			session.Connection.Execute("delete from GiftCards");
			session.Connection.Execute("delete from GiftCardsGroupAddresses");
			session.Connection.Execute("delete from GiftCardsGroups");

			var group = CreateGiftCardsGroup("Подарочный сертификат 1000 руб.", 1000.00m);
			AddGiftCard(group, "11111111");
			AddGiftCard(group, "22222222");
			session.Flush();

			if (session.Transaction.IsActive)
				session.Transaction.Commit();

			scheduler.Start();
			model = Open(new EditGiftCards(group.Id));
			model.IsChanged.Value = true;
			model.CanSaveAndClose.Value = true;
			scheduler.AdvanceByMs(2000);
		}

		private GiftCardsGroup CreateGiftCardsGroup(string cardName, decimal nominal)
		{
			var group = new GiftCardsGroup()
			{
				Name = cardName,
				Nominal = nominal
			};

			group.Save(session);
			return group;
		}

		private GiftCard AddGiftCard(GiftCardsGroup group, string barcode)
		{
			var card = new GiftCard()
			{
				GiftCardsGroup = group,
				Barcode = barcode
			};

			group.Cards.Add(card);
			group.Save(session);
			return card;
		}

		[Test]
		public void LoadData()
		{
			Assert.AreEqual(model.Doc.Value.Name, "Подарочный сертификат 1000 руб.");
			Assert.AreEqual(model.Doc.Value.Nominal, 1000.0m);

			var cards = model.Lines;
			Assert.AreEqual(cards.Count, 2);
			Assert.AreEqual(cards[0].Barcode, "11111111");
			Assert.AreEqual(cards[1].Barcode, "22222222");

			Assert.AreEqual(model.Addresses.Length, 1);
			Assert.AreEqual(model.Addresses[0].Name, address.Name);
			Assert.That(model.AddressList.Value, Does.Contain(this.address.Name));
			// Проверяем корректность отображения (доступности) кнопок
			Assert.IsTrue(model.CanSaveAndClose.Value);
			Assert.IsTrue(model.CanPost.Value);
		}
		[Test]
		public void ValidSwitch()
		{
			var items = model.ValitityTypes.Value;
			Assert.AreEqual(items.Count, 3);

			model.CurrentValitityType.Value = items[0];
			Assert.AreEqual(model.Doc.Value.ValitityType, GiftCardValitityType.DaysAfterSale);
			Assert.AreEqual(model.CurrentValitityType.Value, "Дней после продажи");
			Assert.IsTrue(model.ValidDaysVisible.HasValue);
			Assert.IsFalse(model.ValidDateVisible.HasValue);

			model.CurrentValitityType.Value = items[1];
			Assert.AreEqual(model.Doc.Value.ValitityType, GiftCardValitityType.ToDate);
			Assert.AreEqual(model.CurrentValitityType.Value, "До даты");
			Assert.IsTrue(model.ValidDateVisible.HasValue);
			Assert.IsFalse(model.ValidDaysVisible.HasValue);

			model.CurrentValitityType.Value = items[2];
			Assert.AreEqual(model.Doc.Value.ValitityType, GiftCardValitityType.Unlimited);
			Assert.AreEqual(model.CurrentValitityType.Value, "Без ограничения по сроку действия");
			Assert.IsFalse(model.ValidDateVisible.HasValue);
			Assert.IsFalse(model.ValidDaysVisible.HasValue);

		}

		[Test]
		public void SwitchBarcode()
		{
			model.IsBarcodeUse.Value = true;
			Assert.IsTrue(model.Doc.Value.UseBarcode);

			model.IsNumberUse.Value = true;
			Assert.IsFalse(model.Doc.Value.UseBarcode);
		}

		[Test]
		public void Save()
		{
			model.Save();
			scheduler.AdvanceByMs(2000);
			Assert.IsFalse(model.IsChanged.Value);
			Assert.IsTrue(model.CanPost.Value);
			Assert.IsFalse(model.CanSaveAndClose.Value);
			model.Doc.Value.Name = "Изменение наименования сертификата";
			scheduler.AdvanceByMs(2000);
			Assert.IsTrue(model.IsChanged.Value);
			Assert.IsTrue(model.CanSaveAndClose.Value);

			manager.DefaultResult = MessageBoxResult.Yes;
			manager.DefaultQuestsionResult = MessageBoxResult.Yes;
			model.Close();
			var msg = manager.MessageBoxes;
			Assert.GreaterOrEqual(msg[0], "Сохранить изменения?");
		}

		[Test]
		public void Post()
		{
			Assert.IsTrue(model.IsChanged.Value);
			Assert.IsTrue(model.CanPost.Value);
			Assert.IsTrue(model.CanSaveAndClose.Value);
			model.Post();
			scheduler.AdvanceByMs(2000);
			Assert.IsFalse(model.IsChanged.Value);
			Assert.IsFalse(model.CanPost.Value);
			Assert.IsFalse(model.CanSaveAndClose.Value);
		}

		[Test]
		public void Delete()
		{
			var items = model.Lines;
			Assert.AreEqual(items.Count, 2);

			manager.DefaultResult = MessageBoxResult.Yes;
			manager.DefaultQuestsionResult = MessageBoxResult.Yes;

			model.CurrentLine.Value = items[0];
			model.Delete();
			scheduler.AdvanceByMs(2000);

			var msg = manager.MessageBoxes;
			Assert.GreaterOrEqual(msg.Count, 1);
			Assert.GreaterOrEqual(msg[0], "Удалить выбранный штрих-код/номер?");

			items = model.Lines;
			Assert.AreEqual(items.Count, 1);
			Assert.AreEqual(items[0].Barcode, "22222222");
		}

		[Test]
		public void Add()
		{
			var items = model.Lines;
			Assert.AreEqual(items.Count, 2);

			manager.DefaultResult = MessageBoxResult.Yes;
			manager.DefaultQuestsionResult = MessageBoxResult.Yes;

			var result = model.Add().GetEnumerator();
			var dialog = (GiftCardNumber) Next<DialogResult>(result).Model;
			dialog.Number = "33333333";
			dialog.OK();
			dialog = (GiftCardNumber)Next<DialogResult>(result).Model;
			dialog.Number = "44444444";
			dialog.OK();
			dialog = (GiftCardNumber)Next<DialogResult>(result).Model;
			dialog.Number = "33333333";
			dialog.OK();
			result.MoveNext();
			result.MoveNext();

			var msg = manager.MessageBoxes;
			Assert.GreaterOrEqual(msg.Count, 1);
			Assert.GreaterOrEqual(msg[0], model.Doc.Value.UseBarcode ? "Карта с таким штрих-кодом уже есть" : "Карта с таким номером уже есть");


			items = model.Lines;
			Assert.AreEqual(items.Count, 4);
			Assert.AreEqual(items[2].Barcode, "33333333");
			Assert.AreEqual(items[3].Barcode, "44444444");
		}

	}
}
