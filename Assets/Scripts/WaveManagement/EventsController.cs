using System.Collections.Generic;
using UnityEngine;

public class EventsController : MonoBehaviour
{
    float currentEventCooldown = 0f;

    public EventData[] events;

    [Tooltip("How long to wait before this becomes active.")]
    public float firstTriggerDelay = 180f;

    [Tooltip("How long to wait between each event.")]
    public float triggerInterval = 30f;

    public static EventsController Instance;

    [System.Serializable]
    public class Event
    {
        public EventData eventData;
        public float duration, cooldown = 0f;
    }

    List<Event> runningEvents = new List<Event>();

    Player[] allPlayers;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            currentEventCooldown = firstTriggerDelay > 0 ? firstTriggerDelay : triggerInterval;
            allPlayers = FindObjectsByType<Player>(FindObjectsSortMode.None);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        currentEventCooldown -= Time.deltaTime;
        if (currentEventCooldown <= 0)
        {
            EventData e = GetRandomEvent();
            if (e && e.CheckIfEventWillOccur(allPlayers[UnityEngine.Random.Range(0, allPlayers.Length)]))
            {
                runningEvents.Add(new Event
                {
                    eventData = e,
                    duration = e.duration
                });

                currentEventCooldown = triggerInterval;
            }
        }

        List<Event> toRemove = new List<Event>();

        foreach (Event e in runningEvents)
        {
            e.duration -= Time.deltaTime;
            if (e.duration <= 0)
            {
                toRemove.Add(e);
                continue;
            }

            e.cooldown -= Time.deltaTime;
            if (e.cooldown <= 0)
            {
                e.eventData.Activate(allPlayers[UnityEngine.Random.Range(0, allPlayers.Length)]);
                e.cooldown = e.eventData.GetSpawnInterval();
            }
        }

        foreach (Event e in toRemove) runningEvents.Remove(e);
    }

    public EventData GetRandomEvent()
    {
        if (events.Length <= 0) return null;

        List<EventData> possibleEvents = new List<EventData>(events);

        foreach (EventData e in events)
        {
            if (e.IsActive())
            {
                possibleEvents.Add(e);
            }
        }

        if (possibleEvents.Count > 0)
        {
            EventData result = possibleEvents[UnityEngine.Random.Range(0, possibleEvents.Count)];
            return result;
        }

        return null;
    }
}
