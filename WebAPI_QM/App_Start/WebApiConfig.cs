using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebAPI_QM
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            config.EnableCors(new EnableCorsAttribute("*", "*", "*") { SupportsCredentials = true });
            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            ScheduleTask.Task1.Start(); //启动"量具校准提前通知到OA"服务
            ScheduleTask.Task2.Start(); //启动"量具校准完成通知到OA"服务
        }
    }
}
