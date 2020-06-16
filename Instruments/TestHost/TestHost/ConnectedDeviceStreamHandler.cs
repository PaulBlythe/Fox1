using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TestHost
{
    public class ConnectedDeviceStreamHandler
    {
        public List<String> StreamIDs = new List<string>();
        public IPEndPoint ip;

        public ConnectedDeviceStreamHandler(String ipaddr)
        {
            byte[] bytes = new byte[] { 127, 0, 0, 1 };

            ip = new IPEndPoint(new IPAddress(bytes), 5151);
        }

        public void Update(UdpClient client)
        {
            String res = "";

            foreach (String s in StreamIDs)
            {
                string[] parts = s.Split('.');
                switch (parts[0])
                {
                    case "missile":
                        break;
                    case "rwr":
                        {
                            int limit = int.Parse(parts[1]);
                            res += GetRWR(limit);
                        }
                        break;
                }
            }
            byte[] bytes = Encoding.ASCII.GetBytes(res);
            client.Send(bytes, bytes.Length, ip);
        }

        public void AddStream(String str)
        {
            StreamIDs.Add(str);
        }

        private float ModifyByRange(float EffectiveRange, float CurrentRange, float BaseThreat)
        {
            float res = BaseThreat;
            if (CurrentRange > EffectiveRange)
            {
                res *= 0.01f;
            }
            else
            {
                float t = CurrentRange / EffectiveRange;
                float max = BaseThreat * 2;
                float min = BaseThreat * 0.01f;
                t = (1 - t);
                res = min + ((max - min) * t);
            }
            return res;
        }
        private String GetRWR(int max)
        {
            String res = "";
            List<Threat> threats = new List<Threat>();

            foreach (WorldObject wo in Form1.Instance.Objects)
            {
                Threat thr = new Threat();

                float basethreat = 0;
                float dx = (wo.x - 512) * 60 / 512;
                float dy = (wo.y - 512) * 60 / 512;
                float r2 = (dx * dx) + (dy * dy);
                float r = (float)Math.Sqrt(r2);     // in miles

                switch (wo.Type)
                {
                    case Form1.AddTypes.AAA:
                        basethreat = ModifyByRange(6,r,100);
                        thr.Type = 1;
                        break;
                    case Form1.AddTypes.SAMEmitting:
                        basethreat = ModifyByRange(20, r, 500);
                        thr.Type = 2;
                        break;
                    case Form1.AddTypes.FighterEmitting:
                        basethreat = ModifyByRange(80, r, 700);
                        thr.Type = 3;
                        break;
                    case Form1.AddTypes.Helicopter:
                        basethreat = ModifyByRange(1, r, 30);
                        thr.Type = 3;
                        break;
                    case Form1.AddTypes.NavalEmitting:
                        basethreat = ModifyByRange(20, r, 600);
                        thr.Type = 4;
                        break;
                    case Form1.AddTypes.Transport:
                        basethreat = ModifyByRange(1, r, 10);
                        thr.Type = 0;
                        break;
                }
                thr.ThreatLevel = basethreat;
                double ang = Math.Atan2(dy, dx);
                ang = (ang * 360) / (Math.PI * 2);
                ang -= Form1.Instance.Heading;
                if (ang > 360)
                    ang -= 360;
                if (ang < 0)
                    ang = 360 + ang;
                thr.Bearing = (float)ang;

                threats.Add(thr);
            }
            List<Threat> SortedList = threats.OrderBy(o => o.ThreatLevel).ToList();
            int m = max;
            if (m > SortedList.Count)
                m = SortedList.Count;
            for (int i=0; i<m; i++)
            {
                res += SortedList[i].Format();
            }
            return res;
        }
    }
}
