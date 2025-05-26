using Microsoft.AspNetCore.Http;
using Saras.eMarking.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Saras.eMarking.Business
{
    public static class Utilities
    {
        public static IList<AppSettingModel> BuildAppKeyTree(this IEnumerable<AppSettingModel> source)
        {
            List<AppSettingModel> roots = new();
            try
            {
                var groups = source.GroupBy(i => i.ParentAppsettingKeyID);

                roots = groups.FirstOrDefault(g => !g.Key.HasValue).ToList();

                if (roots.Count > 0)
                {
                    var dict = groups.Where(g => g.Key.HasValue).ToDictionary(g => g.Key.Value, g => g.ToList());
                    for (int i = 0; i < roots.Count; i++)
                        AddAppKeyChildren(roots[i], dict);
                }
            }
            catch
            {
                return roots;
            }

            return roots;
        }

        private static void AddAppKeyChildren(AppSettingModel node, IDictionary<long, List<AppSettingModel>> source)
        {
            if (source.ContainsKey(node.AppSettingKeyID))
            {
                node.Children = source[node.AppSettingKeyID];
                for (int i = 0; i < node.Children.Count; i++)
                    AddAppKeyChildren(node.Children[i], source);
            }
            else
            {
                node.Children = new List<AppSettingModel>();
            }
        }

        public static EnumAppSettingValueType GetValueType(string appkeyCode)
        {
            EnumAppSettingValueType valType = appkeyCode switch
            {
                "DSCRT" or "HOLSTC" => EnumAppSettingValueType.Bit,
                "GRCPRD" => EnumAppSettingValueType.Integer,
                "ALLQSTNTYPE" or "ATMTCTYPE" or "SMATMTCTYPE" or "MNLTYPE" => EnumAppSettingValueType.Integer,
                "RLAO" or "RLCM" or "RLACM" or "RLTL" or "RLATL" or "MRKR" => EnumAppSettingValueType.String,
                _ => EnumAppSettingValueType.String,
            };
            return valType;
        }

        public static List<AppSettingModel> FlattenAppsettings(List<AppSettingModel> appSettingModels)
        {
            List<AppSettingModel> appSettings = new();
            if (appSettingModels != null && appSettingModels.Count > 0)
            {
                appSettingModels.ForEach(element =>
                {
                    if (element.Children != null && element.Children.Count > 0)
                    {
                        element.Children.ForEach(child =>
                        {
                            appSettings.Add(child);
                        });
                        element.Children = null;
                    }
                    appSettings.Add(element);
                });

            }
            return appSettings;
        }

        public static void InsertStringToCookie(HttpContext httpContext, string cookieName, string cookieObject, double CookieExpiryInMinutes = 0)
        {
            if (httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (string.IsNullOrEmpty(cookieName))
            {
                throw new ArgumentException($"'{nameof(cookieName)}' cannot be null or empty.", nameof(cookieName));
            }

            if (string.IsNullOrEmpty(cookieObject))
            {
                throw new ArgumentException($"'{nameof(cookieObject)}' cannot be null or empty.", nameof(cookieObject));
            }

            if (httpContext.Response != null && httpContext.Request != null)
            {
                httpContext.Response.Cookies.Append(cookieName, cookieObject, new CookieOptions()
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = CookieExpiryInMinutes <= 0 ? null : DateTime.UtcNow.AddMinutes(CookieExpiryInMinutes),
                    SameSite = SameSiteMode.Strict
                });
            }

        }
        public static string GetValueFromCookie(HttpContext httpContext, string cookieName)
        {

            if ((httpContext.Request == null) || (httpContext.Request.Cookies.Count > 0))
            { return string.Empty; }

            string cokkieContext = httpContext.Request.Cookies[cookieName];

            return cokkieContext ?? string.Empty;

        }  
    }
}
