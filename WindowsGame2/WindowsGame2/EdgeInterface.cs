using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;


namespace BibbleGame
{
    public class EdgeScene
    {
        const String EDLE_URL = "edge.com";
        const bool WEB_REQUEST = false;
        public Dictionary<String, EdgeObject> Objects = new Dictionary<string, EdgeObject>();


        internal static List<SimpleCollidableLine> getEdgeData(BibbleGame g)
        {
            
            List<SimpleCollidableLine> l = new List<SimpleCollidableLine>();
            String json_data;
            if (WEB_REQUEST)
            {
                using (var w = new WebClient())
                {
                    // attempt to download JSON data as a string
                    try
                    {
                        json_data = w.DownloadString(EDLE_URL);
                    }
                    catch (Exception) { }
                    // if string with JSON data is not empty, deserialize it to class and return its instance 
                    if (string.IsNullOrEmpty(json_data))
                    {
                        return null;
                    }
                }
            }
            else
            {
                json_data = TEST_JSON;
            }
            EdgeScene e = JsonConvert.DeserializeObject<EdgeScene>(TEST_JSON);
            Console.WriteLine("parsed JSON:");
            Console.WriteLine(e.Objects.Count + " Polygons parsed");
            foreach (EdgeObject o in e.Objects.Values) {
                if (o.Points.Count() <= 1)
                {
                    Console.WriteLine("Polygon has not enough points to make lines");
                    continue;
                }
                for (int i = 0; i < o.Points.Count(); i++)
                {
                    l.Add(new SimpleCollidableLine(g, o.getPoint(i, g), o.getPoint((i + 1) % (o.Points.Count() - 1), g)));
                }
            }
            return l;
        }

        #region test
        public static void test()
        {
            EdgeScene e = new EdgeScene();
            EdgeObject o = new EdgeObject();
            o.Points = new Dictionary<string, float>[2];
            o.Points[0] = new Dictionary<string, float>();
            o.Points[0].Add("x", 0.03434f);
            o.Points[0].Add("y", 0.03434f);
            o.Points[1] = new Dictionary<string, float>();
            o.Points[1].Add("x", 0.03434f);
            o.Points[1].Add("y", 0.03434f);
            e.Objects.Add("5", o);
            Console.Write(JsonConvert.SerializeObject(e));


            Console.Write("a");
        }
        #endregion
        public class EdgeObject
        {
            public bool UsesTexture;
            public Dictionary<String, int> Color;
            public bool Maskenabled;
            public float X, Y;
            public String Type;
            public float Scale;
            public float Angle;
            public Dictionary<String, float>[] Points;
            public int ZOrder;
            public bool Visible;
            public int Id;

            public Vector2 getPoint(int index, Game g)
            {
                return new Vector2(Points[index]["x"] * g.Window.ClientBounds.Width, Points[index]["y"] * g.Window.ClientBounds.Height);
            }
        }

        #region TEST JSON STRING
        const String TEST_JSON = @"{
  ""objects"": {
    ""5"": {
      ""usesTexture"": false,
      ""color"": {
        ""b"": 0,
        ""g"": 0,
        ""r"": 255
      },
      ""maskEnabled"": false,
      ""x"": 0,
      ""type"": ""polygon"",
      ""scale"": 1,
      ""angle"": 0,
      ""points"": [
        {
          ""x"": 0.574219,
          ""y"": 0.238281
        },
        {
          ""x"": 0.585449,
          ""y"": 0.369141
        },
        {
          ""x"": 0.635742,
          ""y"": 0.371745
        },
        {
          ""x"": 0.636719,
          ""y"": 0.240234
        }
      ],
      ""zorder"": 3,
      ""visible"": true,
      ""y"": 0,
      ""id"": 5
    }
  }
}";
        #endregion
    }
}
