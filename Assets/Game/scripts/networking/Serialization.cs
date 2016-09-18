using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.Networking;

namespace Raider.Game.Networking
{
    public static class Serialization
    {
        public static string DoSerialize<T>(T _deserialized)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, _deserialized);
            return System.Convert.ToBase64String(ms.ToArray());
        }

        public static T DoDeserialize<T>(string _serialized)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(System.Convert.FromBase64String(_serialized));
            return (T)bf.Deserialize(ms);
        }
    }
}
