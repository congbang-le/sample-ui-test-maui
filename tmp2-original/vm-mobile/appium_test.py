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
    el11 = driver.find_element(by=AppiumBy.ACCESSIBILITY_ID, value="Ongoing")
    el11.click()
    
    
    
    for i in range(10):
        wait_for_text("Upcoming Booking")
        el12 = driver.find_element(by=AppiumBy.CLASS_NAME, value="android.widget.Button")
        el12.click()
        # wait for ongoing booking page open
        wait_for_text("Service User")
        
        
        # do_incident()
        # time.sleep(3)
        # completing task
        driver.swipe(1056, 2000, 1056, 1420, 188)
        # sibling_count = len(driver.find_elements(By.XPATH, "//*[@text='Tasks']/following-sibling::*"))
        following_sibling = driver.find_element(By.XPATH, "//android.widget.TextView[@text='Tasks']/following-sibling::*[1]")
        children = following_sibling.find_elements(By.XPATH, "./*/*")
        sibling_count = len(children)
        print("Task sibling_count", sibling_count)
        for i in range(sibling_count):
            print("task", i)
            driver.find_element(By.XPATH, f"//android.widget.TextView[@text='Tasks']/following-sibling::*[1]/*[{i+1}]").click()
            sleep(1)
            # driver.find_element(By.XPATH, f"//*[@text='Tasks']/following-sibling::*[{i+1}]").click()
            driver.find_element(By.XPATH, "//*[@text='Completion Status']").click()
            driver.find_element(By.XPATH, "//*[@text='COMPLETED']").click()
            # notes
            el1 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.widget.EditText\").instance(1)")
            el1.send_keys("task note")
            
            # do_atta_bmap()
            
            driver.find_element(By.XPATH, "//*[@text='SAVE']").click()
            
        time.sleep(3)
        driver.swipe(523, 2457, 596, 1592, 283)
        
        if element_exists(By.XPATH, "//*[@text='Record fluid balance']"):
            driver.find_element(By.XPATH, "//*[@text='Record fluid balance']").click()
            time.sleep(3)
            el1 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.widget.EditText\").instance(1)")
            el1.send_keys("12")
            el2 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.widget.EditText\").instance(2)")
            el2.send_keys("12")
            el3 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.widget.EditText\").instance(3)")
            el3.send_keys("12")
            el4 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.widget.EditText\").instance(4)")
            el4.send_keys("12")
            el5 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.widget.EditText\").instance(5)")
            el5.send_keys("12")
            el6 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.widget.EditText\").instance(6)")
            el6.send_keys("12")
            el7 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.widget.EditText\").instance(7)")
            el7.send_keys("12")
            
            driver.swipe(753, 2357, 765, 1180, 246)
            time.sleep(2)
            # do_atta_bmap()
            driver.find_element(By.XPATH, "//*[@text='SAVE']").click()
        
        # medication
        if not element_exists(By.XPATH, '//android.widget.TextView[@text="No medications found!"]'):
            # sibling_count = len(driver.find_elements(By.XPATH, "//*[@text='Medications']/following-sibling::*"))
            following_sibling = driver.find_element(By.XPATH, "//android.widget.TextView[@text='Medications']/following-sibling::*[1]")
            children = following_sibling.find_elements(By.XPATH, "./*/*")
            sibling_count = len(children)
            for i in range(sibling_count):
                print("medication", i)
                driver.find_element(By.XPATH, f"//*[@text='Medications']/following-sibling::*[1]/*[{i+1}]").click()
                time.sleep(3)
                driver.swipe(753, 2357, 765, 1180, 246)
                driver.find_element(By.XPATH, "//*[@text='Completion Status']").click()
                driver.find_element(By.XPATH, "//*[@text='COMPLETED']").click()
                driver.find_element(By.XPATH, "//*[@text='Status Detail']").click()
                driver.find_element(By.XPATH, "//*[@text='BY CARE WORKER']").click()
                # do_atta_bmap()
                driver.find_element(By.XPATH, "//*[@text='SAVE']").click()
        
        # visit summary
        driver.find_element(By.XPATH, "//*[@text='Visit Summary']/following-sibling::*[1]").click()
        # driver.find_element(By.XPATH, "//*[@text='Short Remarks']").click()
        driver.find_element(By.XPATH, "//*[@text='Faulty mobility equipment']").click()
        driver.find_element(By.XPATH, "//*[@text='OK']").click()
        el6 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.widget.EditText\").instance(0)")
        el6.send_keys("additional notes")
        driver.swipe(738, 2223, 838, 930, 237)
        # driver.find_element(By.XPATH, "//*[@text='Health Status']").click()
        driver.find_element(By.XPATH, "//*[@text='Visit Summary']/following-sibling::*[3]").click()
        driver.find_element(By.XPATH, "//*[@text='SU is in a gloomy mood - not urgent']").click()
        driver.find_element(By.XPATH, "//*[@text='OK']").click()
        el7 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.widget.EditText\").instance(1)")
        el7.send_keys("hand notes")
        time.sleep(3)
        # do_atta_bmap()
        driver.find_element(By.XPATH, "//*[@text='UPLOAD VISIT REPORT']").click()
        time.sleep(5)


while True:
    try:
        do_launch()
        do()
    except Exception as e:
        print(e)
        driver.quit()

# el17 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Budesonide inhalers\")")
# el17.click()
# time.sleep(2)
# driver.swipe(730, 2243, 808, 895, 1201)
# el18 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.view.ViewGroup\").instance(38)")
# el18.click()
# el19 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"COMPLETED\")")
# el19.click()
# time.sleep(2)
# driver.swipe(730, 2243, 808, 895, 1201)
# el20 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.view.ViewGroup\").instance(44)")
# el20.click()
# el21 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"BY CARE WORKER\")")
# el21.click()
# el22 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.view.ViewGroup\").instance(67)")
# el22.click()
# el23 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.view.ViewGroup\").instance(57)")
# el23.click()
# el24 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.view.ViewGroup\").instance(42)")
# el24.click()
# el25 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Injested complete meal\")")
# el25.click()
# el26 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"OK\")")
# el26.click()
# el27 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.view.ViewGroup\").instance(56)")
# el27.click()
# el28 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"SU is seriously unwell - urgent\")")
# el28.click()
# el29 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"OK\")")
# el29.click()
# el30 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.view.ViewGroup\").instance(68)")
# el30.click()



















