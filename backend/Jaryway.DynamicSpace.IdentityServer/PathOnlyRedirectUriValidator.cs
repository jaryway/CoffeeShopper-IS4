using Jaryway.IdentityServer.Models;
using Jaryway.IdentityServer.Validation;
using Jaryway.IdentityServer.Extensions;
using System.Linq;

namespace Jaryway.DynamicSpace.IdentityServer
{
    public class PathOnlyRedirectUriValidator : IRedirectUriValidator
    {
        /// <summary>
        /// Checks if a given URI string is in a collection of strings (using ordinal ignore case comparison)
        /// </summary>
        /// <param name="uris">The uris.</param>
        /// <param name="requestedUri">The requested URI.</param>
        /// <returns></returns>
        protected bool StringCollectionContainsString(IEnumerable<string> uris, string requestedUri)
        {
            var uri = new Uri(requestedUri);
            var pathname = uri.AbsolutePath;

            if (uris.IsNullOrEmpty()) return false;

            var pathnames = uris.Select(uri =>
            {
                var _uri = new Uri(requestedUri);
                return _uri.AbsolutePath;
            });

            return pathnames.Contains(pathname, StringComparer.OrdinalIgnoreCase);
        }
        public Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            return Task.FromResult(StringCollectionContainsString(client.PostLogoutRedirectUris, requestedUri));
        }
        public Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            return Task.FromResult(StringCollectionContainsString(client.RedirectUris, requestedUri));
        }
    }
}
