using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Timers;
using OAuthST.Models;
using Newtonsoft.Json;

namespace OAuthST
{
    /// <summary>
    /// An OAuth controller that handles ROPC grant type authorization obtaining and auto token refreshing.
    /// </summary>
    public class AuthController
    {
        private const int MillisecondFactor = 1000;     // factor when multiplied converts second to millisecond

        private const string ExceptionMsg = "\nAn exception raised:\n";

        private const string UsernameOauthName = "username";
        private const string PasswordOauthName = "password";
        private const string ScopeOauthName = "scope";
        private const string GranttypeOauthName = "grant_type";
        private const string RefreshtokenOauthName = "refresh_token";

        private const int SecondsBeforeExpiry = 1;    // number of seconds before current token expiry that the renew token process starts

        private const string GrantTypeRopc = "password";
        private const string GrantTypeRefresh = "refresh_token";

        /// <summary>
        /// <see cref="OAuthResponse"/> object that stores current token info
        /// </summary>
        public OAuthResponse Info { get; set; }

        /// <summary>
        /// Application that seeks authorization from the authorization server.
        /// </summary>
        public IOAuthClient Client { get; }

        private Timer _authTimer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client"><seealso cref="IOAuthClient"/></param>
        public AuthController(IOAuthClient client)
        {
            Client = client;
        }

        #region RetrieveAccessToken overloads

        /// <summary>
        /// Create parameters required for the ROPC grant and call <see cref="RetrieveAccessToken(NameValueCollection)"/>
        /// </summary>
        /// <param name="username">username of the resource owner</param>
        /// <param name="password">password of the resource owner</param>
        /// <returns>return the result of <see cref="RetrieveAccessToken(NameValueCollection)"/></returns>
        public bool RetrieveAccessToken(string username, string password)
        {
            var parameters = new NameValueCollection();

            parameters.Add(UsernameOauthName, username);
            parameters.Add(PasswordOauthName, password);
            parameters.Add(GranttypeOauthName, GrantTypeRopc);
            parameters.Add(ScopeOauthName, Client.AccessScope);

            return RetrieveAccessToken(parameters);
        }

        /// <summary>
        /// Create parameters required for refreshing an access token and call <see cref="RetrieveAccessToken(NameValueCollection)"/>
        /// </summary>
        /// <param name="withScope">whether the refresh token request should attach a scope parameter, default to false</param>
        /// <returns>return the result of <see cref="RetrieveAccessToken(NameValueCollection)"/></returns>
        public bool RetrieveAccessToken(bool withScope = false)
        {
            try
            {
                var parameters = new NameValueCollection();

                parameters.Add(RefreshtokenOauthName, Info.RefreshToken);
                parameters.Add(GranttypeOauthName, GrantTypeRefresh);

                if (withScope)
                {
                    parameters.Add(ScopeOauthName, Client.AccessScope);
                }

                return RetrieveAccessToken(parameters);
            }
            catch (Exception e)
            {
                return FailureWithError(e);
            }
        }

        /// <summary>
        /// Obtain authorization from server and store the token info.
        /// </summary>
        /// <remarks>
        /// This method demonstrates the workflow of obtaining authorization from server and should be used
        /// as the primary source for examples of recommended steps of actions
        /// </remarks>
        /// <param name="parameters">
        /// parameters to send with the request to retrieve an access token. See also: 
        /// <seealso cref="RetrieveAccessToken(string, string)"/>, <seealso cref="RetrieveAccessToken(bool)"/>
        /// </param>
        /// <returns>
        /// true if <see cref="OAuthResponse.AccessToken"/> is retrieved, false otherwise
        /// </returns>
        public bool RetrieveAccessToken(NameValueCollection parameters)
        {
            string responseString = null;

            using (var httpClient = new WebClient())
            {
                #region include base64 encoded "clientID:clientSecret" string inside Authorization Header

                var userPassString = string.Format("{0}:{1}", Client.ClientID, Client.ClientSecret);
                var userPassBytes = Encoding.UTF8.GetBytes(userPassString);
                var base64UserPass = Convert.ToBase64String(userPassBytes);

                httpClient.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", base64UserPass);

                #endregion

                #region send request and get response
                try
                {
                    // WebClient.UploadValues() uses POST method by default, 
                    // otherwise put WebClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded" before sending the request
                    var responseArray = httpClient.UploadValues(Client.TokenEndPoint, parameters);

                    responseString = Encoding.UTF8.GetString(responseArray);
                }
                catch (WebException e)
                {
                    // server returned an error
                    string errorResponse = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();

                    Info = JsonConvert.DeserializeObject<OAuthResponse>(errorResponse);
                }
                catch (Exception e)
                {
                    return FailureWithError(e);
                }
                #endregion
            }

            if (responseString == null)
            {
                return false;
            }

            // store retrieved info
            Info = JsonConvert.DeserializeObject<OAuthResponse>(responseString);

            return true;
        }

