using Saras.eMarking.Domain.ViewModels;
using System;

namespace Saras.eMarking.Business
{
    public static class SqlHelper
    {
        public static T GetValue<T>(object obj)
        {
            if (obj is not DBNull)
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
            return default;
        }

        public static T GetValue<T>(object obj, object defaultValue)
        {
            if (obj is not DBNull)
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
            return (T)defaultValue;
        }

        public static object GetAppSettingValue(object obj, byte valuetype)
        {

            return valuetype switch
            {
                (byte)EnumAppSettingValueType.String => GetValue<string>(obj, null),
                (byte)EnumAppSettingValueType.Integer => GetValue<long>(obj, null),
                (byte)EnumAppSettingValueType.Float => GetValue<float>(obj, null),
                (byte)EnumAppSettingValueType.XML => GetValue<string>(obj, null),
                (byte)EnumAppSettingValueType.DateTime => GetValue<DateTime>(obj, null),
                (byte)EnumAppSettingValueType.Bit => GetValue<bool>(obj, null),
                (byte)EnumAppSettingValueType.Int => GetValue<int>(obj, null),
                (byte)EnumAppSettingValueType.BigInt => GetValue<long>(obj, null),
                _ => GetValue<string>(obj, null),
            };
        }
    }
}
