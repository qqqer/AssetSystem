using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Models.UniversalModels
{
    public class DiscardSlip
    {
        public string AssetID { get; set; }//资产编号
        public string AssetName { get; set; }//资产名称
        public string AssetCategory { get; set; }//资产类别
        public DateTime? DiscardDate { get; set; }//报废日期
        public string TransactorName { get; set; }//经办人
        public string DiscardReason{ get; set; }//报废原因
        public decimal DiscardAmount { get; set; }//报废金额
        public int ID { get; set; }


        public SqlTransaction sqlTransaction = null;

        public void CopyTo(DiscardSlip A, bool IsCopyNullValue)
        {
            Common.ConvertHelper.Mapper(this, ref A, IsCopyNullValue);
        }

        public static DiscardSlip GetBy(int ID)
        {
            string sql = "select * from DiscardSlip where ID = " + ID;
            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql);

            DiscardSlip DiscardSlip = Common.ConvertHelper.DataTableToList<DiscardSlip>(dt).First();
            return DiscardSlip;
        }


        public static List<DiscardSlip> GetBy(string AssetID)
        {
            string sql = "select * from DiscardSlip where AssetID = '" + AssetID + "' order by DiscardDate asc";
            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql);

            List<DiscardSlip> DiscardSlips = Common.ConvertHelper.DataTableToList<DiscardSlip>(dt);
            return DiscardSlips;
        }


        public void Delete()
        {
            string sql = @"delete from  DiscardSlip where ID = " + ID + "";

            if (sqlTransaction == null)
                Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql, null);
            else if (sqlTransaction != null)
                Common.SQLHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql, null);
        }

        public void Add()
        {
            string sql = @"INSERT INTO [dbo].[DiscardSlip] values
                   ( @AssetID
                    ,@AssetName
                    ,@AssetCategory
                    ,@DiscardDate
                    ,@TransactorName
                    ,@DiscardReason
                    ,@DiscardAmount
                    )";

            SqlParameter[] ps = GetSqlParameters();

            if (sqlTransaction == null)
                Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql, ps);
            else if (sqlTransaction != null)
                Common.SQLHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql, ps);
        }

        public void Update()
        {
            string sql = @"update  DiscardSlip  set               
                        AssetID=@AssetID,
                        AssetName=@AssetName,
                        AssetCategory=@AssetCategory,
                        DiscardDate=@DiscardDate,
                        TransactorName=@TransactorName,
                        DiscardReason=@DiscardReason,
                        DiscardAmount=@DiscardAmount
                        where ID=@ID";

            SqlParameter[] ps = GetSqlParameters();



            if (sqlTransaction == null)
                Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql, ps);
            else if (sqlTransaction != null)
                Common.SQLHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql, ps);
        }
        private SqlParameter[] GetSqlParameters()
        {
            SqlParameter[] ps = new SqlParameter[] {
            new SqlParameter("@AssetID", Common.ConvertHelper.ConvertToSqlParameterValue(AssetID)),
            new SqlParameter("@AssetName", Common.ConvertHelper.ConvertToSqlParameterValue(AssetName)),
            new SqlParameter("@AssetCategory", Common.ConvertHelper.ConvertToSqlParameterValue(AssetCategory)),
            new SqlParameter("@DiscardDate", Common.ConvertHelper.ConvertToSqlParameterValue(DiscardDate)),
            new SqlParameter("@TransactorName", Common.ConvertHelper.ConvertToSqlParameterValue(TransactorName)),
            new SqlParameter("@DiscardReason", Common.ConvertHelper.ConvertToSqlParameterValue(DiscardReason)),
            new SqlParameter("@DiscardAmount", Common.ConvertHelper.ConvertToSqlParameterValue(DiscardAmount)),
            new SqlParameter("@ID", Common.ConvertHelper.ConvertToSqlParameterValue(ID)),
};
            return ps;
        }
    }
}