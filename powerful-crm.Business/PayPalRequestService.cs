using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using powerful_crm.Business.Models;
using powerful_crm.Core.CustomExceptions;
using powerful_crm.Core.PayPal.Models;
using powerful_crm.Core.Settings;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
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
        private decimal _comissionPercent;
        public PayPalRequestService(IOptions<PayPalSettings> options)
        {
            _username = options.Value.POWERFUL_CRM_PAYPAL_USERNAME;
            _password = options.Value.POWERFUL_CRM_PAYPAL_PASSWORD;
            _client = new RestClient(_baseUrl);
            _token = GetToken();
            _client.Authenticator = new JwtAuthenticator(_token);
            _comissionPercent = options.Value.POWERFUL_CRM_COMMISSION_PERCENT;
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
            request.AddParameter("application/json", System.Text.Json.JsonSerializer.Serialize(order), ParameterType.RequestBody);
            var response = await _client.ExecuteAsync<OrderOutPutModel>(request);                   
            return response.Data;          
        }

        public void TakeComission(ref PayoutInputModel inputModel)
        {
            inputModel.Items[0].Amount.Value = inputModel.Items[0].Amount.Value - inputModel.Items[0].Amount.Value*_comissionPercent / 100;
        }
    }
}
