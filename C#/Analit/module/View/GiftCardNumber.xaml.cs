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

namespace AnalitF.Net.Client.Views.Dialogs
{
	/// <summary>
	/// Логика взаимодействия для GiftCardNumber.xaml
	/// </summary>
	public partial class GiftCardNumber : UserControl
	{

		public GiftCardNumber()
		{
			InitializeComponent();

			Loaded += (sender, args) => {
				Number.Focus();
			};
		}
  }
}
