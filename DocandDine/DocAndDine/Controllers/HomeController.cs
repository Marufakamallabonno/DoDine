using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using DocAndDine.Models;

namespace DocAndDine.Controllers
{
    public interface IEncryption
    {
        string EncryptString(string str);
        string DecryptString(string str);

    }
    public interface IRecipes
    {
        ActionResult Recipes();
        ActionResult RecipeSearch(FormCollection form);
        ActionResult IngredientAdd(FormCollection form);
        ActionResult IngredientDelete(FormCollection form);
        ActionResult ViewRecipe();
        ActionResult ViewRecipeClick(int id);
        ActionResult RecipeComment(FormCollection form);
        ActionResult RecipeRating(FormCollection form);
        ActionResult ViewRecipeWriterClick(int id);
        ActionResult RecipeWriterProfile();
        ActionResult RecipePost(FormCollection form);
    }

    public interface IRestaurants
    {
        ActionResult Restaurants();
        ActionResult RestaurantSearch(FormCollection form);
        ActionResult RestaurantClicked(int id);
        ActionResult ResMenuPage();
        ActionResult RestaurantRating(FormCollection form);
        ActionResult MenuPost(FormCollection form);
        ActionResult Reserve(FormCollection formCollection);
    }

    public interface IHomeFoods
    {
        ActionResult HomeMadeFoods();
        ActionResult HomeFoodSearch(FormCollection form);
        ActionResult HomeChefProfile();
        ActionResult ViewHomeChefClick(int id);
        ActionResult HomeChefComments();
        ActionResult viewChefComments(int id);
        ActionResult DeleteHomeFood(int dId);
        ActionResult HomeChefRating(FormCollection form);
        ActionResult HomeMadeFoodPost(FormCollection form);
    }

    public interface IBlogs
    {
        ActionResult FoodBlogs();
        ActionResult BlogSearch(FormCollection form);
        ActionResult BlogComment(FormCollection form);
        ActionResult FullBlog();
        ActionResult Blogger();
        ActionResult BlogClick(int id);
        ActionResult ViewBloggerClick(int id);
        ActionResult Blogpost(FormCollection form);
        ActionResult joinCommunity(FormCollection form);
        ActionResult leaveCommunity();
        ActionResult communityPage(FormCollection form);
        ActionResult communityPage();
    }




    public class HomeController : Controller, IEncryption, IRecipes , IRestaurants ,IHomeFoods, IBlogs
    {

        string connectionString = @"Data Source = DESKTOP-J5UVD2C\SQLEXPRESS; Initial Catalog = DocDine; Integrated Security = True";

        public ActionResult Index()
        {

            if (Session["track"] == null)

            {

                TempData["recipePosted"] = 0;
                Session["userActive"] = 0;
                Session["recipeWriterActive"] = 0;
                Session["homechefActive"] = 0;
                Session["bloggerActive"] = 0;
                Session["communityJoined"] = 0;
                TempData["resStar"] = 0;
                TempData["recipeStar"] = 0;
            }
            TempData["resStar"] = 0;
            TempData["recipeStar"] = 0;
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            var path = "";
            string userName = formCollection["UsernameInput"];

            string Email = formCollection["EmailInput"];
            string passwordToEncrypt = formCollection["passwordInput"];
            string password = EncryptString(passwordToEncrypt);
            string rePassword = EncryptString(formCollection["Re-enterPassswordInput"]);

            string phoneNo = formCollection["phoneNoInput"];

            HttpPostedFileBase file = Request.Files["pic"];

            SignUpUser user = new SignUpUser();
            user.userName = userName;
            user.Email = Email;
            user.password = password;
            user.rePassword = rePassword;
            user.phoneNo = phoneNo;


            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                file.SaveAs(path);
            }

            user.path = path;


            var value = formCollection["userChoice"];
            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "insert into [User] values(@UserName,@Email,@Password,@PhoneNo,@Picture)";


                var sqlCmd = new SqlCommand(query, sqlCon);

                if (!string.IsNullOrEmpty(userName))
                {
                    string name = userName.ToString();
                    if (name.Length <= 20 && name.Length >= 3)
                    {
                        sqlCmd.Parameters.AddWithValue("@UserName", user.userName);
                    }
                    else
                    {
                        TempData["data"] = "1";

                        return RedirectToAction("SignUp");
                    }

                }
                else
                {
                    TempData["data"] = "1";

                    return RedirectToAction("SignUp");
                }
                if (!string.IsNullOrEmpty(Email))
                {
                    string mail = Email.ToString();
                    if (mail.EndsWith("@gmail.com") || mail.EndsWith("@yahoo.com") || mail.EndsWith("@outlook.com"))
                    {
                        sqlCmd.Parameters.AddWithValue("@Email", user.Email);
                    }
                    else
                    {
                        TempData["email"] = "4";
                        return RedirectToAction("SignUp");
                    }

                }
                else
                {
                    TempData["email"] = "4";
                    return RedirectToAction("SignUp");
                }
                if (!string.IsNullOrEmpty(password))
                {
                    if (password.Length >= 3)
                    {
                        if (string.Equals(password, rePassword))
                        {
                            sqlCmd.Parameters.AddWithValue("@Password", user.password);

                        }
                        else
                        {
                            TempData["message"] = "2";

                            return RedirectToAction("SignUp");
                        }

                    }
                    else
                    {
                        TempData["message"] = "2";

                        return RedirectToAction("SignUp");
                    }


                }

                else
                {
                    TempData["pass"] = "3";
                    return RedirectToAction("SignUp");
                }
                if (!string.IsNullOrEmpty(user.phoneNo))
                {
                    string phn = phoneNo;
                    if (phn.Length == 11)
                    {
                        sqlCmd.Parameters.AddWithValue("@PhoneNo", user.phoneNo);
                    }
                    else
                    {
                        TempData["phoneNo"] = "5";

                        return RedirectToAction("SignUp");
                    }

                }
                else
                {
                    TempData["phoneNo"] = "5";

                    return RedirectToAction("SignUp");
                }
                sqlCmd.Parameters.AddWithValue("@Picture", user.path);
                sqlCmd.ExecuteNonQuery();
            }

            if (value.Equals("user"))
            {
                using (var sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string queryUserId = "SELECT TOP 1 UserId FROM [User] ORDER BY UserId DESC";

                    var dtbUser = new DataTable();
                    using (var sqlCon1 = new SqlConnection(connectionString))
                    {
                        sqlCon1.Open();
                        SqlDataAdapter sqlDa = new SqlDataAdapter(queryUserId, sqlCon1);
                        sqlDa.Fill(dtbUser);
                        if (dtbUser.Rows.Count > 0)
                        {
                            var lastUserId = dtbUser.Rows[0][0];

                            string query = "insert into [User_access] values (@UserId,@homechefaccess,@bloggeraccess,@recipewriteraccess)";
                            var sqlCmd = new SqlCommand(query, sqlCon1);
                            sqlCmd.Parameters.AddWithValue("@UserId", lastUserId);
                            sqlCmd.Parameters.AddWithValue("@homechefaccess", 0);
                            sqlCmd.Parameters.AddWithValue("@bloggeraccess", 0);
                            sqlCmd.Parameters.AddWithValue("@recipewriteraccess", 0);
                            sqlCmd.ExecuteNonQuery();
                        }
                    }
                    Response.Redirect("~/Home/Index");
                }
            }
            if (value.Equals("homechef"))
            {
                using (var sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string queryUserId = "SELECT TOP 1 UserId FROM [User] ORDER BY UserId DESC";

                    var dtbUser = new DataTable();
                    using (var sqlCon1 = new SqlConnection(connectionString))
                    {
                        sqlCon1.Open();
                        SqlDataAdapter sqlDa = new SqlDataAdapter(queryUserId, sqlCon1);
                        sqlDa.Fill(dtbUser);
                        if (dtbUser.Rows.Count > 0)
                        {
                            var lastUserId = dtbUser.Rows[0][0];

                            string query = "insert into [User_access] values (@UserId,@homechefaccess,@bloggeraccess,@recipewriteraccess)";
                            var sqlCmd = new SqlCommand(query, sqlCon1);
                            sqlCmd.Parameters.AddWithValue("@UserId", lastUserId);
                            sqlCmd.Parameters.AddWithValue("@homechefaccess", 1);
                            sqlCmd.Parameters.AddWithValue("@bloggeraccess", 0);
                            sqlCmd.Parameters.AddWithValue("@recipewriteraccess", 0);
                            sqlCmd.ExecuteNonQuery();
                        }
                    }

                    Response.Redirect("~/Home/HomeChefSignUp");
                }
            }

            if (value.Equals("recipeWriter"))
            {
                using (var sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string queryUserId = "SELECT TOP 1 UserId FROM [User] ORDER BY UserId DESC";

                    var dtbUser = new DataTable();
                    using (var sqlCon1 = new SqlConnection(connectionString))
                    {
                        sqlCon1.Open();
                        SqlDataAdapter sqlDa = new SqlDataAdapter(queryUserId, sqlCon1);
                        sqlDa.Fill(dtbUser);
                        if (dtbUser.Rows.Count > 0)
                        {
                            var lastUserId = dtbUser.Rows[0][0];

                            string query = "insert into [User_access] values (@UserId,@homechefaccess,@bloggeraccess,@recipewriteraccess)";
                            var sqlCmd = new SqlCommand(query, sqlCon1);
                            sqlCmd.Parameters.AddWithValue("@UserId", lastUserId);
                            sqlCmd.Parameters.AddWithValue("@homechefaccess", 0);
                            sqlCmd.Parameters.AddWithValue("@bloggeraccess", 0);
                            sqlCmd.Parameters.AddWithValue("@recipewriteraccess", 1);
                            sqlCmd.ExecuteNonQuery();
                        }
                    }
                    Response.Redirect("~/Home/RecipeWriterSignup");
                }
            }

