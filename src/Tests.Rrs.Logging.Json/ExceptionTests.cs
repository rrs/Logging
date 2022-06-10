using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rrs.Logging.Json;
using System.Net.Sockets;

namespace Tests.Rrs.Logging.Json;

[TestClass]
public class ExceptionTests
{
    [TestMethod]
    public void TestWatsonBucketsRemoved()
    {
        try
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Send(new byte[0]);
        }
        catch(Exception e)
        {
            var json = new JsonLogObjectSerializer().Serialize(e);
            var json2 = JsonConvert.SerializeObject(e);
            var o = JObject.Parse(json);
            var o2 = JObject.Parse(json2);
            
            Assert.IsFalse(o.ContainsKey("WatsonBuckets"));
            Assert.IsTrue(o2.ContainsKey("WatsonBuckets"));
            _ = JsonConvert.DeserializeObject<Exception>(json); // make sure we can deserialize it
        }
    }

    [TestMethod]
    public void TestWatsonBucketsNotRemoved()
    {
        try
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Send(new byte[0]);
        }
        catch (Exception e)
        {
            var json = new JsonLogObjectSerializer(false).Serialize(e);
            var o = JObject.Parse(json);
            Assert.IsTrue(o.ContainsKey("WatsonBuckets"));
            _ = JsonConvert.DeserializeObject<Exception>(json); // make sure we can deserialize it
        }
    }

    [TestMethod]
    public void TestCustomException()
    {
        var e = new TestException();
        var json = new JsonLogObjectSerializer().Serialize(e);
        _ = JsonConvert.DeserializeObject<Exception>(json); // make sure we can deserialize it
    }
}