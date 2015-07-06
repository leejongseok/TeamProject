using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Google.Apis.Json;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using System.Net.Http;
using Google.Apis.Http;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Requests;
using System.Threading;
using Google.Apis.Util.Store;
namespace Google.Apis.Auth.OAuth2
{
    /// <summary>A helper utility to manage the authorization code flow.</summary>
    public class GoogleWebAuthorizationBroker
    {
        /// <summary>The folder which is used by the <see cref="Google.Apis.Util.Store.FileDataStore"/>.</summary>
        /// <remarks>
        /// The reason that this is not 'private const' is that a user can change it and store the credentials in a
        /// different location.
        /// </remarks>
        public static string Folder = "Google.Apis.Auth";

        /// <summary>Asynchronously authorizes the specified user.</summary>
        /// <remarks>
        /// In case no data store is specified, <see cref="Google.Apis.Util.Store.FileDataStore"/> will be used by 
        /// default.
        /// </remarks>
        /// <param name="clientSecrets">The client secrets.</param>
        /// <param name="scopes">
        /// The scopes which indicate the Google API access your application is requesting.
        /// </param>
        /// <param name="user">The user to authorize.</param>
        /// <param name="taskCancellationToken">Cancellation token to cancel an operation.</param>
        /// <param name="dataStore">The data store, if not specified a file data store will be used.</param>
        /// <returns>User credential.</returns>
        public static async Task<UserCredential> AuthorizeAsync(ClientSecrets clientSecrets,
            IEnumerable<string> scopes, string user, CancellationToken taskCancellationToken,
            IDataStore dataStore = null)
        {
            var initializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
            };
            return await AuthorizeAsyncCore(initializer, scopes, user, taskCancellationToken, dataStore)
                .ConfigureAwait(false);
        }

        /// <summary>Asynchronously authorizes the specified user.</summary>
        /// <remarks>
        /// In case no data store is specified, <see cref="Google.Apis.Util.Store.FileDataStore"/> will be used by 
        /// default.
        /// </remarks>
        /// <param name="clientSecretsStream">
        /// The client secrets stream. The authorization code flow constructor is responsible for disposing the stream.
        /// </param>
        /// <param name="scopes">
        /// The scopes which indicate the Google API access your application is requesting.
        /// </param>
        /// <param name="user">The user to authorize.</param>
        /// <param name="taskCancellationToken">Cancellation token to cancel an operation.</param>
        /// <param name="dataStore">The data store, if not specified a file data store will be used.</param>
        /// <returns>User credential.</returns>
        public static async Task<UserCredential> AuthorizeAsync(Stream clientSecretsStream,
            IEnumerable<string> scopes, string user, CancellationToken taskCancellationToken,
            IDataStore dataStore = null)
        {
            var initializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecretsStream = clientSecretsStream,
            };
            return await AuthorizeAsyncCore(initializer, scopes, user, taskCancellationToken, dataStore)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously reauthorizes the user. This method should be called if the users want to authorize after 
        /// they revoked the token.
        /// </summary>
        /// <param name="userCredential">The current user credential. Its <see cref="UserCredential.Token"/> will be
        /// updated. </param>
        /// <param name="taskCancellationToken">Cancellation token to cancel an operation.</param>
        public static async Task ReauthorizeAsync(UserCredential userCredential,
            CancellationToken taskCancellationToken)
        {
            // Create an authorization code installed app instance and authorize the user.
            UserCredential newUserCredential = await new AuthorizationCodeInstalledApp(userCredential.Flow,
                new LocalServerCodeReceiver()).AuthorizeAsync
                (userCredential.UderId, taskCancellationToken).ConfigureAwait(false);
            userCredential.Token = newUserCredential.Token;
        }

        /// <summary>The core logic for asynchronously authorizing the specified user.</summary>
        /// <param name="initializer">The authorization code initializer.</param>
        /// <param name="scopes">
        /// The scopes which indicate the Google API access your application is requesting.
        /// </param>
        /// <param name="user">The user to authorize.</param>
        /// <param name="taskCancellationToken">Cancellation token to cancel an operation.</param>
        /// <param name="dataStore">The data store, if not specified a file data store will be used.</param>
        /// <returns>User credential.</returns>
        private static async Task<UserCredential> AuthorizeAsyncCore(
            GoogleAuthorizationCodeFlow.Initializer initializer, IEnumerable<string> scopes, string user,
            CancellationToken taskCancellationToken, IDataStore dataStore = null)
        {
            initializer.Scopes = scopes;
            initializer.DataStore = dataStore ?? new FileDataStore(Folder);
            var flow = new GoogleAuthorizationCodeFlow(initializer);

            // Create an authorization code installed app instance and authorize the user.
            return await new AuthorizationCodeInstalledApp(flow, new LocalServerCodeReceiver()).AuthorizeAsync
                (user, taskCancellationToken).ConfigureAwait(false);
        }
    }
}