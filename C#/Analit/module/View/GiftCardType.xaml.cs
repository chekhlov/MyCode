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
	/// Логика взаимодействия для GiftCardType.xaml
	/// </summary>
	public partial class GiftCardType : UserControl
	{

		public GiftCardType()
		{
			InitializeComponent();

			Loaded += (sender, args) => {
				Cards_Value.Focus();
			};
		}
  }
}
