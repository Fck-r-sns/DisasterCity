public abstract class Process
{
    static int IdGenerator;

    public int id { get; private set; }
    public string name { get; private set; }
    public float progress { get; protected set; }

    protected Process(string name)
    {
        this.id = ++IdGenerator;
        this.name = name;
    }

    public abstract void OnStart();
    public abstract bool OnUpdate(float dt);
    public abstract void OnFinish();
    public abstract void OnTerminate();
}
