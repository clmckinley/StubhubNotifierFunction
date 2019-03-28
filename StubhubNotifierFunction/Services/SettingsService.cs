using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StubhubNotifierFunction.Extensions;

namespace StubhubNotifierFunction.Services
{
    public static class SettingsService
    {
        public static T GetSetting<T>(string key, T nullVal = default(T))
        {
            T rVal = nullVal;
            try
            {
                string val = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);

                if (string.IsNullOrWhiteSpace(val) == true || val.ToString().To<T>(out rVal) == false)
                {
                    rVal = (T)nullVal;
                }

                return rVal;
            }
            catch(Exception exc)
            { }
            return rVal;
        }
    }
}
