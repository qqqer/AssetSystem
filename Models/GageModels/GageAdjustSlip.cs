using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Models.GageModels
{
    public class GageAdjustSlip
    {
        public string AssetID { get; set; }//资产编号
        public string AssetName { get; set; }//资产名称
        public string Organization { get; set; }//校准机构
        public string Spot { get; set; }//具体校准位置
        public DateTime? AdjustDate { get; set; }//校准日期
        public DateTime? ReturnDate { get; set; }//校准交回日期
        public string Type { get; set; }//校准类型
        public string TransactorName { get; set; }//校准经办人
        public string AdjustRemark { get; set; }//校准备注
        public string AdjustResult { get; set; }//校准结果
        public decimal? AdjustAmount { get; set; }//本次校准金额
        public DateTime? SysArchiveTime { get; set; }//结束校准时间



        public int ID { get; set; }


        public SqlTransaction sqlTransaction = null;

        public void CopyTo(GageAdjustSlip A, bool IsCopyNullValue)
        {
            Common.ConvertHelper.Mapper(this, ref A, IsCopyNullValue);
        }

        public static GageAdjustSlip GetBy(int ID)
        {
            string sql = "select * from GageAdjustSlip where ID = " + ID;
            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql);

            GageAdjustSlip AdjustSlip = Common.ConvertHelper.DataTableToList<GageAdjustSlip>(dt).First();
            return AdjustSlip;
        }


        public static List<GageAdjustSlip> GetBy(string AssetID)
        {
            string sql = "select * from GageAdjustSlip where AssetID = '" + AssetID + "' order by ReturnDate asc";
            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql);

            List<GageAdjustSlip> AdjustSlips = Common.ConvertHelper.DataTableToList<GageAdjustSlip>(dt);
            return AdjustSlips;
        }


        public static List<GageAdjustSlip> GetAll()
        {
            string sql = "select * from GageAdjustSlip  order by AdjustDate desc ";
            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql);

            List<GageAdjustSlip> AdjustSlips = Common.ConvertHelper.DataTableToList<GageAdjustSlip>(dt);
            return AdjustSlips;
        }


        public void Delete()
        {
            string sql = @"delete from  GageAdjustSlip where ID = " + ID + "";

            if (sqlTransaction == null)
                Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql, null);
            else if (sqlTransaction != null)
                Common.SQLHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql, null);
        }


        public void Update()
        {
            string sql = @"update GageAdjustSlip set
                AssetID=@AssetID,
                AssetName=@AssetName,
                Organization=@Organization,
                Spot=@Spot,
                AdjustDate=@AdjustDate,
                ReturnDate=@ReturnDate,
                Type=@Type,
                TransactorName=@TransactorName,
                AdjustRemark=@AdjustRemark,
                AdjustResult=@AdjustResult,
                AdjustAmount=@AdjustAmount,
                SysArchiveTime=@SysArchiveTime
                where ID=@ID";


            SqlParameter[] ps = GetSqlParameters();


            if (sqlTransaction == null)
                Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql, ps);
            else if (sqlTransaction != null)
                Common.SQLHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql, ps);

        }



        public void Add()
        {
            string sql = @"insert into GageAdjustSlip values(
                 @AssetID
                ,@AssetName
                ,@Organization
                ,@Spot
                ,@AdjustDate
                ,@ReturnDate
                ,@Type
                ,@TransactorName
                ,@AdjustRemark
                ,@AdjustResult
                ,@AdjustAmount
                ,@SysArchiveTime) ";

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
            new SqlParameter("@Organization", Common.ConvertHelper.ConvertToSqlParameterValue(Organization)),
            new SqlParameter("@Spot", Common.ConvertHelper.ConvertToSqlParameterValue(Spot)),
            new SqlParameter("@AdjustDate", Common.ConvertHelper.ConvertToSqlParameterValue(AdjustDate)),
            new SqlParameter("@ReturnDate", Common.ConvertHelper.ConvertToSqlParameterValue(ReturnDate)),
            new SqlParameter("@Type", Common.ConvertHelper.ConvertToSqlParameterValue(Type)),
            new SqlParameter("@TransactorName", Common.ConvertHelper.ConvertToSqlParameterValue(TransactorName)),
            new SqlParameter("@AdjustRemark", Common.ConvertHelper.ConvertToSqlParameterValue(AdjustRemark)),
            new SqlParameter("@AdjustResult", Common.ConvertHelper.ConvertToSqlParameterValue(AdjustResult)),
            new SqlParameter("@AdjustAmount", Common.ConvertHelper.ConvertToSqlParameterValue(AdjustAmount)),
            new SqlParameter("@SysArchiveTime", Common.ConvertHelper.ConvertToSqlParameterValue(SysArchiveTime)),
            new SqlParameter("@ID", Common.ConvertHelper.ConvertToSqlParameterValue(ID))
            };
            return ps;
        }

    }
}