using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.DebugHelpers
{
    public class DebugMessage
    {
        public String Text;
        public float EndTime;
    }

    public class DebugMessageQueue
    {
        public static DebugMessageQueue Instance;
        public Queue<DebugMessage> messages = new Queue<DebugMessage>();

        float time = 0;

        public DebugMessageQueue()
        {
            Instance = this;
        }

        public void AddDebugMessage(String s)
        {
            DebugMessage dm = new DebugMessage();
            dm.Text = s;
            dm.EndTime = time + 5;
            messages.Enqueue(dm);
        }

        public void Update(float dt)
        {
            time += dt;

            if (messages.Count > 0)
            {
                if (messages.Peek().EndTime <= time)
                    messages.Dequeue();
            }
        }
    }
}
