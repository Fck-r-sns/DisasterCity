using System;

public abstract class Process
{
    static int IdGenerator;

    public event Action onStarted;
    public event Action onUpdated;
    public event Action onFinished;
    public event Action onTerminated;

    public int id { get; private set; }
    public string name { get; private set; }
    public float progress { get; protected set; }

    protected Process(string name)
    {
        this.id = ++IdGenerator;
        this.name = name;
    }

    public void Start()
    {
        OnStart();
        if (onStarted != null)
            onStarted();
    }

    public bool Update(float dt)
    {
        bool res = OnUpdate(dt);
        if (onUpdated != null)
            onUpdated();
        return res;
    }

    public void Finish()
    {
        OnFinish();
        if (onFinished != null)
            onFinished();
    }

    public void Terminate()
    {
        OnTerminate();
        if (onTerminated != null)
            onTerminated();
    }

    protected virtual void OnStart() { }
    protected virtual bool OnUpdate(float dt) { return true; }
    protected virtual void OnFinish() { }
    protected virtual void OnTerminate() { }
}
