using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RGAuth.Enums;
using RGAuth.Models.xfApi;
using System;
using System.Collections.Generic;
using System.IO;
using RGAuth.Models;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace RGAuth.Code
{
    public static class Authorization
    {
        private const string defaultAuthMessage = "Allow the application/plugin to access the following:" +
                                                  "\n-RedGuides Account Information" +
                                                  "\n-Account Content(s) (Threads, Posts, Resources, etc.)" +
                                                  "\n-Access account permissions" +
                                                  "\n-Change account settings";
        private readonly static string defaultTokenDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RGL\";
        private static string TokenEndpoint = "https://www.redguides.com/community/redapi/index.php/oauth/token";
        private const string code_challenge_method = "S256";
        private static byte[] salt = Encryption.RandomNumberGenerator();
        private static AuthorizationInfo authInfo = new AuthorizationInfo();

        #region Authorization Functionality

        #region Callable Methods

        public static Tuple<Boolean, AuthToken, String> CheckForExistingToken(string tokenDir = null!)
        {
            if (tokenDir == null)
            {
                tokenDir = defaultTokenDir;
            }

            return GetStoredTokenData(tokenDir);
        }

        public static async Task<AuthorizationInfo> Authorize(string user, string password, string ClientID, string ClientSecret,
                                                              string authMessage = defaultAuthMessage, string port = "62897", string tokenDir = null!)
        {
            Common.ClearTempHistory();

            string redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, port);

            if (tokenDir == null)
            {
                tokenDir = defaultTokenDir;
            }

            if (authInfo.errInfo != null && authInfo.errInfo.Length > 0)
            {
                authInfo.errInfo = null!;
            }

            if (!string.IsNullOrEmpty(user) && !string.IsNullOrWhiteSpace(user)
                && !string.IsNullOrEmpty(password) && !string.IsNullOrWhiteSpace(password))
            {
                string code;
                string incoming_state;
                HttpListener listener = new HttpListener();
                HttpListenerContext context;
                string state = randomDataBase64url(32);
                string code_verifier = randomDataBase64url(32);
                string code_challenge = base64urlencodeNoPadding(sha256(code_verifier));
                string authorizationRequest = string.Format("{0}?response_type=code&scope=read%20post%20usercp%20conversate&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
                    "https://www.redguides.com/community/account/authorize",
                    Uri.EscapeDataString(redirectURI),
                    ClientID,
                    state,
                    code_challenge,
                    code_challenge_method);

                try
                {
                    listener.Prefixes.Add(redirectURI);
                    listener.Start();

                    Browser browser = await InitializeBrowser(new Browser(), user, password, authMessage, authorizationRequest, listener, redirectURI);

                    context = await listener.GetContextAsync();

                    if (authInfo.errInfo == null)
                    {
                        await HandleResponse(context, listener, browser);

                        code = context.Request.QueryString.Get("code")!;
                        incoming_state = context.Request.QueryString.Get("state")!;

                        authInfo = await TokenHandshake(code, code_verifier, ClientID, ClientSecret, false);

                        if (authInfo.errInfo == null)
                        {
                            await GetApiKey();

                            authInfo.user = await GetUserInfo();

                            SaveTokenData(authInfo.token, tokenDir);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Failed to listen on prefix '" + redirectURI + "' because it conflicts with an existing registration on the machine."))
                    {
                        authInfo.errInfo = "ERR000: Prefix " + redirectURI + " was occupied by another service.";
                    }
                }

                return authInfo;
            }
            else
            {
                authInfo.errInfo = "ERR001: Empty Username and/or Password";

                return authInfo;
            }
        }

        public static String Logout(string tokenDir = null!)
        {
            String logoutRet = "";

            try
            {
                if (tokenDir == null)
                {
                    tokenDir = defaultTokenDir;
                }

                if (Directory.Exists(Path.GetDirectoryName(tokenDir)))
                {
                    Directory.Delete(tokenDir, true);
                }

                authInfo.token = null!;
                authInfo.apiKey = null!;
                authInfo.user = null!;

                Common.ClearTempHistory();

                logoutRet = "Successfully Logged Out";
            }
            catch (Exception ex)
            {
                logoutRet = "ERR008: Issue Loggin Out, stack: " + ex.ToString();
            }

            return logoutRet;
        }

        //Written by R1ptide, modified by Ionis
        public static async Task<AuthorizationInfo> TokenHandshake(string code, string code_verifier, string ClientID, 
                                                                   string ClientSecret, bool isRefresh, string port = "62897",
                                                                   string tokenDir = null!)
        {
            string redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, port);

            AuthorizationInfo tokenInfo = new AuthorizationInfo();
            tokenInfo.token = new AuthToken();
            tokenInfo.token.Status = TokenStatus.Valid;
            tokenInfo.token.StatusReason = "";

            // builds the  request
            string tokenRequestURI = TokenEndpoint;
            string tokenRequestBody = "";

            if (tokenDir == null)
            {
                tokenDir = defaultTokenDir;
            }

            if (authInfo.errInfo != null && authInfo.errInfo.Length > 0)
            {
                authInfo.errInfo = null!;
            }

            if (!isRefresh)
            {
                tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code",
                    code,
                    Uri.EscapeDataString(redirectURI),
                    ClientID,
                    code_verifier,
                    ClientSecret
                    );
            }
            else
            {
                var unsecRefreshToken = Encryption.ToInsecureString(Encryption.DecryptString(code, salt));
                tokenRequestBody = string.Format("refresh_token={0}&redirect_uri={1}&client_id={2}&client_secret={3}&scope=&grant_type=refresh_token",
                        unsecRefreshToken,
                        Uri.EscapeDataString(redirectURI),
                        ClientID,
                        ClientSecret
                        );
            }

            // sends the request
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenRequestURI);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=application/json";
            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                // gets the response
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    // reads response body
                    string responseText = await reader.ReadToEndAsync();
                    Dictionary<string, dynamic> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseText)!;

                    bool success = int.TryParse(tokenEndpointDecoded["expires_in"], out int x);
                    if (success)
                    {
                        tokenInfo.token.Expires = DateTime.Now.AddSeconds(x);
                    }
                    else
                    {
                        tokenInfo.token.Status = TokenStatus.Invalid;
                        tokenInfo.token.StatusReason = "Failed to calculate token expiration";
                    }

                    tokenInfo.token.AccessToken = Encryption.EncryptString(Encryption.ToSecureString(tokenEndpointDecoded["access_token"]), salt);
                    tokenInfo.token.TokenType = tokenEndpointDecoded["token_type"];
                    tokenInfo.token.RefreshToken = Encryption.EncryptString(Encryption.ToSecureString(tokenEndpointDecoded["refresh_token"]), salt);
                    tokenInfo.token.Scope = tokenEndpointDecoded["scope"];
                    tokenInfo.token.UserId = tokenEndpointDecoded["user_id"];
                }

                if (isRefresh)
                {
                    authInfo = tokenInfo;

                    await GetApiKey();

                    authInfo.user = await GetUserInfo();

                    SaveTokenData(authInfo.token, tokenDir);

                    tokenInfo = authInfo;
                }
            }
            catch (WebException ex)
            {
                await Task.Run(() =>
                {
                    tokenInfo.errInfo = InvalidToken(tokenDir);
                });
            }

            return tokenInfo;
        }

        public static async Task<List<Resource>> GetAllResources(List<string> invalidCategories = null!)
        {
            var resources = new List<Resource>();

            try
            {
                var client = InitializeRestSharpClient();
                var Request = new RestRequest("resources", Method.Get);
                var Response = await client.ExecuteAsync(Request);

                if (Response.IsSuccessful)
                {
                    var Json = JObject.Parse(Response.Content!);

                    if (Json != null && Json.HasValues)
                    {
                        if (Json.ContainsKey("pagination"))
                        {
                            var pages = JsonConvert.DeserializeObject<Pagination>(Json["pagination"]!.ToString());

                            for (long i = 1; i <= pages.LastPage; i++)
                            {
                                var pageRequest = new RestRequest(string.Format("resources?page={0}", i), Method.Get);
                                var pageResponse = await client.ExecuteAsync(pageRequest);
                                var resourcePage = (List<Resource>)null!;
                                var pageJson = JObject.Parse(pageResponse.Content!);

                                if (pageJson != null && Json.HasValues)
                                {
                                    if (pageJson.ContainsKey("resources"))
                                    {
                                        if (invalidCategories != null && invalidCategories.Count > 0)
                                        {
                                            resourcePage = JsonConvert.DeserializeObject<List<Resource>>(pageJson["resources"]!.ToString())!
                                                                      .Where(x => !invalidCategories.Contains(x.Category.Title))
                                                                      .ToList();
                                        }
                                        else
                                        {
                                            resourcePage = JsonConvert.DeserializeObject<List<Resource>>(pageJson["resources"]!.ToString());
                                        }
                                    }
                                }

                                resources.AddRange(resourcePage!);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                authInfo.errInfo = "ERR005: Issue Getting Resources, Stack: \n" + ex.ToString();
            }

            return resources;
        }

        public static async Task<Resource> GetResource(string resourceId)
        {
            var resource = (Resource)null!;

            try
            {
                var client = InitializeRestSharpClient();
                var Request = new RestRequest("resources", Method.Get);
                var Response = await client.ExecuteAsync(Request);

                if (Response.IsSuccessful)
                {
                    var Json = JObject.Parse(Response.Content!);

                    if (Json != null && Json.HasValues)
                    {
                        if (Json.ContainsKey("pagination"))
                        {
                            var pages = JsonConvert.DeserializeObject<Pagination>(Json["pagination"]!.ToString());

                            for (long i = 1; i <= pages!.LastPage; i++)
                            {
                                var pageRequest = new RestRequest(string.Format("resources?page={0}", i), Method.Get);
                                var pageResponse = await client.ExecuteAsync(pageRequest);
                                var pageJson = JObject.Parse(pageResponse.Content!);
                                resource = null!;

                                if (pageJson != null && Json.HasValues)
                                {
                                    if (pageJson.ContainsKey("resources"))
                                    {
                                        resource = JsonConvert.DeserializeObject<List<Resource>>(pageJson["resources"]!.ToString())!
                                                              .Where(x => (long)Convert.ToDouble(resourceId) == x.ResourceId)
                                                              .FirstOrDefault();

                                        if (resource != null)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                authInfo.errInfo = "ERR006: Issue Getting Resource, Stack: \n" + ex.ToString();
            }

            return resource!;
        }

        public static async Task<Models.xfApi.Version> GetLatestResourceVersion(long resourceId)
        {
            //var versions = new List<xfApi.Version>();
            var pulledVer = new Models.xfApi.Version();

            try
            {
                var client = InitializeRestSharpClient();
                var Request = new RestRequest(string.Format("resources/" + resourceId + "/versions"), Method.Get);

                var Response = await client.ExecuteAsync(Request);

                if (Response.IsSuccessful)
                {
                    var Json = JObject.Parse(Response.Content!);

                    if (Json != null && Json.HasValues)
                    {
                        if (Json.ContainsKey("versions"))
                        {
                            var versions = JsonConvert.DeserializeObject<List<Models.xfApi.Version>>(Json["versions"]!.ToString());
                            pulledVer = versions!.OrderByDescending(x => x.ReleaseDate).FirstOrDefault()!;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.GetType().ToString());
            }

            return pulledVer!;
        }
        #endregion Callable Methods

        #region Methods Not Intended For Calls
        private static async Task<Browser> InitializeBrowser(Browser browser, string user, string password, string authMessage, string request, HttpListener listener, string redirectURI)
        {
            await Task.Run(() =>
            {
                browser.authUri = request;
                browser.listener = listener;
                browser.redirectUri = redirectURI;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    browser.authMessage = authMessage;
                    browser.user = user;
                    browser.password = SSEncryption.ToSecureString(password);
                    browser.auth_webBrowser.Navigate(request);
                    browser.Show();
                });
            });

            return browser;
        }

        private static async Task HandleResponse(HttpListenerContext context, HttpListener listener, Browser browser)
        {
            byte[] buffer;
            Stream responseOutput;
            HttpListenerResponse response;
            string responseString = string.Format("<html><head><meta http-equiv='refresh' " +
                                                  "content='10;url=https://www.redguides.com/community'>" +
                                                  "</head><body>Please return to the app.</body></html>");

            response = context.Response;
            buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            responseOutput = response.OutputStream;

            await Task.Run(() =>
            {
                responseOutput.WriteAsync(buffer, 0, buffer.Length);
                responseOutput.Close();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    browser.Close();
                });
            });
        }

        public static MessageBoxResult AskForAuth(string authMessage)
        {
            MessageBoxResult result = MessageBox.Show(authMessage,
                                                      "Confirm Authorization",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Exclamation);

            return result;
        }

        private static string InvalidToken(string tokenDir)
        {
            Logout(tokenDir);

            return "ERR002: Expired/Invalid Token";
        }

        public static void LoginFailed(string badDataText)
        {
            authInfo.errInfo = badDataText;
        }

        //Start R1pt1de code
        //This section of authentication was slightly modified by Ionis, but written by R1pt1de
        private static async Task GetApiKey()
        {
            string _apiKey = "";

            string apiKeyRequestBody = "";
            apiKeyRequestBody = string.Format("user_id={0}",
                    authInfo.token.UserId
                    );

            // sends the request
            HttpWebRequest apiKeyRequest = (HttpWebRequest)WebRequest.Create(string.Format("https://www.redguides.com/community/redapi/index.php/users/{0}/api?oauth_token={1}", authInfo.token.UserId, Encryption.ToInsecureString(Encryption.DecryptString(authInfo.token.AccessToken, salt))));
            apiKeyRequest.Method = "POST";
            apiKeyRequest.ContentType = "application/x-www-form-urlencoded";
            apiKeyRequest.Accept = "Accept=application/json";
            byte[] _byteVersion = Encoding.ASCII.GetBytes(apiKeyRequestBody);
            apiKeyRequest.ContentLength = _byteVersion.Length;
            Stream stream = apiKeyRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                // gets the response
                WebResponse apiKeyResponse = await apiKeyRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(apiKeyResponse.GetResponseStream()))
                {
                    string responseText = await reader.ReadToEndAsync();

                    Dictionary<string, dynamic> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseText)!;

                    _apiKey = tokenEndpointDecoded["api_key"]!;

                    if (!string.IsNullOrWhiteSpace(_apiKey))
                    {
                        authInfo.apiKey = _apiKey;
                        //apiKey = "mX1Bii-voidUUrrBiNA-LBt3DydWQlfI";
                    }
                    else
                    {
                        authInfo.errInfo = "ERR003: APIKey is already set.";
                    }
                }
                //MessageBox.Show(token.UserId + ", " + apiKey);
            }
            catch (Exception ex)
            {
                authInfo.errInfo = "ERR004: Unhandled Exception Getting APIKey, Stack: \n" + ex.ToString();
            }
        }

        public static RestClient InitializeRestSharpClient()
        {
            var client = new RestClient();
            long apiUser = authInfo.token.UserId;
            var accessToken = Encryption.ToInsecureString(Encryption.DecryptString(authInfo.token.AccessToken, salt));
            var _apiKey = authInfo.apiKey; //this will eventually come from a settings file and will need to be decrypted
            client = new RestClient("https://www.redguides.com/community/api/");
            client.AddDefaultParameter("oauth_token", accessToken, ParameterType.QueryString);
            client.AddDefaultHeader("XF-Api-Key", _apiKey);
            client.AddDefaultHeader("XF-Api-User", apiUser.ToString());
            return client;
        }

        private static async Task<Models.User> GetUserInfo()
        {
            var client = InitializeRestSharpClient();
            var User = (Models.User)null!;
            var Request = new RestRequest("?users/me", Method.Get);
            var Response = await client.ExecuteAsync(Request);

            if (Response.IsSuccessful)
            {
                var Json = JObject.Parse(Response.Content!);

                if (Json != null && Json.HasValues)
                {
                    if (Json.ContainsKey("user"))
                    {
                        User = JsonConvert.DeserializeObject<Models.User>(Json["user"]!.ToString());
                    }
                }
            }
            //await GetVVStatus("https://www.redguides.com/update/ready.json");
            return User!;
        }

        private static void SaveTokenData(AuthToken _token, String tokenDir)
        {
            try
            {
                var _tS = new TokenStorage();
                var _a = Encryption.ToInsecureString(Encryption.DecryptString(_token.AccessToken, salt));
                var _r = Encryption.ToInsecureString(Encryption.DecryptString(_token.RefreshToken, salt));

                _tS.AccessToken = Encryption2.Encrypt(Encryption2.KeyType.UserKey, _a);
                _tS.RefreshToken = Encryption2.Encrypt(Encryption2.KeyType.UserKey, _r);

                string tokenFile = tokenDir + "Token.txt";

                if (!Directory.Exists(Path.GetDirectoryName(tokenDir)))
                {
                    Directory.CreateDirectory(tokenDir);
                    string serializedToken = JsonConvert.SerializeObject(_tS);

                    using (StreamWriter lsw = new StreamWriter(tokenFile))
                    {
                        lsw.WriteLine(serializedToken);
                        lsw.WriteLine(authInfo.user.Username);
                    }
                }
                else
                {
                    List<string> newData = new List<string>();
                    string serializedToken = JsonConvert.SerializeObject(_tS);

                    newData.Add(serializedToken);
                    newData.Add(authInfo.user.Username);

                    File.WriteAllLines(tokenFile, newData);
                }

                /*
                using (StreamWriter file = File.CreateText(dataFile))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, _tS);
                }

                using (StreamWriter lsw = new StreamWriter(Path.Combine(dataPath, "user.txt")))
                {
                    if (user != null)
                    {
                        lsw.WriteLine(user.Username);
                    }
                }
                */
            }
            catch (Exception ex)
            {
                authInfo.errInfo = "ERR007: Unhandled Exception Saving Token, Stack: \n" + ex.ToString();
            }
        }

        private static Tuple<Boolean, AuthToken, String> GetStoredTokenData(string tokenDir)
        {
            string response = "Success!";

            try
            {
                var _tS = new TokenStorage();
                var _token = new AuthToken();

                var dataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RGL\";
                string dataFile = Path.Combine(dataPath, @"Token.txt");

                if (File.Exists(dataFile))
                {
                    List<string> allLines = File.ReadAllLines(dataFile).ToList();

                    _tS = JsonConvert.DeserializeObject<TokenStorage>(allLines[0]);

                    var _aUs = Encryption2.Decrypt(_tS!.AccessToken);
                    var _rUs = Encryption2.Decrypt(_tS.RefreshToken);

                    _token.AccessToken = Encryption.EncryptString(Encryption.ToSecureString(_aUs), salt);
                    _token.RefreshToken = Encryption.EncryptString(Encryption.ToSecureString(_rUs), salt);
                }
                else
                {
                    response = "Directory Doesn't Exist";
                }

                return Tuple.Create(response.Equals("Success!") ? true : false, _token, response);
            }
            catch (Exception ex)
            {
                response = "ERR002: Expired/Invalid Token";

                return Tuple.Create(false, new AuthToken(), InvalidToken(tokenDir));
            }
        }

        private static string randomDataBase64url(uint length)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return base64urlencodeNoPadding(bytes);
        }

        private static byte[] sha256(string inputStirng)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
            SHA256Managed sha256 = new SHA256Managed();
            return sha256.ComputeHash(bytes);
        }

        private static string base64urlencodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }
        //End R1p code
        #endregion Methods Not Intended For Calls

        #endregion Authorization Functionality
    }
}
