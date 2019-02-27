using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Threading;

namespace XamarinNetworkTools.Sample
{
	public partial class MainPage : ContentPage
	{
		public ObservableCollection<NetworkDevice> Devices;
		private CancellationTokenSource cancelToken;

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

		void Handle_Clicked(object sender, System.EventArgs e)
		{
			cancelToken?.Cancel();
			cancelToken = null;
		}

		void Handle_Refreshing(object sender, System.EventArgs e)
		{
			this.LoadData();
		}

		async Task LoadData()
		{
			using (this.cancelToken = new CancellationTokenSource())
			{
				try
				{
					this.list.IsRefreshing = true;

					this.Devices.Clear();

					await NetworkTools.Instance
						.FindDevicesOnNetwork()
						.ForEachAsync((device) =>
						{
							this.Devices.Add(device);
						}, cancelToken.Token);

					this.list.IsRefreshing = false;
				}
				catch(OperationCanceledException)
				{
					this.list.IsRefreshing = false;
				}
			}
		}
	}
}
