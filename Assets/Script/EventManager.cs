using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    // 单例实例
    private static EventManager _instance;
    public static EventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EventManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("EventManager");
                    _instance = obj.AddComponent<EventManager>();
                }
            }
            return _instance;
        }
    }

    // 存储所有事件的字典
    private Dictionary<System.Type, UnityEvent<GameEvent>> eventDictionary;

    // 存储监听器的引用
    private Dictionary<System.Type, UnityAction<GameEvent>> listenerReferences;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        eventDictionary = new Dictionary<System.Type, UnityEvent<GameEvent>>();
        listenerReferences = new Dictionary<System.Type, UnityAction<GameEvent>>();
    }

    // 注册事件监听
    public static void StartListening<T>(UnityAction<T> listener) where T : GameEvent
    {
        System.Type eventType = typeof(T);

        // 如果字典中没有该事件类型，创建一个新的 UnityEvent
        if (!Instance.eventDictionary.TryGetValue(eventType, out UnityEvent<GameEvent> thisEvent))
        {
            thisEvent = new UnityEvent<GameEvent>();
            Instance.eventDictionary.Add(eventType, thisEvent);
        }

        // 创建监听器引用
        UnityAction<GameEvent> action = (e) => listener((T)e);
        Instance.listenerReferences[eventType] = action;

        // 添加监听器
        thisEvent.AddListener(action);
    }

    // 注销事件监听
    public static void StopListening<T>(UnityAction<T> listener) where T : GameEvent
    {
        if (_instance == null) return;

        System.Type eventType = typeof(T);

        // 如果字典中存在该事件类型
        if (Instance.eventDictionary.TryGetValue(eventType, out UnityEvent<GameEvent> thisEvent))
        {
            // 获取监听器引用
            if (Instance.listenerReferences.TryGetValue(eventType, out UnityAction<GameEvent> action))
            {
                // 移除监听器
                thisEvent.RemoveListener(action);
                Instance.listenerReferences.Remove(eventType);
            }
        }
    }

    // 触发事件
    public static void TriggerEvent<T>(T eventData) where T : GameEvent
    {
        System.Type eventType = typeof(T);
        if (Instance.eventDictionary.TryGetValue(eventType, out UnityEvent<GameEvent> thisEvent))
        {
            thisEvent.Invoke(eventData);
        }
    }
}
// GameEvent.cs
public abstract class GameEvent { }