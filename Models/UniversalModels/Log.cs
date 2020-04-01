using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;

namespace Models.UniversalModels
{
    public class Log
    {
        public int RequestID { get; set; }
        public User User { get; set; }
        public int API { get; set; }

        public Log(int APIID, User User)
        {
            string sql = "insert into RequestID values(null) select scope_identity()";
            this.RequestID = Convert.ToInt32(Common.SQLHelper.ExecuteScalar(Common.SQLHelper.Asset_strConn, CommandType.Text, sql));

            this.API = APIID;
            this.User = User;
        }

        public void Add(string Detail)
        {
            if (Detail != null && Detail.Substring(0, 10) == "Exception：")
            {
                string[] arr = Detail.Split('\n');
                Detail = Detail.Split('|')[0] + "|";
                foreach (var v in arr)
                {
                    if (v.Contains(".cs:"))
                        Detail += v;
                }
            }


            string sql = @"insert into [Log] values(
                @RequestID
               ,@UserID
               ,@UserName
               ,@API
               ,@Date
               ,@Detail) ";

            SqlParameter[] ps = new SqlParameter[] {
                new SqlParameter("@RequestID", Common.ConvertHelper.ConvertToSqlParameterValue(RequestID)),
                new SqlParameter("@UserID", Common.ConvertHelper.ConvertToSqlParameterValue(User.UserID)),
                new SqlParameter("@UserName", Common.ConvertHelper.ConvertToSqlParameterValue(User.Name)),
                new SqlParameter("@Date", Common.ConvertHelper.ConvertToSqlParameterValue(DateTime.Now)),
                new SqlParameter("@API", Common.ConvertHelper.ConvertToSqlParameterValue(API)),
                new SqlParameter("@Detail", Common.ConvertHelper.ConvertToSqlParameterValue(Detail))};

            Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql, ps);
        }
    }
}