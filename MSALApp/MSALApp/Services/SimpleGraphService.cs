using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;

namespace MSALApp.Services
{
    public class SimpleGraphService
    {
        static string userId = string.Empty;
        public async Task<string> GetNameAsync()
        {
            using (var client = new HttpClient())
            {
                var token = await SecureStorage.GetAsync("AccessToken");

                if (!string.IsNullOrEmpty(token))
                {
                    var message = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
                    message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var response = await client.SendAsync(message);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var data = (JObject)JsonConvert.DeserializeObject(json);

                        if (data.ContainsKey("id"))
                        {
                            userId = data["id"].Value<string>();
                        }

                        if (data.ContainsKey("givenName"))
                            return data["givenName"].Value<string>();
                        else
                            return "Mr. No Name";
                        //currentUser = JsonConvert.DeserializeObject<User>(json);
                    }
                }
                else
                {
                    return "Token Invalid";
                }
            }

            return "Name unknown";
        }

        public async Task<byte[]> GetProfilePicture()
        {
            using (var client = new HttpClient())
            {
                var token = await SecureStorage.GetAsync("AccessToken");

                if (!string.IsNullOrEmpty(token))
                {
                    var message = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me/photo/$value");
                    message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    //message.Headers.Add("Accept", "application/json");

                    var response = await client.SendAsync(message);

                    if (response.IsSuccessStatusCode)
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        byte[] bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, (int)stream.Length);
                        return bytes;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
    }
}
