using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AnalitF.Net.Client.Helpers;
using AnalitF.Net.Client.Models;
using System.Windows.Controls.Primitives;
using AnalitF.Net.Client.Models.Inventory;

namespace AnalitF.Net.Client.Views.Inventory
{
	/// <summary>
	/// Логика взаимодействия для EditGiftCard.xaml
	/// </summary>
	public partial class EditGiftCards : UserControl
	{
		public ViewModels.Inventory.EditGiftCards Model => DataContext as ViewModels.Inventory.EditGiftCards;

		public EditGiftCards()
		{
			InitializeComponent();

			Loaded += (sender, args) => {
				Doc_Name.Focus();
				Doc_Name.SelectAll();
			};

			KeyDown += (sender, args) =>
			{
				if (args.Key == Key.Enter) {

					if (Doc_Name.IsFocused) {
						Doc_Nominal.Focus();
					}
					else if (Doc_Nominal.IsFocused) {
						ValitityTypes.Focus();
						Model.Save();
					}
					else if (ValitityTypes.IsFocused) {
						if (Doc_ValidDate.Visibility == Visibility.Visible)
							Doc_ValidDate.Focus();
						else if (Doc_ValidDays.Visibility == Visibility.Visible)
							Doc_ValidDate.Focus();
						else
							IsNumberUse.Focus();
					}
					else if (Doc_ValidDate.IsFocused || Doc_ValidDays.IsFocused) {
						IsNumberUse.Focus();
					}
					else if (IsBarcodeUse.IsFocused || IsNumberUse.IsFocused) {
//						Addresses2.Focus();
					}
				}

				if (args.Key == Key.Escape)
				{
					args.Handled = true;
					Model.TryClose();
				}
			};
		}
	}
}
