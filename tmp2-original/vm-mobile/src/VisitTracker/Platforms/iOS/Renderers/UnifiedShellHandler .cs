using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;
using UIKit;
using Microsoft.Maui.Platform;

namespace VisitTracker;

public class UnifiedShellHandler : ShellRenderer
{
    protected override IShellTabBarAppearanceTracker CreateTabBarAppearanceTracker()
    {
        return new UnifiedTabBarAppearanceTracker();
    }
}
class UnifiedTabBarAppearanceTracker : ShellTabBarAppearanceTracker
{
    UITabBarAppearance _tabBarAppearance;
    UIColor _defaultBarTint;
    UIColor _defaultTint;
    UIColor _defaultUnselectedTint;

    public override void SetAppearance(UITabBarController controller, ShellAppearance appearance)
    {
        IShellAppearanceElement appearanceElement = appearance;
        var backgroundColor = appearanceElement.EffectiveTabBarBackgroundColor;
        var foregroundColor = appearanceElement.EffectiveTabBarForegroundColor;
        var unselectedColor = appearanceElement.EffectiveTabBarUnselectedColor;
        var titleColor = appearanceElement.EffectiveTabBarTitleColor;

        var tabBar = controller.TabBar;

        if (_defaultTint == null)
        {
            _defaultBarTint = tabBar.BarTintColor;
            _defaultTint = tabBar.TintColor;
            _defaultUnselectedTint = tabBar.UnselectedItemTintColor;
        }

        if (OperatingSystem.IsIOSVersionAtLeast(15))
            UpdateiOS15TabBarAppearance(controller, appearance);
        else
            UpdateTabBarAppearance(controller);
    }

    void UpdateiOS15TabBarAppearance(UITabBarController controller, ShellAppearance appearance)
    {
        IShellAppearanceElement appearanceElement = appearance;
        var backgroundColor = appearanceElement.EffectiveTabBarBackgroundColor;
        var foregroundColor = appearanceElement.EffectiveTabBarForegroundColor;
        var unselectedColor = appearanceElement.EffectiveTabBarUnselectedColor;
        var titleColor = appearanceElement.EffectiveTabBarTitleColor;

        var paragraphStyle = NSParagraphStyle.Default;

        var selectedAttributes = new UIStringAttributes
        {
            Font = UIFont.BoldSystemFontOfSize(13f),
            ForegroundColor = (titleColor ?? foregroundColor)?.ToPlatform() ?? UIColor.Black,
            ParagraphStyle = paragraphStyle
        };

        var unselectedAttributes = new UIStringAttributes
        {
            Font = UIFont.SystemFontOfSize(12f),
            ForegroundColor = unselectedColor?.ToPlatform() ?? UIColor.Gray,
            ParagraphStyle = paragraphStyle
            
        };

        _tabBarAppearance ??= new UITabBarAppearance();
        _tabBarAppearance.ConfigureWithDefaultBackground();

        if (backgroundColor is not null && backgroundColor.IsNotDefault())
            _tabBarAppearance.BackgroundColor = backgroundColor.ToPlatform();

        _tabBarAppearance.StackedLayoutAppearance.Selected.TitleTextAttributes = selectedAttributes;
        _tabBarAppearance.StackedLayoutAppearance.Normal.TitleTextAttributes = unselectedAttributes;
        _tabBarAppearance.InlineLayoutAppearance.Selected.TitleTextAttributes = selectedAttributes;
        _tabBarAppearance.InlineLayoutAppearance.Normal.TitleTextAttributes = unselectedAttributes;
        _tabBarAppearance.CompactInlineLayoutAppearance.Selected.TitleTextAttributes = selectedAttributes;
        _tabBarAppearance.CompactInlineLayoutAppearance.Normal.TitleTextAttributes = unselectedAttributes;

        controller.TabBar.StandardAppearance = controller.TabBar.ScrollEdgeAppearance = _tabBarAppearance;
    }

    void UpdateTabBarAppearance(UITabBarController controller)
    {
        float fontSize = 13f;
        var font = UIFont.SystemFontOfSize(fontSize);

        UITabBarItem.Appearance.SetTitleTextAttributes(new UIStringAttributes { Font = font }, UIControlState.Normal);
        UITabBarItem.Appearance.SetTitleTextAttributes(new UIStringAttributes { Font = font }, UIControlState.Selected);
    }

    public override void UpdateLayout(UITabBarController controller)
    {
        base.UpdateLayout(controller);

        if (!WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.TabBadgeCountMessage>(this))
        {
            WeakReferenceMessenger.Default.Register<MessagingEvents.TabBadgeCountMessage>(this,
                (recipient, message) =>
                {
                    UIApplication.SharedApplication.InvokeOnMainThread(() =>
                    {
                        var items = controller.TabBar?.Items;
                        int index = message.Value.Item1;

                        if (items == null || index < 0 || index >= items.Length)
                            return;

                        var tabBarItem = items[index];

                        if (message.Value.Item2 <= 0)
                        {
                            tabBarItem.BadgeValue = null;
                        }
                        else
                        {
                            tabBarItem.BadgeValue = message.Value.Item2.ToString();
                            tabBarItem.BadgeColor = Colors.Red.ToPlatform();
                        }
                    });
                });
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.TabBadgeCountMessage>(this);
    }
}
