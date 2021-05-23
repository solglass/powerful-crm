using Microsoft.Extensions.Options;
using powerful_crm.Business.Models;
using powerful_crm.Core.PayPal.Models;
using powerful_crm.Core.Settings;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Business
{
    public class PayPalRequestService : IPayPalRequestService
    {
        private const string _baseUrl = "https://api-m.sandbox.paypal.com";
        private const string _auth = "v1/oauth2/token";
        private readonly string _username;
        private readonly string _password;
        public PayPalRequestService(IOptions<PayPalSettings> options)
        {
            _username = options.Value.USERNAME;
            _password = options.Value.PASSWORD;
        }
        public Invoice CreateDraftInvoice()
        {
            throw new NotImplementedException();
        }

        public string GetToken()
        {
            var client = new RestClient(_baseUrl);           
            var request = new RestRequest($"{_auth}", Method.POST);
            client.Authenticator = new HttpBasicAuthenticator(_username, _password);
            request.AddParameter("grant_type", "client_credentials");
            var response = client.Execute<PayPalAccessToken>(request);
            var token = response.Data.access_token;
            return response.Data.access_token;
        }
    }
}
