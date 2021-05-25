using Microsoft.Extensions.Options;
using powerful_crm.Business.Models;
using powerful_crm.Core.PayPal.Models;
using powerful_crm.Core.Settings;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace powerful_crm.Business
{
    public class PayPalRequestService : IPayPalRequestService
    {
        private const string _baseUrl = "https://api-m.sandbox.paypal.com";
        private const string _auth = "v1/oauth2/token";
        private const string _batchPayout = "v1/payments/payouts";
        private readonly string _username;
        private readonly string _password;
        private string _token;
        private RestClient _client;
        public PayPalRequestService(IOptions<PayPalSettings> options)
        {
            _username = options.Value.USERNAME;
            _password = options.Value.PASSWORD;
            _client = new RestClient(_baseUrl);
            _token = GetToken();
            _client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(_token, "Bearer");
        }
        public Invoice CreateDraftInvoice()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Payout>> CreateBatchPayoutAsync(PayoutInputModel inputModel)
        {
            var request = new RestRequest($"{ _batchPayout }", Method.POST);
            request.AddParameter("application/json", JsonSerializer.Serialize(inputModel), ParameterType.RequestBody);
            var response = await _client.ExecuteAsync<List<Payout>>(request);
            return response.Data;
        }

        public string GetToken()
        {         
            var request = new RestRequest($"{_auth}", Method.POST);
            _client.Authenticator = new HttpBasicAuthenticator(_username, _password);
            request.AddParameter("grant_type", "client_credentials");
            var response = _client.Execute<PayPalAccessToken>(request);
            return response.Data.access_token;
        }
    }
}
