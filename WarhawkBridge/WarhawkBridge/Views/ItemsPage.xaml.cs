﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WarhawkBridge.Models;
using WarhawkBridge.Views;
using WarhawkBridge.ViewModels;

namespace WarhawkBridge.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ItemsPage : ContentPage
	{
		ServerListViewModel viewModel;
		
		public ItemsPage()
		{
			InitializeComponent();

			BindingContext = viewModel = new ServerListViewModel();

			Switch s = this.FindByName<Switch>("ServiceToggleSwitch");
			viewModel.ToggleServiceCommand(s.IsToggled);
		}

		async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
		{
			var item = args.SelectedItem as Item;
			if (item == null)
				return;

			await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

			// Manually deselect item.
			ItemsListView.SelectedItem = null;
		}

		async void AddItem_Clicked(object sender, EventArgs e)
		{
			//await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (viewModel.Servers.Count == 0)
				viewModel.LoadItemsCommand.Execute(null);
		}

		private void ServiceToggleSwitch_Toggled(object sender, ToggledEventArgs e)
		{
			viewModel.ToggleServiceCommand(e.Value);
		}
	}
}