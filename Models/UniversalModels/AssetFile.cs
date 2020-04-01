using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Models.UniversalModels
{
    public class AssetFile
    {
        public string FK_AssetID { get; set; }
        public string Category { get; set; }
        public string Path { get; set; }
        public DateTime Date { get; set; }//

        public int ID { get; set; }


        public SqlTransaction sqlTransaction = null;

        public static IEnumerable<AssetFile> GetFilesOf(string AssetID, string FileCategory)
        {
            string sql = "select * from AssetFile where  FK_AssetID = '"+AssetID+ "' and Category = '"+FileCategory+"'";
            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql);
            List<AssetFile> Files = Common.ConvertHelper.DataTableToList<AssetFile>(dt);

            return Files;
        }

        public static AssetFile GetBy(int ID)
        {
            string sql = "select * from AssetFile where ID = " + ID;
            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql);

            AssetFile AssetFile = Common.ConvertHelper.DataTableToList<AssetFile>(dt).First();
            return AssetFile;
        }

        public void Add()
        {
            string sql = @"insert into AssetFile values(
             @FK_AssetID
            ,@Category
            ,@Path
            ,@Date)";

            SqlParameter[] ps = GetSqlParameters();

            if (sqlTransaction == null)
                Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql, ps);
            else if (sqlTransaction != null)
                Common.SQLHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql, ps);
        }

        public void Delete()
        {
            string sql = @"delete from  AssetFile where ID = " + ID;

            if (sqlTransaction == null)
                Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql, null);
            else if (sqlTransaction != null)
                Common.SQLHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql, null);
        }

        private SqlParameter[] GetSqlParameters()
        {
            SqlParameter[] ps = new SqlParameter[] {
            new SqlParameter("@FK_AssetID", Common.ConvertHelper.ConvertToSqlParameterValue(FK_AssetID)),
            new SqlParameter("@Category", Common.ConvertHelper.ConvertToSqlParameterValue(Category)),
            new SqlParameter("@Path", Common.ConvertHelper.ConvertToSqlParameterValue(Path)),
            new SqlParameter("@Date", Common.ConvertHelper.ConvertToSqlParameterValue(Date)),
            new SqlParameter("@ID", Common.ConvertHelper.ConvertToSqlParameterValue(ID))};
            return ps;
        }
    }
}