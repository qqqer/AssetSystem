using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Models.UniversalModels
{
    public class LendSlip
    {
        public string AssetID { get; set; }//资产编号
        public string AssetName { get; set; }//资产名称
        public string AssetCategory { get; set; }//资产类别
        public string ApplicantID { get; set; }//借用申请人
        public string ApplicantName { get; set; }//借用申请人
        public DateTime? LendDate { get; set; }//借用日期
        public DateTime? ReturnDate { get; set; }//归还日期
        public string TransactorName { get; set; }//经办人
        public string ReceiverDepartment { get; set; }//借用部门
        public string LendRemark { get; set; }//借出备注
        public string WhereUsing { get; set; }//使用位置
        public string ReturnRemark { get; set; }//归还备注

        public int ID { get; set; }


        public SqlTransaction sqlTransaction = null;


        public void CopyTo(LendSlip A, bool IsCopyNullValue)
        {
            Common.ConvertHelper.Mapper<LendSlip, LendSlip>(this, ref A, IsCopyNullValue);
        }

        public static LendSlip GetBy(int ID)
        {
            string sql = "select * from LendSlip where ID = " + ID;
            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql);

            LendSlip LendSlip = Common.ConvertHelper.DataTableToList<LendSlip>(dt).First();
            return LendSlip;
        }


        public static List<LendSlip> GetAll()
        {
            string sql = " select * from LendSlip order by LendDate desc";
            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql);

            if (dt == null)
                return null;

            List<LendSlip> LendSlips = Common.ConvertHelper.DataTableToList<LendSlip>(dt);
            return LendSlips;
        }


        public static List<LendSlip> GetBy(string AssetID)
        {
            string sql = "select * from LendSlip where AssetID = '" + AssetID + "' order by ReturnDate asc";
            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql);

            if (dt == null)
                return null;

            List<LendSlip> LendSlips = Common.ConvertHelper.DataTableToList<LendSlip>(dt);
            return LendSlips;
        }


        public void Delete()
        {
            string sql = @"delete from  LendSlip where ID = " + ID + "";

            if (sqlTransaction == null)
                Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql, null);
            else if (sqlTransaction != null)
                Common.SQLHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql, null);
        }

        public void Add()
        {
            string sql = @"INSERT INTO [dbo].[LendSlip] values
                   ( @AssetID
                    ,@AssetName
                    ,@AssetCategory
                    ,@ApplicantID
                    ,@ApplicantName
                    ,@LendDate
                    ,@ReturnDate
                    ,@TransactorName
                    ,@ReceiverDepartment
                    ,@LendRemark
                    ,@WhereUsing
                    ,@ReturnRemark
                    )";

            SqlParameter[] ps = GetSqlParameters();

            if (sqlTransaction == null)
                Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql, ps);
            else if (sqlTransaction != null)
                Common.SQLHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql, ps);
        }

        public void Update()
        {
            string sql = @"update  LendSlip  set               
                AssetID=@AssetID,
                AssetName=@AssetName,
                AssetCategory=@AssetCategory,
                ApplicantID=@ApplicantID,
                ApplicantName=@ApplicantName,
                LendDate=@LendDate,
                ReturnDate=@ReturnDate,
                TransactorName=@TransactorName,
                ReceiverDepartment=@ReceiverDepartment,
                LendRemark=@LendRemark,
                WhereUsing=@WhereUsing,
                ReturnRemark=@ReturnRemark
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
            new SqlParameter("@LendDate", Common.ConvertHelper.ConvertToSqlParameterValue(LendDate)),
            new SqlParameter("@ReturnDate", Common.ConvertHelper.ConvertToSqlParameterValue(ReturnDate)),
            new SqlParameter("@TransactorName", Common.ConvertHelper.ConvertToSqlParameterValue(TransactorName)),
            new SqlParameter("@ReceiverDepartment", Common.ConvertHelper.ConvertToSqlParameterValue(ReceiverDepartment)),
            new SqlParameter("@LendRemark", Common.ConvertHelper.ConvertToSqlParameterValue(LendRemark)),
            new SqlParameter("@WhereUsing", Common.ConvertHelper.ConvertToSqlParameterValue(WhereUsing)),
            new SqlParameter("@ReturnRemark", Common.ConvertHelper.ConvertToSqlParameterValue(ReturnRemark)),
            new SqlParameter("@ID", Common.ConvertHelper.ConvertToSqlParameterValue(ID)),};
            return ps;
        }
    }
}