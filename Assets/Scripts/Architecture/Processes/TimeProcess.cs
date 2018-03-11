public class TimeProcess : Process
{
    float _time;
    float _duration;

    public TimeProcess(string name, float duration) : base(name)
    {
        _duration = duration;
    }

    protected override bool OnUpdate(float dt)
    {
        _time += dt;
        return _time >= _duration;
    }
}
