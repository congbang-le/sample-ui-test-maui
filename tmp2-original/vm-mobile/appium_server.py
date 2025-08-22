from appium.webdriver.appium_service import AppiumService
appium_service = AppiumService()
appium_service.start(args=['--address', '127.0.0.1', '--port', '4723'])
if appium_service.is_running:
    print("Appium server started successfully.")
appium_service.stop()
