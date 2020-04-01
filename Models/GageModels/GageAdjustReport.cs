using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Models.GageModels
{
    public class GageAdjustReport
    {
        public int FK_AdjustSlip_ID { get; set; } //校准ID
        public string Path { get; set; }//报告路径
        public DateTime? Date { get; set; }//报告时间
        public int ID { get; set; }


        public SqlTransaction sqlTransaction = null;


        public static GageAdjustReport GetBy(int ID)
        {
            string sql = "select * from GageAdjustReport where ID = " + ID;
            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql);

            GageAdjustReport AdjustReport = Common.ConvertHelper.DataTableToList<GageAdjustReport>(dt).First();
            return AdjustReport;
        }


        public static IEnumerable<GageAdjustReport> GetReportsOf(int AdjustSlip_ID)
        {
            string sql = "select * from GageAdjustReport where  FK_AdjustSlip_ID = " + AdjustSlip_ID;
            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql);
            List<GageAdjustReport> Reports = Common.ConvertHelper.DataTableToList<GageAdjustReport>(dt);

            return Reports;
        }

        public void Add()
        {
            string sql = @"insert into GageAdjustReport values(
                 @FK_AdjustSlip_ID
                ,@Path
                ,@Date
                ) ";

            SqlParameter[] ps = GetSqlParameters();

            if (sqlTransaction == null)
                Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql, ps);
            else if (sqlTransaction != null)
                Common.SQLHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql, ps);
        }

        public void Delete()
        {
            string sql = @"delete from  GageAdjustReport where ID = " + ID;

            if (sqlTransaction == null)
                Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql, null);
            else if (sqlTransaction != null)
                Common.SQLHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql, null);
        }

        private SqlParameter[] GetSqlParameters()
        {
            SqlParameter[] ps = new SqlParameter[] {
            new SqlParameter("@FK_AdjustSlip_ID", Common.ConvertHelper.ConvertToSqlParameterValue(FK_AdjustSlip_ID)),
            new SqlParameter("@Path", Common.ConvertHelper.ConvertToSqlParameterValue(Path)),
            new SqlParameter("@Date", Common.ConvertHelper.ConvertToSqlParameterValue(Date)),
            new SqlParameter("@ID", Common.ConvertHelper.ConvertToSqlParameterValue(ID))};
            return ps;
        }
    }
}