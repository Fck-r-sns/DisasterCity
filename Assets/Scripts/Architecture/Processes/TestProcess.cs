using UnityEngine;

public class TestProcess : Process
{
    float _time;
    float _duration;

    public TestProcess(string name, float duration) : base(name)
    {
        _duration = duration;
    }

    public override void OnStart()
    {
        DebugUtilities.Print(name, "OnStart");
    }

    public override bool OnUpdate(float dt)
    {
        _time += dt;
        progress = Mathf.Clamp01(_time / _duration);
        DebugUtilities.Print(name, "OnUpdate", dt, _time);
        return _time >= _duration;
    }

    public override void OnFinish()
    {
        DebugUtilities.Print(name, "OnFinish");
    }

    public override void OnTerminate()
    {
        DebugUtilities.Print(name, "OnTerminate");
    }
}