        #endregion

        /// <summary>
        /// Set up timer and refresh authorization before current token expired
        /// </summary>
        /// <returns>true if no exception is raised, false otherwise</returns>
        public bool RenewTokenBeforeExpiry()
        {
            try
            {
                _authTimer = new Timer();

                #region register elapsed event
                _authTimer.Elapsed += (sender, e) =>
                {
                    OnRenewTokenStarted(EventArgs.Empty);
                };
                _authTimer.Elapsed += CurrentTokenExpired;
                #endregion

                _authTimer.AutoReset = false;

                SetTimerIntervalAndStart(_authTimer);

                return true;
            }
            catch (Exception e)
            {
                return FailureWithError(e);
            }
        }

        /// <summary>
        /// Store error messages to <see cref="Info"/>
        /// </summary>
        /// <param name="e">Exception raised by application</param>
        /// <returns>always return false</returns>
        private bool FailureWithError(Exception e)
        {
            Info = new OAuthResponse(ExceptionMsg, e.ToString());
            return false;
        }

        /// <summary>
        /// Set <paramref name="timer"/> interval to the length of time before current token expired 
        /// minus <see cref="SecondsBeforeExpiry"/> second, then start the <paramref name="timer"/>.
        /// </summary>
        /// <param name="timer"></param>
        private void SetTimerIntervalAndStart(Timer timer)
        {
            timer.Interval = (Info.ExpiresIn - SecondsBeforeExpiry) * MillisecondFactor;
            //timer.Interval = 20000;
            timer.Start();
        }

        /// <summary>
        /// EventHandler of the <see cref="Timer.Elapsed"/> event.
        /// Refresh token and update timer(<paramref name="sender"/>). 
        /// Fire failed or renewed events depending on the refresh token result
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentTokenExpired(object sender, ElapsedEventArgs e)
        {
            var timer = sender as Timer;

            if (timer == null)
            {
                OnRenewTokenFailed(EventArgs.Empty);
                return;
            }

            timer.Stop();

            //try refreshing the token
            var renewalSuccessful = RetrieveAccessToken();

            if (!renewalSuccessful)
            {
                OnRenewTokenFailed(EventArgs.Empty);
                return;
            }

            SetTimerIntervalAndStart(timer);

            var args = new TokenRenewedEventArgs {TokenInfo = Info};

            OnTokenRenewed(args);
        }

        /// <summary>
        /// Fire the <see cref="RenewTokenFailed"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRenewTokenFailed(EventArgs e)
        {
            RenewTokenFailed?.Invoke(this, e);
        }

        /// <summary>
        /// Fire the <see cref="TokenRenewed"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTokenRenewed(TokenRenewedEventArgs e)
        {
            TokenRenewed?.Invoke(this, e);
        }

        /// <summary>
        /// Fire the <see cref="OnRenewTokenStarted(EventArgs)"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRenewTokenStarted(EventArgs e)
        {
            RenewTokenStarted?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when renew token failed.
        /// </summary>
        public event EventHandler RenewTokenFailed;
        /// <summary>
        /// Occurs when token has been successfully renewed.
        /// </summary>
        public event EventHandler<TokenRenewedEventArgs> TokenRenewed;
        /// <summary>
        /// Occurs when current token is expired.
        /// </summary>
        public event EventHandler RenewTokenStarted;
    }
}
