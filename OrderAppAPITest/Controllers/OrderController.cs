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
                    if (idBowlType != "")
                    {
                        row.Add("idBowlType", int.Parse(idBowlType));
                    }
                    row.Add("idDrink", idDrink);
                    row.Add("idAddFoodType", idFoodAdd);
                    row.Add("Total_order_price", int.Parse(idFood) < 7 ? (decimal.Parse(totalBowl) + decimal.Parse(totalDrink) + decimal.Parse(totalFoodAdd)) * int.Parse(quantityFood) : 30000 * int.Parse(quantityFood));
                    row.Add("Quantity", int.Parse(quantityFood));
                    row.Add("Note", note);
                    row.Add("is_TakeAway", is_TakeAway);
                    row.Add("idDelivery", 1);
                    int insert = ConnectionDB.SqlInsertGetID("OrderDetails", row);
                    if (insert == 0)
                    {
                        return resulterr;
                    }
                    if (list.Value["listFoodTypeID"].ToString() != "" && list.Value["listFoodTypeID"].ToString().Length > 2)
                    {
                        JObject FoodType = JObject.Parse(list.Value["listFoodTypeID"].ToString());
                        foreach (var listFT in FoodType)
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
                    if (list.Value["listFoodExceptID"].ToString() != "" && list.Value["listFoodExceptID"].ToString().Length > 2)
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

        [Route("getListOrdered")]
        [AcceptVerbs("GET")]
        public string getListOrdered()
        {
            List<Dictionary<string, object>> lst = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row = new Dictionary<string, object>();
            string idOrder = "",idTable = "",idFood = "",LoaiMon = "", KhongLay = "", Mon = "", LoaiTo = "", MonThem = "", Nuoc = "", SoLuong = "", Tien = "", GhiChu = "", NguoiDat = "", Ban = "", createDate = "", id = "", status = "", MangVe = "", idDelivery = "";
            string sqlQuery = "SELECT c.id,d.Name AS Ban,e.Name AS Mon,bt.Name AS LoaiTo,t.Name AS MonThem, dr.Name AS Nuoc,c.Quantity AS SoLuong,c.Total_order_price AS Tien,c.Note AS GhiChu,b.Fullname AS NguoiDat,ft.Name AS LoaiMon, fe.Name AS KhongLay,ds.Description AS TrangThai,CASE WHEN c.is_TakeAway = 1 THEN N'Mang Về' ELSE N'Tại Bàn' END MangVe, CONVERT(NVARCHAR(10),a.createDate,103) + ' ' + RIGHT(CONVERT(VARCHAR, a.createDate, 0), 7) createDate,c.idDelivery, c.idFood" +
                    ", c.idTable, a.id AS idOrder FROM dbo.[Order] (NOLOCK) a LEFT JOIN dbo.Users (NOLOCK) b ON b.id = a.id_User LEFT JOIN dbo.OrderDetails (NOLOCK) c ON c.idOrder = a.id LEFT JOIN dbo.Food_Table (NOLOCK) d ON d.id = c.idTable LEFT JOIN dbo.Food (NOLOCK) e ON e.id = c.idFood" +
                    " LEFT JOIN dbo.Food_Add_Type (NOLOCK) t ON t.id = c.idAddFoodType LEFT JOIN dbo.Drink (NOLOCK) dr ON dr.id = c.idDrink LEFT JOIN dbo.Bowl_Type (NOLOCK) bt ON bt.id = c.idBowlType LEFT JOIN dbo.Type_OrderDetail (NOLOCK) tod ON tod.idOrderDetail = c.id" +
                    " LEFT JOIN dbo.Food_Type (NOLOCK) ft ON ft.id = tod.idFoodType LEFT JOIN dbo.Except_OrderDetail (NOLOCK) eod ON eod.idOrderDetail = c.id LEFT JOIN dbo.Food_Except (NOLOCK) fe ON fe.id = eod.idFoodExcept LEFT JOIN dbo.Delivery_status (NOLOCK) ds ON ds.id = c.idDelivery WHERE c.idDelivery IN (1,2) AND CAST(a.createDate AS Date) = CAST(GETDATE() AS DATE) ORDER BY a.createDate DESC,c.idDelivery ASC";
            rows = ConnectionDB.SqlSelect(sqlQuery, row);
            if (rows.Count > 0)
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    if (rows.Count - i > 1)
                    {
                        LoaiMon = rows[i]["LoaiMon"] == null ? "" : rows[i]["LoaiMon"].ToString();
                        KhongLay = rows[i]["KhongLay"] == null ? "" : rows[i]["KhongLay"].ToString();
                        List<string> arrKhongLay = new List<string>();
                        arrKhongLay.Add(KhongLay);
                        if (rows[i]["id"].ToString() == rows[i + 1]["id"].ToString())
                        {
                            for (int y = i; y < rows.Count; y++)
                            {
                                if(rows.Count - y > 1)
                                {
                                    if (rows[y]["id"].ToString() == rows[y + 1]["id"].ToString())
                                    {
                                        if (rows[y]["LoaiMon"].ToString() != rows[y + 1]["LoaiMon"].ToString())
                                        {
                                            LoaiMon += "," + rows[y + 1]["LoaiMon"].ToString();
                                        }
                                        if (rows[y]["KhongLay"].ToString() != rows[y + 1]["KhongLay"].ToString() && Array.IndexOf(arrKhongLay.ToArray(), rows[y + 1]["KhongLay"]) < 0)
                                        {
                                            arrKhongLay.Add(rows[y + 1]["KhongLay"].ToString());
                                            KhongLay += "," + rows[y + 1]["KhongLay"].ToString();
                                        }
                                        i++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }                                
                            }
                        }
                        Mon = rows[i]["Mon"].ToString();
                        LoaiTo = rows[i]["LoaiTo"] == null ? "" : rows[i]["LoaiTo"].ToString();
                        MonThem = rows[i]["MonThem"] == null ? "" : rows[i]["MonThem"].ToString();
                        Nuoc = rows[i]["Nuoc"] == null ? "" : rows[i]["Nuoc"].ToString();
                        SoLuong = rows[i]["SoLuong"] == null ? "0" : rows[i]["SoLuong"].ToString();
                        Tien = rows[i]["Tien"] == null ? "0" : rows[i]["Tien"].ToString();
                        GhiChu = rows[i]["GhiChu"] == null ? "" : rows[i]["GhiChu"].ToString();
                        NguoiDat = rows[i]["NguoiDat"] == null ? "" : rows[i]["NguoiDat"].ToString();
                        Ban = rows[i]["Ban"].ToString();
                        id = rows[i]["id"].ToString();
                        status = rows[i]["TrangThai"].ToString();
                        MangVe = rows[i]["MangVe"].ToString();
                        createDate = rows[i]["createDate"].ToString();
                        idDelivery = rows[i]["idDelivery"].ToString();
                        idFood = rows[i]["idFood"].ToString();
                        idTable = rows[i]["idTable"].ToString();
                        idOrder = rows[i]["idOrder"].ToString();
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("id", int.Parse(id));
                        dic.Add("Mon", Mon);
                        dic.Add("MonThem", MonThem);
                        dic.Add("LoaiTo", LoaiTo);
                        dic.Add("Nuoc", Nuoc);
                        dic.Add("SoLuong", int.Parse(SoLuong));
                        dic.Add("Tien", Decimal.Parse(Tien));
                        dic.Add("GhiChu", GhiChu);
                        dic.Add("NguoiDat", NguoiDat);
                        dic.Add("Ban", Ban);
                        dic.Add("NgayTao", createDate);
                        dic.Add("LoaiMon", LoaiMon);
                        dic.Add("KhongLay", KhongLay);
                        dic.Add("TrangThai", status);
                        dic.Add("MangVe", MangVe);
                        dic.Add("idDelivery", idDelivery);
                        dic.Add("idFood", idFood);
                        dic.Add("idTable", idTable);
                        dic.Add("idOrder", idOrder);
                        lst.Add(dic);
                    }
                    else
                    {
                        LoaiMon = rows[i]["LoaiMon"] == null ? "" : rows[i]["LoaiMon"].ToString();
                        KhongLay = rows[i]["KhongLay"] == null ? "" : rows[i]["KhongLay"].ToString();
                        Mon = rows[i]["Mon"].ToString();
                        LoaiTo = rows[i]["LoaiTo"] == null ? "" : rows[i]["LoaiTo"].ToString();
                        MonThem = rows[i]["MonThem"] == null ? "" : rows[i]["MonThem"].ToString();
                        Nuoc = rows[i]["Nuoc"] == null ? "" : rows[i]["Nuoc"].ToString();
                        SoLuong = rows[i]["SoLuong"] == null ? "0" : rows[i]["SoLuong"].ToString();
                        Tien = rows[i]["Tien"] == null ? "0" : rows[i]["Tien"].ToString();
                        GhiChu = rows[i]["GhiChu"] == null ? "" : rows[i]["GhiChu"].ToString();
                        NguoiDat = rows[i]["NguoiDat"] == null ? "" : rows[i]["NguoiDat"].ToString();
                        Ban = rows[i]["Ban"].ToString();
                        id = rows[i]["id"].ToString();
                        status = rows[i]["TrangThai"].ToString();
                        MangVe = rows[i]["MangVe"].ToString();
                        createDate = rows[i]["createDate"].ToString();
                        idDelivery = rows[i]["idDelivery"].ToString();
                        idFood = rows[i]["idFood"].ToString();
                        idTable = rows[i]["idTable"].ToString();
                        idOrder = rows[i]["idOrder"].ToString();
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("id", int.Parse(id));
                        dic.Add("Mon", Mon);
                        dic.Add("MonThem", MonThem);
                        dic.Add("LoaiTo", LoaiTo);
                        dic.Add("Nuoc", Nuoc);
                        dic.Add("SoLuong", int.Parse(SoLuong));
                        dic.Add("Tien", Decimal.Parse(Tien));
                        dic.Add("GhiChu", GhiChu);
                        dic.Add("NguoiDat", NguoiDat);
                        dic.Add("Ban", Ban);
                        dic.Add("NgayTao", createDate);
                        dic.Add("LoaiMon", LoaiMon);
                        dic.Add("KhongLay", KhongLay);
                        dic.Add("TrangThai", status);
                        dic.Add("MangVe", MangVe);
                        dic.Add("idDelivery", idDelivery);
                        dic.Add("idFood", idFood);
                        dic.Add("idTable", idTable);
                        dic.Add("idOrder", idOrder);
                        lst.Add(dic);
                    }
                }
            }
            else
            {
                var resulterr = new { status = "Empty", list = "" };
                return Newtonsoft.Json.JsonConvert.SerializeObject(resulterr);
            }
            var result = new { status = "success", list = lst };
            return Newtonsoft.Json.JsonConvert.SerializeObject(result);
        }

        [Route("postUpdateStatusOrder/{id_OrderDetail}")]
        [AcceptVerbs("POST")]
        public string postUpdateStatusOrder(string id_OrderDetail)
        {
            string result = "";
            try
            {
                result = "{" + "\"status\":" + "\"success\"" + "}";
                Dictionary<String, Object> dic = new Dictionary<String, Object>();
                dic.Add("idDelivery", 2);
                bool update = ConnectionDB.SqlUpdate("OrderDetails", dic, Int32.Parse(id_OrderDetail));
                if (!update)
                {
                    result = "{" + "\"status\":" + "\"fail\"" + "}";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return result;
        }
        [Route("postUpdateOrderOnlyFood/{idOrderDetail}")]
        [AcceptVerbs("POST")]
        public String postUpdateOrderOnlyFood([FromBody] Object data, string idOrderDetail)
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
                string idBowlType = "", totalBowl = "0";
                string idDrink = "", totalDrink = "0";
                string idFoodAdd = "", totalFoodAdd = "0";
                foreach (var list in jObject)
                {
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
                    row.Add("idTable", Convert.ToInt32(idTable));
                    row.Add("idFood", Convert.ToInt32(idFood));
                    if (idBowlType != "")
                    {
                        row.Add("idBowlType", Convert.ToInt32(idBowlType));
                    }
                    row.Add("idDrink", idDrink);
                    row.Add("idAddFoodType", idFoodAdd);
                    row.Add("Total_order_price", Convert.ToInt32(idFood) < 7 ? (Convert.ToDecimal(totalBowl) + Convert.ToDecimal(totalDrink) + Convert.ToDecimal(totalFoodAdd)) * Convert.ToInt32(quantityFood) : 30000 * Convert.ToInt32(quantityFood));
                    row.Add("Quantity", Convert.ToInt32(quantityFood));
                    row.Add("Note", note);
                    row.Add("is_TakeAway", is_TakeAway);
                    row.Add("idDelivery", 1);
                    bool insert = ConnectionDB.SqlUpdate("OrderDetails", row, Convert.ToInt32(idOrderDetail));
                    if (!insert)
                    {
                        return resulterr;
                    }
                    if (list.Value["listFoodTypeID"].ToString() != "" && list.Value["listFoodTypeID"].ToString().Length > 2)
                    {
                        ConnectionDB.SqlDelete("Type_OrderDetail", Convert.ToInt32(idOrderDetail), "idOrderDetail");
                        JObject FoodType = JObject.Parse(list.Value["listFoodTypeID"].ToString());
                        foreach (var listFT in FoodType)
                        {
                            string idFT = listFT.Value["id"].ToString();
                            row.Clear();
                            try
                            {
                                row.Add("idFoodType", idFT);
                                row.Add("idOrderDetail", Convert.ToInt32(idOrderDetail));
                                ConnectionDB.SqlInsert("Type_OrderDetail", row);
                            }
                            catch (Exception e) { }
                        }
                    }
                    if (list.Value["listFoodExceptID"].ToString() != "" && list.Value["listFoodExceptID"].ToString().Length > 2)
                    {
                        ConnectionDB.SqlDelete("Except_OrderDetail", Convert.ToInt32(idOrderDetail), "idOrderDetail");
                        JObject FoodExcept = JObject.Parse(list.Value["listFoodExceptID"].ToString());
                        foreach (var listFE in FoodExcept)
                        {
                            string idFE = listFE.Value["id"].ToString();
                            row.Clear();
                            try
                            {
                                row.Add("idFoodExcept", idFE);
                                row.Add("idOrderDetail", Convert.ToInt32(idOrderDetail));
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

        [Route("getOrderDetail/{idOrderDetail}")]
        [AcceptVerbs("GET")]
        public String getOrderDetail(string idOrderDetail)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            Dictionary<string, string> row = new Dictionary<string, string>();
            row.Add("id", idOrderDetail);
            string SqlQuery = "SELECT * FROM dbo.OrderDetails (NOLOCK) WHERE id = @id";
            rows = ConnectionDB.SqlSelectString(SqlQuery, row);
            if (rows.Count > 0)
            {
                result.Add(rows[0]);
            }
            var jsonResult = new { result = result };
            return Newtonsoft.Json.JsonConvert.SerializeObject(jsonResult);
        }
        [Route("getExceptOrderDetail/{idOrderDetail}")]
        [AcceptVerbs("GET")]
        public String getExceptOrderDetail(string idOrderDetail)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            Dictionary<string, string> row = new Dictionary<string, string>();
            row.Add("idOrderDetail", idOrderDetail);
            string SqlQuery = "SELECT * FROM dbo.Except_OrderDetail (NOLOCK) WHERE idOrderDetail = @idOrderDetail";
            rows = ConnectionDB.SqlSelectString(SqlQuery, row);
            if (rows.Count > 0)
            {
                foreach(var rowResult in rows)
                {
                    result.Add(rowResult);
                }                
            }
            var jsonResult = new { result = result };
            return Newtonsoft.Json.JsonConvert.SerializeObject(jsonResult);
        }
        [Route("getTypeOrderDetail/{idOrderDetail}")]
        [AcceptVerbs("GET")]
        public String getTypeOrderDetail(string idOrderDetail)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            Dictionary<string, string> row = new Dictionary<string, string>();
            row.Add("idOrderDetail", idOrderDetail);
            string SqlQuery = "SELECT * FROM dbo.Type_OrderDetail (NOLOCK) WHERE idOrderDetail = @idOrderDetail";
            rows = ConnectionDB.SqlSelectString(SqlQuery, row);
            if (rows.Count > 0)
            {
                foreach (var rowResult in rows)
                {
                    result.Add(rowResult);
                }
            }
            var jsonResult = new { result = result };
            return Newtonsoft.Json.JsonConvert.SerializeObject(jsonResult);
        }

        [Route("postDeleteOrderDetail/{idOrder}/{idOrderDetail}")]
        [AcceptVerbs("POST")]
        public String postDeleteOrderDetail(string idOrder,string idOrderDetail)
        {
            string result = "{" + "\"status\":" + "\"fail\"" + "}";
            try
            {
                bool delete = false;
                List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
                Dictionary<string, string> row = new Dictionary<string, string>();
                row.Add("idOrderDetail", idOrderDetail);
                string SqlQuery = "Select * From dbo.Except_OrderDetail (nolock) where idOrderDetail = @idOrderDetail";
                rows = ConnectionDB.SqlSelectString(SqlQuery, row);
                if(rows.Count > 0)
                {
                    delete = ConnectionDB.SqlDelete("Except_OrderDetail", Convert.ToInt32(idOrderDetail), "idOrderDetail");
                    if(!delete)
                    {
                        return result;
                    }
                }
                SqlQuery = "Select * from dbo.Type_OrderDetail (nolock) where idOrderDetail = @idOrderDetail";
                rows = ConnectionDB.SqlSelectString(SqlQuery, row);
                if(rows.Count > 0)
                {
                    delete = ConnectionDB.SqlDelete("Type_OrderDetail", Convert.ToInt32(idOrderDetail), "idOrderDetail");
                    if (!delete)
                    {
                        return result;
                    }
                }
                delete = ConnectionDB.SqlDelete("OrderDetails", Convert.ToInt32(idOrderDetail));
                if(!delete)
                {
                    return result;
                }
                row.Clear();
                row.Add("idOrder", idOrder);
                SqlQuery = "Select * from OrderDetails (nolock) where idOrder = @idOrder";
                rows = ConnectionDB.SqlSelectString(SqlQuery, row);
                if(rows.Count <= 0)
                {
                    delete = ConnectionDB.SqlDelete("dbo.[Order]", Convert.ToInt32(idOrder));
                    if (!delete)
                    {
                        return result;
                    }
                }
                result = "{" + "\"status\":" + "\"success\"" + "}";
            }
            catch (Exception e) { }
            return result;
        }
    }
}
