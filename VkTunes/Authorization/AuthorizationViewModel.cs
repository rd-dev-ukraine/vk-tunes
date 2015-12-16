using System;
using System.Windows;
using System.Windows.Controls;

using Caliburn.Micro;

using VkTunes.Api.Authorization;
using VkTunes.AudioShell;
using VkTunes.Infrastructure.Navigation;

namespace VkTunes.Authorization
{
    public class AuthorizationViewModel : Screen
    {
        private readonly INavigator navigator;
        private readonly IAuthorization authorizationService;

        public AuthorizationViewModel(INavigator navigator, IAuthorization authorizationService)
        {
            if (navigator == null)
                throw new ArgumentNullException(nameof(navigator));
            if (authorizationService == null)
                throw new ArgumentNullException(nameof(authorizationService));

            this.navigator = navigator;
            this.authorizationService = authorizationService;
        }

        protected override void OnViewLoaded(Object view)
        {
            base.OnViewLoaded(view);

            var frameworkElement = view as FrameworkElement;

            var browser = (WebBrowser)frameworkElement?.FindName("Browser");
            if (browser != null)
            {
                browser.Source = new Uri(authorizationService.AuthorizationUrl());

                browser.Navigated += (_, e) =>
                {
                    if (authorizationService.ExtractTokenFromUrl(e.Uri.ToString()))
                        navigator.GoTo<AudioShellViewModel>();
                };
            }
        }

        public string AuthorizationUrl { get; set; }
    }
}