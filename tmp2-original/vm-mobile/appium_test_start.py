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
desired_caps = {
    'platformName': 'Android',
    'deviceName': '21191FDEE003GC',
    # 'deviceName': 'emulator-5554',
    'appPackage': 'com.artivis.vm',
    'appActivity': 'com.artivis.vm.activity',
    'automationName': 'UiAutomator2'
}

import time
from time import sleep

print("start")
capabilities_options = UiAutomator2Options().load_capabilities(desired_caps)
# driver = webdriver.Remote('http://127.0.0.1:4723/wd/hub', desired_caps)
driver = webdriver.Remote(command_executor='http://127.0.0.1:4723',options=capabilities_options)
driver.implicitly_wait(10)

def element_exists(by, value):
    return len(driver.find_elements(by, value)) > 0


def do_incident():
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



