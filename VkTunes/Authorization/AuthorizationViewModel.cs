using System;

using Caliburn.Micro;

using VkTunes.Api.Authorization;
using VkTunes.Infrastructure;

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
    }
}