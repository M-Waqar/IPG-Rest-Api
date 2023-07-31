using FD_Rest_Api_Implementation_New.Models;
using System;
using System.Web.Mvc;

namespace FD_Rest_Api_Implementation_New.Controllers
{
    public class HomeController : Controller
    {
        string json;
        string response;
        static SecureIdEnrollmentResponseModel modell = new SecureIdEnrollmentResponseModel();
        Servic ser = new Servic();

        public ActionResult Index()
        {
            //string url = "transactions?allianceCode=&startDate=2021-02-01&endDate=2021-02-28";
            //response = ser.executeHTTPMethodd(url, "", "GET", null);
            return View();
        }


        public ActionResult Token()
        {
            //string url = "transactions?allianceCode=&startDate=2021-02-01&endDate=2021-02-28";
            //response = ser.executeHTTPMethodd(url, "", "GET", null);
            return View();
        }

        [HttpPost]
        public ActionResult TokenSale(PaymentModel model)
        {
            try
            {
                json = ser.Payloads("PaymentTokenSaleTransaction", model, null);
                json = json.Replace("\"false\"", "false");
                json = json.Replace("\"true\"", "true");
                response = ser.executeHTTPMethod("payments", json, "POST", null);

                ViewBag.Payload = json;
                ViewBag.Operation = "TOKEN SALE";
                ViewBag.Method = "POST";
                ViewBag.RequestUrl = "https://prod.emea.api.fiservapps.com/sandbox/ipp/payments-gateway/v2/payments";
                ViewBag.Response = JsonHelper.prettyPrint(response);
            }
            catch (Exception e)
            {

            }

            return View();
        }


        [HttpPost]
        public ActionResult Sale(PaymentModel model)
        {
            if (model.secure == "true")
            {

                json = ser.Payloads("3DSPaymentCardSaleTransaction", model, null);

                json = json.Replace("\"false\"", "false");
                json = json.Replace("\"true\"", "true");

                response = ser.executeHTTPMethod("payments", json, "POST", null);

                try
                {
                    modell = modell.toSecureIdEnrollmentResponseModel(response);

                    if (modell.code != null && modell.message != null)
                    {
                        ViewBag.Payload = json;
                        ViewBag.Operation = "3DSPaymentCardSaleTransaction";
                        ViewBag.Method = "POST";
                        ViewBag.RequestUrl = "https://prod.emea.api.fiservapps.com/sandbox/ipp/payments-gateway/v2/payments";
                        ViewBag.Response = JsonHelper.prettyPrint(response);
                        return View();
                    }
                }
                catch (Exception e)
                {

                }

                if (modell.acsURL != null)
                {
                    if (modell.ThreeDSversion == "1.0")
                    {
                        TempData["3dreponse"] = "<form  method=\"post\" action=\"" + modell.acsURL + "\" name=\"frm\" id=\"frm\"><input value=\"" + modell.payerAuthenticationRequest + "\" name=\"PaReq\" type=\"hidden\"/><input value=\"" + modell.cReq + "\" name=\"CReq\" type=\"hidden\"/><input value=\"" + modell.termURL + "\" name=\"TermUrl\" type=\"hidden\"/><input value=\"" + modell.merchantData + "\" name=\"MD\" type=\"hidden\"/></form><script xmlns=\"http://www.w3.org/1999/xhtml\" type=\"text/javascript\">document.getElementById(\"frm\").submit();</script>";
                    }
                    else
                    {
                        TempData["3dreponse"] = "<form  method=\"post\" action=\"" + modell.acsURL + "\" name=\"frm\" id=\"frm\"><input value=\"" + modell.cReq + "\" name=\"CReq\" type=\"hidden\"/><input value=\"\" name=\"threeDSSessionData\" type=\"hidden\"/></form><script xmlns=\"http://www.w3.org/1999/xhtml\" type=\"text/javascript\">document.getElementById(\"frm\").submit();</script>";
                    }
                }
                else
                {
                    TempData["3dreponse"] = modell.methodForm;
                }

                return RedirectToAction("Viewprocess3dSecureMethodNotification", "Home");
            }
            else
            {
                //json = ser.Payloads("PaymentCardPreAuthTransaction", model, null);//PaymentCardPreAuthTransaction //PaymentCardSaleTransaction
                json = ser.Payloads("PaymentCardSaleTransaction", model, null);
                response = ser.executeHTTPMethod("payments", json, "POST", null);
                ViewBag.Payload = json;
                ViewBag.Operation = "SALE";
                ViewBag.Method = "POST";
                ViewBag.RequestUrl = "https://prod.emea.api.fiservapps.com/sandbox/ipp/payments-gateway/v2/payments";
                ViewBag.Response = JsonHelper.prettyPrint(response);
            }

            return View();
        }

