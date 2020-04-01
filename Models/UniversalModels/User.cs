using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Models.UniversalModels
{
    public class User : Personnel
    {
        public string UserID { get; set; }
        public string APIPermission { get; set; }
        public bool APIPermissionEnabled { get; set; }

        public new static User GetBy(string UserID)
        {
            string sql = "select * from [User] where UserID = '" + UserID + "'";

            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql, null);
            User User = Common.ConvertHelper.DataTableToList<User>(dt)?.First();

            return User;
        }

        public static string Login(string UserID, string Password)
        {
            string token = null;
            if (IsValidAccount(UserID, Password))
                token = GetBase64TokenOf(UserID);
            return token;
        }

        public static bool IsValidAccount(string UserID, string Password)
        {
            User User = GetBy(UserID);

            if (User == null)
            {
                return false;
            }
            else
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] t = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Password));
                md5.Dispose();


                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < t.Length; i++)
                {
                    sb.Append(t[i].ToString("X2"));
                }

                string sql = "select loginid from [dbo].[HrmResource] where loginid = '" + UserID + "' and password = '" + sb.ToString() + "' ";
                object loginid = Common.SQLHelper.ExecuteScalarToObject(Common.SQLHelper.OA_strConn, CommandType.Text, sql, null);

                return loginid == null ? false : true;
            }       
        }

        public static string GetBase64TokenOf(string userid)
        {
            string token = userid + "|" + "qqqer";
            byte[] b = System.Text.Encoding.Default.GetBytes(token);
            token = Convert.ToBase64String(b);

            return token;
        }

        public static bool IsValidToken(string token)
        {
            if (token == null) return false;

            byte[] c = Convert.FromBase64String(token);
            string[] arr = Encoding.Default.GetString(c).Split('|');

            if (arr.Length == 2 && arr[1] == "qqqer") //若token格式正确，则检查用户是否存在
            {
                string UserID = User.GetUserIDBy(token);
                if (User.GetBy(UserID) != null)
                    return true;
            }
            return false;
        }

        public static User GetUserByToken(string Token)
        {
            string UserID = User.GetUserIDBy(Token);

            User user = User.GetBy(UserID);
            if (user == null) return null;

            Personnel personnel = Personnel.GetBy(UserID);
            if (personnel == null) return null;

            user.UserID = UserID;
            user.Name = personnel.Name;
            user.Department = personnel.Department;

            return user;
        }

        public static bool IsContainsCurrentAPIPermission(string UserID, int API)
        {
            string APIID = API.ToString();

            User user = User.GetBy(UserID);

            if (user == null || user.APIPermissionEnabled == false)
                return false;

            string UserPermission = user.APIPermission;

            string[] permissionArry = UserPermission.Split('~');

            for (int i = 0; i < permissionArry.Length; i++)
                permissionArry[i] = permissionArry[i].Trim();

            return permissionArry.Contains(APIID);
        }

        public static string GetUserIDBy(string token)
        {
            byte[] c = Convert.FromBase64String(token);
            string userid = System.Text.Encoding.Default.GetString(c).Split('|')[0];

            return userid;
        }
    }
}