            if (value.Equals("blogger"))
            {
                using (var sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string queryUserId = "SELECT TOP 1 UserId FROM [User] ORDER BY UserId DESC";

                    var dtbUser = new DataTable();
                    using (var sqlCon1 = new SqlConnection(connectionString))
                    {
                        sqlCon1.Open();
                        SqlDataAdapter sqlDa = new SqlDataAdapter(queryUserId, sqlCon1);
                        sqlDa.Fill(dtbUser);
                        if (dtbUser.Rows.Count > 0)
                        {
                            var lastUserId = dtbUser.Rows[0][0];

                            string query = "insert into [User_access] values (@UserId,@homechefaccess,@bloggeraccess,@recipewriteraccess)";
                            var sqlCmd = new SqlCommand(query, sqlCon1);
                            sqlCmd.Parameters.AddWithValue("@UserId", lastUserId);
                            sqlCmd.Parameters.AddWithValue("@homechefaccess", 0);
                            sqlCmd.Parameters.AddWithValue("@bloggeraccess", 1);
                            sqlCmd.Parameters.AddWithValue("@recipewriteraccess", 0);
                            sqlCmd.ExecuteNonQuery();
                        }
                    }
                    Response.Redirect("~/Home/BloggerSignUp");
                }
            }
            if (value.Equals("premiumUser"))
            {
                using (var sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string queryUserId = "SELECT TOP 1 UserId FROM [User] ORDER BY UserId DESC";

                    var dtbUser = new DataTable();
                    using (var sqlCon1 = new SqlConnection(connectionString))
                    {
                        sqlCon1.Open();
                        SqlDataAdapter sqlDa = new SqlDataAdapter(queryUserId, sqlCon1);
                        sqlDa.Fill(dtbUser);
                        if (dtbUser.Rows.Count > 0)
                        {
                            var lastUserId = dtbUser.Rows[0][0];

                            string query = "insert into PremiumUser values (@UserId,@PaymentStatus)";
                            var sqlCmd = new SqlCommand(query, sqlCon1);
                            sqlCmd.Parameters.AddWithValue("@UserId", lastUserId);
                            sqlCmd.Parameters.AddWithValue("@PaymentStatus", "NotPaid");

                            sqlCmd.ExecuteNonQuery();
                        }
                    }

                    Response.Redirect("~/Home/Index");
                }
            }
            return View();
        }




        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            var mail = form["email"];
            var pass = EncryptString(form["pass"]);

            Login login = new Login();
            login.email = mail;
            login.password = pass;


            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "select * from [User] where Email='" + mail + "'and Password='" + pass + "'";
                var sqlCmd = new SqlCommand(query, sqlCon);

                SqlDataReader dr = sqlCmd.ExecuteReader();
                if (dr.Read())
                {
                    int userIdLoggedIn = dr.GetInt32(0);
                    sqlCon.Close();
                    TempData["userIdloggedIn"] = userIdLoggedIn;

                    sqlCon.Open();
                    string activeUserQuery = "select * from User_access where UserId='" + userIdLoggedIn + "'";
                    var sqlCmd2 = new SqlCommand(activeUserQuery, sqlCon);
                    SqlDataReader dr2 = sqlCmd2.ExecuteReader();
                    if (dr2.Read())
                    {
                        int homechefAccess = dr2.GetInt32(2);
                        int bloggerAccess = dr2.GetInt32(3);
                        int recipewriterAccess = dr2.GetInt32(4);
                        sqlCon.Close();
                        if (homechefAccess == 1)
                        {
                            Session["track"] = 1;
                            Session["homechefActive"] = userIdLoggedIn;
                            Session["bloggerActive"] = 0;
                            Session["recipeWriterActive"] = 0;
                            Session["userActive"] = 0;
                            Response.Write("<script>alert('login successful')</script>");
                            return RedirectToAction("HomeChefProfile");
                        }
                        else if (bloggerAccess == 1)
                        {
                            Session["track"] = 1;
                            Session["bloggerActive"] = userIdLoggedIn;
                            Session["homechefActive"] = 0;
                            Session["recipeWriterActive"] = 0;
                            Session["userActive"] = 0;
                            Response.Write("<script>alert('login successful')</script>");
                            return RedirectToAction("Blogger");
                        }
                        else if (recipewriterAccess == 1)
                        {
                            Session["track"] = 1;
                            Session["recipeWriterActive"] = userIdLoggedIn;
                            Session["homechefActive"] = 0;
                            Session["bloggerActive"] = 0;
                            Session["userActive"] = 0;
                            Response.Write("<script>alert('login successful')</script>");
                            return RedirectToAction("RecipeWriterProfile");
                        }
                        else
                        {
                            Session["track"] = 1;
                            Session["userActive"] = userIdLoggedIn;
                            Session["recipeWriterActive"] = 0;
                            Session["homechefActive"] = 0;
                            Session["bloggerActive"] = 0;
                            Response.Write("<script>alert('login successful')</script>");
                            return RedirectToAction("UserProfile");
                        }


                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }


                }
                else
                {
                    sqlCon.Close();
                    Response.Write("<script>alert('error! Try again.')</script>");
                    return View("Login");
                }

            }

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        public ActionResult Restaurants()
        {
            TempData["resStar"] = 0;
            var dtbUser = new DataTable();
            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                if (TempData["searchRestaurantFood"] != null)
                {
                    if ((!TempData["location"].Equals("all")) && (!TempData["rating"].Equals("0")) && (TempData["search"] != null) && (TempData["orderBy"].Equals("rating")))
                    {



                        SqlDataAdapter sqlDa = new SqlDataAdapter("select Restaurant.RestaurantId, Restaurant.RestaurantName, Restaurant.Location, Restaurant.Area, Restaurant.Offers, Restaurant.ProfilePic, Restaurant.CoverPic, Restaurant.RestaurantPassword, subquery.avgRate  from Restaurant, (Select RestaurantRating.RestaurantId, avg(RestaurantRating.Rating) as avgRate from RestaurantRating group by RestaurantRating.RestaurantId) subquery where subquery.RestaurantId = Restaurant.RestaurantId AND Restaurant.Area ='" + TempData["location"] + "'and subquery.avgRate >= '" + TempData["rating"] + "'and Restaurant.RestaurantName LIKE '%" + TempData["search"] + "%' ORDER BY subquery.avgRate ASC", sqlCon);

                        sqlDa.Fill(dtbUser);

                    }
                    else if ((!TempData["location"].Equals("all")) && (!TempData["rating"].Equals("0")) && (TempData["orderBy"].Equals("None")))
                    {


                        SqlDataAdapter sqlDa = new SqlDataAdapter("select Restaurant.RestaurantId, Restaurant.RestaurantName, Restaurant.Location, Restaurant.Area, Restaurant.Offers, Restaurant.ProfilePic, Restaurant.CoverPic, Restaurant.RestaurantPassword, subquery.avgRate  from Restaurant, (Select RestaurantRating.RestaurantId, avg(RestaurantRating.Rating) as avgRate from RestaurantRating group by RestaurantRating.RestaurantId) subquery where subquery.RestaurantId = Restaurant.RestaurantId AND Restaurant.Area ='" + TempData["location"] + "'and subquery.avgRate >= '" + TempData["rating"] + "'", sqlCon);
                        sqlDa.Fill(dtbUser);

                    }
                    else if ((!TempData["location"].Equals("all")) && (TempData["search"] != null) && (TempData["orderBy"].Equals("None")))
                    {


                        SqlDataAdapter sqlDa = new SqlDataAdapter("select Restaurant.RestaurantId, Restaurant.RestaurantName, Restaurant.Location, Restaurant.Area, Restaurant.Offers, Restaurant.ProfilePic, Restaurant.CoverPic, Restaurant.RestaurantPassword, subquery.avgRate  from Restaurant, (Select RestaurantRating.RestaurantId, avg(RestaurantRating.Rating) as avgRate from RestaurantRating group by RestaurantRating.RestaurantId) subquery where subquery.RestaurantId = Restaurant.RestaurantId AND Restaurant.Area ='" + TempData["location"] + "'and Restaurant.RestaurantName LIKE '%" + TempData["search"] + "%'", sqlCon);
                        sqlDa.Fill(dtbUser);
                    }
                    else if ((!TempData["rating"].Equals("0")) && (TempData["search"] != null) && (TempData["orderBy"].Equals("None")))
                    {


                        SqlDataAdapter sqlDa = new SqlDataAdapter("select Restaurant.RestaurantId, Restaurant.RestaurantName, Restaurant.Location, Restaurant.Area, Restaurant.Offers, Restaurant.ProfilePic, Restaurant.CoverPic, Restaurant.RestaurantPassword, subquery.avgRate  from Restaurant, (Select RestaurantRating.RestaurantId, avg(RestaurantRating.Rating) as avgRate from RestaurantRating group by RestaurantRating.RestaurantId) subquery where subquery.RestaurantId = Restaurant.RestaurantId AND subquery.avgRate >= '" + TempData["rating"] + "'and RestaurantName LIKE '%" + TempData["search"] + "%'", sqlCon);
                        sqlDa.Fill(dtbUser);
                    }
                    else if (!TempData["location"].Equals("all") && (TempData["orderBy"].Equals("rating")))
                    {

                        SqlDataAdapter sqlDa = new SqlDataAdapter("select Restaurant.RestaurantId, Restaurant.RestaurantName, Restaurant.Location, Restaurant.Area, Restaurant.Offers, Restaurant.ProfilePic, Restaurant.CoverPic, Restaurant.RestaurantPassword, subquery.avgRate  from Restaurant, (Select RestaurantRating.RestaurantId, avg(RestaurantRating.Rating) as avgRate from RestaurantRating group by RestaurantRating.RestaurantId) subquery where subquery.RestaurantId = Restaurant.RestaurantId AND Restaurant.Area ='" + TempData["location"] + "' ORDER BY subquery.avgRate ASC", sqlCon);
                        sqlDa.Fill(dtbUser);
                    }
                    else if (!TempData["rating"].Equals("0") && (TempData["orderBy"].Equals("None")))
                    {


                        SqlDataAdapter sqlDa = new SqlDataAdapter("select Restaurant.RestaurantId, Restaurant.RestaurantName, Restaurant.Location, Restaurant.Area, Restaurant.Offers, Restaurant.ProfilePic, Restaurant.CoverPic, Restaurant.RestaurantPassword, subquery.avgRate  from Restaurant, (Select RestaurantRating.RestaurantId, avg(RestaurantRating.Rating) as avgRate from RestaurantRating group by RestaurantRating.RestaurantId) subquery where subquery.RestaurantId = Restaurant.RestaurantId AND subquery.avgRate >='" + TempData["rating"] + "'", sqlCon);
                        sqlDa.Fill(dtbUser);
                    }
                    else if ((TempData["search"] != null) && (TempData["orderBy"].Equals("None")))
                    {

                        SqlDataAdapter sqlDa = new SqlDataAdapter("select Restaurant.RestaurantId, Restaurant.RestaurantName, Restaurant.Location, Restaurant.Area, Restaurant.Offers, Restaurant.ProfilePic, Restaurant.CoverPic, Restaurant.RestaurantPassword, subquery.avgRate  from Restaurant, (Select RestaurantRating.RestaurantId, avg(RestaurantRating.Rating) as avgRate from RestaurantRating group by RestaurantRating.RestaurantId) subquery where subquery.RestaurantId = Restaurant.RestaurantId AND Restaurant.RestaurantName LIKE '%" + TempData["search"] + "%'", sqlCon);
                        sqlDa.Fill(dtbUser);
                    }
                    else if (TempData["orderBy"].Equals("rating"))
                    {

                        SqlDataAdapter sqlDa = new SqlDataAdapter("select Restaurant.RestaurantId, Restaurant.RestaurantName, Restaurant.Location, Restaurant.Area, Restaurant.Offers, Restaurant.ProfilePic, Restaurant.CoverPic, Restaurant.RestaurantPassword, subquery.avgRate  from Restaurant, (Select RestaurantRating.RestaurantId, avg(RestaurantRating.Rating) as avgRate from RestaurantRating group by RestaurantRating.RestaurantId) subquery where subquery.RestaurantId = Restaurant.RestaurantId ORDER BY subquery.avgRate ASC", sqlCon);
                        sqlDa.Fill(dtbUser);
                    }

                }
                else
                {
                    SqlDataAdapter sqlDa = new SqlDataAdapter(" select Restaurant.RestaurantId, Restaurant.RestaurantName,Restaurant.Location, Restaurant.Area,Restaurant.Offers,Restaurant.ProfilePic,Restaurant.CoverPic,Restaurant.RestaurantPassword,subquery.avgRate  from Restaurant, (Select RestaurantRating.RestaurantId, avg(RestaurantRating.Rating) as avgRate from RestaurantRating group by RestaurantRating.RestaurantId) subquery where subquery.RestaurantId = Restaurant.RestaurantId ", sqlCon);
                    sqlDa.Fill(dtbUser);
                }

            }
            return View(dtbUser);

        }
        public ActionResult HomeMadeFoods()
        {
            var dtbUser = new DataTable();
            using (var sqlCon = new SqlConnection(connectionString))
            {

                sqlCon.Open();
                if (TempData["searchHomeFood"] != null)
                {

                    if ((!TempData["location"].Equals("all")) && (!TempData["rating"].Equals("0")) && (!TempData["Availability"].Equals("all")) && (TempData["search"] != null))
                    {

                        SqlDataAdapter sqlDa = new SqlDataAdapter("select * from HomeMadeFood inner join HomeChef on HomeMadeFood.HomeChefId=HomeChef.HomeChefId inner join [User] on  HomeChef.UserId=[User].UserId inner join HomeChefRating on HomeChef.HomeChefId = HomeChefRating.HomeChefId " +
                            "where Area='" + TempData["location"] + "'and HomeChefRating >= '" + TempData["rating"] + "' and AvailabilityStatus  = '" + TempData["Availability"] + "'and HomemadeFoodName LIKE '%" + TempData["search"] + "%'", sqlCon);
                        sqlDa.Fill(dtbUser);


                    }

                    else if ((!TempData["rating"].Equals("0")) && (!TempData["Availability"].Equals("all")) && (TempData["search"] != null))
                    {

                        SqlDataAdapter sqlDa = new SqlDataAdapter("select * from HomeMadeFood inner join HomeChef on HomeMadeFood.HomeChefId=HomeChef.HomeChefId inner join [User] on  HomeChef.UserId=[User].UserId inner join HomeChefRating on HomeChef.HomeChefId = HomeChefRating.HomeChefId " +
                             "where HomeChefRating >= '" + TempData["rating"] + "' and AvailabilityStatus  = '" + TempData["Availability"] + "'and HomemadeFoodName LIKE '%" + TempData["search"] + "%'", sqlCon);

                        sqlDa.Fill(dtbUser);


                    }
                    else if ((!TempData["rating"].Equals("0")) && (!TempData["location"].Equals("all")) && (TempData["search"] != null))
                    {
                        SqlDataAdapter sqlDa = new SqlDataAdapter("select * from HomeMadeFood inner join HomeChef on HomeMadeFood.HomeChefId=HomeChef.HomeChefId inner join [User] on  HomeChef.UserId=[User].UserId inner join HomeChefRating on HomeChef.HomeChefId = HomeChefRating.HomeChefId " +
                           "where Area='" + TempData["location"] + "'and HomeChefRating >= '" + TempData["rating"] + "'and HomemadeFoodName LIKE '%" + TempData["search"] + "%'", sqlCon);
                        sqlDa.Fill(dtbUser);

                    }

                    else if ((!TempData["rating"].Equals("0")) && (!TempData["location"].Equals("all")) && !TempData["Availability"].Equals("all"))
                    {
                        SqlDataAdapter sqlDa = new SqlDataAdapter("select * from HomeMadeFood inner join HomeChef on HomeMadeFood.HomeChefId=HomeChef.HomeChefId inner join [User] on  HomeChef.UserId=[User].UserId inner join HomeChefRating on HomeChef.HomeChefId = HomeChefRating.HomeChefId " +
                            "where Area='" + TempData["location"] + "'and HomeChefRating >= '" + TempData["rating"] + "' and AvailabilityStatus  = '" + TempData["Availability"] + "'", sqlCon);
                        sqlDa.Fill(dtbUser);
                    }
                    else if (!TempData["location"].Equals("all"))
                    {

                        SqlDataAdapter sqlDa = new SqlDataAdapter("select * from HomeMadeFood inner join HomeChef on HomeMadeFood.HomeChefId=HomeChef.HomeChefId inner join [User] on  HomeChef.UserId=[User].UserId where Area='" + TempData["location"] + "'", sqlCon);
                        sqlDa.Fill(dtbUser);


                    }
                    else if (!TempData["rating"].Equals("0"))
                    {

                        SqlDataAdapter sqlDa = new SqlDataAdapter("select * from HomeMadeFood inner join HomeChef on HomeMadeFood.HomeChefId=HomeChef.HomeChefId inner join [User] on  HomeChef.UserId=[User].UserId inner join HomeChefRating on HomeChef.HomeChefId = HomeChefRating.HomeChefId where HomeChefRating >='" + TempData["rating"] + "'", sqlCon);
                        sqlDa.Fill(dtbUser);

                    }

                    else if (!TempData["Availability"].Equals("all"))
                    {
                        SqlDataAdapter sqlDa = new SqlDataAdapter("select * from HomeMadeFood inner join HomeChef on HomeMadeFood.HomeChefId=HomeChef.HomeChefId inner join [User] on  HomeChef.UserId=[User].UserId where AvailabilityStatus='" + TempData["Availability"] + "'", sqlCon);
                        sqlDa.Fill(dtbUser);

                    }

                    else if (TempData["search"] != null)
                    {


                        SqlDataAdapter sqlDa = new SqlDataAdapter("select * from HomeMadeFood inner join HomeChef on HomeMadeFood.HomeChefId=HomeChef.HomeChefId inner join [User] on  HomeChef.UserId=[User].UserId where HomemadeFoodName LIKE '%" + TempData["search"] + "%'", sqlCon);
                        sqlDa.Fill(dtbUser);

                    }


                }
                else
                {
                    var userIdActive = Session["userActive"];
                    SqlDataAdapter sqlDa = new SqlDataAdapter("select * from HomeMadeFood inner join HomeChef on HomeMadeFood.HomeChefId=HomeChef.HomeChefId inner join [User] on  HomeChef.UserId=[User].UserId;", sqlCon);
                    sqlDa.Fill(dtbUser);

                }

            }

            return View(dtbUser);

        }
        /*for searching HomeFoodSearch*/
        public ActionResult HomeFoodSearch(FormCollection form)
        {
            string search = form["keyword"];
            var Availability = form["Availability"];

            string location = form["HFlocation"];
            string rating = form["HfRating"];
            TempData["Availability"] = Availability;
            TempData["search"] = search;
            TempData["location"] = location;
            TempData["rating"] = rating;
            TempData["searchHomeFood"] = 1;
            return RedirectToAction("HomeMadeFoods");

        }
        /*for searching HomeFoodSearch*/


        public static List<String> ingredientList = new List<string>();

        public ActionResult Recipes()
        {
            TempData["recipeStar"] = 0;
            var dtbUser = new DataTable();
            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                if (TempData["recipeNameSearched"] != null)
                {
                    if ((TempData["recipeName"] != null) && (TempData["ingAddedToList"] == null))
                    {
                        SqlDataAdapter sqlDa = new SqlDataAdapter("select Recipe.RecipeId,Recipe.RecipeWriterId,Recipe.RecipeName,Recipe.RecipePublishDate,Recipe.RecipePicture,Recipe.RecipeCookingTime,Recipe.RecipeServing,Recipe.RecipeIngredients,Recipe.Process,Recipe.Description,RecipeWriter.RecipeWriterId,RecipeWriter.UserId,RecipeWriter.About,RecipeWriter.FbLink,RecipeWriter.InstaLink,RecipeWriter.TwitterLink,[User].UserId,[User].Email,[User].Password,[User].PhoneNo,[User].Picture,[User].UserName,avg(RecipeRating.RecipeRating) AS RecipeRateAVG from Recipe inner join RecipeWriter on Recipe.RecipeWriterId=RecipeWriter.RecipeWriterId inner join [User] on RecipeWriter.UserId=[User].UserId left join RecipeRating on RecipeRating.RecipeId = Recipe.RecipeId group by [User].UserId,[User].Email,[User].Password,[User].PhoneNo,[User].Picture,[User].UserName,Recipe.RecipeId,Recipe.RecipeWriterId,Recipe.RecipeName,Recipe.RecipePublishDate,Recipe.RecipePicture,Recipe.RecipeCookingTime,Recipe.RecipeServing,Recipe.RecipeIngredients,Recipe.Process,Recipe.Description,RecipeWriter.RecipeWriterId,RecipeWriter.UserId,RecipeWriter.About,RecipeWriter.FbLink,RecipeWriter.InstaLink,RecipeWriter.TwitterLink having Recipe.RecipeName LIKE '%" + TempData["recipeName"] + "%'", sqlCon);
                        sqlDa.Fill(dtbUser);

                    }

                    else if ((TempData["recipeName"] != null) && (TempData["ingAddedToList"] != null))
                    {

                        var list = TempData["List"] as List<String>;
                        if (list.Count() == 3)
                        {
                            SqlDataAdapter sqlDa = new SqlDataAdapter("select Recipe.RecipeId,Recipe.RecipeWriterId,Recipe.RecipeName,Recipe.RecipePublishDate,Recipe.RecipePicture,Recipe.RecipeCookingTime,Recipe.RecipeServing,Recipe.RecipeIngredients,Recipe.Process,Recipe.Description,RecipeWriter.RecipeWriterId,RecipeWriter.UserId,RecipeWriter.About,RecipeWriter.FbLink,RecipeWriter.InstaLink,RecipeWriter.TwitterLink,[User].UserId,[User].Email,[User].Password,[User].PhoneNo,[User].Picture,[User].UserName,avg(RecipeRating.RecipeRating) AS RecipeRateAVG from Recipe inner join RecipeWriter on Recipe.RecipeWriterId=RecipeWriter.RecipeWriterId inner join [User] on RecipeWriter.UserId=[User].UserId left join RecipeRating on RecipeRating.RecipeId = Recipe.RecipeId group by [User].UserId,[User].Email,[User].Password,[User].PhoneNo,[User].Picture,[User].UserName,Recipe.RecipeId,Recipe.RecipeWriterId,Recipe.RecipeName,Recipe.RecipePublishDate,Recipe.RecipePicture,Recipe.RecipeCookingTime,Recipe.RecipeServing,Recipe.RecipeIngredients,Recipe.Process,Recipe.Description,RecipeWriter.RecipeWriterId,RecipeWriter.UserId,RecipeWriter.About,RecipeWriter.FbLink,RecipeWriter.InstaLink,RecipeWriter.TwitterLink having Recipe.RecipeIngredients LIKE '%" + list[0] + "%' AND Recipe.RecipeIngredients LIKE '%" + list[1] + "%' AND Recipe.RecipeIngredients LIKE '%" + list[2] + "%'", sqlCon);
                            sqlDa.Fill(dtbUser);
                        }
                        if (list.Count() == 2)
                        {
                            SqlDataAdapter sqlDa = new SqlDataAdapter("select Recipe.RecipeId,Recipe.RecipeWriterId,Recipe.RecipeName,Recipe.RecipePublishDate,Recipe.RecipePicture,Recipe.RecipeCookingTime,Recipe.RecipeServing,Recipe.RecipeIngredients,Recipe.Process,Recipe.Description,RecipeWriter.RecipeWriterId,RecipeWriter.UserId,RecipeWriter.About,RecipeWriter.FbLink,RecipeWriter.InstaLink,RecipeWriter.TwitterLink,[User].UserId,[User].Email,[User].Password,[User].PhoneNo,[User].Picture,[User].UserName,avg(RecipeRating.RecipeRating) AS RecipeRateAVG from Recipe inner join RecipeWriter on Recipe.RecipeWriterId=RecipeWriter.RecipeWriterId inner join [User] on RecipeWriter.UserId=[User].UserId left join RecipeRating on RecipeRating.RecipeId = Recipe.RecipeId group by [User].UserId,[User].Email,[User].Password,[User].PhoneNo,[User].Picture,[User].UserName,Recipe.RecipeId,Recipe.RecipeWriterId,Recipe.RecipeName,Recipe.RecipePublishDate,Recipe.RecipePicture,Recipe.RecipeCookingTime,Recipe.RecipeServing,Recipe.RecipeIngredients,Recipe.Process,Recipe.Description,RecipeWriter.RecipeWriterId,RecipeWriter.UserId,RecipeWriter.About,RecipeWriter.FbLink,RecipeWriter.InstaLink,RecipeWriter.TwitterLink having Recipe.RecipeIngredients LIKE '%" + list[0] + "%' AND Recipe.RecipeIngredients LIKE '%" + list[1] + "%'", sqlCon);
                            sqlDa.Fill(dtbUser);
                        }
                        if (list.Count() == 1)
                        {
                            SqlDataAdapter sqlDa = new SqlDataAdapter("select Recipe.RecipeId,Recipe.RecipeWriterId,Recipe.RecipeName,Recipe.RecipePublishDate,Recipe.RecipePicture,Recipe.RecipeCookingTime,Recipe.RecipeServing,Recipe.RecipeIngredients,Recipe.Process,Recipe.Description,RecipeWriter.RecipeWriterId,RecipeWriter.UserId,RecipeWriter.About,RecipeWriter.FbLink,RecipeWriter.InstaLink,RecipeWriter.TwitterLink,[User].UserId,[User].Email,[User].Password,[User].PhoneNo,[User].Picture,[User].UserName,avg(RecipeRating.RecipeRating) AS RecipeRateAVG from Recipe inner join RecipeWriter on Recipe.RecipeWriterId=RecipeWriter.RecipeWriterId inner join [User] on RecipeWriter.UserId=[User].UserId left join RecipeRating on RecipeRating.RecipeId = Recipe.RecipeId group by [User].UserId,[User].Email,[User].Password,[User].PhoneNo,[User].Picture,[User].UserName,Recipe.RecipeId,Recipe.RecipeWriterId,Recipe.RecipeName,Recipe.RecipePublishDate,Recipe.RecipePicture,Recipe.RecipeCookingTime,Recipe.RecipeServing,Recipe.RecipeIngredients,Recipe.Process,Recipe.Description,RecipeWriter.RecipeWriterId,RecipeWriter.UserId,RecipeWriter.About,RecipeWriter.FbLink,RecipeWriter.InstaLink,RecipeWriter.TwitterLink having Recipe.RecipeIngredients LIKE '%" + list[0] + "%'", sqlCon);
                            sqlDa.Fill(dtbUser);
                        }


                    }
                }

                else
                {
                    SqlDataAdapter sqlDa = new SqlDataAdapter("select Recipe.RecipeId,Recipe.RecipeWriterId,Recipe.RecipeName,Recipe.RecipePublishDate,Recipe.RecipePicture,Recipe.RecipeCookingTime,Recipe.RecipeServing,Recipe.RecipeIngredients,Recipe.Process,Recipe.Description,RecipeWriter.RecipeWriterId,RecipeWriter.UserId,RecipeWriter.About,RecipeWriter.FbLink,RecipeWriter.InstaLink,RecipeWriter.TwitterLink,[User].UserId,[User].Email,[User].Password,[User].PhoneNo,[User].Picture,[User].UserName,avg(RecipeRating.RecipeRating) AS RecipeRateAVG from Recipe inner join RecipeWriter on Recipe.RecipeWriterId=RecipeWriter.RecipeWriterId inner join [User] on RecipeWriter.UserId=[User].UserId left join RecipeRating on RecipeRating.RecipeId = Recipe.RecipeId group by [User].UserId,[User].Email,[User].Password,[User].PhoneNo,[User].Picture,[User].UserName,Recipe.RecipeId,Recipe.RecipeWriterId,Recipe.RecipeName,Recipe.RecipePublishDate,Recipe.RecipePicture,Recipe.RecipeCookingTime,Recipe.RecipeServing,Recipe.RecipeIngredients,Recipe.Process,Recipe.Description,RecipeWriter.RecipeWriterId,RecipeWriter.UserId,RecipeWriter.About,RecipeWriter.FbLink,RecipeWriter.InstaLink,RecipeWriter.TwitterLink", sqlCon);
                    sqlDa.Fill(dtbUser);

                }



            }

            return View(dtbUser);

        }

        //Recipe Search
        public ActionResult RecipeSearch(FormCollection form)
        {
            string searchName = form["keyword"];
            TempData["recipeName"] = searchName;
            TempData["recipeNameSearched"] = 1;
            return RedirectToAction("Recipes");
        }



        //IngredientList add
        public ActionResult IngredientAdd(FormCollection form)
        {

            string item = form["ingr_add"];
            ingredientList.Add(item);
            TempData["List"] = ingredientList.ToList();
            TempData["ingAddedToList"] = 1;
            return RedirectToAction("Recipes");
        }

        //IngredientList Delete
        public ActionResult IngredientDelete(FormCollection form)
        {
            string item = form["ingr_delete"];
            ingredientList.Remove(item);
            TempData["List"] = ingredientList.ToList();

            return RedirectToAction("Recipes");
        }




        public ActionResult FoodBlogs()
        {
            var dtbUser = new DataTable();
            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                if (TempData["searchBlog"] != null && TempData["search"] != null)
                {

                    SqlDataAdapter sqlDa = new SqlDataAdapter("select * from Blog inner join Blogger on Blog.BloggerId=Blogger.BloggerId inner join [User] on Blogger.UserId=[User].UserId where BlogName LIKE '%" + TempData["search"] + "%' or UserName LIKE '%" + TempData["search"] + "%' ;", sqlCon);
                    sqlDa.Fill(dtbUser);
                }
                else
                {

                    SqlDataAdapter sqlDa = new SqlDataAdapter("select * from Blog inner join Blogger on Blog.BloggerId=Blogger.BloggerId inner join [User] on Blogger.UserId=[User].UserId ;", sqlCon);
                    sqlDa.Fill(dtbUser);
                }
            }

            return View(dtbUser);
        }
        /*for searching Blog Search*/
        public ActionResult BlogSearch(FormCollection form)
        {
            string search = form["keyword"];
            TempData["search"] = search;
            TempData["searchBlog"] = 1;
            return RedirectToAction("FoodBlogs");

        }

        //BlogComment add
        public ActionResult BlogComment(FormCollection form)
        {
            string comment = form["comment_add"];
            var userId = Session["userActive"];

            if (!Session["recipeWriterActive"].Equals(0))
            {
                userId = Convert.ToInt32(Session["recipeWriterActive"]);
            }
            else if (!Session["homechefActive"].Equals(0))
            {
                userId = Convert.ToInt32(Session["homechefActive"]);
            }
            else if (!Session["bloggerActive"].Equals(0))
            {
                userId = Convert.ToInt32(Session["bloggerActive"]);
            }
            else
            {
                userId = Convert.ToInt32(Session["userActive"]);
            }


            var blogId = TempData["clickedBlogId2"];
            using (var sqlCon1 = new SqlConnection(connectionString))
            {
                sqlCon1.Open();
                string query = "insert into BlogComment values (@BlogId,@UserId,@BlogComment)";
                var sqlCmd = new SqlCommand(query, sqlCon1);
                sqlCmd.Parameters.AddWithValue("@BlogId", blogId);

                sqlCmd.Parameters.AddWithValue("@UserId", userId);
                sqlCmd.Parameters.AddWithValue("@BlogComment", comment);

                sqlCmd.ExecuteNonQuery();
            }

            return RedirectToAction("BlogClick", "Home", new { id = blogId });


        }

        public ActionResult Login()
        {
            return View();
        }
        public ActionResult SignUp()
        {
            return View();
        }
        public ActionResult ViewRecipe()
        {
            if (TempData["recipePosted"] != null)
            {
                var dtbUser = new DataTable();
                using (var sqlCon = new SqlConnection(connectionString))
                {

                    sqlCon.Open();
                    var recipeId = TempData["lastRecipeId"];
                    SqlDataAdapter sqlDa = new SqlDataAdapter("select *, avg(RecipeRating.RecipeRating) AS RecipeRateAVG from Recipe inner join RecipeWriter on Recipe.RecipeWriterId = RecipeWriter.RecipeWriterId inner join [User] on RecipeWriter.UserId =[User].UserId left join RecipeComment on Recipe.RecipeId = RecipeComment.RecipeId left join [User] as userComment on RecipeComment.UserId = userComment.UserId left join RecipeRating on RecipeRating.RecipeId = Recipe.RecipeId group by [User].UserId,[User].Email,[User].Password,[User].PhoneNo,[User].Picture,[User].UserName, Recipe.RecipeId, Recipe.RecipeWriterId, Recipe.RecipeName, Recipe.RecipePublishDate, Recipe.RecipePicture, Recipe.RecipeCookingTime, Recipe.RecipeServing, Recipe.RecipeIngredients, Recipe.Process, Recipe.Description, RecipeComment.RecipeCommentId, RecipeComment.RecipeComment, RecipeComment.RecipeId, RecipeComment.UserId, RecipeRating.RecipeRatingId,RecipeRating.RecipeRating ,RecipeRating.RecipeId, RecipeRating.UserId, RecipeWriter.RecipeWriterId, RecipeWriter.UserId, RecipeWriter.About, RecipeWriter.FbLink, RecipeWriter.InstaLink, RecipeWriter.TwitterLink, userComment.UserId, userComment.Email, userComment.Password, userComment.PhoneNo, userComment.Picture, userComment.UserName having Recipe.RecipeId = '" + recipeId + "'", sqlCon);
                    sqlDa.Fill(dtbUser);
                }
                return View(dtbUser);
            }
            else
            {
                var dtbUser = new DataTable();
                using (var sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    var recipeId = TempData["clickedRecipeId"];
                    SqlDataAdapter sqlDa = new SqlDataAdapter("select *,avg(RecipeRating.RecipeRating) AS RecipeRateAVG from Recipe inner join RecipeWriter on Recipe.RecipeWriterId = RecipeWriter.RecipeWriterId inner join [User] on RecipeWriter.UserId=[User].UserId left join RecipeComment on Recipe.RecipeId = RecipeComment.RecipeId left join [User] as userComment on RecipeComment.UserId = userComment.UserId left join RecipeRating on RecipeRating.RecipeId = Recipe.RecipeId group by [User].UserId,[User].Email,[User].Password,[User].PhoneNo,[User].Picture,[User].UserName, Recipe.RecipeId, Recipe.RecipeWriterId, Recipe.RecipeName, Recipe.RecipePublishDate, Recipe.RecipePicture, Recipe.RecipeCookingTime, Recipe.RecipeServing, Recipe.RecipeIngredients, Recipe.Process, Recipe.Description, RecipeComment.RecipeCommentId, RecipeComment.RecipeComment, RecipeComment.RecipeId, RecipeComment.UserId, RecipeRating.RecipeRatingId,RecipeRating.RecipeRating, RecipeRating.RecipeId, RecipeRating.UserId, RecipeWriter.RecipeWriterId, RecipeWriter.UserId, RecipeWriter.About, RecipeWriter.FbLink, RecipeWriter.InstaLink, RecipeWriter.TwitterLink, userComment.UserId, userComment.Email, userComment.Password, userComment.PhoneNo, userComment.Picture, userComment.UserName having Recipe.RecipeId = '" + recipeId + "'", sqlCon);
                    sqlDa.Fill(dtbUser);
                    TempData["recipePosted"] = 0;
                }
                return View(dtbUser);

            }



        }
        public ActionResult ViewRecipeClick(int id)
        {

            TempData["clickedRecipeId"] = id;
            TempData["clickRecipeId"] = TempData["clickedRecipeId"];
            return RedirectToAction("ViewRecipe");

        }

        //RecipeComment add
        public ActionResult RecipeComment(FormCollection form)
        {
            string comment = form["comment_add"];
            var userId = Session["userActive"];
            var recipeId = TempData["clickRecipeId"];

            if (!Session["recipeWriterActive"].Equals(0))
            {
                userId = Convert.ToInt32(Session["recipeWriterActive"]);
            }
            else if (!Session["homechefActive"].Equals(0))
            {
                userId = Convert.ToInt32(Session["homechefActive"]);
            }
            else if (!Session["bloggerActive"].Equals(0))
            {
                userId = Convert.ToInt32(Session["bloggerActive"]);
            }
            else
            {
                userId = Convert.ToInt32(Session["userActive"]);
            }

            using (var sqlCon1 = new SqlConnection(connectionString))
            {
                sqlCon1.Open();
                string query = "insert into RecipeComment values (@RecipeId,@RecipeComment,@UserId)";
                var sqlCmd = new SqlCommand(query, sqlCon1);
                sqlCmd.Parameters.AddWithValue("@RecipeId", recipeId);
                sqlCmd.Parameters.AddWithValue("@RecipeComment", comment);
                sqlCmd.Parameters.AddWithValue("@UserId", userId);

                sqlCmd.ExecuteNonQuery();
            }

            return RedirectToAction("ViewRecipeClick", "Home", new { id = recipeId });

        }

        //Recipe Rating 
        [HttpPost]
        public ActionResult RecipeRating(FormCollection form)
        {
            var rating = form["rate"];
            var userId = 0;
            TempData["recipeStar"] = rating;
            if (!Session["recipeWriterActive"].Equals(0))
            {
                userId = Convert.ToInt32(Session["recipeWriterActive"]);
            }
            else if (!Session["homechefActive"].Equals(0))
            {
                userId = Convert.ToInt32(Session["homechefActive"]);
            }
            else if (!Session["bloggerActive"].Equals(0))
            {
                userId = Convert.ToInt32(Session["bloggerActive"]);
            }
            else
            {
                userId = Convert.ToInt32(Session["userActive"]);
            }

            var recipeId = TempData["clickRecipeId"];
            using (var sqlCon1 = new SqlConnection(connectionString))
            {
                sqlCon1.Open();
                string query = "insert into RecipeRating values (@RecipeRating,@RecipeId,@UserId)";
                var sqlCmd = new SqlCommand(query, sqlCon1);
                sqlCmd.Parameters.AddWithValue("@RecipeRating", rating);
                sqlCmd.Parameters.AddWithValue("@RecipeId", recipeId);
                sqlCmd.Parameters.AddWithValue("@UserId", userId);

                sqlCmd.ExecuteNonQuery();
            }

            return RedirectToAction("ViewRecipeClick", "Home", new { id = recipeId });

        }

        public ActionResult ViewRecipeWriterClick(int id)
        {

            TempData["clickedRecipeWriterId"] = id;
            return RedirectToAction("RecipeWriterProfile");

        }
        public ActionResult RecipeWriterProfile()
        {
            var dtbUser = new DataTable();
            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();

                if ((!Session["recipeWriterActive"].Equals(0)) && (TempData["clickedRecipeWriterId"] == null))
                {
                    var userIdActive1 = Session["recipeWriterActive"];
                    SqlDataAdapter sqlDa1 = new SqlDataAdapter("select [User].UserId,[User].Email,[User].Password,[User].PhoneNo,[User].Picture,[User].UserName,Recipe.RecipeId,Recipe.RecipeWriterId,Recipe.RecipeName,Recipe.RecipePublishDate,Recipe.RecipePicture,Recipe.RecipeCookingTime,Recipe.RecipeServing,Recipe.RecipeIngredients,Recipe.Process,Recipe.Description,RecipeWriter.RecipeWriterId,RecipeWriter.UserId,RecipeWriter.About,RecipeWriter.FbLink,RecipeWriter.InstaLink,RecipeWriter.TwitterLink,avg(RecipeRating.RecipeRating) AS RecipeRateAVG from [User] Inner join RecipeWriter on [User].UserId = RecipeWriter.UserId  left join Recipe on Recipe.RecipeWriterId = RecipeWriter.RecipeWriterId left join RecipeRating on RecipeRating.RecipeId = Recipe.RecipeId group by[User].UserId,[User].Email,[User].Password,[User].PhoneNo,[User].Picture,[User].UserName, Recipe.RecipeId, Recipe.RecipeWriterId, Recipe.RecipeName, Recipe.RecipePublishDate, Recipe.RecipePicture, Recipe.RecipeCookingTime, Recipe.RecipeServing, Recipe.RecipeIngredients, Recipe.Process, Recipe.Description, RecipeWriter.RecipeWriterId, RecipeWriter.UserId, RecipeWriter.About, RecipeWriter.FbLink, RecipeWriter.InstaLink, RecipeWriter.TwitterLink having RecipeWriter.UserId = '" + userIdActive1 + "'", sqlCon);
                    sqlDa1.Fill(dtbUser);
                }

                var userIdActive = TempData["clickedRecipeWriterId"];
                SqlDataAdapter sqlDa = new SqlDataAdapter("select [User].UserId,[User].Email,[User].Password,[User].PhoneNo,[User].Picture,[User].UserName,Recipe.RecipeId,Recipe.RecipeWriterId,Recipe.RecipeName,Recipe.RecipePublishDate,Recipe.RecipePicture,Recipe.RecipeCookingTime,Recipe.RecipeServing,Recipe.RecipeIngredients,Recipe.Process,Recipe.Description,RecipeWriter.RecipeWriterId,RecipeWriter.UserId,RecipeWriter.About,RecipeWriter.FbLink,RecipeWriter.InstaLink,RecipeWriter.TwitterLink,avg(RecipeRating.RecipeRating) AS RecipeRateAVG from [User] Inner join RecipeWriter on [User].UserId = RecipeWriter.UserId  left join Recipe on Recipe.RecipeWriterId = RecipeWriter.RecipeWriterId left join RecipeRating on RecipeRating.RecipeId = Recipe.RecipeId group by[User].UserId,[User].Email,[User].Password,[User].PhoneNo,[User].Picture,[User].UserName, Recipe.RecipeId, Recipe.RecipeWriterId, Recipe.RecipeName, Recipe.RecipePublishDate, Recipe.RecipePicture, Recipe.RecipeCookingTime, Recipe.RecipeServing, Recipe.RecipeIngredients, Recipe.Process, Recipe.Description, RecipeWriter.RecipeWriterId, RecipeWriter.UserId, RecipeWriter.About, RecipeWriter.FbLink, RecipeWriter.InstaLink, RecipeWriter.TwitterLink having RecipeWriter.UserId = '" + userIdActive + "'", sqlCon);
                sqlDa.Fill(dtbUser);


            }
            return View(dtbUser);

        }
        /*for searching RestaurantSearch*/

        public ActionResult RestaurantSearch(FormCollection form)
        {
            string search = form["keyword"];
            string location = form["location"];
            string rating = form["rating"];
            string orderBy = form["order_by"];
            TempData["search"] = search;
            TempData["location"] = location;
            TempData["rating"] = rating;
            TempData["orderBy"] = orderBy;
            TempData["searchRestaurantFood"] = 1;
            return RedirectToAction("Restaurants");

        }
        public ActionResult RestaurantClicked(int id)
        {

            TempData["resIdClicked"] = id;
            return RedirectToAction("ResMenuPage");
        }
        public ActionResult ResMenuPage()
        {
            var dtbUser = new DataTable();
            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                var resId = TempData["resIdClicked"];
                TempData["resClick"] = resId;
                SqlDataAdapter sqlDa = new SqlDataAdapter("select * from Menu where RestaurantId = '" + resId + "'", sqlCon);
                sqlDa.Fill(dtbUser);


            }
            return View(dtbUser);
        }

        //Restaurant Rating 
        public ActionResult RestaurantRating(FormCollection form)
        {
            var rating = form["rate"];
            TempData["resStar"] = rating;
            var userId = 0;
            if (!Session["recipeWriterActive"].Equals(0))
            {
                userId = Convert.ToInt32(Session["recipeWriterActive"]);
            }
            else if (!Session["homechefActive"].Equals(0))
            {
                userId = Convert.ToInt32(Session["homechefActive"]);
            }
            else if (!Session["bloggerActive"].Equals(0))
            {
                userId = Convert.ToInt32(Session["bloggerActive"]);
            }
            else
            {
                userId = Convert.ToInt32(Session["userActive"]);
            }

            var restaurantId = TempData["resClick"];
            using (var sqlCon1 = new SqlConnection(connectionString))
            {
                sqlCon1.Open();
                string query = "insert into RestaurantRating values (@Rating,@RestaurantId,@UserId)";
                var sqlCmd = new SqlCommand(query, sqlCon1);
                sqlCmd.Parameters.AddWithValue("@Rating", rating);
                sqlCmd.Parameters.AddWithValue("@RestaurantId", restaurantId);
                sqlCmd.Parameters.AddWithValue("@UserId", userId);

                sqlCmd.ExecuteNonQuery();
            }

            return RedirectToAction("RestaurantClicked", "Home", new { id = restaurantId });


        }

        public ActionResult FullBlog()
        {
            var dtbUser = new DataTable();
            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                var blogId = TempData["clickedBlogId"];
                SqlDataAdapter sqlDa = new SqlDataAdapter("select * from Blog inner join Blogger on Blog.BloggerId=Blogger.BloggerId inner join [User] on Blogger.UserId=[User].UserId left join BlogComment on Blog.BlogId= BlogComment.BlogId left join [User] as userComment on BlogComment.UserId = userComment.UserId where Blog.BlogId = '" + blogId + "'", sqlCon);
                sqlDa.Fill(dtbUser);
                TempData["BlogPosted"] = 0;
            }
            return View(dtbUser);



        }
        public ActionResult Blogger()
        {
            var dtbUser = new DataTable();
            var userIdActive1 = Session["bloggerActive"];
            TempData["BloggerId"] = 0;
            var userIdActive = TempData["BloggerId"];
            using (var sqlCon = new SqlConnection(connectionString))
            {

                sqlCon.Open();
                if (!Session["bloggerActive"].Equals(0))
                {
                    userIdActive1 = Session["bloggerActive"];
                    SqlDataAdapter sqlDa1 = new SqlDataAdapter("select * from Blogger inner join [User] on Blogger.UserId=[User].UserId where Blogger.UserId = '" + userIdActive1 + "'", sqlCon);
                    sqlDa1.Fill(dtbUser);
                }
                userIdActive = TempData["clickedBloggerId"];
                SqlDataAdapter sqlDa = new SqlDataAdapter("select * from Blogger inner join [User] on Blogger.UserId=[User].UserId where Blogger.BloggerId = '" + userIdActive + "'", sqlCon);
                sqlDa.Fill(dtbUser);

            }
            if (Session["bloggerActive"].Equals(0) && userIdActive != TempData["clickedBloggerId"])
            {
                return RedirectToAction("Index");
            }

            return View(dtbUser);

        }

        public ActionResult BlogClick(int id)
        {

            TempData["clickedBlogId"] = id;
            TempData["clickedBlogId2"] = id;
            return RedirectToAction("FullBlog");

        }
        public ActionResult ViewBloggerClick(int id)
        {

            TempData["clickedBloggerId"] = id;

            return RedirectToAction("Blogger");

        }

        public ActionResult HomeChefProfile()
        {

            var dtbUserHomeChef = new DataTable();
            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                //profile
                if ((Session["homeChefActive"] != null) && (TempData["clickedHomeChefId"] == null))
                {

                    var userIdActive1 = Session["homeChefActive"];

                    SqlDataAdapter sqlDaHome1 = new SqlDataAdapter("select *, avg(HomeChefRating.homechefrating) from[User] Inner join HomeChef on[User].UserId = HomeChef.UserId inner join homechefrating on homechefrating.homechefId = homechef.homechefId group by[User].UserId,[User].Email,[User].Password,[User].PhoneNo,[User].Picture,[User].UserName,Homechef.HomeChefId,Homechef.UserId,Homechef.FbLink,Homechef.InstaLink,Homechef.TwitterLink,Homechef.Address,Homechef.Area,homechefrating.homechefratingid, homechefrating.homechefrating, homechefrating.homechefid, homechefrating.userid having HomeChef.UserId = '" + userIdActive1 + "'", sqlCon);

                    sqlDaHome1.Fill(dtbUserHomeChef);
                }
                else
                {
                    var userIdActive = TempData["clickedHomeChefId"];

                    SqlDataAdapter sqlDaHome = new SqlDataAdapter("select *, avg(HomeChefRating.homechefrating) from[User] Inner join HomeChef on[User].UserId = HomeChef.UserId inner join homechefrating on homechefrating.homechefId = homechef.homechefId group by[User].UserId,[User].Email,[User].Password,[User].PhoneNo,[User].Picture,[User].UserName,Homechef.HomeChefId,Homechef.UserId,Homechef.FbLink,Homechef.InstaLink,Homechef.TwitterLink,Homechef.Address,Homechef.Area,homechefrating.homechefratingid, homechefrating.homechefrating, homechefrating.homechefid, homechefrating.userid having HomeChef.HomeChefId = '" + userIdActive + "'", sqlCon);

                    sqlDaHome.Fill(dtbUserHomeChef);
                }
                //check  the rating if user rated or not
                var userId1 = Session["userActive"];
                if (Session["recipeWriterActive"] != null)
                {
                    userId1 = Convert.ToInt32(Session["recipeWriterActive"]);
                }
                else if (Session["homechefActive"] != null)
                {
                    userId1 = Convert.ToInt32(Session["homechefActive"]);
                }
                else if (Session["bloggerActive"] != null)
                {
                    userId1 = Convert.ToInt32(Session["bloggerActive"]);
                }
                else
                {
                    userId1 = Convert.ToInt32(Session["userActive"]);
                }
                TempData["star"] = 0;
                var HomeChefActive2 = TempData["clickedHomeChefId"];
                string starQuery = "select * from HomechefRating where UserId='" + userId1 + "'and homechefId='" + HomeChefActive2 + "'";
                var sqlCmd2 = new SqlCommand(starQuery, sqlCon);
                SqlDataReader star = sqlCmd2.ExecuteReader();
                if (star.Read())
                {
                    int homechefRating = star.GetInt32(1);
                    if (homechefRating == 1)
                    {
                        TempData["star1"] = homechefRating;
                    }
                    else if (homechefRating == 2)
                    {
                        TempData["star2"] = homechefRating;
                    }
                    else if (homechefRating == 3)
                    {
                        TempData["star3"] = homechefRating;
                    }
                    else if (homechefRating == 4)
                    {
                        TempData["star4"] = homechefRating;
                    }
                    else if (homechefRating == 5)
                    {
                        TempData["star5"] = homechefRating;
                    }


                }
                else
                {
                    TempData["star"] = "0";
                }




            }

            return View(dtbUserHomeChef);

        }

        public ActionResult ViewHomeChefClick(int id)
        {
            TempData["clickedHomeChefId"] = id;
            TempData["clickHomeChefId"] = TempData["clickedHomeChefId"];
            return RedirectToAction("HomeChefProfile");

        }

        public ActionResult HomeChefComments()
        {
            var HomeChefId = TempData["clickedHomeChefId"];
            var dtbUserHomeChefFoodItems = new DataTable();
            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();

                SqlDataAdapter sqlDaHome1 = new SqlDataAdapter("select * from HomeChef Inner join HomeMadeFood on HomeChef.HomeChefId =HomeMadeFood.HomeChefId where HomeChef.HomeChefId  = '" + HomeChefId + "'", sqlCon);
                sqlDaHome1.Fill(dtbUserHomeChefFoodItems);
            }
            return View(dtbUserHomeChefFoodItems);

        }

        public ActionResult viewChefComments(int id)
        {
            TempData["clickedHomeChefId"] = id;
            TempData["clickHomeChefId"] = TempData["clickedHomeChefId"];
            return RedirectToAction("HomeChefComments");

        }
        public ActionResult DeleteHomeFood(int dId)
        {

            var DeleteId = dId;
            var dtbDeleteFoodItems = new DataTable();
            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();


                var sqlCmd = new SqlCommand("delete from homemadefood where homefoodid='" + DeleteId + "'", sqlCon);

                sqlCmd.ExecuteNonQuery();
                var dtbUserHomeChef = new DataTable();
                using (var sqlCon1 = new SqlConnection(connectionString))
                {
                    sqlCon1.Open();



                    SqlDataAdapter sqlDaHome1 = new SqlDataAdapter("select HomechefId from HomeMadeFood where HomeFoodID= '" + DeleteId + "'", sqlCon1);
                    sqlDaHome1.Fill(dtbUserHomeChef);
                    if (dtbUserHomeChef.Rows.Count > 0)
                    {
                        var HomeChefId = dtbUserHomeChef.Rows[0][0];
                        return RedirectToAction("HomeChefComments", new { id = HomeChefId });
                    }

                }
            }

            return RedirectToAction("HomeMadeFoods");

        }
        public ActionResult HomeChefRating(FormCollection form)
        {
            int HomeChefRating = Convert.ToInt32(form["rate"]);
            var userId1 = Session["userActive"];
            if (!Session["recipeWriterActive"].Equals(0))
            {
                userId1 = Convert.ToInt32(Session["recipeWriterActive"]);
            }
            else if (!Session["homechefActive"].Equals(0))
            {
                userId1 = Convert.ToInt32(Session["homechefActive"]);
            }
            else if (!Session["bloggerActive"].Equals(0))
            {
                userId1 = Convert.ToInt32(Session["bloggerActive"]);
            }
            
            var HomeChefId = TempData["clickHomeChefId"];
            using (var sqlCon1 = new SqlConnection(connectionString))
            {
                sqlCon1.Open();
                string query = "insert into HomeChefRating values (@HomeChefRating,@HomeChefId,@UserId)";
                var sqlCmd = new SqlCommand(query, sqlCon1);
                sqlCmd.Parameters.AddWithValue("@HomeChefRating", HomeChefRating);
                sqlCmd.Parameters.AddWithValue("@HomeChefId", HomeChefId);
                sqlCmd.Parameters.AddWithValue("@UserId", userId1);

                sqlCmd.ExecuteNonQuery();
            }

            return RedirectToAction("ViewHomeChefClick", "Home", new { id = HomeChefId });

        }
        public ActionResult UpdateHomeChef(int id)
        {
            TempData["homeChefUserId"] = id;
        
            return RedirectToAction("UpdateHomeChefForm");

        }
        [HttpPost]
        public ActionResult UpdateHomeChefForm(FormCollection form)
        {
            var homeChefUserId = TempData["homeChefUserId"];
            string userName = form["HomeChefNameEdit"];
            string number = form["HomeChefNumberEdit"];
            string email = form["HomeCheEmailEdit"];
            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();

              
                string query = " UPDATE [User] SET UserName = '" + userName + "'WHERE UserId ='" + homeChefUserId + "'" ;
                var sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.ExecuteNonQuery();
                

            }
            return RedirectToAction("HomeMadeFoods");

        }

        public ActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ContactUs(FormCollection form)
        {
            Contact contact = new Contact();
            contact.email = form["input_mail"];
            contact.phoneno = form["input_phone"];
            contact.contactMessage = form["input_msg"];
            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "select * from [User] where Email='" + contact.email + "'";
                var sqlCmd = new SqlCommand(query, sqlCon);

                SqlDataReader dr = sqlCmd.ExecuteReader();
                if (dr.Read())
                {
                    int userIdContact = dr.GetInt32(0);
                    contact.userId = userIdContact;
                    sqlCon.Close();
                }
                else
                {
                    sqlCon.Close();
                    Response.Write("<script>alert('No UserId Found!!')</script>");
                }
                sqlCon.Open();

                string query1 = "insert into Contact values (@Email,@Phone,@ContactMessage,@UserId)";
                var sqlCmd1 = new SqlCommand(query1, sqlCon);

                if (!string.IsNullOrEmpty(contact.email))
                {
                    string mail = contact.email.ToString();
                    if (mail.EndsWith("@gmail.com") || mail.EndsWith("@yahoo.com") || mail.EndsWith("@outlook.com"))
                    {
                        sqlCmd1.Parameters.AddWithValue("@Email", contact.email);
                    }
                    else
                    {
                        TempData["email"] = "4";
                        return RedirectToAction("ContactUs");
                    }

                }
                else
                {
                    TempData["email"] = "4";
                    return RedirectToAction("ContactUs");
                }



                if (!string.IsNullOrEmpty(contact.phoneno))
                {
                    string phn = contact.phoneno;
                    if (phn.Length == 11)
                    {
                        sqlCmd1.Parameters.AddWithValue("@Phone", contact.phoneno);
                    }
                    else
                    {
                        TempData["phoneNo"] = "5";

                        return RedirectToAction("ContactUs");
                    }

                }
                else
                {
                    TempData["phoneNo"] = "5";

                    return RedirectToAction("ContactUs");
                }

                if (!string.IsNullOrEmpty(contact.contactMessage))
                {
                    sqlCmd1.Parameters.AddWithValue("@ContactMessage", contact.contactMessage);
                }
                else
                {
                    TempData["message"] = "6";

                    return RedirectToAction("ContactUs");
                }


                sqlCmd1.Parameters.AddWithValue("@UserId", contact.userId);

                sqlCmd1.ExecuteNonQuery();
                Response.Write("<script>alert('Message Submitted')</script>");




            }

            return View();
        }


        public ActionResult RecipeWriterSignup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RecipeWriterSignup(FormCollection formCollection)
        {
            RecipeWriterSignup RecipeWriterSignup = new RecipeWriterSignup();
            RecipeWriterSignup.FbLink = formCollection["fbLinkInput"];
            RecipeWriterSignup.InstaLink = formCollection["instaLinkInput"];
            RecipeWriterSignup.TwitterLink = formCollection["twitterLinkInput"];
            RecipeWriterSignup.About = formCollection["aboutInput"];


            using (var sqlCon7 = new SqlConnection(connectionString))
            {
                sqlCon7.Open();
                string queryUserId = "SELECT TOP 1 UserId FROM [User] ORDER BY UserId DESC";

                var dtbUser = new DataTable();
                using (var sqlCon8 = new SqlConnection(connectionString))
                {
                    sqlCon8.Open();
                    SqlDataAdapter sqlDa = new SqlDataAdapter(queryUserId, sqlCon8);
                    sqlDa.Fill(dtbUser);
                    if (dtbUser.Rows.Count > 0)
                    {
                        var lastUserId = dtbUser.Rows[0][0];

                        string query = "insert into RecipeWriter values(@UserId,@About,@FbLink,@InstaLink,@TwitterLink)";
                        var sqlCmd = new SqlCommand(query, sqlCon8);
                        sqlCmd.Parameters.AddWithValue("@UserId", lastUserId);
                        sqlCmd.Parameters.AddWithValue("@About", RecipeWriterSignup.About);
                        sqlCmd.Parameters.AddWithValue("@FbLink", RecipeWriterSignup.FbLink);
                        sqlCmd.Parameters.AddWithValue("@InstaLink", RecipeWriterSignup.InstaLink);
                        sqlCmd.Parameters.AddWithValue("@TwitterLink", RecipeWriterSignup.TwitterLink);

                        sqlCmd.ExecuteNonQuery();
                    }
                }

            }


            return RedirectToAction("Index");
        }
        public ActionResult RestaurantSignup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RestaurantSignup(FormCollection form)
        {
            Restaurant restaurant = new Restaurant();
            restaurant.restaurantName = form["resNameInput"];
            restaurant.location = form["resLocationInput"];
            restaurant.area = form["resAreaInput"];
            restaurant.offers = form["offerInput"];
            HttpPostedFileBase file = Request.Files["proPic"];
            var path = "";
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                file.SaveAs(path);
            }

            restaurant.profilePic = path;
            HttpPostedFileBase file2 = Request.Files["coverPic"];
            var path2 = "";
            if (file2.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file2.FileName);
                path2 = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                file2.SaveAs(path2);
            }

            restaurant.coverPic = path2;
            restaurant.resPassword = EncryptString(form["resPasswordInput"]);
            string respass = restaurant.resPassword;
            using (var sqlCon1 = new SqlConnection(connectionString))
            {
                sqlCon1.Open();
                string query = "insert into Restaurant values (@RestaurantName,@Location,@Area,@Offers,@ProfilePic,@CoverPic,@RestaurantPassword)";
                var sqlCmd = new SqlCommand(query, sqlCon1);
                if (restaurant.restaurantName.Length >= 3)
                {
                    sqlCmd.Parameters.AddWithValue("@RestaurantName", restaurant.restaurantName);
                }
                else
                {
                    TempData["resname"] = "1";
                    return RedirectToAction("RestaurantSignup");
                }
                if (!restaurant.location.Equals(""))
                {
                    sqlCmd.Parameters.AddWithValue("@Location", restaurant.location);
                }
                else
                {
                    TempData["resloc"] = "2";
                    return RedirectToAction("RestaurantSignup");
                }
                if (!restaurant.area.Equals(""))
                {
                    sqlCmd.Parameters.AddWithValue("@Area", restaurant.area);
                }
                else
                {
                    TempData["resarea"] = "3";
                    return RedirectToAction("RestaurantSignup");
                }


                sqlCmd.Parameters.AddWithValue("@Offers", restaurant.offers);
                sqlCmd.Parameters.AddWithValue("@ProfilePic", restaurant.profilePic);
                sqlCmd.Parameters.AddWithValue("@CoverPic", restaurant.coverPic);
                if (respass.Length >= 3)
                {
                    sqlCmd.Parameters.AddWithValue("@RestaurantPassword", restaurant.resPassword);
                }
                else
                {
                    TempData["respass"] = "4";
                    return RedirectToAction("RestaurantSignup");

                }
                sqlCmd.ExecuteNonQuery();
                //Default restaurant rating:

                string queryUserId = "SELECT TOP 1 RestaurantId FROM Restaurant ORDER BY RestaurantId DESC";
                var dtbUser = new DataTable();
                using (var sqlCon2 = new SqlConnection(connectionString))
                {
                    sqlCon2.Open();
                    SqlDataAdapter sqlDa = new SqlDataAdapter(queryUserId, sqlCon1);
                    sqlDa.Fill(dtbUser);
                    if (dtbUser.Rows.Count > 0)
                    {
                        var lastResId = dtbUser.Rows[0][0];
                        string query2 = "insert into RestaurantRating values (@Rating,@RestaurantId,@UserId)";
                        var sqlCmd2 = new SqlCommand(query2, sqlCon2);
                        sqlCmd2.Parameters.AddWithValue("@Rating", 0);
                        sqlCmd2.Parameters.AddWithValue("@RestaurantId", lastResId);
                        sqlCmd2.Parameters.AddWithValue("@UserId", 1003);
                        sqlCmd2.ExecuteNonQuery();
                    }
                }

            }
            return RedirectToAction("Index");

        }

        public ActionResult BloggerSignup()
        {
            return View();

        }
        [HttpPost]
        public ActionResult BloggerSignup(FormCollection formCollection)
        {
            BloggerSignUp bloggerSignUp = new BloggerSignUp();
            bloggerSignUp.location = formCollection["LocationInput"];
            string age = formCollection["AgeInput"];
            bloggerSignUp.age = int.Parse(age);
            string yearsofblogging = formCollection["YearsofBloggingInput"];
            bloggerSignUp.yearsOfBlogging = int.Parse(yearsofblogging);
            bloggerSignUp.education = formCollection["EducationInput"];
            bloggerSignUp.savoryOrSweet = formCollection["SavoryorSweetInput"];
            bloggerSignUp.story = formCollection["StoryInput"];




            using (var sqlCon3 = new SqlConnection(connectionString))
            {
                sqlCon3.Open();
                string queryUserId = "SELECT TOP 1 UserId FROM [User] ORDER BY UserId DESC";

                var dtbUser = new DataTable();
                using (var sqlCon4 = new SqlConnection(connectionString))
                {
                    sqlCon4.Open();
                    SqlDataAdapter sqlDa = new SqlDataAdapter(queryUserId, sqlCon4);
                    sqlDa.Fill(dtbUser);
                    if (dtbUser.Rows.Count > 0)
                    {
                        var lastUserId = dtbUser.Rows[0][0];

                        string query = "insert into Blogger values(@UserId,@Location,@Story,@YearsofBlogging,@Education,@Age,@SavoryOrSweet)";
                        var sqlCmd = new SqlCommand(query, sqlCon4);
                        sqlCmd.Parameters.AddWithValue("@UserId", lastUserId);
                        sqlCmd.Parameters.AddWithValue("@Location", bloggerSignUp.location);

                        sqlCmd.Parameters.AddWithValue("@Story", bloggerSignUp.story);
                        sqlCmd.Parameters.AddWithValue("@YearsofBlogging", bloggerSignUp.yearsOfBlogging);
                        sqlCmd.Parameters.AddWithValue("@Education", bloggerSignUp.education);
                        sqlCmd.Parameters.AddWithValue("@Age", bloggerSignUp.age);
                        sqlCmd.Parameters.AddWithValue("@SavoryOrSweet", bloggerSignUp.savoryOrSweet);
                        sqlCmd.ExecuteNonQuery();
                    }
                }




            }


            return View();

        }

        public ActionResult HomeChefSignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult HomeChefSignUp(FormCollection formCollection)
        {
            HomeChefSignUp HomeChefSignUp = new HomeChefSignUp();
            HomeChefSignUp.FbLink = formCollection["HomeChefFbLinkInput"];
            HomeChefSignUp.InstaLink = formCollection["HomeChefInstaLinkInput"];
            HomeChefSignUp.TwitterLink = formCollection["HomeChefTwitterLinkInput"];
            HomeChefSignUp.Address = formCollection["HomeChefAddressInput"];
            HomeChefSignUp.Area = formCollection["HomeChefAreaInput"];

            using (var sqlCon5 = new SqlConnection(connectionString))
            {
                sqlCon5.Open();
                string queryUserId = "SELECT TOP 1 UserId FROM [User] ORDER BY UserId DESC";

                var dtbUser = new DataTable();
                using (var sqlCon6 = new SqlConnection(connectionString))
                {
                    sqlCon6.Open();
                    SqlDataAdapter sqlDa = new SqlDataAdapter(queryUserId, sqlCon6);
                    sqlDa.Fill(dtbUser);
                    if (dtbUser.Rows.Count > 0)
                    {
                        var lastUserId = dtbUser.Rows[0][0];

                        string query = "insert into HomeChef values(@UserId,@FbLink,@InstaLink,@TwitterLink,@Address,@Area)";
                        var sqlCmd = new SqlCommand(query, sqlCon6);
                        sqlCmd.Parameters.AddWithValue("@UserId", lastUserId);
                        sqlCmd.Parameters.AddWithValue("@FbLink", HomeChefSignUp.FbLink);
                        sqlCmd.Parameters.AddWithValue("@InstaLink", HomeChefSignUp.InstaLink);

                        sqlCmd.Parameters.AddWithValue("@TwitterLink", HomeChefSignUp.TwitterLink);
                        sqlCmd.Parameters.AddWithValue("@Address", HomeChefSignUp.Address);
                        if (!string.IsNullOrEmpty(HomeChefSignUp.Area))
                        {
                            sqlCmd.Parameters.AddWithValue("@Area", HomeChefSignUp.Area);
                        }
                        else
                        {
                            TempData["HomeChefArea"] = "1";
                            return RedirectToAction("HomeChefSignUp");

                        }
                        sqlCmd.ExecuteNonQuery();
                        using (var sqlCon8 = new SqlConnection(connectionString))
                        {
                            sqlCon8.Open();

                            string HomechefUserId = "SELECT TOP 1 HomeChefId FROM HomeChef ORDER BY HomeChefId DESC";

                            var dtbUser1 = new DataTable();
                            using (var sqlCon7 = new SqlConnection(connectionString))
                            {
                                sqlCon7.Open();
                                SqlDataAdapter sqlDa1 = new SqlDataAdapter(HomechefUserId, sqlCon7);
                                sqlDa1.Fill(dtbUser1);
                                if (dtbUser1.Rows.Count > 0)
                                {
                                    var lastHomechefId = dtbUser1.Rows[0][0];
                                    string query1 = "insert into HomeChefRating values(@HomeChefRating,@HomeChefId,@UserId)";
                                    var sqlCmd1 = new SqlCommand(query1, sqlCon7);
                                    sqlCmd1.Parameters.AddWithValue("@HomeChefRating", 0);
                                    sqlCmd1.Parameters.AddWithValue("@HomeChefId", lastHomechefId);
                                    sqlCmd1.Parameters.AddWithValue("@UserId", lastUserId);



                                    sqlCmd1.ExecuteNonQuery();
                                }
                            }
                        }



                    }
                }

            }

            return RedirectToAction("Index");
        }

        public string EncryptString(string str)
        {
            byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(str);
            String encrypted = Convert.ToBase64String(b);
            return encrypted;
        }

        public string DecryptString(string str)
        {
            byte[] b = Convert.FromBase64String(str);
            string decrypted = System.Text.ASCIIEncoding.ASCII.GetString(b);

            return decrypted;
        }

        [HttpPost]
        public ActionResult RecipePost(FormCollection form)
        {
            Recipe recipe = new Recipe();
            recipe.recipeName = form["recipeNameInput"];
            var date = form["publishDateInput"];

            recipe.recipePublishDate = Convert.ToDateTime(date);

            HttpPostedFileBase file = Request.Files["recipepic"];
            var path = "";
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                file.SaveAs(path);
            }


            recipe.recipePicture = path;
            recipe.cookingTime = form["cookingTimeInput"];
            recipe.recipeServing = form["servingInput"];
            recipe.recipeIngredients = form["ingInput"];
            recipe.recipeProcess = form["input_process"];
            recipe.recipeDescription = form["input_description"];
            var userIdActive = Session["recipeWriterActive"];
            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "SELECT * FROM RecipeWriter WHERE UserId = '" + userIdActive + "'";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                var dtbUser = new DataTable();
                sqlDa.Fill(dtbUser);
                if (dtbUser.Rows.Count > 0)
                {
                    var recipeWriterIdActive = dtbUser.Rows[0][0];
                    recipe.recipeWriterId = (int)recipeWriterIdActive;

                    string query2 = "insert into Recipe values(@RecipeWriterId,@RecipeName,@RecipePublishDate,@RecipePicture,@RecipeCookingTime,@RecipeServing,@RecipeIngredients,@Process,@Description)";
                    var sqlCmd = new SqlCommand(query2, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@RecipeWriterId", recipe.recipeWriterId);
                    sqlCmd.Parameters.AddWithValue("@RecipeName", recipe.recipeName);
                    sqlCmd.Parameters.AddWithValue("@RecipePublishDate", recipe.recipePublishDate);
                    sqlCmd.Parameters.AddWithValue("@RecipePicture", recipe.recipePicture);
                    sqlCmd.Parameters.AddWithValue("@RecipeCookingTime", recipe.cookingTime);
                    sqlCmd.Parameters.AddWithValue("@RecipeServing", recipe.recipeServing);
                    sqlCmd.Parameters.AddWithValue("@RecipeIngredients", recipe.recipeIngredients);
                    sqlCmd.Parameters.AddWithValue("@Process", recipe.recipeProcess);
                    sqlCmd.Parameters.AddWithValue("@Description", recipe.recipeDescription);
                    sqlCmd.ExecuteNonQuery();
                    TempData["RecipePosted"] = 1;
                    Response.Write("<script>alert('New recipe posted')</script>");
                    //Default recipe rating:

                    string queryUserId = "SELECT TOP 1 RecipeId FROM Recipe ORDER BY RecipeId DESC";
                    var dtbUser2 = new DataTable();
                    using (var sqlCon2 = new SqlConnection(connectionString))
                    {
                        sqlCon2.Open();
                        SqlDataAdapter sqlDa2 = new SqlDataAdapter(queryUserId, sqlCon2);
                        sqlDa2.Fill(dtbUser2);
                        if (dtbUser2.Rows.Count > 0)
                        {
                            var lastRecId = dtbUser2.Rows[0][0];
                            string query3 = "insert into RecipeRating values (@RecipeRating,@RecipeId,@UserId)";
                            var sqlCmd2 = new SqlCommand(query3, sqlCon2);
                            sqlCmd2.Parameters.AddWithValue("@RecipeRating", 0);
                            sqlCmd2.Parameters.AddWithValue("@RecipeId", lastRecId);
                            sqlCmd2.Parameters.AddWithValue("@UserId", 1003);
                            sqlCmd2.ExecuteNonQuery();
                        }
                    }

                }
                else
                {
                    Response.Write("<script>alert('Error!')</script>");
                }



            }
            using (var sqlCon1 = new SqlConnection(connectionString))
            {
                sqlCon1.Open();
                string queryUserId = "SELECT TOP 1 RecipeId FROM Recipe ORDER BY RecipeId DESC";
                var dtbUser = new DataTable();
                SqlDataAdapter sqlDa = new SqlDataAdapter(queryUserId, sqlCon1);
                sqlDa.Fill(dtbUser);
                if (dtbUser.Rows.Count > 0)
                {
                    var lastRecipeId = dtbUser.Rows[0][0];
                    TempData["lastRecipeId"] = lastRecipeId;
                    sqlCon1.Close();

                }
            }

            return RedirectToAction("ViewRecipe");
        }

        [HttpPost]
        public ActionResult Blogpost(FormCollection form)
        {

            Blog blog = new Blog();

            blog.blogName = form["BlogNameInput"];
            HttpPostedFileBase file = Request.Files["blogerpic"];
            var path = "";
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                file.SaveAs(path);
            }
            blog.blogPic = path;
            var date = form["publishDateInput"];
            blog.date = Convert.ToDateTime(date);
            blog.blogSummary = form["summaryInput"];
            blog.blogDescription = form["input_description"];

            var userIdActive = Session["bloggerActive"];


            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "SELECT * FROM Blogger WHERE UserId = '" + userIdActive + "'";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                var dtbUser = new DataTable();
                sqlDa.Fill(dtbUser);
                if (dtbUser.Rows.Count > 0)
                {
                    var bloggerIdActive = dtbUser.Rows[0][0];
                    blog.bloggerId = (int)bloggerIdActive;
                    string query2 = "insert into Blog values(@BloggerId,@BlogName,@BlogPic,@BlogPublishDate,@BlogSummary,@BlogDescription)";
                    var sqlCmd = new SqlCommand(query2, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@BloggerId", blog.bloggerId);
                    sqlCmd.Parameters.AddWithValue("@BlogName", blog.blogName);
                    sqlCmd.Parameters.AddWithValue("@BlogPublishDate", blog.date);
                    sqlCmd.Parameters.AddWithValue("@BlogPic", blog.blogPic);
                    sqlCmd.Parameters.AddWithValue("@BlogSummary", blog.blogSummary);
                    sqlCmd.Parameters.AddWithValue("@BlogDescription", blog.blogDescription);
                    sqlCmd.ExecuteNonQuery();
                    TempData["BlogPosted"] = 1;
                    Response.Write("<script>alert('New blog posted')</script>");

                }
                else
                {
                    Response.Write("<script>alert('Error!')</script>");
                }



            }
            using (var sqlCon1 = new SqlConnection(connectionString))
            {
                sqlCon1.Open();
                string queryUserId = "SELECT TOP 1 BloggerId FROM blog ORDER BY BloggerId DESC";
                var dtbUser = new DataTable();
                SqlDataAdapter sqlDa = new SqlDataAdapter(queryUserId, sqlCon1);
                sqlDa.Fill(dtbUser);
                if (dtbUser.Rows.Count > 0)
                {
                    var lastblogId = dtbUser.Rows[0][0];
                    TempData["lastblogId"] = lastblogId;
                    sqlCon1.Close();

                }
            }

            return RedirectToAction("FoodBlogs");

        }



        [HttpPost]
        public ActionResult HomeMadeFoodPost(FormCollection form)
        {
            HomeMadeFood homemadeFood = new HomeMadeFood();
            homemadeFood.homemadeFoodName = form["homemadeFoodName"];
            homemadeFood.date = form["date"];

            HttpPostedFileBase file = Request.Files["homemadeFoodPic"];
            var path = "";
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                file.SaveAs(path);
            }


            homemadeFood.homemadeFoodPic = path;
            homemadeFood.homemadeFoodPrice = form["homemadeFoodPrice"];
            var value = form["availabilityStatus"];
            if (value.Equals("YES"))
            {
                homemadeFood.availabilityStatus = "YES";
            }
            else if (value.Equals("NO"))
            {
                homemadeFood.availabilityStatus = "NO";
            }
            var userIdActive = Session["homeChefActive"];

            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();



                string query = "SELECT * FROM HomeChef WHERE UserId = '" + userIdActive + "'";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                var dtbUser = new DataTable();
                sqlDa.Fill(dtbUser);
                if (dtbUser.Rows.Count > 0)
                {
                    var homeChefIdActive = dtbUser.Rows[0][0];
                    homemadeFood.homeChefId = (int)homeChefIdActive;
                    string query2 = "insert into HomeMadeFood values(@HomeChefId,@HomemadeFoodName,@HomemadeFoodPrice,@HomemadeFoodPic,@AvailabilityStatus,@Date)";
                    var sqlCmd = new SqlCommand(query2, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@HomeChefId", homemadeFood.homeChefId);
                    sqlCmd.Parameters.AddWithValue("@HomemadeFoodName", homemadeFood.homemadeFoodName);
                    sqlCmd.Parameters.AddWithValue("@HomemadeFoodPrice", homemadeFood.homemadeFoodPrice);
                    sqlCmd.Parameters.AddWithValue("@HomemadeFoodPic", homemadeFood.homemadeFoodPic);
                    sqlCmd.Parameters.AddWithValue("@AvailabilityStatus", homemadeFood.availabilityStatus);

                    sqlCmd.Parameters.AddWithValue("@Date", homemadeFood.date);

                    sqlCmd.ExecuteNonQuery();
                    TempData["HomeFoodPost"] = 1;
                    Response.Write("<script>alert('New recipe posted')</script>");

                }
                else
                {
                    Response.Write("<script>alert('Error!')</script>");
                }


            }


            return RedirectToAction("HomeMadeFoods");
        }

        [HttpPost]
        public ActionResult MenuPost(FormCollection form)
        {

            Excel excel = new Excel(@"F:\3-2\sd\Doc&Dine copy38\foodpanda.xlsx", 1);
            Restaurant res = new Restaurant();
            var resPass = EncryptString(form["resPasswordInput"]);

            res.foodName = form["foodNameInput"];
            res.foodIngredient = form["ingrInput"];
            res.Price = Convert.ToDouble(form["priceInput"]);

            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "select * from Restaurant where RestaurantPassword='" + resPass + "'";
                var sqlCmd = new SqlCommand(query, sqlCon);

                SqlDataReader dr = sqlCmd.ExecuteReader();
                if (dr.Read())
                {
                    int restaurantId = dr.GetInt32(0);
                    sqlCon.Close();
                    using (var sqlCon3 = new SqlConnection(connectionString))
                    {
                        sqlCon3.Open();

                        string query3 = "SELECT RestaurantName FROM Restaurant where RestaurantPassword='" + resPass + "'";
                        SqlDataAdapter sqlDa = new SqlDataAdapter(query3, sqlCon3);
                        var dtbUser = new DataTable();
                        sqlDa.Fill(dtbUser);



                        if (dtbUser.Rows.Count > 0)
                        {
                            var resName = dtbUser.Rows[0][0];
                            sqlCon3.Close();

                            int i;
                            var eoffer = "";
                            for (i = 0; i < 1877; i++)
                            {
                                eoffer = excel.ReadCell(i, 0);
                                if (eoffer.Contains(resName.ToString()))
                                {
                                    eoffer = excel.ReadCell(i + 5, 0);
                                    break;
                                }
                                else
                                {
                                    eoffer = "";
                                }

                            }


                            using (var sqlCon1 = new SqlConnection(connectionString))
                            {
                                sqlCon1.Open();

                                string query2 = "insert into Menu values(@FoodName,@FoodIngredient,@Price,@RestaurantId,@FoodPandaOffers,@HungriNakiOffers,@UberEatsOffers)";
                                var sqlCmd2 = new SqlCommand(query2, sqlCon1);
                                sqlCmd2.Parameters.AddWithValue("@FoodName", res.foodName);
                                sqlCmd2.Parameters.AddWithValue("@FoodIngredient", res.foodIngredient);
                                sqlCmd2.Parameters.AddWithValue("@Price", res.Price);
                                sqlCmd2.Parameters.AddWithValue("@RestaurantId", restaurantId);
                                sqlCmd2.Parameters.AddWithValue("@FoodPandaOffers", eoffer);
                                sqlCmd2.Parameters.AddWithValue("@HungriNakiOffers", eoffer);
                                sqlCmd2.Parameters.AddWithValue("@UberEatsOffers", eoffer);

                                sqlCmd2.ExecuteNonQuery();
                                TempData["resIdClicked"] = restaurantId;
                                Response.Write("<script>alert('New menu posted')</script>");
                                return RedirectToAction("ResMenuPage");
                            }
                        }
                        else
                        {
                            Response.Write("<script>alert('Error!')</script>");
                            return RedirectToAction("Index");
                        }

                    }
                }
                else
                {
                    Response.Write("<script>alert('Error!')</script>");
                    return RedirectToAction("Index");
                }
            }
        }

        public ActionResult seeprofile()
        {


            if (!Session["homechefActive"].Equals(0) && Session["bloggerActive"].Equals(0) && Session["recipeWriterActive"].Equals(0) && Session["userActive"].Equals(0))
            {
                return RedirectToAction("HomeChefProfile");
            }
            else if (Session["homechefActive"].Equals(0) && !Session["bloggerActive"].Equals(0) && Session["recipeWriterActive"].Equals(0) && Session["userActive"].Equals(0))
            {
                return RedirectToAction("Blogger");
            }
            else if (Session["homechefActive"].Equals(0) && Session["bloggerActive"].Equals(0) && !Session["recipeWriterActive"].Equals(0) && Session["userActive"].Equals(0))
            {
                return RedirectToAction("RecipeWriterProfile");
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        public ActionResult logout()
        {
            TempData["BlogPosted"] = 0;
            TempData["recipePosted"] = 0;
            Session["userActive"] = 0;
            Session["recipeWriterActive"] = 0;
            Session["homechefActive"] = 0;
            Session["bloggerActive"] = 0;
            Session["track"] = 0;
            Session["communityJoined"] = 0;
            Response.Write("<script>alert('User Logged out')</script>");

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult joinCommunity(FormCollection form)
        {
            var mail = form["email"];
            var pass = EncryptString(form["pass"]);

            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "select * from [User] where Email='" + mail + "'and Password='" + pass + "'";
                var sqlCmd = new SqlCommand(query, sqlCon);
                SqlDataReader dr = sqlCmd.ExecuteReader();
                if (dr.Read())
                {
                    int userIdLoggedIn = dr.GetInt32(0);
                    sqlCon.Close();
                    TempData["userIdloggedIn"] = userIdLoggedIn;

                    sqlCon.Open();
                    string activeUserQuery = "select * from User_access where UserId='" + userIdLoggedIn + "'";
                    var sqlCmd2 = new SqlCommand(activeUserQuery, sqlCon);
                    SqlDataReader dr2 = sqlCmd2.ExecuteReader();
                    if (dr2.Read())
                    {
                        int homechefAccess = dr2.GetInt32(2);
                        int bloggerAccess = dr2.GetInt32(3);
                        int recipewriterAccess = dr2.GetInt32(4);
                        sqlCon.Close();
                        if (bloggerAccess == 1)
                        {
                            Session["track"] = 1;
                            Session["bloggerActive"] = userIdLoggedIn;
                            Session["homechefActive"] = 0;
                            Session["recipeWriterActive"] = 0;
                            Session["userActive"] = 0;
                            Session["communityJoined"] = userIdLoggedIn;
                            Response.Write("<script>alert('login successful')</script>");

                            return RedirectToAction("communityPage");
                        }
                        else
                        {

                            TempData["join"] = "1";

                            return RedirectToAction("FoodBlogs");
                        }


                    }
                    else
                    {
                        TempData["join"] = "1";

                        return RedirectToAction("FoodBlogs");

                    }


                }
                else
                {
                    sqlCon.Close();
                    TempData["join"] = "1";

                    return RedirectToAction("FoodBlogs");
                }

            }


        }


        public ActionResult leaveCommunity()
        {
            @Session["communityJoined"] = 0;
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult communityPage(FormCollection form)
        {
            string comment = form["comment_add"];
            var userId = Session["communityJoined"];
            Community community = new Community();
            community.comment = comment;
            community.userId = Convert.ToInt32(userId);

            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string queryUserId = "select UserName from [User] where UserId='" + userId + "'";
                var dtbUser = new DataTable();
                SqlDataAdapter sqlDa = new SqlDataAdapter(queryUserId, sqlCon);
                sqlDa.Fill(dtbUser);
                if (dtbUser.Rows.Count > 0)
                {
                    community.userName = Convert.ToString(dtbUser.Rows[0][0]);

                    sqlCon.Close();

                }
            }
            using (var sqlCon1 = new SqlConnection(connectionString))
            {
                sqlCon1.Open();
                string query = "insert into community values (@UserId,@Comment)";
                var sqlCmd = new SqlCommand(query, sqlCon1);

                sqlCmd.Parameters.AddWithValue("@UserId", userId);
                sqlCmd.Parameters.AddWithValue("@Comment", comment);

                sqlCmd.ExecuteNonQuery();
            }

            return RedirectToAction("CommunityPage");


        }

        public ActionResult communityPage()
        {

            var dtbUser = new DataTable();
            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();

                SqlDataAdapter sqlDa = new SqlDataAdapter("select * from community inner join [User] on community.UserId =[User].UserId;", sqlCon);
                sqlDa.Fill(dtbUser);

            }

            return View(dtbUser);

        }

        [HttpPost]
        public ActionResult Reserve(FormCollection formCollection)
        {

            var name = formCollection["name"];
            var email = formCollection["email"];
            var resName = formCollection["resName"];
            Reservation reservation = new Reservation();
            reservation.usename = name;
            reservation.email = email;
            reservation.restaurantname = resName;


            using (var sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "select * from [User] where Email='" + email + "'";
                var sqlCmd = new SqlCommand(query, sqlCon);
                SqlDataReader dr = sqlCmd.ExecuteReader();
                if (dr.Read())
                {
                    int userIdLoggedIn = dr.GetInt32(0);
                    sqlCon.Close();
                    TempData["userIdloggedIn"] = userIdLoggedIn;

                    sqlCon.Open();

                    string resQuery = "select RestaurantId from Restaurant where RestaurantName='" + resName + "'";
                    SqlDataAdapter sqlDa = new SqlDataAdapter(resQuery, sqlCon);
                    var dtbUser = new DataTable();
                    sqlDa.Fill(dtbUser);
                    if (dtbUser.Rows.Count > 0)
                    {
                        var resId = dtbUser.Rows[0][0];
                        sqlCon.Close();

                        using (var sqlCon1 = new SqlConnection(connectionString))
                        {
                            sqlCon1.Open();

                            string query2 = "insert into Reservation values(@UserId,@RestaurantId)";
                            var sqlCmd2 = new SqlCommand(query2, sqlCon1);
                            sqlCmd2.Parameters.AddWithValue("@UserId", userIdLoggedIn);
                            sqlCmd2.Parameters.AddWithValue("@RestaurantId", resId);


                            sqlCmd2.ExecuteNonQuery();
                            TempData["reserved"] = "1";
                            return RedirectToAction("Index");
                        }


                    }
                    else
                    {
                        return RedirectToAction("Restaurants");
                    }
                }
                else
                {
                    return RedirectToAction("Recipes");
                }

            }

        }
        public ActionResult UserProfile()
        {
            var dtbUser = new DataTable();
            var userIdActive1 = Session["userActive"];

            using (var sqlCon = new SqlConnection(connectionString))
            {

                sqlCon.Open();
                if (!Session["userActive"].Equals(0))

                    userIdActive1 = Session["userActive"];
                SqlDataAdapter sqlDa1 = new SqlDataAdapter("select * from [user] where UserId = '" + userIdActive1 + "'", sqlCon);
                sqlDa1.Fill(dtbUser);

            }
            return View(dtbUser);

        }

    }
}