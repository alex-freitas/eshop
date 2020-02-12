using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using TaskScheduler.Controllers;

namespace TaskScheduler
{

    public static class ClientActionsGenerator
    {
        private static bool _isCreated = false;

        public static HtmlString RegisterActions(
            this RazorPage viewPage,
            IUrlHelper urlHelper,
            string virtualFilePath)
        {
            //if (!virtualFilePath.StartsWith("~"))
            //    virtualFilePath = "~" + virtualFilePath;

            if (!_isCreated)
            {
                var js = new StringBuilder();
                
                var assembly = typeof(JobController).Assembly;

                GenerateScript(urlHelper, assembly, js);

                try
                {
                    var path = Directory.GetCurrentDirectory();
                    var file = Path.Combine(path, @"wwwroot", virtualFilePath);

                    File.WriteAllText(file, js.ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }

                _isCreated = true;
            }

            return new HtmlString(string.Concat("<script src='", urlHelper.Content(virtualFilePath), "'></script>"));
        }

        private static void GenerateScript(IUrlHelper urlHelper, Assembly a, StringBuilder js)
        {
            var controllers = GetControllers(a);

            foreach (var c in controllers)
            {
                var attr = c.GetCustomAttribute<GenerateClientActionsAttribute>();

                var controllerName = c.Name.Replace("Controller", string.Empty);

                var actions = GetActions(c);

                js.Append(";window.").Append(attr.JsNamespace).AppendLine("=(function(n){");

                js.Append("n.").Append(controllerName).AppendLine("={};");

                foreach (var action in actions)
                {
                    var httpMethod = "GET";

                    if (action.GetCustomAttribute<HttpPostAttribute>() != null)
                        httpMethod = "POST";

                    else if (action.GetCustomAttribute<HttpPutAttribute>() != null)
                        httpMethod = "PUT";

                    else if (action.GetCustomAttribute<HttpDeleteAttribute>() != null)
                        httpMethod = "DELETE";

                    js.Append("n.")
                        .Append(controllerName).Append('.')
                        .Append(action.Name)
                        .Append("=function(p){return $.ajax({")
                        .Append("url:'").Append(urlHelper.Action(action.Name, controllerName)).Append("',")
                        .Append("type:'").Append(httpMethod).Append("',")
                        .AppendLine("data:p})};");
                }

                js.Append("return n;})(window.").Append(attr.JsNamespace).AppendLine("||{});");
            }
        }

        private static List<Type> GetControllers(Assembly a)
        {
            IEnumerable<Type> enumerable = a.DefinedTypes
                .Where(x =>
                    typeof(Controller).IsAssignableFrom(x) &&
                    x.GetCustomAttribute<GenerateClientActionsAttribute>() != null);

            return enumerable.ToList();
        }

        private static List<MethodInfo> GetActions(Type controller)
        {
            IEnumerable<MethodInfo> enumerable = controller
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => !x.IsVirtual)
                .Where(x => x.ReturnType == typeof(ActionResult) || x.ReturnType == typeof(Task<ActionResult>));

            var actions = enumerable.ToList();

            return actions;
        }
    }

    public class GenerateClientActionsAttribute : Attribute
    {
        public string JsNamespace = "actions";
    }
}