using System;
using System.Collections.Generic;
using System.Threading;

public class ThreadDispatcher : MonoSingleton<ThreadDispatcher>
{
    static volatile bool _queued = false;
    static List<Action> _backlog = new List<Action>(8);
    static List<Action> _actions = new List<Action>(8);

    public static void RunAsync(Action action)
    {
        ThreadPool.QueueUserWorkItem(o => action());
    }

    public static void RunOnMainThread(Action action)
    {
        lock (_backlog)
        {
            _backlog.Add(action);
            _queued = true;
        }
    }

    void Update()
    {
        if (_queued)
        {
            lock (_backlog)
            {
                var tmp = _actions;
                _actions = _backlog;
                _backlog = tmp;
                _queued = false;
            }

            foreach (var action in _actions)
                action();

            _actions.Clear();
        }
    }
}
