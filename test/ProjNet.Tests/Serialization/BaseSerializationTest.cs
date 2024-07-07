using System;
using Newtonsoft.Json;

namespace ProjNET.Tests.Serialization
{
    public class BaseSerializationTest
    {

        public static T SanD<T>(T instance)
        {
            string jsonData = JsonConvert.SerializeObject(instance);
            return JsonConvert.DeserializeObject<T>(jsonData);
        }

    }
}
