using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace FD_Rest_Api_Implementation_New.Models
{
    public class PaymentModel
    {
        public string requestType { get; set; }
        public string merchantTransactionId { get; set; }
        public string total { get; set; }
        public string currency { get; set; }
        public string cardnumber { get; set; }
        public string securityCode { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public string secure { get; set; }
        public string orderid { get; set; }
    }

    public class Servic
    {
        string payload;
        public String executeHTTPMethod(String Url, String parms, string method, SecureIdEnrollmentResponseModel model)
        {
            //string body = String.Empty;
            //HttpWebRequest request = WebRequest.Create("https://prod.emea.api.fiservapps.com/sandbox/ipp/payments-gateway/v2/" + Url) as HttpWebRequest;
            //request.Method = method;
            //request.ContentType = "application/json";
            //string clientrequestid = Uuid.NewUuid().ToString();
            //long timeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            //request.Headers.Add("Client-Request-Id", clientrequestid);
            //request.Headers.Add("Api-Key", "UtaCx0qa6nvT8kiZl4tXVZAoDlOIMvbJ");
            //request.Headers.Add("Timestamp", timeStamp.ToString());
            //string values = "UtaCx0qa6nvT8kiZl4tXVZAoDlOIMvbJ" + clientrequestid + timeStamp.ToString() + parms;
            //request.Headers.Add("Message-Signature", HashHMAC(Encoding.ASCII.GetBytes("3pnznNhqr5zGFuS3"), Encoding.ASCII.GetBytes(values)));

            string body = String.Empty;
            HttpWebRequest request = WebRequest.Create("https://prod.emea.api.fiservapps.com/sandbox/ipp/payments-gateway/v2/" + Url) as HttpWebRequest;
            //HttpWebRequest request = WebRequest.Create("https://prod.emea.api.fiservapps.com/ipp/payments-gateway/v2/" + Url) as HttpWebRequest;
            request.Method = method;
            request.ContentType = "application/json";
            string clientrequestid = Uuid.NewUuid().ToString();
            long timeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            request.Headers.Add("Client-Request-Id", clientrequestid);

            request.Headers.Add("Api-Key", "guu7gs1XhGsCkq0YMcFsbXBandW8jUAu"); //Faisal
            //request.Headers.Add("Api-Key", "7kbCOzZyXSZv0ufnLnmAuPrz6uwz4l48");

            request.Headers.Add("Timestamp", timeStamp.ToString());
            string values = "guu7gs1XhGsCkq0YMcFsbXBandW8jUAu" + clientrequestid + timeStamp.ToString() + parms;

            //string hash = HashHMAC(Encoding.ASCII.GetBytes("G5msufcHAWToZ9OuKGfDs9kquBxgQPUiauScvqqhzWu"), Encoding.ASCII.GetBytes(values));
            request.Headers.Add("Message-Signature", HashHMAC(Encoding.ASCII.GetBytes("m0CO2odX602aMeqJSbQsgjqYGLsNBU1NgGQ7xLkeYhM"), Encoding.ASCII.GetBytes(values)));
            //request.Headers.Add("Message-Signature", HashHMAC(Encoding.ASCII.GetBytes("G5msufcHAWToZ9OuKGfDs9kquBxgQPUiauScvqqhzWu"), Encoding.ASCII.GetBytes(values)));

            //Rahaman
            //request.Headers.Add("Message-Signature", HashHMAC(Encoding.ASCII.GetBytes("ypzs1qwER1RAFg2i0znc33TwdygQOuu0csrL4JtHvSM"), Encoding.ASCII.GetBytes(values)));


            try
            {
                if ((method == "PUT" || method == "POST" || method == "PATCH") &&
                !String.IsNullOrEmpty(parms))
                {
                    byte[] utf8bytes = Encoding.UTF8.GetBytes(parms);
                    request.ContentLength = utf8bytes.Length;
                    using (Stream postStream = request.GetRequestStream())
                    {
                        postStream.Write(utf8bytes, 0, utf8bytes.Length);
                    }
                }

                try
                {
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        body = reader.ReadToEnd();
                    }
                }
                catch (WebException wex)
                {
                    StreamReader reader = new StreamReader(wex.Response.GetResponseStream());
                    body = reader.ReadToEnd();
                }

                return body;
            }
            catch (Exception ex)
            {
                return ex.Message + "\n\naddress:\n" + request.Address.ToString() + "\n\nheader:\n" + request.Headers.ToString() + "data submitted:\n" + parms;
            }

        }

        public string Payloads(string roles, PaymentModel model, SecureIdEnrollmentResponseModel modell)
        {
            NameValueCollection nvc = new NameValueCollection();

            if (roles == "PaymentCardSaleTransaction")
            {
                nvc.Add("requestType", "PaymentCardSaleTransaction");
                nvc.Add("merchantTransactionId", generateSampleId());
                nvc.Add("transactionAmount.total", model.total);
                //nvc.Add("orderId", "R-" + Uuid.NewUuid().ToString());
                nvc.Add("transactionAmount.currency", model.currency);
                nvc.Add("paymentMethod.paymentCard.number", model.cardnumber);
                //nvc.Add("paymentMethod.paymentCard.securityCode", model.securityCode);
                nvc.Add("paymentMethod.paymentCard.expiryDate.month", model.month);
                nvc.Add("paymentMethod.paymentCard.expiryDate.year", model.year);

                //nvc.Add("createToken.value", generateSampleId());
                //nvc.Add("createToken.reusable", "true");
                //nvc.Add("createToken.declineDuplicates", "false");
            }
            else if (roles == "PaymentCardPreAuthTransaction")
            {
                nvc.Add("requestType", "PaymentCardPreAuthTransaction");
                nvc.Add("merchantTransactionId", generateSampleId());
                nvc.Add("transactionAmount.total", model.total);
                //nvc.Add("orderId", "R-" + Uuid.NewUuid().ToString());
                nvc.Add("transactionAmount.currency", model.currency);
                nvc.Add("paymentMethod.paymentCard.number", model.cardnumber);
                nvc.Add("paymentMethod.paymentCard.securityCode", model.securityCode);
                nvc.Add("paymentMethod.paymentCard.expiryDate.month", model.month);
                nvc.Add("paymentMethod.paymentCard.expiryDate.year", model.year);
            }
            else if (roles == "3DSPaymentCardSaleTransaction")
            {
                nvc.Add("requestType", "PaymentCardSaleTransaction");
                nvc.Add("merchantTransactionId", generateSampleId());
                nvc.Add("transactionAmount.total", model.total);
                nvc.Add("transactionAmount.currency", model.currency);
                //nvc.Add("orderId", "R-" + Uuid.NewUuid().ToString());
                nvc.Add("paymentMethod.paymentCard.number", model.cardnumber);
                nvc.Add("paymentMethod.paymentCard.securityCode", model.securityCode);
                nvc.Add("paymentMethod.paymentCard.expiryDate.month", model.month);
                nvc.Add("paymentMethod.paymentCard.expiryDate.year", model.year);
                nvc.Add("authenticationRequest.authenticationType", "Secure3D21AuthenticationRequest");
                nvc.Add("authenticationRequest.termURL", "https://localhost:44387/Home/process3dSecure");
                nvc.Add("authenticationRequest.methodNotificationURL", "https://localhost:44387/Home/process3dSecureMethodNotification?transactionReferenceNumber=" + Uuid.NewUuid().ToString());
                nvc.Add("authenticationRequest.challengeIndicator", "04");
                nvc.Add("authenticationRequest.challengeWindowSize", "05");

                nvc.Add("createToken.value", generateSampleId());
                nvc.Add("createToken.reusable", "true");
                nvc.Add("createToken.declineDuplicates", "false");


            }
            else if (roles == "Secure3D21AuthenticationUpdateRequest")
            {
                nvc.Add("authenticationType", "Secure3D21AuthenticationUpdateRequest");
                nvc.Add("storeId", "811187409");
                nvc.Add("billingAddress.company", "Test Company");
                nvc.Add("billingAddress.address1", "5565 Glenridge Conn");
                nvc.Add("billingAddress.address2", "Suite 123");
                nvc.Add("billingAddress.city", "Atlanta");
                nvc.Add("billingAddress.region", "Georgia");
                nvc.Add("billingAddress.postalCode", "30342");
                nvc.Add("billingAddress.country", "USA");
                //nvc.Add("securityCode",model.securityCode);
                nvc.Add("methodNotificationStatus", "RECEIVED");
            }
            else if (roles == "Secure3D21AuthenticationUpdateRequest2")
            {
                if (modell.ThreeDSversion == "1.0")
                {
                    nvc.Add("authenticationType", "Secure3D10AuthenticationUpdateRequest");
                }
                else
                {
                    nvc.Add("authenticationType", "Secure3D21AuthenticationUpdateRequest");
                }
                nvc.Add("storeId", "811187409");
                nvc.Add("billingAddress.company", "Test Company");
                nvc.Add("billingAddress.address1", "5565 Glenridge Conn");
                nvc.Add("billingAddress.address2", "Suite 123");
                nvc.Add("billingAddress.city", "Atlanta");
                nvc.Add("billingAddress.region", "Georgia");
                nvc.Add("billingAddress.postalCode", "30342");
                nvc.Add("billingAddress.country", "USA");
                //nvc.Add("securityCode",model.securityCode);
                if (modell.ThreeDSversion == "1.0")
                {
                    nvc.Add("acsResponse.PaRes", modell.payerAuthenticationRequest);
                    nvc.Add("acsResponse.MD", modell.merchantData);
                }
                else
                {
                    nvc.Add("acsResponse.cRes", modell.cReq);
                }
            }
            else if (roles == "CREATEURL")
            {
                nvc.Add("transactionAmount.total", "10.67");
                nvc.Add("transactionAmount.currency", "AED");
                nvc.Add("orderId", "12333ll3333");
                nvc.Add("transactionType", "SALE");
                nvc.Add("transactionNotificationURL", "https");
                nvc.Add("expiration", "4102358400");
                nvc.Add("authenticateTransaction","true");
                nvc.Add("dynamicMerchantName", "FAB");
                nvc.Add("invoiceNumber", "29062021-012");
                nvc.Add("purchaseOrderNumber", "29062021-032");
                nvc.Add("hostedPaymentPageText", "FAB");
                nvc.Add("billing.name", "Test Name");
                nvc.Add("billing.birthDate", "1980-01-31");
                nvc.Add("billing.contact.phone", "1234567890");
                nvc.Add("billing.contact.mobilePhone", "1234567890");
                nvc.Add("billing.contact.fax", "1234567890");
                nvc.Add("billing.contact.email", "muhammad.saghir@bankfab.com");
            }
            else if (roles == "ReturnTransaction")
            {
                nvc.Add("requestType", "ReturnTransaction");
                nvc.Add("transactionAmount.total", model.total);
                nvc.Add("transactionAmount.currency", model.currency);

            }
            else if (roles == "PaymentTokenSaleTransaction")
            {
                nvc.Add("requestType", "PaymentTokenSaleTransaction");
                nvc.Add("merchantTransactionId", generateSampleId());
                nvc.Add("transactionAmount.total", model.total);

                if (string.IsNullOrEmpty(model.total))
                {
                    nvc.Add("transactionAmount.total", "100");
                }

                nvc.Add("transactionAmount.currency", model.currency);

                //nvc.Add("paymentMethod.paymentToken.value", "4D4770E945");

                if(string.IsNullOrEmpty(model.cardnumber))
                {
                    nvc.Add("paymentMethod.paymentToken.value", "D1AFEAAF71");
                }
                else
                {
                    nvc.Add("paymentMethod.paymentToken.value", model.cardnumber);
                }

                nvc.Add("paymentMethod.paymentToken.function", "DEBIT");
                nvc.Add("paymentMethod.paymentToken.securityCode", "002");

                nvc.Add("storedCredentials.sequence", "FIRST");
                nvc.Add("storedCredentials.scheduled", "false");

            }
            payload = JsonHelper.BuildJsonFromNVC(nvc);
            return payload;
        }

        public string generateSampleId()
        {
            return Uuid.NewUuid().ToString().Replace("-", string.Empty).Substring(0, 10);
        }

        public static string HashHMAC(byte[] secretKey, byte[] data)
        {
            HMACSHA256 hmac = new HMACSHA256(secretKey);
            byte[] hmacBytes = hmac.ComputeHash(data);
            //String hexHmac = ByteArrayToString(hmacBytes);

            //// Convert to Base64
            //byte[] hexBytes = Encoding.ASCII.GetBytes(hexHmac);
            String signature = Convert.ToBase64String(hmacBytes);
            return signature;
        }

        private static string ByteArrayToString(byte[] input)
        {
            int i;
            StringBuilder output = new StringBuilder(input.Length);
            for (i = 0; i < input.Length; i++)
            {
                output.Append(input[i].ToString("x2"));
            }
            return output.ToString();
        }

        public String executeHTTPMethodd(String Url, String parms, string method, SecureIdEnrollmentResponseModel model)
        {
            string body = String.Empty;
            HttpWebRequest request = WebRequest.Create("https://prod.api.firstdata.com/sandbox/ipp/v1/" + Url) as HttpWebRequest;
            request.Method = method;
            request.ContentType = "application/json";
            string clientrequestid = Uuid.NewUuid().ToString();
            long timeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            request.Headers.Add("Client-Request-Id", clientrequestid);
            request.Headers.Add("AppKey", "UtaCx0qa6nvT8kiZl4tXVZAoDlOIMvbJ");
            request.Headers.Add("Timestamp", timeStamp.ToString());
            string values = "UtaCx0qa6nvT8kiZl4tXVZAoDlOIMvbJ" + clientrequestid + timeStamp.ToString() + parms;
            request.Headers.Add("Message-Signature", HashHMAC(Encoding.ASCII.GetBytes("3pnznNhqr5zGFuS3"), Encoding.ASCII.GetBytes(values)));

            try
            {
                if ((method == "PUT" || method == "POST" || method == "PATCH") &&
                !String.IsNullOrEmpty(parms))
                {
                    byte[] utf8bytes = Encoding.UTF8.GetBytes(parms);
                    request.ContentLength = utf8bytes.Length;
                    using (Stream postStream = request.GetRequestStream())
                    {
                        postStream.Write(utf8bytes, 0, utf8bytes.Length);
                    }
                }

                try
                {
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        body = reader.ReadToEnd();
                    }
                }
                catch (WebException wex)
                {
                    StreamReader reader = new StreamReader(wex.Response.GetResponseStream());
                    body = reader.ReadToEnd();
                }

                return body;
            }
            catch (Exception ex)
            {
                return ex.Message + "\n\naddress:\n" + request.Address.ToString() + "\n\nheader:\n" + request.Headers.ToString() + "data submitted:\n" + parms;
            }

        }

    }
}