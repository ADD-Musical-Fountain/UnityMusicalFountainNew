using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicEventNameSpace
{
    public class MusicEvent
    {
        public float duration;
        public string name;

        public MusicEvent(float dur, string n)
        {
            duration = dur;
            name = n;
        }
    }

    public class EventEmitter
    {
        public List<MusicEvent> eventList;
        public string currentStatus;
        public List<MusicEvent> getEventList()
        {
            List<MusicEvent> list = new List<MusicEvent>();
            list.Add(new MusicEvent(5, "A"));
            list.Add(new MusicEvent(10, "B"));
            list.Add(new MusicEvent(15, "C"));
            eventList = list;
            return list;
        }

        public void setCurrStatus(string name)
        {
            currentStatus = name;
        }

        public void monitorCurrentStatus()
        {
            List<MusicEvent> list = getEventList();
            while (list.Count != 0)
            {
                MusicEvent currEvent = list[0];
                list.RemoveAt(0);
                currentStatus = currEvent.name;
                while (true)
                {
                    if(Time.time > currEvent.duration)
                    {
                        break;
                    }
                }
            }
        }
    }
}

