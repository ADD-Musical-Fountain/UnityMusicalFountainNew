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
            list.Add(new MusicEvent(3, "A"));
            list.Add(new MusicEvent(6, "B"));
            list.Add(new MusicEvent(8, "C1"));
            list.Add(new MusicEvent(10, "C2"));
            list.Add(new MusicEvent(12, "C3"));
            list.Add(new MusicEvent(14, "D1"));
            list.Add(new MusicEvent(16, "D2"));
            list.Add(new MusicEvent(18, "D3"));
            list.Add(new MusicEvent(21, "E"));
            list.Add(new MusicEvent(25, "F"));
            list.Add(new MusicEvent(27, "D1"));
            list.Add(new MusicEvent(29, "D2"));
            list.Add(new MusicEvent(31, "D3"));
            list.Add(new MusicEvent(35, "G"));
            list.Add(new MusicEvent(40, "H"));
            list.Add(new MusicEvent(45, "I"));
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

