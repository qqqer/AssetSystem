using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading;

namespace WebAPI_QM.ScheduleTask
{
    public static class Task1 //量具校准提前通知到OA
    {
        public static void Start()
        {
            Thread Thread = new Thread(new ThreadStart(f));
            Thread.IsBackground = true;
            Thread.Start();
        }

        public static void f()
        { 
            string s = "N";
            bool pushed = false;
            while (true)
            {
                if (s.ToUpper() == "Y" || (DateTime.Now.Hour == int.Parse(ConfigurationManager.AppSettings["Task1_push_time"]) && !pushed))
                {
                    //Console.WriteLine(DateTime.Now + "  pushing...");
                    string sql = @"select Asset.Status, Asset.AssetID, Asset.AssetName, Asset.remark, g.NextAdjustDate, a.ApplicantID, a.ApplicantName,g.AdjustNoticeDays from Gage g left join  Asset  on g.FK_AssetID = Asset.AssetID  
                        left join (select * from LendSlip where ReturnDate is null) a on g.FK_AssetID  = a.AssetID 
                        where StandardAdjustType != '免校' and  CHARINDEX('在校', Status) = 0 and Status != '报废'
                        and  DATEADD(day, AdjustNoticeDays, CONVERT(varchar(100), GETDATE(), 23)) >= CONVERT(varchar(100), NextAdjustDate, 23)";
                    DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql); //获取满足提前通知天数的待校量具

                    if (dt != null)
                    {
                        Dictionary<int, string> requests = new Dictionary<int, string>();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (IsNeedPush(dt.Rows[i]))//判断是否需要推送
                            {
                                if ((string)dt.Rows[i]["Status"] == "在库")
                                {
                                    string[] AdminId = ConfigurationManager.AppSettings["Task1_Admin"].ToString().Split(',');
                                    for (int k = 0; k < AdminId.Length; k++)
                                    {
                                        int id = int.Parse(AdminId[k].Trim());
                                        if (requests.ContainsKey(id))
                                        {
                                            requests[id] += AppendRequestXml(dt.Rows[i]);
                                        }
                                        else
                                        {
                                            requests.Add(id, CreateRequestFor(id) + AppendRequestXml(dt.Rows[i]));
                                        }
                                    }
                                }
                                else//不在库
                                {
                                    sql = @"select id from HrmResource where loginid = '" + (string)dt.Rows[i]["ApplicantID"] + "'";
                                    int id = (int)Common.SQLHelper.ExecuteScalarToObject(Common.SQLHelper.OA_strConn, CommandType.Text, sql, null); //获取满足提前通知天数的待校量具

                                    if (requests.ContainsKey(id))
                                    {
                                        requests[id] += AppendRequestXml(dt.Rows[i]);
                                    }
                                    else
                                    {
                                        requests.Add(id, CreateRequestFor(id) + AppendRequestXml(dt.Rows[i]));
                                    }
                                }
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

                            //Console.Write(v + "(" + io + "),");
                        }

                        //Console.WriteLine("\n" + DateTime.Now + "  completed\n");
                    }

                    s = "N";
                    pushed = true;
                }

                if (DateTime.Now.Hour < int.Parse(ConfigurationManager.AppSettings["Task1_push_time"])) //新的一天重置pushed为未推送
                    pushed = false;
            }
        }

        private static bool IsNeedPush(DataRow dataRow)
        {
            DateTime fistday = ((DateTime)dataRow["NextAdjustDate"]).AddDays(-(int)dataRow["AdjustNoticeDays"]);
            //DateTime right = (DateTime)dataRow["NextAdjustDate"];

            TimeSpan timeSpan = DateTime.Now - fistday;

            if (timeSpan.Days % int.Parse(ConfigurationManager.AppSettings["Task1_Interval_time"]) == 0)
            {
                bool stock_push = bool.Parse(ConfigurationManager.AppSettings["Task1_stock_push"].ToString());
                if (((string)dataRow["Status"] == "在库" && stock_push) || (string)dataRow["Status"] != "在库")
                {
                    return true;
                }
            }
            return false;
        }

        private static string CreateRequestFor(int id)
        {
            string xml = @"<WorkflowRequestInfo>
                                                <creatorId>1012</creatorId>
                                                <requestName>量具校准通知</requestName>     
            
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
                                                                <fieldValue>0</fieldValue>
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

        private static string AppendRequestXml(DataRow dataRow)
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
                                        <weaver.workflow.webservices.WorkflowRequestTableField>
							                <fieldName>adjusttime</fieldName>
							                <fieldValue>{2}</fieldValue>
							                <fieldOrder>0</fieldOrder>
							                <isView>true</isView>
							                <isEdit>true</isEdit>
							                <isMand>false</isMand>
						                </weaver.workflow.webservices.WorkflowRequestTableField>
                                        <weaver.workflow.webservices.WorkflowRequestTableField>
							                <fieldName>remark</fieldName>
							                <fieldValue>{3}</fieldValue>
							                <fieldOrder>0</fieldOrder>
							                <isView>true</isView>
							                <isEdit>true</isEdit>
							                <isMand>false</isMand>
						                </weaver.workflow.webservices.WorkflowRequestTableField>
                                    </workflowRequestTableFields>
                                </weaver.workflow.webservices.WorkflowRequestTableRecord>";
            append = string.Format(append, (string)dataRow["AssetID"], (string)dataRow["AssetName"], dataRow["NextAdjustDate"].ToString(), dataRow["remark"].ToString());
            return append;
        }
    }
}