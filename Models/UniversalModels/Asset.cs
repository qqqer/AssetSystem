using Models.GageModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Models.UniversalModels
{
    public class Asset
    {
        public string AssetID { get; set; }//资产编号
        public string AssetName { get; set; }//资产名称
        public string AssetCategory { get; set; }//资产大类
        public string Status { get; set; }//状态
        public string Remark { get; set; }//备注
        public DateTime? StorageDate { get; set; }//入库日期
        public string CurrentWhereOrganization { get; set; }//当前所在机构
        public string CurrentWhereSpot { get; set; }//机构中的地点

        public string  Department { get; set; }//所属部门

        public SqlTransaction sqlTransaction = null;

        public void RefreshPosition()
        {
            List<string> status = new List<string>(Status.Split('|'));

            if (status[status.Count - 1] == "在库")
            {
                CurrentWhereOrganization = "";
                CurrentWhereSpot = Department + "仓库";
            }
            else if (status[status.Count - 1] == "维修")
            {
                List<RepairSlip> RepairSlips = RepairSlip.GetBy(AssetID);
                CurrentWhereOrganization = RepairSlips[RepairSlips.Count - 1].Organization;
                CurrentWhereSpot = RepairSlips[RepairSlips.Count - 1].Spot;
            }
            else if (status[status.Count - 1] == "领用")
            {
                List<LendSlip> LendSlips = LendSlip.GetBy(AssetID);
                CurrentWhereOrganization = LendSlips[LendSlips.Count - 1].ReceiverDepartment;
                CurrentWhereSpot = LendSlips[LendSlips.Count - 1].WhereUsing;
            }
            else if (status[status.Count - 1] == "在校")
            {
                List<GageAdjustSlip> AdjustSlips = GageAdjustSlip.GetBy(AssetID);
                CurrentWhereOrganization = AdjustSlips[AdjustSlips.Count - 1].Organization;
                CurrentWhereSpot = AdjustSlips[AdjustSlips.Count - 1].Spot;
            }
        }


        public void CopyTo(Asset A, bool IsCopyNullValue)
        {
            Common.ConvertHelper.Mapper<Asset, Asset>(this,ref A, IsCopyNullValue);
        }

        public string AddStatus(string Status)
        {
           return  this.Status == "在库" ? Status : this.Status + "|" + Status;
        }

        public void RemoveStatus(string Status)
        {
            List<string> status = new List<string>(this.Status.Split('|'));
            status.Remove(Status);

            if (status.Count == 0)
            {
                this.Status = "在库";
            }
            else
            {
                this.Status = "";
                foreach (var s in status)
                    this.Status += s + "|";
                this.Status = this.Status.Substring(0, this.Status.Length - 1);
            }
        }

        #region Persistent
        public static Asset GetBy(string AssetID)
        {
            string sql = "select * from Asset where AssetID = '" + AssetID + "'";
            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql);

            if (dt == null)
                return null;

            Asset Asset = Common.ConvertHelper.DataTableToList<Asset>(dt).First();
            return Asset;
        }


        private void Delete()
        {
            string sql = @"delete from  Asset where AssetID = '" + AssetID + "'";

            if (sqlTransaction == null)
                Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql, null);
            else if (sqlTransaction != null)
                Common.SQLHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql, null);
        }

        public void Add()
        {
            string sql = @"INSERT INTO [dbo].[Asset] values
                   ( @AssetID
                    ,@AssetName
                    ,@AssetCategory
                    ,@Status
                    ,@Remark
                    ,@StorageDate
                    ,@CurrentWhereOrganization
                    ,@CurrentWhereSpot
                    ,@Department)";

            SqlParameter[] ps = GetSqlParameters();

            if (sqlTransaction == null)
                Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql, ps);
            else if (sqlTransaction != null)
                Common.SQLHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql, ps);
        }


        public void Update()
        {
            string sql = @"update  Asset  set               
                AssetCategory=@AssetCategory,
                AssetName=@AssetName,
                Status=@Status,
                Remark=@Remark,
                StorageDate=@StorageDate,
                CurrentWhereOrganization=@CurrentWhereOrganization,
                CurrentWhereSpot=@CurrentWhereSpot,
                Department=@Department
                where AssetID=@AssetID";

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
            new SqlParameter("@Status", Common.ConvertHelper.ConvertToSqlParameterValue(Status)),
            new SqlParameter("@Remark", Common.ConvertHelper.ConvertToSqlParameterValue(Remark)),
            new SqlParameter("@StorageDate", Common.ConvertHelper.ConvertToSqlParameterValue(StorageDate)),
            new SqlParameter("@CurrentWhereOrganization", Common.ConvertHelper.ConvertToSqlParameterValue(CurrentWhereOrganization)),
            new SqlParameter("@CurrentWhereSpot", Common.ConvertHelper.ConvertToSqlParameterValue(CurrentWhereSpot)),
            new SqlParameter("@Department", Common.ConvertHelper.ConvertToSqlParameterValue(Department))};
            return ps;
        }
        #endregion
    }

    //public static class StringExtension
    //{
    //    public static string Remove(this string str, string Status)
    //    {
    //        List<string> status = new List<string>(str.Split('|'));
    //        status.Remove(Status);

    //        if (status.Count == 0)
    //        {
    //            str = "在库";
    //        }
    //        else if (status.Count > 0)
    //        {
    //            str = "";
    //            foreach (var s in status)
    //                str += s + "|";
    //            str = str.Substring(0, str.Length - 1);
    //        }

    //        return str;
    //    }
    //}

}