using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Models.UniversalModels
{
    public class RepairSlip
    {
        public string AssetID { get; set; } //资产编号
        public string AssetName { get; set; } //资产编号
        public string AssetCategory { get; set; }//资产类别
        public string ApplicantID { get; set; } //维修申请人
        public string ApplicantName { get; set; }//维修申请人
        public DateTime? RepairDate { get; set; }//维修开始时间
        public string TransactorName { get; set; }//经办人
        public string RepairReason { get; set; }//维修原因
        public string Organization { get; set; }//维修地部门
        public string Spot { get; set; }//维修地点
        public string RepairResult { get; set; }//维修结果
        public DateTime?  RepairEndDate { get; set; }//维修结束时间
        public decimal? RepairAmount { get; set; }//本次维修金额

        public int ID { get; set; }


        public SqlTransaction sqlTransaction = null;

        public void CopyTo(RepairSlip A, bool IsCopyNullValue)
        {
            Common.ConvertHelper.Mapper<RepairSlip, RepairSlip>(this, ref A, IsCopyNullValue);
        }

        public static RepairSlip GetBy(int ID)
        {
            string sql = "select * from RepairSlip where ID = " + ID;
            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql);

            if (dt == null)
                return null;

            RepairSlip RepairSlip = Common.ConvertHelper.DataTableToList<RepairSlip>(dt).First();
            return RepairSlip;
        }

        public static List<RepairSlip> GetBy(string AssetID)
        {
            string sql = "select * from RepairSlip where AssetID = '" + AssetID + "' order by RepairEndDate asc";
            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql);

            if (dt == null)
                return null;

            List<RepairSlip> RepairSlips = Common.ConvertHelper.DataTableToList<RepairSlip>(dt);
            return RepairSlips;
        }

        public void Delete()
        {
            string sql = @"delete from  RepairSlip where ID = " + ID + "";

            if (sqlTransaction == null)
                Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql, null);
            else if (sqlTransaction != null)
                Common.SQLHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql, null);
        }

        public void Add()
        {
            string sql = @"INSERT INTO [dbo].[RepairSlip] values
                   ( @AssetID
                    ,@AssetName
                    ,@AssetCategory
                    ,@ApplicantID
                    ,@ApplicantName
                    ,@RepairDate
                    ,@TransactorName
                    ,@RepairReason
                    ,@Organization
                    ,@Spot
                    ,@RepairResult
                    ,@RepairEndDate
                    ,@RepairAmount
                    )";

            SqlParameter[] ps = GetSqlParameters();

            if (sqlTransaction == null)
                Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql, ps);
            else if (sqlTransaction != null)
                Common.SQLHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql, ps);
        }

        public void Update()
        {
            string sql = @"update RepairSlip  set               
                AssetID=@AssetID,
                AssetName=@AssetName,
                AssetCategory=@AssetCategory,
                ApplicantID=@ApplicantID,
                ApplicantName=@ApplicantName,
                RepairDate=@RepairDate,
                TransactorName=@TransactorName,
                RepairReason=@RepairReason,
                Organization=@Organization,
                Spot=@Spot,
                RepairResult=@RepairResult,
                RepairEndDate=@RepairEndDate,
                RepairAmount = @RepairAmount
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
            new SqlParameter("@ApplicantID", Common.ConvertHelper.ConvertToSqlParameterValue(ApplicantID)),
            new SqlParameter("@ApplicantName", Common.ConvertHelper.ConvertToSqlParameterValue(ApplicantName)),
            new SqlParameter("@RepairDate", Common.ConvertHelper.ConvertToSqlParameterValue(RepairDate)),
            new SqlParameter("@TransactorName", Common.ConvertHelper.ConvertToSqlParameterValue(TransactorName)),
            new SqlParameter("@RepairReason", Common.ConvertHelper.ConvertToSqlParameterValue(RepairReason)),
            new SqlParameter("@Organization", Common.ConvertHelper.ConvertToSqlParameterValue(Organization)),
            new SqlParameter("@Spot", Common.ConvertHelper.ConvertToSqlParameterValue(Spot)),
            new SqlParameter("@RepairResult", Common.ConvertHelper.ConvertToSqlParameterValue(RepairResult)),
            new SqlParameter("@RepairEndDate", Common.ConvertHelper.ConvertToSqlParameterValue(RepairEndDate)),
            new SqlParameter("@RepairAmount", Common.ConvertHelper.ConvertToSqlParameterValue(RepairAmount)),
            new SqlParameter("@ID", Common.ConvertHelper.ConvertToSqlParameterValue(ID)),
            };
            return ps;
        }
    }
}