using Models.UniversalModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Models.GageModels
{
    public class Gage : Asset
    {
        public decimal? Price { get; set; }//购入价格
        public string Brand { get; set; }//品牌
        public string SerialNumber { get; set; }//出厂编号
        public string Specification { get; set; }//规格
        public string Precision { get; set; }//精度
        public string Type { get; set; }//类别
        public string StandardAdjustType { get; set; }//标准在校类型
        public int? AdjustNoticeDays { get; set; }//在校提前通知天数
        public int? AdjustPeriod { get; set; }//在校周期
        public string DefaultAdjustRequirement { get; set; }//默认在校要求
        public string Manufacturer { get; set; } //制造商
        public string Grade { get; set; } //量具等级
        public string Unit { get; set; } //量具数量单位
        public DateTime? NextAdjustDate { get; set; }//下次在校日期部分
        public string AdjustSpecification { get; set; } //校准规范
        public DateTime? PurchaseDate { get; set; }//购入日期
        public DateTime? AdjustBaseDate { get; set; }//初始校准时间， 只有内校或外校才会存在

        public string FK_AssetID { get; set; }//量具编号



        public new SqlTransaction sqlTransaction = null;


        public  void CopyTo<T>(T A, bool IsCopyNullValue)
        {
            Common.ConvertHelper.Mapper(this, ref A, IsCopyNullValue);
        }

        public new static IEnumerable<Gage> GetBy(string AssetID)
        {
            string sql = "select * from GAGE t left join Asset a on t.FK_AssetID = a.AssetID where a.AssetID = '"+AssetID+"'" ;

            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql, null);
            List<Gage> Gages = Common.ConvertHelper.DataTableToList<Gage>(dt);

            return Gages;
        }

        public static List<Gage> CombineSelectByStandardAdjustType(string[] standardAdjustType)
        {
            string sql = "select * from GAGE t left join Asset a on t.FK_AssetID = a.AssetID";

            if (!standardAdjustType.Contains("全部"))
            {
                sql += " where ";
                foreach (var s in standardAdjustType)
                    sql += " standardAdjustType = '" + s + "' or";

                sql = sql.Remove(sql.Length - 2);
            }

            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql, null);
            List<Gage> Gages = Common.ConvertHelper.DataTableToList<Gage>(dt);

            return Gages;
        }

        public static List<Gage> CombineSelectByStatuses(string [] statuses)
        {
            string sql = "select * from GAGE t left join Asset a on t.FK_AssetID = a.AssetID";

            if(!statuses.Contains("全部"))
            {
                sql += " where ";
                foreach (var s in statuses)
                    sql += " a.status = '" + s + "' or";

                sql = sql.Remove(sql.Length - 2);
            }

            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql, null);
            List<Gage> Gages = Common.ConvertHelper.DataTableToList<Gage>(dt);

            return Gages;
        }


        public static List<Gage> RangeSelectByNextAdjustDate(string [] NextAdjustDate)
        {

            string sql = "";
            if (NextAdjustDate[0] == "" && NextAdjustDate[1] == "")
            {
                sql = "select * from GAGE t left join Asset a on t.FK_AssetID = a.AssetID ";
            }
            else if (NextAdjustDate[0] == "" && NextAdjustDate[1] != "")
            {
                DateTime right = DateTime.Parse(NextAdjustDate[1]);

                sql = "select * from GAGE t left join Asset a on t.FK_AssetID = a.AssetID " +
                    " where  t.NextAdjustDate is not null and t.NextAdjustDate <= '{0}'";
                sql = string.Format(sql, right.Date);
            }
            else if (NextAdjustDate[0] != "" && NextAdjustDate[1] == "")
            {
                DateTime left = DateTime.Parse(NextAdjustDate[0]);

                sql = "select * from GAGE t left join Asset a on t.FK_AssetID = a.AssetID " +
                    " where  t.NextAdjustDate is not null and t.NextAdjustDate >= '{0}'";
                sql = string.Format(sql, left.Date);
            }
            else
            {
                DateTime left = DateTime.Parse(NextAdjustDate[0]), right = DateTime.Parse(NextAdjustDate[1]);

                sql = "select * from GAGE t left join Asset a on t.FK_AssetID = a.AssetID " +
                    " where  '{0}' <= t.NextAdjustDate and t.NextAdjustDate <= '{1}' and t.NextAdjustDate is not null ";
                sql = string.Format(sql, left.Date, right.Date);
            }

            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql, null);
            List<Gage> Gages = Common.ConvertHelper.DataTableToList<Gage>(dt);

            return Gages;
        }


        public new void Add()
        {
            string sql = @"insert into Gage values(
                             @Price
                            ,@Brand
                            ,@SerialNumber
                            ,@Specification
                            ,@Precision
                            ,@Type
                            ,@StandardAdjustType
                            ,@AdjustNoticeDays
                            ,@AdjustPeriod
                            ,@DefaultAdjustRequirement
                            ,@Manufacturer
                            ,@Grade
                            ,@Unit
                            ,@NextAdjustDate
                            ,@AdjustSpecification
                            ,@PurchaseDate
                            ,@AdjustBaseDate
                            ,@FK_AssetID)";

            SqlParameter[] ps = GetSqlParameters();


            if (sqlTransaction == null)
                Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql, ps);
            else if (sqlTransaction != null)
                Common.SQLHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql, ps);
        }

        public new void Update()
        {
            string sql = @"update  Gage  set 
                Price=@Price,
                Brand=@Brand,
                SerialNumber=@SerialNumber,
                Specification=@Specification,
                Precision=@Precision,
                Type=@Type,
                StandardAdjustType=@StandardAdjustType,
                AdjustNoticeDays=@AdjustNoticeDays,
                AdjustPeriod=@AdjustPeriod,
                DefaultAdjustRequirement=@DefaultAdjustRequirement,
                Manufacturer=@Manufacturer,
                Grade=@Grade,
                Unit=@Unit,
                NextAdjustDate=@NextAdjustDate,
                AdjustSpecification=@AdjustSpecification,
                PurchaseDate=@PurchaseDate,
                AdjustBaseDate=@AdjustBaseDate
                where FK_AssetID=@FK_AssetID";

            SqlParameter[] ps = GetSqlParameters();

            if (sqlTransaction == null)
                Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql, ps);
            else if (sqlTransaction != null)
                Common.SQLHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql, ps);
        }

        public void Delete()
        {
            string sql = @"delete from  Gage where GageID = '" + FK_AssetID + "'";

            if (sqlTransaction == null)
                Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql, null);
            else if (sqlTransaction != null)
                Common.SQLHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql, null);
        }

        private SqlParameter[] GetSqlParameters()
        {
            SqlParameter[] ps = new SqlParameter[] {
                new SqlParameter("@Price", Common.ConvertHelper.ConvertToSqlParameterValue(Price)),
                new SqlParameter("@Brand", Common.ConvertHelper.ConvertToSqlParameterValue(Brand)),
                new SqlParameter("@SerialNumber", Common.ConvertHelper.ConvertToSqlParameterValue(SerialNumber)),
                new SqlParameter("@Specification", Common.ConvertHelper.ConvertToSqlParameterValue(Specification)),
                new SqlParameter("@Precision", Common.ConvertHelper.ConvertToSqlParameterValue(Precision)),
                new SqlParameter("@Type", Common.ConvertHelper.ConvertToSqlParameterValue(Type)),
                new SqlParameter("@StandardAdjustType", Common.ConvertHelper.ConvertToSqlParameterValue(StandardAdjustType)),
                new SqlParameter("@AdjustNoticeDays", Common.ConvertHelper.ConvertToSqlParameterValue(AdjustNoticeDays)),
                new SqlParameter("@AdjustPeriod", Common.ConvertHelper.ConvertToSqlParameterValue(AdjustPeriod)),
                new SqlParameter("@DefaultAdjustRequirement", Common.ConvertHelper.ConvertToSqlParameterValue(DefaultAdjustRequirement)),
                new SqlParameter("@Manufacturer", Common.ConvertHelper.ConvertToSqlParameterValue(Manufacturer)),
                new SqlParameter("@Grade", Common.ConvertHelper.ConvertToSqlParameterValue(Grade)),
                new SqlParameter("@Unit", Common.ConvertHelper.ConvertToSqlParameterValue(Unit)),
                new SqlParameter("@NextAdjustDate", Common.ConvertHelper.ConvertToSqlParameterValue(NextAdjustDate)),
                new SqlParameter("@AdjustSpecification", Common.ConvertHelper.ConvertToSqlParameterValue(AdjustSpecification)),
                new SqlParameter("@PurchaseDate", Common.ConvertHelper.ConvertToSqlParameterValue(PurchaseDate)),
                new SqlParameter("@AdjustBaseDate", Common.ConvertHelper.ConvertToSqlParameterValue(AdjustBaseDate)),
                new SqlParameter("@FK_AssetID", Common.ConvertHelper.ConvertToSqlParameterValue(FK_AssetID)) };
            return ps;
        }

    }
}