using Newtonsoft.Json.Linq;
using OrderAppAPITest.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OrderAppAPITest.Controllers
{
    [RoutePrefix("api/Apps")]
    public class AppsController : ApiController
    {
        [Route("postAccountLogin")]
        [AcceptVerbs("GET","POST")]
        public String postAccountLogin([FromBody] Object data)
        {
            String resulterr = "{" + "\"status\":" + "\"fail\"" + "}";
            if (data == null || ModelState.IsValid == false)
            {
                return resulterr;
            }
            JObject jObject = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(data));
            //order infor
            String login = jObject["username"] == null ? "" : jObject["username"].ToString();
            String password = jObject["password"] == null ? "" : jObject["password"].ToString();
            String passwordencrypt = Security.sha256(password);
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row = new Dictionary<string, object>();
            row.Add("username", login);
            row.Add("password", passwordencrypt);
            string SqlQuery = "SELECT id,username,fullname,email FROM dbo.Users where username=@username and password=@password ";
            rows = ConnectionDB.SqlSelect(SqlQuery, row);
            if (rows.Count > 0)
            {
                Dictionary<string, object> res = rows[0];
                string username = res["username"].ToString();
                Debug.WriteLine("rs=" + Newtonsoft.Json.JsonConvert.SerializeObject(res));
                var result = new {status = "success",token = TokenManager.GenerateToken(username), user = res };
                return Newtonsoft.Json.JsonConvert.SerializeObject(result);
            }
            else
            {
                return resulterr;
            }
        }
        [Route("postCheck_Login")]
        [AcceptVerbs("POST")]
        public String postCheck_Login([FromBody] Object data)
        {
            String resulterr = "{" + "\"status\":" + "\"fail\"" + "}";
            if (data == null || ModelState.IsValid == false)
            {
                return resulterr;
            }
            JObject jObject = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(data));
            String token = jObject["token"] == null ? "" : jObject["token"].ToString();
            try
            {
                string username = TokenManager.ValidateToken(token);
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row = new Dictionary<string, object>();
                row.Add("username", username);
                string SqlQuery = "SELECT id,username,fullname,email FROM dbo.Users where username=@username";
                rows = ConnectionDB.SqlSelect(SqlQuery, row);
                if(rows.Count > 0)
                {
                    Dictionary<string, object> res = rows[0];
                    var result = new { status = "success", token = TokenManager.GenerateToken(username), user = res };
                    return Newtonsoft.Json.JsonConvert.SerializeObject(result);
                }
                else
                {
                    return resulterr;
                }
            }
            catch (Exception e)
            {
                return resulterr;
            }
        }
        [Route("getFoodOnMorning")]
        [AcceptVerbs("GET")]
        public String getFoodOnMorning()
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            Dictionary<string, string> row = new Dictionary<string, string>();
            string SqlQuery = "SELECT * FROM dbo.Food (NOLOCK) WHERE is_Morning = 1 OR is_Morning IS NULL";
            rows = ConnectionDB.SqlSelectString(SqlQuery, row);
            if (rows.Count > 0)
            {
                foreach (var res in rows)
                {
                    result.Add(res);
                }
            }
            Debug.WriteLine("rs=" + Newtonsoft.Json.JsonConvert.SerializeObject(result));
            var jsonResult = new { result = result };
            return Newtonsoft.Json.JsonConvert.SerializeObject(jsonResult);
        }
        [Route("getFoodOnAfternoon")]
        [AcceptVerbs("GET")]
        public String getFoodOnAfternoon()
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            Dictionary<string, string> row = new Dictionary<string, string>();
            string SqlQuery = "SELECT * FROM dbo.Food (NOLOCK) WHERE is_Morning = 0 OR is_Morning IS NULL ORDER BY id DESC";
            rows = ConnectionDB.SqlSelectString(SqlQuery, row);
            if (rows.Count > 0)
            {
                foreach (var res in rows)
                {
                    result.Add(res);
                }
            }
            Debug.WriteLine("rs=" + Newtonsoft.Json.JsonConvert.SerializeObject(result));
            var jsonResult = new { result = result };
            return Newtonsoft.Json.JsonConvert.SerializeObject(jsonResult);
        }
        [Route("getFoodType/{idFood}")]
        [AcceptVerbs("GET")]
        public String getFoodType(string idFood)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            Dictionary<string, string> row = new Dictionary<string, string>();
            row.Add("idFood", idFood);
            string SqlQuery = "SELECT * FROM dbo.Food_Type (NOLOCK) WHERE idFood = @idFood";
            rows = ConnectionDB.SqlSelectString(SqlQuery, row);
            if (rows.Count > 0)
            {
                foreach (var res in rows)
                {
                    result.Add(res);
                }
            }
            Debug.WriteLine("rs=" + Newtonsoft.Json.JsonConvert.SerializeObject(result));
            var jsonResult = new { result = result };
            return Newtonsoft.Json.JsonConvert.SerializeObject(jsonResult);
        }
        [Route("getDrink")]
        [AcceptVerbs("GET")]
        public String getDrink()
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            Dictionary<string, string> row = new Dictionary<string, string>();
            string SqlQuery = "SELECT * FROM dbo.Drink (NOLOCK)";
            rows = ConnectionDB.SqlSelectString(SqlQuery, row);
            if (rows.Count > 0)
            {
                foreach (var res in rows)
                {
                    result.Add(res);
                }
            }
            Debug.WriteLine("rs=" + Newtonsoft.Json.JsonConvert.SerializeObject(result));
            var jsonResult = new { result = result };
            return Newtonsoft.Json.JsonConvert.SerializeObject(jsonResult);
        }
        [Route("getFoodExcept/{is_morning}")]
        [AcceptVerbs("GET")]
        public String getFoodExcept(string is_morning)
        {
            //bool morning = bool.Parse(is_morning);
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            Dictionary<string, string> row = new Dictionary<string, string>();
            string SqlQuery = "SELECT * FROM dbo.Food_Except (NOLOCK) where is_Morning = " + is_morning;
            rows = ConnectionDB.SqlSelectString(SqlQuery, row);
            if (rows.Count > 0)
            {
                foreach (var res in rows)
                {
                    result.Add(res);
                }
            }
            Debug.WriteLine("rs=" + Newtonsoft.Json.JsonConvert.SerializeObject(result));
            var jsonResult = new { result = result };
            return Newtonsoft.Json.JsonConvert.SerializeObject(jsonResult);
        }
        [Route("getFoodAdd")]
        [AcceptVerbs("GET")]
        public String getFoodAdd()
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            Dictionary<string, string> row = new Dictionary<string, string>();
            string SqlQuery = "SELECT * FROM dbo.Food_Add_Type (NOLOCK)";
            rows = ConnectionDB.SqlSelectString(SqlQuery, row);
            if (rows.Count > 0)
            {
                foreach (var res in rows)
                {
                    result.Add(res);
                }
            }
            Debug.WriteLine("rs=" + Newtonsoft.Json.JsonConvert.SerializeObject(result));
            var jsonResult = new { result = result };
            return Newtonsoft.Json.JsonConvert.SerializeObject(jsonResult);
        }

        [Route("getFoodAddUse")]
        [AcceptVerbs("GET")]
        public String getFoodAddUse()
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            Dictionary<string, string> row = new Dictionary<string, string>();
            string SqlQuery = "SELECT * FROM dbo.Food_Type (NOLOCK) WHERE is_FoodAdd = 1";
            rows = ConnectionDB.SqlSelectString(SqlQuery, row);
            if (rows.Count > 0)
            {
                foreach (var res in rows)
                {
                    result.Add(res);
                }
            }
            Debug.WriteLine("rs=" + Newtonsoft.Json.JsonConvert.SerializeObject(result));
            var jsonResult = new { result = result };
            return Newtonsoft.Json.JsonConvert.SerializeObject(jsonResult);
        }

        [Route("getBowlType")]
        [AcceptVerbs("GET")]
        public String getBowlType()
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            Dictionary<string, string> row = new Dictionary<string, string>();
            string SqlQuery = "SELECT * FROM dbo.Bowl_Type (NOLOCK)";
            rows = ConnectionDB.SqlSelectString(SqlQuery, row);
            if (rows.Count > 0)
            {
                foreach (var res in rows)
                {
                    result.Add(res);
                }
            }
            Debug.WriteLine("rs=" + Newtonsoft.Json.JsonConvert.SerializeObject(result));
            var jsonResult = new { result = result };
            return Newtonsoft.Json.JsonConvert.SerializeObject(jsonResult);
        }
        //CRUD Food begin
        [Route("postFood")]
        [AcceptVerbs("POST")]
        public String postFood([FromBody] Object data)
        {
            String result = "";
            if (data == null || ModelState.IsValid == false)
            {
                result = "{" + "\"status\":" + "\"fail\"" + "}";
                Debug.WriteLine("data=" + data + "\t" + "value=" + Newtonsoft.Json.JsonConvert.SerializeObject(data));
                return result;
            }
            try
            {
                result = "{" + "\"status\":" + "\"success\"" + "}";
                JObject jObject = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(data));
                String Name = jObject["Name"] == null || jObject["Name"].ToString() == "" ? "" : jObject["Name"].ToString();
                Dictionary<String, Object> dic = new Dictionary<String, Object>();
                dic.Add("Name", Name);
                bool insert = ConnectionDB.SqlInsert("Food", dic);
                if (!insert)
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

        [Route("postEditFood/{id}")]
        [AcceptVerbs("POST")]
        public String postEditFood(String id, [FromBody] Object data)
        {
            String result = "";
            if (data == null || ModelState.IsValid == false)
            {
                result = "{" + "\"status\":" + "\"fail\"" + "}";
                Debug.WriteLine("data=" + data + "\t" + "value=" + Newtonsoft.Json.JsonConvert.SerializeObject(data));
                return result;
            }
            try
            {
                result = "{" + "\"status\":" + "\"success\"" + "}";
                JObject jObject = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(data));
                String Name = jObject["Name"] == null || jObject["Name"].ToString() == "" ? "" : jObject["Name"].ToString();
                Dictionary<String, Object> dic = new Dictionary<String, Object>();
                dic.Add("Name", Name);
                bool update = ConnectionDB.SqlUpdate("Food", dic, Int32.Parse(id));
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

        [Route("postDeleteFood/{id}")]
        [AcceptVerbs("POST")]
        public String postDeleteFood(String id)
        {
            String result = "";
            try
            {
                //Check idFood exists before delete?
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row = new Dictionary<string, object>();
                row.Add("idFood", Int32.Parse(id));
                string SqlQuery = " select * from Food_Type (NOLOCK) where idFood = @idFood";
                rows = ConnectionDB.SqlSelect(SqlQuery, row);
                if(rows.Count > 0)
                {
                    result = "{" + "\"status\":" + "\"failCount\"" + "}";
                    return result;
                }
                //End
                result = "{" + "\"status\":" + "\"success\"" + "}";
                bool delete = ConnectionDB.SqlDelete("Food", Int32.Parse(id));
                if (!delete)
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
        //CRUD Food end
        //CRUD FOOD_TYPE begin
        [Route("postFoodType/{idFood}")]
        [AcceptVerbs("POST")]
        public String postFoodType(String idFood, [FromBody] Object data)
        {
            String result = "";
            if (data == null || ModelState.IsValid == false)
            {
                result = "{" + "\"status\":" + "\"fail\"" + "}";
                Debug.WriteLine("data=" + data + "\t" + "value=" + Newtonsoft.Json.JsonConvert.SerializeObject(data));
                return result;
            }
            try
            {
                result = "{" + "\"status\":" + "\"success\"" + "}";
                JObject jObject = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(data));
                String Name = jObject["Name"] == null || jObject["Name"].ToString() == "" ? "" : jObject["Name"].ToString();
                bool is_FoodAdd = bool.Parse(jObject["is_FoodAdd"] == null ? "false" : jObject["is_FoodAdd"].ToString());
                Dictionary<String, Object> dic = new Dictionary<String, Object>();
                dic.Add("Name", Name);
                dic.Add("is_FoodAdd", is_FoodAdd);
                dic.Add("idFood", Int32.Parse(idFood));
                bool insert = ConnectionDB.SqlInsert("Food_Type", dic);
                if (!insert)
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

        [Route("postEditFoodType/{id}")]
        [AcceptVerbs("POST")]
        public String postEditFoodType(String id, [FromBody] Object data)
        {
            String result = "";
            if (data == null || ModelState.IsValid == false)
            {
                result = "{" + "\"status\":" + "\"fail\"" + "}";
                Debug.WriteLine("data=" + data + "\t" + "value=" + Newtonsoft.Json.JsonConvert.SerializeObject(data));
                return result;
            }
            try
            {
                result = "{" + "\"status\":" + "\"success\"" + "}";
                JObject jObject = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(data));
                String Name = jObject["Name"] == null || jObject["Name"].ToString() == "" ? "" : jObject["Name"].ToString();
                bool is_FoodAdd = bool.Parse(jObject["is_FoodAdd"] == null ? "false" : jObject["is_FoodAdd"].ToString());
                Dictionary<String, Object> dic = new Dictionary<String, Object>();
                dic.Add("Name", Name);
                dic.Add("is_FoodAdd", is_FoodAdd);
                bool update = ConnectionDB.SqlUpdate("Food_Type", dic, Int32.Parse(id));
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

        [Route("postDeleteFoodType/{id}")]
        [AcceptVerbs("POST")]
        public String postDeleteFoodType(String id)
        {
            String result = "";
            try
            {
                result = "{" + "\"status\":" + "\"success\"" + "}";
                bool delete = ConnectionDB.SqlDelete("Food_Type", Int32.Parse(id));
                if (!delete)
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
        //CRUD FOOD_TYPE end
        //CRUD Order
        [Route("getTable")]
        [AcceptVerbs("GET")]
        public String getTable()
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            Dictionary<string, string> row = new Dictionary<string, string>();
            string SqlQuery = "SELECT * FROM dbo.Food_Table (NOLOCK)";
            rows = ConnectionDB.SqlSelectString(SqlQuery, row);
            if (rows.Count > 0)
            {
                foreach (var res in rows)
                {
                    result.Add(res);
                }
            }
            Debug.WriteLine("rs=" + Newtonsoft.Json.JsonConvert.SerializeObject(result));
            var jsonResult = new { result = result };
            return Newtonsoft.Json.JsonConvert.SerializeObject(jsonResult);
        }

        //End
    }
}
