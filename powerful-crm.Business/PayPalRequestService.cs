using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using powerful_crm.Business.Models;
using powerful_crm.Core.CustomExceptions;
using powerful_crm.Core.PayPal.Models;
using powerful_crm.Core.Settings;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
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

        public async Task<Object> CreateBatchPayoutAsync(PayoutInputModel inputModel)
        {
            var request = new RestRequest($"{ _batchPayout }", Method.POST);
            var requestBody = JsonConvert.SerializeObject(inputModel);
            request.AddParameter("application/json", requestBody, ParameterType.RequestBody);
            var response = await _client.ExecuteAsync(request);
            if (!response.IsSuccessful)
            {
                var deserializedResponse = JsonConvert.DeserializeObject<JObject>(response.Content);
                throw new PayPalException($"{deserializedResponse }");
            }
            
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var deserializedResponse = JsonConvert.DeserializeObject<JObject>(response.Content);
                return deserializedResponse;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var deserializedResponse = JsonConvert.DeserializeObject<PayoutResponse>(response.Content);
                return deserializedResponse;
            
            }
             return response.StatusCode ;
  
        }

        private string GetToken()
        {         
            var request = new RestRequest($"{_auth}", Method.POST);
            _client.Authenticator = new HttpBasicAuthenticator(_username, _password);
            request.AddParameter("grant_type", "client_credentials");
            var response = _client.Execute<PayPalAccessToken>(request);
            return response.Data.access_token;
        }
        public async Task<BraintreeHttp.HttpResponse> CreateOrder(bool debug = false)
        {
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(BuildRequestBody());
            //3. Call PayPal to set up a transaction
            var response = await PayPalClient.client().Execute(request);

            if (debug)
            {
                var result = response.Result<Order>();
                Console.WriteLine("Status: {0}", result.Status);
                Console.WriteLine("Order Id: {0}", result.Id);
                Console.WriteLine("Intent: {0}", result.Intent);
                Console.WriteLine("Links:");
                foreach (LinkDescription link in result.Links)
                {
                    Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel, link.Href, link.Method);
                }
                AmountWithBreakdown amount = result.PurchaseUnits[0].Amount;
                Console.WriteLine("Total Amount: {0} {1}", amount.CurrencyCode, amount.Value);
            }

            return response;
        }
        private static OrderRequest BuildRequestBody()
        {
            OrderRequest orderRequest = new OrderRequest()
            {
                Intent = "CAPTURE",

                ApplicationContext = new ApplicationContext
                {
                    BrandName = "EXAMPLE INC",
                    LandingPage = "BILLING",
                    UserAction = "CONTINUE",
                    ShippingPreference = "SET_PROVIDED_ADDRESS"
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
        {
          new PurchaseUnitRequest{
            ReferenceId =  "PUHF",
            Description = "Sporting Goods",
            CustomId = "200",
            SoftDescriptor = "CUST-HighFashions",
            Amount = new AmountWithBreakdown
            {
              CurrencyCode = "USD",
              Value = "230.00",
              Breakdown = new AmountBreakdown
              {
                ItemTotal = new Money
                {
                  CurrencyCode = "USD",
                  Value = "180.00"
                },
                Shipping = new Money
                {
                  CurrencyCode = "USD",
                  Value = "30.00"
                },
                Handling = new Money
                {
                  CurrencyCode = "USD",
                  Value = "10.00"
                },
                TaxTotal = new Money
                {
                  CurrencyCode = "USD",
                  Value = "20.00"
                },
                ShippingDiscount = new Money
                {
                  CurrencyCode = "USD",
                  Value = "10.00"
                }
              }
            },
            Items = new List<Item>
            {             
              new Item
              {
                Name = "Shoes",
                Description = "Running, Size 10.5",
                Sku = "sku02",
                UnitAmount = new Money
                {
                  CurrencyCode = "USD",
                  Value = "45.00"
                },
                Tax = new Money
                {
                  CurrencyCode = "USD",
                  Value = "5.00"
                },
                Quantity = "2",
                Category = "PHYSICAL_GOODS"
              }
            },
            Shipping = new ShippingDetails
            {
              Name = new Name
              {
                FullName = "John Doe"
              },
              AddressPortable = new AddressPortable
              {
                AddressLine1 = "123 Townsend St",
                AddressLine2 = "Floor 6",
                AdminArea2 = "San Francisco",
                AdminArea1 = "CA",
                PostalCode = "94107",
                CountryCode = "US"
              }
            }
          }
        }
            };

            return orderRequest;
        }

    }
}