        public ActionResult process3dSecureMethodNotification(string transactionReferenceNumber)
        {
            TempData["3dreponse"] = "";
            modell.transactionReferenceNumber = transactionReferenceNumber;
            modell.threeDSMethodData = Request.Params["threeDSMethodData"].ToString();
            return RedirectToAction("Viewprocess3dSecureMethodNotification", "Home");
        }

        public ActionResult Viewprocess3dSecureMethodNotification()
        {
            if (modell.acsURL == null)
            {
                json = ser.Payloads("Secure3D21AuthenticationUpdateRequest", null, modell);
                response = ser.executeHTTPMethod("payments/" + modell.ipgTransactionId, json, "PATCH", modell);

                try
                {
                    modell = modell.toSecureIdEnrollmentResponseModel(response);

                    if (modell.code != null && modell.message != null)
                    {
                        ViewBag.Payload = json;
                        ViewBag.Operation = "Secure3D21AuthenticationUpdateRequest";
                        ViewBag.Method = "PATCH";
                        ViewBag.RequestUrl = "https://prod.emea.api.fiservapps.com/sandbox/ipp/payments-gateway/v2/payments/"+ modell.ipgTransactionId;
                        ViewBag.Response = JsonHelper.prettyPrint(response);
                        return View();
                    }
                }
                catch (Exception e)
                {

                }

                if (response.Contains("acsURL"))
                {
                    modell = modell.toSecureIdEnrollmentResponseModel(response);
                    TempData["3dreponse"] = "<form  method=\"post\" action=\"" + modell.acsURL + "\" name=\"frm\" id=\"frm\"><input value=\"" + modell.cReq + "\" name=\"creq\" type=\"hidden\"/></form><script xmlns=\"http://www.w3.org/1999/xhtml\" type=\"text/javascript\">document.getElementById(\"frm\").submit();</script>";
                }
            }
            ViewBag.Response = TempData["3dreponse"];
            return View();
        }

        [HttpPost]
        public ActionResult SecurityCode(PaymentModel model)
        {
            json = ser.Payloads("Secure3D21AuthenticationUpdateRequest", model, modell);
            response = ser.executeHTTPMethod("payments/" + modell.ipgTransactionId, json, "PATCH", modell);
            return View();
        }

        public ActionResult process3dSecure(string cres, string PaRes, string MD)
        {
            if (modell.ThreeDSversion == "1.0")
            {
                modell.payerAuthenticationRequest = Request.Params["PaRes"].ToString();
                modell.merchantData = Request.Params["MD"].ToString();
            }
            else
            {
                modell.cReq = Request.Params["cres"].ToString();
            }

            json = ser.Payloads("Secure3D21AuthenticationUpdateRequest2", null, modell);
            response = ser.executeHTTPMethod("payments/" + modell.ipgTransactionId, json, "PATCH", modell);
            ViewBag.Payload = json;
            ViewBag.Operation = "3DS SALE";
            ViewBag.Method = "PATCH";
            ViewBag.RequestUrl = "https://prod.emea.api.fiservapps.com/sandbox/ipp/payments-gateway/v2/payments/" + modell.ipgTransactionId;
            ViewBag.Response = JsonHelper.prettyPrint(response);
            return View();
        }

        public ActionResult GetOrder()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpPost]
        public ActionResult RetriveOrder(PaymentModel model)
        {

            json = ser.Payloads("CREATEURL", model, null);
            response = ser.executeHTTPMethod("payment-url", json, "POST", null);

            //response = ser.executeHTTPMethod("orders/" + model.orderid, "", "GET", null);
            ViewBag.Payload = ""; // json;
            ViewBag.Operation = "Payment Url";
            ViewBag.Method = "GET";
            ViewBag.RequestUrl = "https://prod.emea.api.fiservapps.com/sandbox/ipp/payments-gateway/v2/orders/" + model.orderid;
            ViewBag.Response = JsonHelper.prettyPrint(response);
            return View();
        }

        [HttpPost]
        public ActionResult Void(PaymentModel model)
        {
            json = ser.Payloads("ReturnTransaction", model, null);
            response = ser.executeHTTPMethod("orders/" + model.orderid, json, "POST", null);
            ViewBag.Payload = json;
            ViewBag.Operation = "VOID";
            ViewBag.Method = "POST";
            ViewBag.RequestUrl = "https://prod.emea.api.fiservapps.com/sandbox/ipp/payments-gateway/v2/orders/" + model.orderid;
            ViewBag.Response = JsonHelper.prettyPrint(response);
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}