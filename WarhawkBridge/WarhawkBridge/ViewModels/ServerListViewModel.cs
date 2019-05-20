using System;
using System.Collections.Generic;
using System.Text;
using WarhawkBridge.Models;
using WarhawkBridge.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using System.Diagnostics;

using Xamarin.Forms;

namespace WarhawkBridge.ViewModels
{
    class ServerListViewModel : BaseViewModel
    {
		public ObservableCollection<ServerEntry> Servers { get; set; }
		public Command LoadItemsCommand { get; set; }
		public WebApi ServerApi;
		WarhawkBridgeService service;

		public ServerListViewModel()
		{
			Title = "Active Servers";
			Servers = new ObservableCollection<ServerEntry>();
			LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
			ServerApi = new WebApi();

			service = new WarhawkBridgeService();
		}

		public void ToggleServiceCommand(bool isActive)
		{
			service.ToggleServiceActive(isActive);			
		}


		async Task ExecuteLoadItemsCommand()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				Servers.Clear();
				var items = ServerApi.GetServers();
				foreach (var item in items)
				{
					Servers.Add(item);
					Debug.WriteLine("Add item " + item);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
			finally
			{
				service.SetServerList(Servers);
				IsBusy = false;
			}
		}
	}
}
