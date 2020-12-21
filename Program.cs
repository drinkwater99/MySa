using System;
using Amazon.Runtime;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using System.Threading.Tasks;
using Amazon;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace MySa
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string clientId = "19efs8tgqe942atbqmot5m36t3";
            string poolId = "us-east-1_GUFWfhI7g";

            string userId = "user"; // your mysa login username
            string password = "password"; // your mysa login password

            var provider = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), RegionEndpoint.USEast1);
            var userPool = new CognitoUserPool(poolId, clientId, provider);
            var user = new CognitoUser(userId, clientId, userPool, provider);

            var authResponse = await user.StartWithSrpAuthAsync(new InitiateSrpAuthRequest()
            {
                Password = password
            }).ConfigureAwait(false);

            using (var client = new HttpClient())
            {
                var url = "https://app-prod.mysa.cloud/users/readingsForUser";
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + authResponse.AuthenticationResult.IdToken);
                
                var response = await client.GetStringAsync(url); // response contains the json response, parse and use as needed

                dynamic obj = JsonConvert.DeserializeObject(response);

                Console.WriteLine("JSON: " + obj);
            }
        }
    }
}
