from appium import webdriver
from appium.webdriver.common.appiumby import AppiumBy
from appium.options.android import UiAutomator2Options
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC


from selenium.webdriver.common.action_chains import ActionChains
from selenium.webdriver.common.actions import interaction
from selenium.webdriver.common.actions.action_builder import ActionBuilder
from selenium.webdriver.common.actions.pointer_input import PointerInput

import random
import time
from time import sleep


desired_caps = {
    'platformName': 'Android',
    'deviceName': '21191FDEE003GC',
    # 'deviceName': 'emulator-5554',
    'appPackage': 'com.artivis.vm',
    'appActivity': 'com.artivis.vm.activity',
    'automationName': 'UiAutomator2'
}



capabilities_options = UiAutomator2Options().load_capabilities(desired_caps)
driver = None
action = None



def wait_for_text(text, timeout=30):
    return WebDriverWait(driver, timeout).until(EC.text_to_be_present_in_element((By.XPATH, f"//*[contains(@text, '{text}')]"), text))


def do_launch():
    global driver
    global actions
    print("Doing launch")
    driver = webdriver.Remote(command_executor='http://127.0.0.1:4723',options=capabilities_options)
    driver.implicitly_wait(10)
    actions = ActionChains(driver)
    time.sleep(5)


def do_user_login(email = "cw@hcms.com"):
    print("Doing user login")
    el3 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.widget.EditText\").instance(0)")
    el3.clear()
    el3.send_keys(email)
    el4 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"LOGIN\")")
    el4.click()



def do_permission():
    print("Doing permission")
    WebDriverWait(driver, 30).until(EC.presence_of_element_located((By.XPATH,"//*[@text='Missing Permission(s)!']")))
    if element_exists(By.XPATH, "//*[@text='Tap here to disable Battery optimizations.']"):
        el1 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Tap here to disable Battery optimizations.\")")
        el1.click()
        el2 = driver.find_element(by=AppiumBy.ID, value="android:id/button1")
        el2.click()
    
    el4 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Tap here to enable Location permission (All the time).\")")
    el4.click()
    el5 = driver.find_element(by=AppiumBy.ID, value="com.android.permissioncontroller:id/permission_allow_foreground_only_button")
    el5.click()
    el6 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Tap here to enable Physical activity permission.\")")
    el6.click()
    el7 = driver.find_element(by=AppiumBy.ID, value="com.android.permissioncontroller:id/permission_allow_button")
    el7.click()
    el8 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Tap here to enable Notifications.\")")
    el8.click()
    el9 = driver.find_element(by=AppiumBy.ID, value="com.android.permissioncontroller:id/permission_allow_button")
    el9.click()
    el10 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"DONE\")")
    el10.click()




def do_provider_login():
    print("Doing provider login")
    el2 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.view.ViewGroup\").instance(18)")
    el2.click()
    el3 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.view.ViewGroup\").instance(12)")
    el3.click()



def element_exists(by, value):
    return len(driver.find_elements(by, value)) > 0


def start_app_if_not_running(driver, package_name, activity_name):
    current_package = driver.current_package
    
    if current_package != package_name:
        print(f"Bringing app {package_name} to the foreground.")
        driver.start_activity(package_name, activity_name)

def do_random():
    while True:
        start_app_if_not_running(driver, "com.artivis.vm", 'com.artivis.vm.activity')
        # input_fields = driver.find_elements(AppiumBy.CLASS_NAME, "android.widget.EditText")
        # for field in input_fields:
        #     field.send_keys("Test Input")
        #     print("Input into:", field)
        clickable_elements = driver.find_elements(AppiumBy.ANDROID_UIAUTOMATOR, 'new UiSelector().clickable(true)')
        print(len(clickable_elements))
        if clickable_elements:
            element = random.choice(clickable_elements)
            print("Clicked on:", element.text)
            element.click()



def do_atta_bmap():
    print("Doing attachment bodymap")
    # attachment
    el2 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Insert\nPhoto\")")
    el2.click()
    el3 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().resourceId(\"com.google.android.providers.media.module:id/icon_thumbnail\").instance(0)")
    el3.click()
    time.sleep(2)
    # bodymap
    el4 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Body Maps\")")
    el4.click()
    el5 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"New Body Map\")")
    el5.click()
    
    actions.w3c_actions = ActionBuilder(driver, mouse=PointerInput(interaction.POINTER_TOUCH, "touch"))
    actions.w3c_actions.pointer_action.move_to_location(831, 924)
    actions.w3c_actions.pointer_action.pointer_down()
    actions.w3c_actions.pointer_action.pause(0.1)
    actions.w3c_actions.pointer_action.release()
    actions.perform()
    
    el8 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Notes\")")
    el8.click()
    el9 = driver.find_element(by=AppiumBy.CLASS_NAME, value="android.widget.EditText")
    el9.send_keys("b notes")
    el10 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Insert\nPhoto\")")
    el10.click()
    el11 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().resourceId(\"com.google.android.providers.media.module:id/icon_thumbnail\").instance(0)")
    el11.click()
    time.sleep(5)
    el12 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"SAVE\")")
    el12.click()
    time.sleep(5)

def do_incident():
    print("Doing incident")
    el3 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Incident Report\")")
    el3.click()
    sleep(3)
    el4 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Incident Type\")")
    el4.click()
    el5 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"ACCIDENT\")")
    el5.click()
    el6 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Injury\")")
    el6.click()
    el7 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"HEMATOMA\")")
    el7.click()
    el8 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Treatment\")")
    el8.click()
    el9 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"FIRST AID ADMINISTERED\")")
    el9.click()
    actions = ActionChains(driver)
    actions.w3c_actions = ActionBuilder(driver, mouse=PointerInput(interaction.POINTER_TOUCH, "touch"))
    actions.w3c_actions.pointer_action.move_to_location(763, 2085)
    actions.w3c_actions.pointer_action.pointer_down()
    actions.w3c_actions.pointer_action.move_to_location(807, 927)
    actions.w3c_actions.pointer_action.release()
    actions.perform()
    el11 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.widget.EditText\").instance(5)")
    el11.send_keys("in notes")
    el12 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"SAVE\")")
    el12.click()


def do():
    # provider login
    do_provider_login()
    
    # user login
    do_user_login(email="cw@hcms.com")
    
    # do permission page
    do_permission()

    

# while True:
#     try:
#         do_launch()
#         do()
#         do_random()
#     except Exception as e:
#         print(e)
#         driver.quit()


