using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace OrderAppAPITest.Controllers
{
    [RoutePrefix("api/SendNoti")]
    public class SendNotiController : ApiController
    {
        [Route("SendMessage")]
        [AcceptVerbs("POST")]
        public IHttpActionResult SendMessage([FromBody] Object data)
        {
            String resulterr = "{" + "\"status\":" + "\"fail\"" + "}";
            if (data == null || ModelState.IsValid == false)
            {
                return BadRequest();
            }
            JObject jObject = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(data));
            String tokenDevice = jObject["tokenDevice"] == null ? "" : jObject["tokenDevice"].ToString();
            var data1 = new
            {
                to = tokenDevice,
                data = new
                {
                    message = "You have a new order",
                    name = "Waitress",
                    userID = "1",
                    status = true
                }
            };
            SendNotification(data1);
            return Ok();
        }
        public void SendNotification(object data)
        {
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            Byte[] byteArray = Encoding.UTF8.GetBytes(json);

            SendNotification(byteArray);
        }
        public void SendNotification(Byte[] byteArray)
        {
            try
            {
                string server_api_key = ConfigurationManager.AppSettings["SERVER_API_KEY"];
                string sender_id = ConfigurationManager.AppSettings["SENDER_ID"];

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add($"Authorization: key={server_api_key}");
                tRequest.Headers.Add($"Sennder: id={sender_id}");

                tRequest.ContentLength = byteArray.Length;
                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse tresponse = tRequest.GetResponse();
                dataStream = tresponse.GetResponseStream();
                StreamReader tReader = new StreamReader(dataStream);

                string sResponseFromServer = tReader.ReadToEnd();

                tReader.Close();
                dataStream.Close();
                tresponse.Close();
            }
            catch (Exception) { }
        }
    }
}
