using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace StatusMonitor
{
    public class MicrosoftGraphInterface
    {
        //Set the API Endpoint to Graph 'me' endpoint
        //string _graphAPIEndpoint = "https://graph.microsoft.com/me";
        string[] scopes = new string[] { "user.read", "presence.read", "groupmember.read.all", "group.read.all", "directory.read.all"};

        public string ResultText { get; set; }

        public string TokenInfoText { get; set; }

        public MicrosoftGraphInterface()
        {

        }

        public async Task<bool> StartConnection()
        {
            AuthenticationResult authResult = await Authenticate();
            var graphServiceClient = new Microsoft.Graph.GraphServiceClient(new Microsoft.Graph.DelegateAuthenticationProvider((requestMessage) =>
            {
                requestMessage
                    .Headers
                    .Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", authResult.AccessToken);

                return Task.FromResult(0);
            }));

            return graphServiceClient.Me != null; ;
        }

        public async Task<Presence> GetPresence()
        {
            AuthenticationResult authResult = await Authenticate();
            var graphServiceClient = new Microsoft.Graph.GraphServiceClient(new Microsoft.Graph.DelegateAuthenticationProvider((requestMessage) =>
            {
                requestMessage
                    .Headers
                    .Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", authResult.AccessToken);

                return Task.FromResult(0);
            }));

            Presence myPresence = await graphServiceClient.Me.Presence.Request().GetAsync();

            return myPresence;
        }

        public async Task<Presence> GetPresence(string User)
        {
            AuthenticationResult authResult = await Authenticate();
            var graphServiceClient = new Microsoft.Graph.GraphServiceClient(new Microsoft.Graph.DelegateAuthenticationProvider((requestMessage) =>
            {
                requestMessage
                    .Headers
                    .Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", authResult.AccessToken);

                return Task.FromResult(0);
            }));

            Presence myPresence = await graphServiceClient.Users[User].Presence.Request().GetAsync();

            return myPresence;
        }

        public async Task<List<User>> GetGroupMembers(string GroupId)
        {
            AuthenticationResult authResult = await Authenticate();
            var graphServiceClient = new Microsoft.Graph.GraphServiceClient(new Microsoft.Graph.DelegateAuthenticationProvider((requestMessage) =>
            {
                requestMessage
                    .Headers
                    .Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", authResult.AccessToken);

                return Task.FromResult(0);
            }));

            var Members = await graphServiceClient.Groups[GroupId].Members.Request().GetAsync();

            List<User> GroupUsers = new List<User>();
            foreach(var Member in Members)
            {
                GroupUsers.Add(Member as User);
            }

            return GroupUsers;
        }

        public async Task<SortedList<string,string>> GetGroups()
        {
            AuthenticationResult authResult = await Authenticate();
            var graphServiceClient = new Microsoft.Graph.GraphServiceClient(new Microsoft.Graph.DelegateAuthenticationProvider((requestMessage) =>
            {
                requestMessage
                    .Headers
                    .Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", authResult.AccessToken);

                return Task.FromResult(0);
            }));

            var Groups = await graphServiceClient.Groups.Request().GetAsync();

            SortedList<string, string> GroupIDs = new SortedList<string, string>();
            foreach(var group in Groups)
            {
                GroupIDs.Add(group.Id, group.DisplayName);
            }

            return GroupIDs;
        }

        public async Task<SortedList<string,string>> GetUsers(string emailFilter)
        {
            AuthenticationResult authResult = await Authenticate();
            var graphServiceClient = new Microsoft.Graph.GraphServiceClient(new Microsoft.Graph.DelegateAuthenticationProvider((requestMessage) =>
            {
                requestMessage
                    .Headers
                    .Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", authResult.AccessToken);

                return Task.FromResult(0);
            }));

            SortedList<string, string> UserIDs = new SortedList<string, string>();
            List<User> userList = new List<User>();

            var queryOptions = new List<QueryOption>()
{
    new QueryOption("$count", "true"),
};
            IGraphServiceUsersCollectionPage Users = await graphServiceClient.Users.Request(queryOptions).Header("ConsistencyLevel", "eventual")
                                                                                            .Filter("endsWith(mail,'"+emailFilter+"')")
                                                                                            .Select("id,displayName,mail")
                                                                                            .OrderBy("displayName")
                                                                                            .GetAsync();



            userList.AddRange(Users.CurrentPage);
            while(Users.NextPageRequest != null)
            {
                Users = await Users.NextPageRequest.GetAsync();
                userList.AddRange(Users.CurrentPage);
            }

            foreach (User user in userList)
            {
                if (!UserIDs.ContainsKey(user.DisplayName)) { UserIDs.Add(user.DisplayName, user.Id); }
            }
            

            return UserIDs;
        }

        #region Authentication
        private async Task<AuthenticationResult> Authenticate()
        {
            AuthenticationResult authResult = null;
            var app = App.PublicClientApp;

            var accounts = await app.GetAccountsAsync();
            var firstAccount = accounts.FirstOrDefault();

            try
            {
                authResult = await app.AcquireTokenSilent(scopes, firstAccount)
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilent. 
                // This indicates you need to call AcquireTokenInteractive to acquire a token
                System.Diagnostics.Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                try
                {
                    authResult = await app.AcquireTokenInteractive(scopes)
                        .WithAccount(accounts.FirstOrDefault())
                        .WithPrompt(Microsoft.Identity.Client.Prompt.SelectAccount)
                        .ExecuteAsync();
                }
                catch (MsalException msalex)
                {
                    //ResultText.Text = $"Error Acquiring Token:{System.Environment.NewLine}{msalex}";
                }
            }
            catch (Exception ex)
            {
                //ResultText.Text = $"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}";
                //return;
            }

            return authResult;
        }

        /// <summary>
        /// Perform an HTTP GET request to a URL using an HTTP Authorization header
        /// </summary>
        /// <param name="url">The URL</param>
        /// <param name="token">The token</param>
        /// <returns>String containing the results of the GET operation</returns>
        public async Task<string> GetHttpContentWithToken(string url, string token)
        {
            var httpClient = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage response;
            try
            {
                var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, url);
                //Add the token in Authorization header
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Sign out the current user
        /// </summary>
        public async void SingOut()
        {
            var accounts = await App.PublicClientApp.GetAccountsAsync();

            if (accounts.Any())
            {
                try
                {
                    await App.PublicClientApp.RemoveAsync(accounts.FirstOrDefault());
                    this.ResultText = "User has signed-out";
                }
                catch (MsalException ex)
                {
                    ResultText = $"Error signing-out user: {ex.Message}";
                }
            }
        }

        /// <summary>
        /// Display basic information contained in the token
        /// </summary>
        private string DisplayBasicTokenInfo(AuthenticationResult authResult)
        {
            TokenInfoText = "";
            if (authResult != null)
            {
                TokenInfoText += $"Username: {authResult.Account.Username}" + Environment.NewLine;
                TokenInfoText += $"Token Expires: {authResult.ExpiresOn.ToLocalTime()}" + Environment.NewLine;
                //TokenInfoText += $"Access Token: {authResult.AccessToken}" + Environment.NewLine;
            }

            return TokenInfoText;
        }
        #endregion
    }
}
