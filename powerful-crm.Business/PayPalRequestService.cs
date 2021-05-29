using Microsoft.Extensions.Options;
using powerful_crm.Business.Models;
using powerful_crm.Core.CustomExceptions;
using powerful_crm.Core.PayPal.Models;
using powerful_crm.Core.Settings;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace powerful_crm.Business
{
    public class PayPalRequestService : IPayPalRequestService
    {
        private const string _baseUrl = "https://api-m.sandbox.paypal.com";
        private const string _auth = "v1/oauth2/token";
        private const string _batchPayout = "v1/payments/payouts";
        private const string _order = "v2/checkout/orders";
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
            _client.Authenticator = new JwtAuthenticator(_token);
        }
        public Invoice CreateDraftInvoice()
        {
            throw new NotImplementedException();
        }

        public async Task<PayoutResponse> CreateBatchPayoutAsync(PayoutInputModel inputModel)
        {
            var request = new RestRequest($"{ _batchPayout }", Method.POST);
            var requestBody = JsonConvert.SerializeObject(inputModel);
            request.AddParameter("application/json", requestBody, ParameterType.RequestBody);
            var response = await _client.ExecuteAsync(request);
            if (!response.IsSuccessful || response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var deserializedResponseError = JsonConvert.DeserializeObject<JObject>(response.Content);
                throw new PayPalException(deserializedResponseError.ToString());
            }

            var deserializedResponse = JsonConvert.DeserializeObject<PayoutResponse>(response.Content);
            return deserializedResponse;


        }

        private string GetToken()
        {
            var request = new RestRequest($"{_auth}", Method.POST);
            _client.Authenticator = new HttpBasicAuthenticator(_username, _password);
            request.AddParameter("grant_type", "client_credentials");
            var response = _client.Execute<PayPalAccessToken>(request);
            return response.Data.access_token;
        }
        public async Task<OrderOutPutModel> CreateOrder(PayPalOrderInputModel payPalOrderInputModel)
        {
            var order = new Order
            {
                Intent = "CAPTURE",
                Purchase_units = new List<PayPalPurchase>{
                    new PayPalPurchase
                    {
                        Amount = new Amount
                        {
                            Currency_code = payPalOrderInputModel.Currency,
                            Value = Convert.ToString(payPalOrderInputModel.Amount)
                        }
                    }
                }
            };
            var request = new RestRequest($"{_order}", Method.POST);     
            request.AddParameter("application/json", JsonSerializer.Serialize(order), ParameterType.RequestBody);
            var response = await _client.ExecuteAsync<OrderOutPutModel>(request);                   
            return response.Data;          
        }                  
    }
}
