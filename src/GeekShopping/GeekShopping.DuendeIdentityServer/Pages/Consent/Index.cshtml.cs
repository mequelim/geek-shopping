using Duende.IdentityModel;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GeekShopping.DuendeIdentityServer.Pages.Consent
{
    [Authorize]
    [SecurityHeaders]
    public class Index : PageModel
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;
        private readonly ILogger<Index> _logger;

        public Index(
            IIdentityServerInteractionService interaction,
            IEventService events,
            ILogger<Index> logger
        )
        {
            _interaction = interaction;
            _events = events;
            _logger = logger;
        }

        public ViewModel View { get; set; } = default!;

        [BindProperty] public InputModel Input { get; set; } = default!;

        public async Task<IActionResult> OnGet(string? returnUrl)
        {
            if(!await SetViewModelAsync(returnUrl)) return RedirectToPage("/Home/Error/Index");

            Input = new InputModel { ReturnUrl = returnUrl, };

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            // validate return url is still valid
            var request = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);
            if(request == null) return RedirectToPage("/Home/Error/Index");

            ConsentResponse? grantedConsent = null;

            // user clicked 'no' - send back the standard 'access_denied' response
            if(Input.Button == "no")
            {
                grantedConsent = new ConsentResponse { Error = AuthorizationError.AccessDenied };

                // Emit event
                await _events.RaiseAsync(
                    new ConsentDeniedEvent(
                        User.GetSubjectId(),
                        request.Client.ClientId,
                        request.ValidatedResources.RawScopeValues
                    )
                );

                Telemetry.Metrics.ConsentDenied(
                    request.Client.ClientId,
                    request.ValidatedResources.ParsedScopes.Select((s) => s.ParsedName)
                );
            }
            // User clicked 'yes' - validate the data
            else if(Input.Button == "yes")
            {
                // if the user consented to some scope, build the response model
                if(Input.ScopesConsented.Any())
                {
                    IEnumerable<string> scopes = Input.ScopesConsented;

                    if(ConsentOptions.EnableOfflineAccess.Equals(false))
                    {
                        scopes = scopes.Where(x => x != Duende.IdentityServer.IdentityServerConstants.StandardScopes.OfflineAccess);
                    }

                    grantedConsent = new ConsentResponse
                    {
                        RememberConsent = Input.RememberConsent,
                        ScopesValuesConsented = scopes.ToArray(),
                        Description = Input.Description
                    };

                    // Emit event:
                    await _events.RaiseAsync(
                        new ConsentGrantedEvent(
                            User.GetSubjectId(),
                            request.Client.ClientId,
                            request.ValidatedResources.RawScopeValues,
                            grantedConsent.ScopesValuesConsented,
                            grantedConsent.RememberConsent
                        )
                    );

                    Telemetry.Metrics.ConsentGranted(request.Client.ClientId, grantedConsent.ScopesValuesConsented, grantedConsent.RememberConsent);

                    IEnumerable<string> denied = request
                        .ValidatedResources
                        .ParsedScopes
                        .Select((s) => s.ParsedName)
                        .Except(grantedConsent.ScopesValuesConsented);

                    Telemetry.Metrics.ConsentDenied(request.Client.ClientId, denied);
                }
                else
                {
                    ModelState.AddModelError("", ConsentOptions.MustChooseOneErrorMessage);
                }
            }
            else
            {
                ModelState.AddModelError("", ConsentOptions.InvalidSelectionErrorMessage);
            }

            if(grantedConsent != null)
            {
                ArgumentNullException.ThrowIfNull(Input.ReturnUrl, nameof(Input.ReturnUrl));

                // Communicate outcomes of consent back to IdentityServer:
                await _interaction.GrantConsentAsync(request, grantedConsent);

                // Redirect back to the authorization endpoint:
                if(request.IsNativeClient() is true)
                {
                    // The client is native, so this change in how to return the response is for better UX for the end user.
                    return this.LoadingPage(Input.ReturnUrl);
                }

                return Redirect(Input.ReturnUrl);
            }

            // We need to redisplay the consent UI:
            if(!await SetViewModelAsync(Input.ReturnUrl))
            {
                return RedirectToPage("/Home/Error/Index");
            }

            return Page();
        }

        private async Task<bool> SetViewModelAsync(string? returnUrl)
        {
            ArgumentNullException.ThrowIfNull(returnUrl);

            AuthorizationRequest? request = await _interaction.GetAuthorizationContextAsync(returnUrl);

            if(request != null)
            {
                View = CreateConsentViewModel(request);
                return true;
            }
            else
            {
                _logger.NoConsentMatchingRequest(returnUrl);
                return false;
            }
        }

        private ViewModel CreateConsentViewModel(AuthorizationRequest request)
        {
            ViewModel vm = new ViewModel {
                ClientName = request.Client.ClientName ?? request.Client.ClientId,
                ClientUrl = request.Client.ClientUri,
                ClientLogoUrl = request.Client.LogoUri,
                AllowRememberConsent = request.Client.AllowRememberConsent,
                IdentityScopes = request.ValidatedResources
                    .Resources
                    .IdentityResources
                    .Select((x) =>
                        CreateScopeViewModel(
                            x,
                            (Input is null) || (Input.ScopesConsented.Contains(x.Name))
                        )
                    )
                    .ToArray()
            };

            IEnumerable<string> resourceIndicators = request
                .Parameters
                .GetValues(OidcConstants.AuthorizeRequest.Resource) ?? Enumerable.Empty<string>();

            IEnumerable<ApiResource> apiResources = request
                .ValidatedResources
                .Resources
                .ApiResources
                .Where((x) => resourceIndicators.Contains(x.Name));

            List<ScopeViewModel> apiScopes = new List<ScopeViewModel>();

            foreach(ParsedScopeValue parsedScope in request.ValidatedResources.ParsedScopes)
            {
                ApiScope? apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);

                if(apiScope is not null)
                {
                    ScopeViewModel scopeVm = CreateScopeViewModel(
                        parsedScope,
                        apiScope,
                        (Input is null) || Input.ScopesConsented.Contains(parsedScope.RawValue)
                    );

                    scopeVm.Resources = apiResources
                        .Where((x) => x.Scopes.Contains(parsedScope.ParsedName))
                        .Select((x) => new ResourceViewModel
                        {
                            Name = x.Name,
                            DisplayName = x.DisplayName ?? x.Name,
                        }).ToArray();
                    apiScopes.Add(scopeVm);
                }
            }

            if(ConsentOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
            {
                apiScopes.Add(CreateOfflineAccessScope(
                    (Input is null) ||
                    Input.ScopesConsented.Contains(Duende.IdentityServer.IdentityServerConstants.StandardScopes.OfflineAccess))
                );
            }

            vm.ApiScopes = apiScopes;

            return vm;
        }

        private static ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
        {
            return new ScopeViewModel
            {
                Name = identity.Name,
                Value = identity.Name,
                DisplayName = identity.DisplayName ?? identity.Name,
                Description = identity.Description,
                Emphasize = identity.Emphasize,
                Required = identity.Required,
                Checked = check || identity.Required
            };
        }

        private static ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
        {
            string displayName = apiScope.DisplayName ?? apiScope.Name;

            if(!string.IsNullOrWhiteSpace(parsedScopeValue.ParsedParameter))
            {
                displayName += ":" + parsedScopeValue.ParsedParameter;
            }

            return new ScopeViewModel
            {
                Name = parsedScopeValue.ParsedName,
                Value = parsedScopeValue.RawValue,
                DisplayName = displayName,
                Description = apiScope.Description,
                Emphasize = apiScope.Emphasize,
                Required = apiScope.Required,
                Checked = check || apiScope.Required
            };
        }

        private static ScopeViewModel CreateOfflineAccessScope(bool check)
        {
            return new ScopeViewModel
            {
                Value = Duende.IdentityServer.IdentityServerConstants.StandardScopes.OfflineAccess,
                DisplayName = ConsentOptions.OfflineAccessDisplayName,
                Description = ConsentOptions.OfflineAccessDescription,
                Emphasize = true,
                Checked = check
            };
        }
    }
}