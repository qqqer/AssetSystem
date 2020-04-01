using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Configuration;
using System.Data;

namespace WebAPI_QM.ScheduleTask
{
    public class Task2 //量具校准完成通知到OA
    {
        public static void Start()
        {
            Thread Thread = new Thread(new ThreadStart(f));
            Thread.IsBackground = true;
            Thread.Start();
        }

        public static void f()
        {
            while (true)
            {
                DateTime dateTime = DateTime.Now;
                if (dateTime.Minute % int.Parse(ConfigurationManager.AppSettings["Task2_Interval_time"]) == 0)
                {
                    string sql = @"select AA.AssetID, BB.AssetName,BB.ApplicantID,bb.ApplicantName  from 
                                (select ranked.AssetID,ranked.SysArchiveTime,ranked.rowNum,ranked.AdjustDate from Gage g left join 
                                (select *, ROW_NUMBER() over(partition by AssetID  order by SysArchiveTime desc)
                                as rowNum  from GageAdjustSlip a where a.SysArchiveTime is not null and  a.SysArchiveTime >= '{0}' and a.SysArchiveTime < '{1}'  ) as ranked
                                on g.FK_AssetID = ranked.AssetID
                                 where ranked.rowNum = 1
                                 ) as AA  left join 
                                (select *, ROW_NUMBER() over(partition by AssetID  order by LendDate desc)
                                as rowNum  from LendSlip ld) as BB
                                 on AA.rowNum = bb.rowNum and aa.AssetID = bb.AssetID
                                 where AA.rowNum = 1 and bb.ReturnDate is null and BB.LendDate < aa.AdjustDate";

                    sql = string.Format(sql, dateTime.AddMinutes(-10).ToString("yyyy-MM-dd HH:mm"), dateTime.ToString("yyyy-MM-dd HH:mm"));
                    DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql);

                    if (dt != null)
                    {
                        Dictionary<int, string> requests = new Dictionary<int, string>();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            sql = @"select id from HrmResource where loginid = '" + (string)dt.Rows[i]["ApplicantID"] + "'";
                            int id = (int)Common.SQLHelper.ExecuteScalarToObject(Common.SQLHelper.OA_strConn, CommandType.Text, sql, null); //获取满足提前通知天数的待校量具

                            if (requests.ContainsKey(id))
                            {
                                requests[id] += AppendRequest(dt.Rows[i]);
                            }
                            else
                            {
                                requests.Add(id, CreateRequestFor(id) + AppendRequest(dt.Rows[i]));
                            }
                        }


                        List<int> ids = new List<int>();
                        foreach (var v in requests.Keys)
                        {
                            ids.Add(v);
                        }

                        foreach (var v in ids)
                        {
                            string end = @"</workflowRequestTableRecords>
		                                </weaver.workflow.webservices.WorkflowDetailTableInfo>
	                                </workflowDetailTableInfos>
                                 </WorkflowRequestInfo>";
                            requests[v] += end;

                            OAServiceReference.WorkflowServiceXmlPortTypeClient client = new OAServiceReference.WorkflowServiceXmlPortTypeClient();
                            string io = client.doCreateWorkflowRequest(requests[v], 1012);
                        }
                    }

                }


                Thread.Sleep(60000);
            }
        }

        private static string CreateRequestFor(int id)
        {
            string xml = @"<WorkflowRequestInfo>
                                                <creatorId>1012</creatorId>
                                                <requestName>量具校准完成通知</requestName>     
            
                                                <workflowBaseInfo>
                                                    <workflowId>2269</workflowId>
                                                </workflowBaseInfo>
                                                <workflowMainTableInfo>
                                                    <requestRecords>
                                                        <weaver.workflow.webservices.WorkflowRequestTableRecord>   
                                                            <workflowRequestTableFields>
                                                            <weaver.workflow.webservices.WorkflowRequestTableField>     
                                                                <fieldName>gly</fieldName>
                                                                <fieldValue>{0}</fieldValue>
                                                                <isView>true</isView>
                                                                <isEdit>true</isEdit>
                                                            </weaver.workflow.webservices.WorkflowRequestTableField>
                                                            <weaver.workflow.webservices.WorkflowRequestTableField>     
                                                                <fieldName>jzqk</fieldName>
                                                                <fieldValue>1</fieldValue>
                                                                <isView>true</isView>
                                                                <isEdit>true</isEdit>
                                                            </weaver.workflow.webservices.WorkflowRequestTableField>
                                                            </workflowRequestTableFields>
                                                        </weaver.workflow.webservices.WorkflowRequestTableRecord>
                                                    </requestRecords>
                                                </workflowMainTableInfo>
                                                <workflowDetailTableInfos>
		                                                <weaver.workflow.webservices.WorkflowDetailTableInfo>
			                                                <workflowRequestTableRecords>";

            xml = string.Format(xml, id);
            return xml;
        }

        private static string AppendRequest(DataRow dataRow)
        {
            string append = @"<weaver.workflow.webservices.WorkflowRequestTableRecord>
					                <recordOrder>0</recordOrder>
					                <workflowRequestTableFields>
						                <weaver.workflow.webservices.WorkflowRequestTableField>
							                <fieldName>assetid</fieldName>
							                <fieldValue>{0}</fieldValue>
							                <fieldOrder>0</fieldOrder>
							                <isView>true</isView>
							                <isEdit>true</isEdit>
							                <isMand>false</isMand>
						                </weaver.workflow.webservices.WorkflowRequestTableField>
                                        <weaver.workflow.webservices.WorkflowRequestTableField>
							                <fieldName>assetname</fieldName>
							                <fieldValue>{1}</fieldValue>
							                <fieldOrder>0</fieldOrder>
							                <isView>true</isView>
							                <isEdit>true</isEdit>
							                <isMand>false</isMand>
						                </weaver.workflow.webservices.WorkflowRequestTableField>
                                    </workflowRequestTableFields>
                                </weaver.workflow.webservices.WorkflowRequestTableRecord>";
            append = string.Format(append, (string)dataRow["AssetID"], (string)dataRow["AssetName"]);
            return append;
        }
    }
}
