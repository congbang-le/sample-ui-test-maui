using NUnit.Framework;

// You will have to make sure that all the namespaces match
// between the different platform specific projects and the shared
// code files. This has to do with how we initialize the AppiumDriver
// through the AppiumSetup.cs files and NUnit SetUpFixture attributes.
// Also see: https://docs.nunit.org/articles/nunit/writing-tests/attributes/setupfixture.html
namespace UITests;

// This is an example of tests that do not need anything platform specific
public class MainPageTests : BaseTest
{
    private bool ProviderLogin(string ProviderCode) 
    {
        try
        {
            // Step 1: Wait for Login Page
            WaitForElement("LoginProviderPage");
            // Step 2: Enter provider code using inner EditText strategy
            EnterText("ProviderCode", "654332");
            // Step 3: Tap Login Button
            Click("LoginButton");
            // Step 4: Wait for ConfirmButton to be clickable
            WaitForElement("ConfirmButton");
            // Step 5: Tap Confirm Button
            Click("ConfirmButton");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during provider login: {ex.Message}");
            return false;
        }
    }

    private bool UserLogin(string email = "sup@hcms.com", string password = "qwer$#@!")
    {
        try
        {
            Console.WriteLine($"Doing user login with email: {email}");

            // Step 1: Wait for the Login Page
            WaitForElement("LoginPage");

            // Step 2: Enter Email and Password
            EnterText("Email", email);
            EnterText("Password", password);

            // Step 3: Click Login Button
            Click("LoginButton");

            Console.WriteLine("User login attempted.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during user login: {ex.Message}");
            return false;
        }
    }


    [Test]
	public void Login()
	{
        try
        {
            // Arrange
            App.StartRecordingScreen();
            // Act
            Assert.That(ProviderLogin("654332"), Is.True, "Provider login failed");
            Assert.That(UserLogin(), Is.True, "User login failed.");
            WaitForElement("SupervisorHomePage",120);
            Task.Delay(5000).Wait(); // Wait for the click to register and show up on the screenshot
            Console.WriteLine(GetCurrentActivity());

            // Assert
            App.GetScreenshot().SaveAsFile($"{nameof(Login)}.png");
            //Assert.That(element.Text, Is.EqualTo("Clicked 1 time"));
        }
        catch (Exception ex)
        {
            Assert.Fail($"Main page did not load: {ex.Message}");
        }
        finally
        {
            string videoBase64 = App.StopRecordingScreen();
            string fileName = DateTime.Now.ToString("yyyyMMdd-HHmm") + ".mp4";
            string filePath = Path.Combine("/home/app/output", fileName);

            File.WriteAllBytes(filePath, Convert.FromBase64String(videoBase64));
            Console.WriteLine($"Recording saved to {filePath}");
        }


    }
}