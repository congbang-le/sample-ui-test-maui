using Microsoft.Maui.Controls.Shapes;
using System.Windows.Input;
using UraniumUI.Extensions;
using UraniumUI.Pages;
using UraniumUI.Resources;
using UraniumUI.Views;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace VisitTracker;

/// <summary>
/// CustomChip is a custom control that represents a chip with a label and a close button.
/// It inherits from Border and provides properties for setting the text, text color, and a command to execute when the close button is clicked.
/// </summary>
public class CustomChip : Border
{
    public event EventHandler DestroyClicked;

    protected Label label = new Label
    {
        VerticalOptions = LayoutOptions.Center,
    };

    protected StatefulContentView closeButton = new StatefulContentView
    {
        Content = new Path
        {
            Data = UraniumShapes.XCircle,
        },
        VerticalOptions = LayoutOptions.Center,
    };

    public CustomChip()
    {
        this.HorizontalOptions = LayoutOptions.Start;
        this.Padding = 8;
        this.Margin = new Thickness(1);
        this.StrokeShape = new RoundRectangle
        {
            CornerRadius = 20,
        };
        var defaultAccent = InputKit.Shared.InputKitOptions.GetAccentColor();
        this.SetAppThemeColor(
            BackgroundColorProperty,
            ColorResource.GetColor("Primary", defaultAccent),
            ColorResource.GetColor("PrimaryDark", defaultAccent));

        (closeButton.Content as Path).SetAppTheme(
            Path.FillProperty,
            Colors.Black.ToSolidColorBrush(),
            Colors.Black.ToSolidColorBrush()
            );

        label.SetAppThemeColor(
            Label.TextColorProperty,
            ColorResource.GetColor("OnPrimary", Colors.White),
            ColorResource.GetColor("OnPrimaryDark", Colors.DarkGray));

        Content = new HorizontalStackLayout
        {
            Spacing = 5,
            Children =
            {
                label,
                closeButton
            }
        };

        var tapGesture = new TapGestureRecognizer
        {
            Command = new Command(() =>
            {
                DestroyCommand?.Execute(this);
                DestroyClicked?.Invoke(this, new EventArgs());
            })
        };
        closeButton.GestureRecognizers.Add(tapGesture);
    }

    public ICommand DestroyCommand { get => (ICommand)GetValue(DestroyCommandProperty); set => SetValue(DestroyCommandProperty, value); }

    public static readonly BindableProperty DestroyCommandProperty = BindableProperty.Create(
            nameof(DestroyCommand),
            typeof(ICommand),
            typeof(CustomChip));

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(CustomChip),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is CustomChip chip)
            {
                chip.label.Text = (string)newValue;
            }
        });

    public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(CustomChip),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is CustomChip chip)
                {
                    chip.label.TextColor = (Color)newValue;
                }
            });
}