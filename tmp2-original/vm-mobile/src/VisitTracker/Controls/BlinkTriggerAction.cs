namespace VisitTracker;

/// <summary>
/// BlinkTriggerAction is a trigger action that makes a visual element blink by animating its opacity.
/// It uses a linear easing function to create a smooth transition between fully opaque and fully transparent states.
/// </summary>
public class BlinkTriggerAction : TriggerAction<VisualElement>
{
    protected override void Invoke(VisualElement sender)
    {
        new Animation
        {
            { 0, 0.5,new Animation(d => sender.Opacity = d, 1, 0, Easing.Linear) },
            { 0.5, 1, new Animation(d => sender.Opacity = d, 0, 1, Easing.Linear) }
        }.Commit(sender, "ChildAnimations", 8, 1000, Easing.SinInOut, repeat: () => true);
    }
}