using Newtonsoft.Json.Linq;
using OrderAppAPITest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OrderAppAPITest.Controllers
{
    [RoutePrefix("api/Order")]
    public class OrderController : ApiController
    {
        [Route("postOrderOnlyFood/{user_id}")]
        [AcceptVerbs("POST")]
        public String postOrderOnlyFood([FromBody] Object data, string user_id)
        {
            String resulterr = "{" + "\"status\":" + "\"fail\"" + "}";
            if (data == null || ModelState.IsValid == false)
            {
                return resulterr;
            }
            String result = "{" + "\"status\":" + "\"success\"" + "}";
            JObject jObject = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(data));
            try
            {
                int idOrder = 0;
                foreach (var list in jObject)
                {
                    String idFood = list.Value["listFoodID"].ToString();
                    String idFoodType = list.Value["listFoodTypeID"].ToString();
                    String idFoodExcept = list.Value["listFoodExceptID"] == null ? "" : list.Value["listFoodExceptID"].ToString();
                    JObject BowlType = JObject.Parse(list.Value["listBowlTypeID"].ToString());
                    JObject BowlType0 = JObject.Parse(BowlType["0"].ToString());
                    String idBowlType = BowlType0["id"].ToString();
                    String total = BowlType0["total"].ToString();
                    String quantityFood = list.Value["quantityFood"].ToString();
                    String idTable = list.Value["idTable"].ToString();
                    String note = list.Value["note"].ToString();
                    Boolean is_TakeAway = Boolean.Parse(list.Value["is_TakeAwayDetail"].ToString());
                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row = new Dictionary<string, object>();
                    row.Add("id", idOrder);
                    String SqlQuery = "select * from dbo.[Order] (nolock) where id = @id";
                    rows = ConnectionDB.SqlSelect(SqlQuery, row);
                    if(rows.Count == 0)
                    {
                        SqlQuery = "INSERT INTO dbo.[Order] (id_User,id_Delivery,createDate) VALUES(" + int.Parse(user_id) + ",1,GETDATE()); SELECT SCOPE_IDENTITY()";
                        idOrder = ConnectionDB.SqlInsertGetID(SqlQuery);
                    }
                    row.Clear();
                    row.Add("idOrder", idOrder);
                    row.Add("idTable", int.Parse(idTable));
                    row.Add("idFood", int.Parse(idFood));
                    row.Add("idFoodType", idFoodType);
                    row.Add("idFoodExcept", idFoodExcept);
                    row.Add("idBowlType", int.Parse(idBowlType));
                    row.Add("Total_order_price", decimal.Parse(total));
                    row.Add("Quantity", int.Parse(quantityFood));
                    row.Add("Note", note);
                    row.Add("is_TakeAway", is_TakeAway);
                    bool insert = ConnectionDB.SqlInsert("OrderDetails", row);
                    if(insert == false)
                    {
                        return resulterr;
                    }
                }
            }
            catch (Exception e) {}

            return result;

        }
    }
}
