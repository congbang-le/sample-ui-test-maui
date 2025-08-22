using Google.Android.Material.BottomNavigation;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Platform;

namespace VisitTracker;

public class TabbarBadgeRenderer : ShellRenderer
{
    protected override IShellBottomNavViewAppearanceTracker CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
    {
        return new BadgeShellBottomNavViewAppearanceTracker(this, shellItem);
    }
}

internal class BadgeShellBottomNavViewAppearanceTracker : ShellBottomNavViewAppearanceTracker
{
    public BadgeShellBottomNavViewAppearanceTracker(IShellContext shellContext, ShellItem shellItem) : base(shellContext, shellItem)
    { }

    public override void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
    {
        base.SetAppearance(bottomView, appearance);

        var isRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.TabBadgeCountMessage>(this);
        if (!isRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.TabBadgeCountMessage>(this,
            (recipient, message) =>
            {
                var badgeDrawable = bottomView.GetOrCreateBadge(message.Value.Item1);
                if (message.Value.Item2 <= 0) badgeDrawable.SetVisible(false);
                else
                {
                    badgeDrawable.Number = message.Value.Item2;
                    badgeDrawable.BackgroundColor = Colors.Red.ToPlatform();
                    badgeDrawable.BadgeTextColor = Colors.White.ToPlatform();
                    badgeDrawable.SetVisible(true);
                }
            });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.TabBadgeCountMessage>(this);
    }
}