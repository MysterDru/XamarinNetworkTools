using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Reactive.Linq;

namespace XamarinNetworkTools.Sample
{
	public partial class MainPage : ContentPage
	{
		public ObservableCollection<NetworkDevice> Devices;

		public MainPage()
		{
			Devices = new ObservableCollection<NetworkDevice>();
			InitializeComponent();

			this.list.ItemsSource = Devices;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			this.LoadData();
		}

		void Handle_Refreshing(object sender, System.EventArgs e)
		{
			this.LoadData();
		}

		async Task LoadData()
		{
			this.list.IsRefreshing = true;

			this.Devices.Clear();

			await NetworkTools.Instance
				.FindDevicesOnNetwork()
				.ForEachAsync((device) =>
				{
					this.Devices.Add(device);
				});

			this.list.IsRefreshing = false;
		}
	}
}
