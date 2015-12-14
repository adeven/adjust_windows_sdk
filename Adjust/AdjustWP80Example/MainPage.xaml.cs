﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AdjustWP80Example.Resources;
using AdjustSdk;

namespace AdjustWP80Example
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void btnSimpleEvent_Click(object sender, RoutedEventArgs e)
        {
            var simpleEvent = new AdjustEvent("{yourSimpleEventToken}");
            Adjust.TrackEvent(simpleEvent);
        }

        private void btnRevenueEvent_Click(object sender, RoutedEventArgs e)
        {
            var revenueEvent = new AdjustEvent("{yourRevenueEventToken}");
            revenueEvent.SetRevenue(0.01, "EUR");
            Adjust.TrackEvent(revenueEvent);
        }

        private void btnCallbakEvent_Click(object sender, RoutedEventArgs e)
        {
            var callbackEvent = new AdjustEvent("{yourCallbackEventToken}");
            callbackEvent.AddPartnerParameter("key", "value");
            Adjust.TrackEvent(callbackEvent);
        }

        private void btnPartnerEvent_Click(object sender, RoutedEventArgs e)
        {
            var partnerEvent = new AdjustEvent("{yourPartnerEventToken}");
            partnerEvent.AddPartnerParameter("foo", "bar");
            Adjust.TrackEvent(partnerEvent);
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}