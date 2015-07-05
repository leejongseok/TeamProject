using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2.Flows;
namespace googlecloud1.login
{
    class GoogleWebAuthorization : GoogleWebAuthorizationBroker
    {
        public static async Task<UserCredential> LoginAuthorizationCodeFlowAsync(ClientSecrets client, IEnumerable<string> scopes, string username, CancellationToken taskCancellationToken, FileDataStore store)
        {
            var initializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = client,
            };
            return await AuthorizeAsyncCore(initializer, scopes, username, taskCancellationToken, store)
                .ConfigureAwait(false);
        }
        private static async Task<UserCredential> AuthorizeAsyncCore(
            GoogleAuthorizationCodeFlow.Initializer initializer, IEnumerable<string> scopes, string user,
            CancellationToken taskCancellationToken, IDataStore dataStore = null)
        {
            initializer.Scopes = scopes;
            initializer.DataStore = dataStore ?? new FileDataStore(Folder);
            GoogleAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(initializer);

            // Create an authorization code installed app instance and authorize the user.
            return await new googleauth(flow, new LocalServerCodeReceiver()).AuthorizeAsync
                (user, taskCancellationToken).ConfigureAwait(false);
        }
    }
}
