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
                string idBowlType = "", totalBowl = "0";
                string idDrink = "", totalDrink = "0";
                string idFoodAdd = "", totalFoodAdd = "0";
                foreach (var list in jObject)
                {
                    idBowlType = ""; idDrink = ""; idFoodAdd = "";
                    totalBowl = "0"; totalDrink = "0"; totalFoodAdd = "0";
                    String idFood = list.Value["listFoodID"].ToString();
                    if (list.Value["listBowlTypeID"].ToString() != "")
                    {
                        JObject BowlType = JObject.Parse(list.Value["listBowlTypeID"].ToString());
                        if (BowlType.Count > 0)
                        {
                            JObject BowlType0 = JObject.Parse(BowlType["0"].ToString());
                            idBowlType = BowlType0["id"].ToString();
                            totalBowl = BowlType0["total"].ToString();
                        }
                    }
                    if (list.Value["listDrinkID"].ToString() != "")
                    {
                        JObject Drink = JObject.Parse(list.Value["listDrinkID"].ToString());
                        if (Drink.Count > 0)
                        {
                            JObject Drink0 = JObject.Parse(Drink["0"].ToString());
                            idDrink = Drink0["id"].ToString();
                            totalDrink = Drink0["total"].ToString();
                        }
                    }
                    if (list.Value["listFoodAddID"].ToString() != "")
                    {
                        JObject FoodAdd = JObject.Parse(list.Value["listFoodAddID"].ToString());
                        if (FoodAdd.Count > 0)
                        {
                            JObject FoodAdd0 = JObject.Parse(FoodAdd["0"].ToString());
                            idFoodAdd = FoodAdd0["id"].ToString();
                            totalFoodAdd = FoodAdd0["total"].ToString();
                        }
                    }
                    String quantityFood = list.Value["quantityFood"].ToString();
                    String idTable = list.Value["idTable"].ToString();
                    String note = list.Value["note"].ToString();
                    Boolean is_TakeAway = Boolean.Parse(list.Value["is_TakeAwayDetail"].ToString());
                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row = new Dictionary<string, object>();
                    row.Add("id", idOrder);
                    String SqlQuery = "select * from dbo.[Order] (nolock) where id = @id";
                    rows = ConnectionDB.SqlSelect(SqlQuery, row);
                    if (rows.Count == 0)
                    {
                        SqlQuery = "INSERT INTO dbo.[Order] (id_User,id_Delivery,createDate) VALUES(" + int.Parse(user_id) + ",1,GETDATE()); SELECT SCOPE_IDENTITY()";
                        idOrder = ConnectionDB.SqlInsertGetID(SqlQuery);
                    }
                    row.Clear();
                    row.Add("idOrder", idOrder);
                    row.Add("idTable", int.Parse(idTable));
                    row.Add("idFood", int.Parse(idFood));
                    if(idBowlType != "")
                    {
                        row.Add("idBowlType", int.Parse(idBowlType));
                    }                    
                    row.Add("idDrink", idDrink);
                    row.Add("idAddFoodType", idFoodAdd);
                    row.Add("Total_order_price", (decimal.Parse(totalBowl) + decimal.Parse(totalDrink) + decimal.Parse(totalFoodAdd)) * int.Parse(quantityFood));
                    row.Add("Quantity", int.Parse(quantityFood));
                    row.Add("Note", note);
                    row.Add("is_TakeAway", is_TakeAway);
                    int insert = ConnectionDB.SqlInsertGetID("OrderDetails", row);
                    if (insert == 0)
                    {
                        return resulterr;
                    }
                    if(list.Value["listFoodTypeID"].ToString() != "")
                    {
                        JObject FoodType = JObject.Parse(list.Value["listFoodTypeID"].ToString());
                        foreach(var listFT in FoodType)
                        {
                            string idFT = listFT.Value["id"].ToString();
                            row.Clear();
                            try
                            {
                                row.Add("idFoodType", idFT);
                                row.Add("idOrderDetail", insert);
                                ConnectionDB.SqlInsert("Type_OrderDetail", row);
                            }
                            catch (Exception e) { }                            
                        }
                    }
                    if (list.Value["listFoodExceptID"].ToString() != "")
                    {
                        JObject FoodExcept = JObject.Parse(list.Value["listFoodExceptID"].ToString());
                        foreach (var listFE in FoodExcept)
                        {
                            string idFE = listFE.Value["id"].ToString();
                            row.Clear();
                            try
                            {
                                row.Add("idFoodExcept", idFE);
                                row.Add("idOrderDetail", insert);
                                ConnectionDB.SqlInsert("Except_OrderDetail", row);
                            }
                            catch (Exception e) { }
                        }
                    }
                }
            }
            catch (Exception e) { }

            return result;

        }
    }
}
