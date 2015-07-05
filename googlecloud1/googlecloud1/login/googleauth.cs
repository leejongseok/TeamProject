using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Logging;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Requests;
namespace googlecloud1.login
{
    class googleauth : IAuthorizationCodeInstalledApp
    {
        private readonly GoogleAuthorizationCodeFlow flow;
        private readonly ICodeReceiver codeReceiver;
        public string StartUrl { get; private set; }
        public string EndUrl { get; private set; }
        public googleauth(GoogleAuthorizationCodeFlow flow, ICodeReceiver codeReceiver)
        { 
            this.flow = flow;
            this.codeReceiver = codeReceiver;
        }

        public IAuthorizationCodeFlow Flow
        {
            get { return flow; }
        }

        /// <summary>Gets the code receiver which is responsible for receiving the authorization code.</summary>
        public ICodeReceiver CodeReceiver
        {
            get { return codeReceiver; }
        }

        public async Task<UserCredential> AuthorizeAsync(string userId, CancellationToken taskCancellationToken)
        {
            // Try to load a token from the data store.
            var token = await flow.LoadTokenAsync(userId, taskCancellationToken).ConfigureAwait(false);
            if (token == null || (token.RefreshToken == null && token.IsExpired(flow.Clock)))
            {
                var authorizationCode = await loginform.GetAuthenticationToken(flow.ClientSecrets.ClientId, flow. Scopes, userId);
                if (string.IsNullOrEmpty(authorizationCode))
                    return null;
                //Logger.Debug("Received \"{0}\" code", response.Code);

                // Get the token based on the code.
                token = await flow.ExchangeCodeForTokenAsync(userId, authorizationCode, GoogleAuthConsts.InstalledAppRedirectUri,
                    taskCancellationToken).ConfigureAwait(false);
            }

            return new UserCredential(flow, userId, token);
        }
    }
}
