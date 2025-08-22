using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Support.UI;
using System;

namespace UITests;

public abstract class BaseTest
{
    protected AppiumDriver App => AppiumSetup.App;

    // Default timeout for waits
    private readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(30);

    // Wait for an element by ID or AccessibilityId (based on platform)
    protected AppiumElement WaitForElement(string id, int timeoutSeconds = 30)
    {
        var wait = new WebDriverWait(App, TimeSpan.FromSeconds(timeoutSeconds));

        return wait.Until(driver =>
        {
            try
            {
                return FindElementWithoutWait(id);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        });
    }

    // Internal find logic without wait — used inside the wait
    private AppiumElement FindElementWithoutWait(string id)
    {
        if (App is WindowsDriver)
        {
            return (AppiumElement)App.FindElement(MobileBy.AccessibilityId(id));
        }

        return (AppiumElement)App.FindElement(MobileBy.Id(id));
    }

    // This becomes your standard "Find + Wait" for UI elements
    protected AppiumElement FindUIElement(string id)
    {
        return WaitForElement(id, (int)_defaultTimeout.TotalSeconds);
    }

    protected void EnterText(string id, string text)
    {
        var element = FindUIElement(id); // Waits for the element using your base method

        if (App is AndroidDriver)
        {
            // Drill down to the EditText inside the container
            var innerEditText = element.FindElement(
                MobileBy.AndroidUIAutomator("new UiSelector().className(\"android.widget.EditText\")")
            );

            innerEditText.Clear();
            innerEditText.SendKeys(text);
        }
        else
        {
            throw new NotImplementedException("EnterTextDeep is not implemented for iOS.");
        }
    }


    protected void Click(string id)
    {
        FindUIElement(id).Click();
    }

    protected string? GetCurrentActivity()
    {
        if (App is AndroidDriver androidDriver)
        {
            return androidDriver.CurrentActivity;
        }
        return null;
    }
}
