using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FD_Rest_Api_Implementation_New.Models
{
    public class SecureIdEnrollmentResponseModel
    {
        public string clientRequestId { get; set; }
        public string apiTraceId { get; set; }
        public string ipgTransactionId { get; set; }
        public string orderId { get; set; }
        public string merchantTransactionId { get; set; }
        public string methodForm { get; set; }
        public string secure3dTransId { get; set; }
        public string transactionReferenceNumber { get; set; }
        public string threeDSMethodData { get; set; }
        public string termURL { get; set; }
        public string acsURL { get; set; }
        public string cReq { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public string sessiondata { get; set; }
        public string ThreeDSversion { get; set; }
        public string payerAuthenticationRequest { get; set; }
        public string merchantData { get; set; }

        public SecureIdEnrollmentResponseModel toSecureIdEnrollmentResponseModel(string response)
        {
            JObject jObject = JObject.Parse(response);
            SecureIdEnrollmentResponseModel model = new SecureIdEnrollmentResponseModel();
            model.clientRequestId = jObject["clientRequestId"].Value<string>();
            model.apiTraceId = jObject["apiTraceId"].Value<string>();
            model.ipgTransactionId = jObject["ipgTransactionId"].Value<string>();
            model.orderId = jObject["orderId"].Value<string>();
            model.merchantTransactionId = jObject["merchantTransactionId"].Value<string>();

            if (response.Contains("termURL"))
            {
                if (response.Contains("1.0"))
                {
                    model.termURL = jObject["authenticationResponse"]["params"]["termURL"].Value<string>();
                    model.acsURL = jObject["authenticationResponse"]["params"]["acsURL"].Value<string>();
                    model.payerAuthenticationRequest = jObject["authenticationResponse"]["params"]["payerAuthenticationRequest"].Value<string>();
                    model.merchantData = jObject["authenticationResponse"]["params"]["merchantData"].Value<string>();
                    model.ThreeDSversion = jObject["authenticationResponse"]["version"].Value<string>();
                }
                else
                {
                    model.termURL = jObject["authenticationResponse"]["params"]["termURL"].Value<string>();
                    model.acsURL = jObject["authenticationResponse"]["params"]["acsURL"].Value<string>();
                    model.cReq = jObject["authenticationResponse"]["params"]["cReq"].Value<string>();
                    model.ThreeDSversion = jObject["authenticationResponse"]["version"].Value<string>();
                }
                if (response.Contains("sessiondata"))
                {
                    model.acsURL = jObject["authenticationResponse"]["params"]["sessiondata"].Value<string>();
                }
            }
            else if (response.Contains("error"))
            {
                model.code = jObject["error"]["code"].Value<string>();
                model.message = jObject["error"]["message"].Value<string>();
            }
            else
            {
                model.secure3dTransId = jObject["authenticationResponse"]["secure3dMethod"]["secure3dTransId"].Value<string>();
                model.methodForm = jObject["authenticationResponse"]["secure3dMethod"]["methodForm"].Value<string>();
            }

            return model;
        }
    }
}