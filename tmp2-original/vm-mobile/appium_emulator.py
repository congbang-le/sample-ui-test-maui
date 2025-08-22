from appium import webdriver
from appium.webdriver.common.appiumby import AppiumBy
from appium.options.android import UiAutomator2Options
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
import base64

from selenium.webdriver.common.action_chains import ActionChains
from selenium.webdriver.common.actions import interaction
from selenium.webdriver.common.actions.action_builder import ActionBuilder
from selenium.webdriver.common.actions.pointer_input import PointerInput
from datetime import datetime, timedelta
import random
import time
from time import sleep

from launch_emulator import create_emulator 
from launch_emulator import install_app
from appium.webdriver.appium_service import AppiumService

is_android = True
driver = None
action = None
wait_time = 10
port = random.randint(4723, 4999)
print("prot choosen", port)


def _do_click(value):
    driver.find_element(By.ID, value).click()
    
def element_exists(by, value):
    driver.implicitly_wait(0)
    found = len(driver.find_elements(by, value)) > 0
    driver.implicitly_wait(wait_time)
    return found

def wait_for_text(text, timeout=30):
    return WebDriverWait(driver, timeout).until(EC.text_to_be_present_in_element((By.XPATH, f"//*[contains(@text, '{text}')]"), text))

def _enter_text(id, text):
    el = driver.find_element(By.ID, id)
    if is_android:
        el = el.find_element(AppiumBy.ANDROID_UIAUTOMATOR, 'new UiSelector().className("android.widget.EditText")')
    else:
        raise NotImplementedError("Not implemented for iOS")
    el.clear()
    el.send_keys(text)

def _is_in_page(page_name):
    try:
        # Try to find the element with the specific AutomationId
        el2 = element_exists(by=AppiumBy.ID, value=f"com.artivis.vm:id/{page_name}")
        print(f"You are on the {page_name} page")
        return True
    except:
        print(f"Not on the {page_name} page")
        return False

def _is_in_notification_page():
    return _is_in_page("NotificationsPage")

def _is_in_booking_page():
    return _is_in_page("BookingsPage")

def _click_inicident():
    el3 = driver.find_element(by=AppiumBy.ID, value="IncidentReportButton")
    el3.click()
def _click_office():
    el3 = driver.find_element(by=AppiumBy.ID, value="CallOfficeButton")
    el3.click()   
def _click_emergency():
    el3 = driver.find_element(by=AppiumBy.ID, value="EmergencyButton")
    el3.click()
def _click_fluid_balance():
    el3 = driver.find_element(by=AppiumBy.ID, value="FluidChartButton")
    el3.click()
def _click_logrequest():
    el3 = driver.find_element(by=AppiumBy.ID, value="LogRequestButton")
    el3.click()
def _click_mar():
    el3 = driver.find_element(by=AppiumBy.ID, value="MarChartButton")
    el3.click()
def _click_form():
    el3 = driver.find_element(by=AppiumBy.ID, value="SubmitFormsButton")
    el3.click()

def _go_to_booking_page():
    print("Going to booking page")
    el3 = driver.find_element(by=AppiumBy.ACCESSIBILITY_ID, value="Bookings")
    el3.click()


def _go_to_notification_page():
    print("Going to notification page")
    el1 = driver.find_element(by=AppiumBy.ACCESSIBILITY_ID, value="Notifications")
    el1.click()

def _go_to_more_page():
    print("Going to more page")
    el2 = driver.find_element(by=AppiumBy.ACCESSIBILITY_ID, value="More")
    el2.click()
    
def _go_to_ongoing_booking_page():
    print("Going to ongoing booking page")
    if element_exists(by=AppiumBy.ACCESSIBILITY_ID, value="Ongoing"):
        el3 = driver.find_element(by=AppiumBy.ACCESSIBILITY_ID, value="Ongoing")
        el3.click()
    elif element_exists(by=AppiumBy.ACCESSIBILITY_ID, value="Upcoming"):
        el3 = driver.find_element(by=AppiumBy.ACCESSIBILITY_ID, value="Upcoming")
        el3.click()
    else:
        return False
    return True

def _go_to_dashboard_page():
    print("Going to dashboard page")
    el5 = driver.find_element(by=AppiumBy.ACCESSIBILITY_ID, value="Dashboard")
    el5.click()

def _go_to_su_page():
    print("Going to service user page")
    el5 = driver.find_element(by=AppiumBy.ACCESSIBILITY_ID, value="Service Users")
    el5.click()

def _go_to_cw_page():
    print("Going to care worker page")
    el5 = driver.find_element(by=AppiumBy.ACCESSIBILITY_ID, value="Care Workers")
    el5.click()

def _is_in_ongoing_page():
    return _is_in_page("OngoingPage")

def _is_in_permission_page():
    return _is_in_page("PermissionErrorPage")

def _scroll_to_by_id(id):
    try:
        driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value=f'new UiScrollable(new UiSelector().scrollable(true)).scrollIntoView(new UiSelector().resourceId("com.artivis.vm:id/{id}"))')
        return True
    except:
        return False
     
def _scroll_to_by_text(name):
    driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR,value=f'new UiScrollable(new UiSelector().scrollable(true)).scrollTextIntoView("{name}")')

def _set_location(latitude, longitude):
    global driver
    altitude = 0
    try:
        driver.set_location(latitude, longitude, altitude)
        print(f"Location set to: {latitude}, {longitude}")
    except Exception as e:
        print(f"Failed to set location: {e}")

def do_launch(adb_name='emulator-5554'):
    global driver
    global actions
    capabilities_options = UiAutomator2Options()
    capabilities_options.platform_name = "Android"
    capabilities_options.udid = adb_name
    capabilities_options.app_package = "com.artivis.vm"
    capabilities_options.app_activity = "com.artivis.vm.activity"
    capabilities_options.automation_name = "UiAutomator2"
    print("Doing launch")
    driver = webdriver.Remote(command_executor=f'http://127.0.0.1:{port}',options=capabilities_options)
    driver.implicitly_wait(wait_time)
    actions = ActionChains(driver)
    time.sleep(5)
    print("Launch done")
    _set_location(51.48636729841107, -0.10239351837004435) # london location



def do_login(email="cw@hcms.com", password="qwer$#@!"):
    print(f"Doing user login {email}")
    WebDriverWait(driver, 30).until(EC.presence_of_element_located((By.ID, "LoginPage")))
    _enter_text(id="Email", text=email)
    _enter_text(id="Password", text=password)
    login_button = driver.find_element(By.ID, "LoginButton")
    login_button.click()
    print("User login done")



def do_permission():
    print("Doing permission")
    WebDriverWait(driver, 30).until(EC.presence_of_element_located((By.XPATH,"//*[@text='Missing Permissions']")))
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
    el8 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Enable Notification Permission\")")
    el8.click()
    el9 = driver.find_element(by=AppiumBy.ID, value="com.android.permissioncontroller:id/permission_allow_button")
    el9.click()
    # no need to do done button
    if element_exists(By.ID, "DoneSettings"):
        el10 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"DONE\")")
        el10.click()
    print("Permission done")


def do_provider_login(provider_code="180410"):
    WebDriverWait(driver, 30).until(EC.presence_of_element_located((By.ID, "LoginProviderPage")))
    _enter_text(id = "ProviderCode", text = provider_code)
    login_button = driver.find_element(By.ID, "LoginButton")
    login_button.click()
    WebDriverWait(driver, 30).until(EC.element_to_be_clickable((By.ID, "ConfirmButton")))
    # qr_button = driver.find_element(By.ID, "QRCodeButton")
    # back_button = driver.find_element(By.ID, "BackButton")
    confirm_button = driver.find_element(By.ID, "ConfirmButton")
    confirm_button.click()



def do_notification():
    print("Doing notification")
    el2 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Notifications\")")
    el2.click()
    selector = "new UiSelector().resourceId(\"com.artivis.vm:id/NotificationItem\")"
    if element_exists(AppiumBy.ANDROID_UIAUTOMATOR, selector):
        instances = driver.find_elements(AppiumBy.ANDROID_UIAUTOMATOR, selector)
        num_instances = len(instances)
        print(f"Number of notification found: {num_instances}")
        print("select the first notification")
        el2 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().resourceId(\"com.artivis.vm:id/NotificationItem\").instance(0)")
        el2.click()
        time.sleep(2)
        if not _is_in_booking_page():
            driver.back()
    print("Notification done")

def do_past_reports():
    print("Doing past reports")
    _go_to_booking_page()
    el16 = driver.find_element(by=AppiumBy.ID, value="PastReportsButton")
    el16.click()
    el16 = driver.find_element(by=AppiumBy.ID, value="DatePicker")
    el16.click()
    # minuum 1 day
    date = (datetime.now() - timedelta(days=random.randint(1,7))).strftime("%d %B %Y")
    print("Selecting date", date)
    driver.find_element(by=AppiumBy.ACCESSIBILITY_ID, value=date).click()
    driver.find_element(by=AppiumBy.ID, value="android:id/button1").click()
    # driver.find_element(by=AppiumBy.ID, value="DownloadButton").click()    # download button removed recently  
        
    if element_exists(By.ID, "android:id/button2"):
        el21 = driver.find_element(by=AppiumBy.ID, value="android:id/button2").click()
        time.sleep(1)
    driver.back()
    print("Past reports done")
    

def do_past_incident():
    print("Doing past incident")
    el16 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Past Reports\")")
    el16.click()    
    el23 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Incidents\")")
    el23.click()
    el24 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"DOWNLOAD\")")
    el24.click()
    if element_exists(By.ID, "android:id/button2"):
        el21 = driver.find_element(by=AppiumBy.ID, value="android:id/button2")
        el21.click()
        time.sleep(1)
    driver.back()
    print("Past incident done")


def _do_atta(image=False, camera=False):
    if camera :
        print("Camera image")
        el5 = driver.find_element(by=AppiumBy.ID, value="TakePhoto").click()
        time.sleep(2)
        if element_exists(AppiumBy.ID, "com.android.permissioncontroller:id/permission_allow_foreground_only_button"):
            driver.find_element(by=AppiumBy.ID, value="com.android.permissioncontroller:id/permission_allow_foreground_only_button").click()
        driver.find_element(by=AppiumBy.ACCESSIBILITY_ID, value="Shutter").click()
        driver.find_element(by=AppiumBy.ACCESSIBILITY_ID, value="Done").click()
    if image : 
        # attachment
        print("Attachment image")
        driver.find_element(by=AppiumBy.ID, value="InsertPhoto").click()
        driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().resourceId(\"com.google.android.providers.media.module:id/icon_thumbnail\").instance(0)").click()
        time.sleep(2)

def do_atta_bmap(image=False, camera=False):
    print("Doing attachment bodymap")
    _do_atta(image, camera)
    # bodymap
    driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Body Maps\")").click()
    driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"New Body Map\")").click()
    time.sleep(2)
    
    actions.w3c_actions = ActionBuilder(driver, mouse=PointerInput(interaction.POINTER_TOUCH, "touch"))
    actions.w3c_actions.pointer_action.move_to_location(831, 924)
    actions.w3c_actions.pointer_action.pointer_down()
    actions.w3c_actions.pointer_action.pause(0.1)
    actions.w3c_actions.pointer_action.release()
    actions.perform()
    driver.find_element(by=AppiumBy.ID, value="com.artivis.vm:id/ChangeBodyMap").click()
    actions.w3c_actions = ActionBuilder(driver, mouse=PointerInput(interaction.POINTER_TOUCH, "touch"))
    actions.w3c_actions.pointer_action.move_to_location(831, 924)
    actions.w3c_actions.pointer_action.pointer_down()
    actions.w3c_actions.pointer_action.pause(0.1)
    actions.w3c_actions.pointer_action.release()
    actions.perform()
    
    driver.find_element(by=AppiumBy.ID, value="Notes").click()
    _enter_text(id="Notes", text="bodymap notes")
    _do_atta(image, camera)    
    time.sleep(3)
    driver.find_element(by=AppiumBy.ID, value="Save").click()
    time.sleep(3)
    print("Attachment bodymap done")

def do_incident():
    print("Doing incident")
    _click_inicident()
    sleep(3)
    el4 = driver.find_element(by=AppiumBy.ID, value="IncidentType")
    el4.click()
    el5 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Fall\")")
    el5.click()
    el6 = driver.find_element(by=AppiumBy.ID, value="Injury")
    el6.click()
    el7 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Swelling\")")
    el7.click()
    el8 = driver.find_element(by=AppiumBy.ID, value="Treatment")
    el8.click()
    el9 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Refused\")")
    el9.click()
    driver.swipe(805, 1396, 805, 700,100)
    _enter_text(id="Notes", text="incident notes")
    el12 = driver.find_element(by=AppiumBy.ID, value="Save")
    el12.click()
    print("Incident done")

def do_task():
    print("Doing task")
    selector = "new UiSelector().resourceId(\"com.artivis.vm:id/TaskButton\")"
    if element_exists(AppiumBy.ANDROID_UIAUTOMATOR, selector):
        instances = driver.find_elements(AppiumBy.ANDROID_UIAUTOMATOR, selector)
        num_instances = len(instances)
        print(f"Number of task found: {num_instances}")
        for i in range(num_instances):
            el2 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value=f"new UiSelector().resourceId(\"com.artivis.vm:id/TaskButton\").instance({i})")
            el2.click()
            driver.find_element(By.ID, "CompletionStatus").click()
            driver.find_element(By.XPATH, "//*[@text='Completed']").click()
            # notes
            _enter_text("Notes", "task note")
            do_atta_bmap()
            driver.find_element(By.ID, "SaveButton").click()
            time.sleep(1)
    print("Task done")

def do_consumable():
    print("Doing consumable")
    selector = "new UiSelector().resourceId(\"com.artivis.vm:id/ConsumableLabel\")"
    if element_exists(AppiumBy.ANDROID_UIAUTOMATOR, selector):
        selector = "new UiSelector().resourceId(\"com.artivis.vm:id/ConsumableUsed\")"
        instances = driver.find_elements(AppiumBy.ANDROID_UIAUTOMATOR, selector)
        num_instances = len(instances)
        print(f"Number of consumable found: {num_instances}")
        for i in range(num_instances):
            el2 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value=f"new UiSelector().resourceId(\"com.artivis.vm:id/ConsumableUsed\").instance({i})")
            el2 = el2.find_element(AppiumBy.ANDROID_UIAUTOMATOR, 'new UiSelector().className("android.widget.EditText")')
            el2.send_keys(random.randint(0, 4))

def do_fluid_balance():
    print("Doing fluid balance")
    sleep(3)
    if element_exists(By.ID, "FluidButton"):
        driver.find_element(By.ID, "FluidButton").click()
        time.sleep(3)
        for i in range(1, 8):
            el = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value=f"new UiSelector().className(\"android.widget.EditText\").instance({i})")
            el.send_keys(f"{random.randint(1, 10)*100}")
        driver.swipe(753, 2357, 765, 1180, 246)
        time.sleep(2)
        # do_atta_bmap()
        driver.find_element(By.XPATH, "//*[@text='SAVE']").click()
        time.sleep(1)
    print("Fluid balance done")

def do_medication():
    # medication
    print("Doing medication")
    selector = "new UiSelector().resourceId(\"com.artivis.vm:id/MedicationButton\")"
    if not element_exists(By.ID, selector):
        instances = driver.find_elements(AppiumBy.ANDROID_UIAUTOMATOR, selector)
        num_instances = len(instances)
        print(f"Number of medication found: {num_instances}")
        for i in range(num_instances):
            el2 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value=f"new UiSelector().resourceId(\"com.artivis.vm:id/MedicationButton\").instance({i})")
            el2.click()
            _scroll_to_by_id("CompletionStatus")
            driver.find_element(By.ID, "CompletionStatus").click()
            driver.find_element(By.XPATH, "//*[@text='COMPLETED']").click()
            _scroll_to_by_id("StatusDetail")
            driver.find_element(By.ID, "StatusDetail").click()
            driver.find_element(By.XPATH, "//*[@text='BY_CAREWORKER']").click()
            # do_atta_bmap()
            _scroll_to_by_id("SaveButton")
            driver.find_element(By.ID, "SaveButton").click()
            time.sleep(3)
    print("Medication done")
 
def is_not_master():
    print("Checking if not master booking")
    driver.swipe(1056, 2000, 1056, 1420, 100)
    res = element_exists(By.ID, "SubmitButtonNonMaster")
    driver.swipe(1056, 1420 , 1056, 2000, 100)
    print("Is not Master?", res)
    return res

def do_booking():
    print("Doing booking")
    if _go_to_ongoing_booking_page():
        print("Ongoing booking page")
    else:
        print("[Error] No ongoing or upcoming booking page")
        return
        
    
    for i in range(1):
        wait_for_text("Upcoming Booking")
        print("Upcoming booking page")
        print("Click on Start Visit")
        _do_click("StartVisitButton")
        time.sleep(3)
        while element_exists(By.ID, "StartVisitButton"):
            print("[WARNING] Reclicking on Start visit button")
            _do_click("StartVisitButton")
            time.sleep(3)
        # wait for ongoing booking page open
        wait_for_text("Service User")
        print("Visit started")
        
        if is_not_master():
            print("Not master booking")
            driver.swipe(1056, 2000, 1056, 1420, 100)
            _do_click("SubmitButtonNonMaster")
            continue
        time.sleep(3)
        # add incidents
        # if random.choice([True, False]):
        if True:
            do_incident()
            time.sleep(3)
        
        # completing task
        _scroll_to_by_id("TaskLabel")
        do_task()
            
        time.sleep(1)
        if _scroll_to_by_id("FluidButton"):
            do_fluid_balance()
        
        if _scroll_to_by_id("MedicationButton"):
            do_medication()
        
        _scroll_to_by_id("VisitSummaryLabel")
        # visit summary
        driver.find_element(By.ID, "ShortRemarks").click()
        driver.find_element(By.XPATH, "//*[@text='Faulty mobility equipment']").click()
        driver.find_element(By.XPATH, "//*[@text='OK']").click()
        _enter_text("AdditionalNotes", "additional notes")
        driver.find_element(By.ID, "HealthStatus").click()
        driver.find_element(By.XPATH, "//*[@text='SU is in a gloomy mood - not urgent']").click()
        driver.find_element(By.XPATH, "//*[@text='OK']").click()
        _enter_text('HandOverNotes', "hand notes")
        # do_atta_bmap()
        _scroll_to_by_id("ConsumableLabel")
        driver.swipe(1056, 2000, 1056, 1420, 100)
        do_consumable()
        driver.find_element(By.ID, "SubmitButton").click()

def do_edit():
    _go_to_booking_page()
    not_found = True
    while not_found:
        # do swipe and look for vr
        time.sleep(3)
        driver.swipe(805, 1396, 805, 700,800)
        time.sleep(2)
        elements = driver.find_elements(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().textMatches(\"VR-V\\d+\")")
        no_elements = len(elements)
        if no_elements == 0:
            print("No VR found")
            # if random.choice([True, False]):
            if True:
                print("Go to previous page")
                driver.find_element(By.ID, "PrevDayButton").click()
                continue
            else:
                return
        id = 0
        elem=None
        max_id = 2
        skip = False
        for index, el2 in enumerate(elements):
            print(el2.text)
            new_id = int(el2.text.split("-")[1][1])
            if new_id > max_id:
                if index == no_elements - 1:
                    skip = True
                    break
                else:
                    id = 0
                    elem = None
                    continue
            if new_id > id:
                id = new_id
                elem = el2
            else:
                break
        
        if skip:
            print("Skipping")
            continue
        elem.click()
        time.sleep(3)
        driver.find_element(by=AppiumBy.ACCESSIBILITY_ID, value="EditButton").click()
        # check has edit acess granted
        # check an element exit el21 = driver.find_element(by=AppiumBy.ID, value="android:id/button2")
        if element_exists(By.ID, "android:id/message"):
            if driver.find_element(By.ID, "android:id/message").text == 'Booking cannot be edited! Check again later!':
                el21 = driver.find_element(by=AppiumBy.ID, value="android:id/button2")
                el21.click()
                return 
        time.sleep(5)
        _scroll_to_by_id("ConsumableLabel")
        do_consumable()
        _scroll_to_by_id("SubmitButton")
        driver.find_element(by=AppiumBy.ID, value="SubmitButton").click()
        not_found = False

def do_cw():
    # user login
    # cw_list = ['kunbo@hcms.com', 'adla@hcms.com', 'ajones@gmail.com', 'mina@hcms.com', 'andrew@hcms.com', 'jarat@hcms.com', 'tito@hcms.com', 'ddyer@gmail.com', 'den@hcms.com', 'eida@hcms.com', 'geor@hcms.com', 'glori@hcms.com', 'gace@hcms.com', 'hmaxter@gmail.com', 'jhanavi@gmail.com', 'jad@hcms.com', 'janet@hcms.com', 'kclarke@gmail.com', 'kat@hcms.com', 'voro@hcms.com', 'ley@hcms.com', 'lian@hcms.com', 'lnorman@gmail.com', 'marina@hcms.com', 'moyo@hcms.com', 'moren@hcms.com', 'natt@hcms.com', 'yemina@hcms.com', 'lola@hcms.com', 'femi@hcms.com', 'nome@hcms.com', 'mola@hcms.com', 'pharvey@gmail.com', 'tina@hcms.com', 'shiv@hcms.com', 'abomi@hcms.com', 'southw@hcms.com', 'tclark@gmail.com', 'ynep@hcms.com']
    cw_list = ["cw2@hcms.com", "cw1@hcms.com"]
    email = random.choice(cw_list)
    do_login(email=email)
        
    # do permission page
    do_permission()
    while True:
        if random.randint(1, 10) < 9:
        # if True:
            # do booking
            do_booking()
        else:
            # do edit
            do_edit()
            
        if random.choice([True, False]):
            # do notification
            do_notification()
        if random.choice([True, False]):
            # do past reports
            do_past_reports()



################# SUP
def sup_do_su(su_name):
    el5 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Service Users\")")
    el5.click()
    el6 = driver.find_element(by=AppiumBy.ACCESSIBILITY_ID, value="Search Service Users")
    el6.click()
    el7 = driver.find_element(by=AppiumBy.CLASS_NAME, value="android.widget.AutoCompleteTextView")
    el7.send_keys(su_name)
    el8 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Pull to refresh\")")
    el8.click()
    
    # check mar chart
    if False:
    # if random.choice([True, False]):
        print("Checking MAR chart")
        el9 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"MAR Chart\")")
        el9.click()
        time.sleep(2)
        driver.swipe(805, 1396, 805, 700,100)
        driver.back()
    
    # check mar chart
    # if random.choice([True, False]):
    if False:
        print("Checking 24-Hr chart")
        el9 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"24-Hr Chart\")")
        el9.click()
        time.sleep(2)
        driver.swipe(805, 1396, 805, 700,100)
        driver.back()
        
    # if random.choice([True, False]):
    if False:
        print("Adding incidents")
        do_incident()
        
    #  do fingreprint
    if True:
        print("Doing fingerprint")
        el3 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Onsite Actions\")")
        el3.click()
        el10 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"START FINGERPRINT\")")
        el10.click()
        if element_exists(by= AppiumBy.ANDROID_UIAUTOMATOR, value = "new UiSelector().text(\"Missing Permission(s)!\")"):
            do_permission()
            el10 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"START FINGERPRINT\")")
            el10.click()
        time.sleep(5)
        # click ok if ask to overwrite finger print
        # time.sleep(2000)

        if element_exists(by=AppiumBy.ID, value="android:id/message"):
            el14 = driver.find_element(by=AppiumBy.ID, value="android:id/button1")
            el14.click()
        time.sleep(5)
        while element_exists(by= AppiumBy.ANDROID_UIAUTOMATOR, value = "new UiSelector().text(\"Fingerprint in progress...\")"):
            time.sleep(5)
        print("Fingerprint done")
                    
    # if random.choice([True, False]):
    if False:
        print("Checking past incidents")
        el23 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Incidents\")")
        el23.click()
        el24 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"DOWNLOAD\")")
        el24.click()
        time.sleep(2)
        if element_exists(By.ID, "android:id/button2"):
            el21 = driver.find_element(by=AppiumBy.ID, value="android:id/button2")
            el21.click()
            time.sleep(1)
    
    # if random.choice([True, False]):
    if False:
        print("Checking past reports")
        el1 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Rosters\")")
        el1.click()
        el17 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Choose Date\")")
        el17.click()
        # minuum 1 day
        date = (datetime.now() - timedelta(days=random.randint(1,7))).strftime("%d %B %Y")
        el18 = driver.find_element(by=AppiumBy.ACCESSIBILITY_ID, value=date)
        el18.click()
        el19 = driver.find_element(by=AppiumBy.ID, value="android:id/button1")
        el19.click()
        el2 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().className(\"android.view.ViewGroup\").instance(81)")
        el2.click()
        if element_exists(By.ID, "android:id/button2"):
            el21 = driver.find_element(by=AppiumBy.ID, value="android:id/button2")
            el21.click()
            time.sleep(1)
    
    # if random.choice([True, False]):
    if True:
        print("update ground truth")
        el3 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Away Actions\")")
        el3.click()
        el4 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"UPDATE GROUND TRUTH\")")
        el4.click()
        time.sleep(1)
        el1 = driver.find_element(by=AppiumBy.CLASS_NAME, value="android.widget.EditText")
        el1.clear()
        # el1.send_keys("1.34442668624643") # singapore
        el1.send_keys("51.48636729841107")
        el5 = driver.find_element(by=AppiumBy.ID, value="android:id/button1")
        el5.click()
        time.sleep(1)
        el1 = driver.find_element(by=AppiumBy.CLASS_NAME, value="android.widget.EditText")
        el1.clear()
        # el1.send_keys("103.711462801071") #singapore
        el1.send_keys("-0.10239351837004435")
        el6 = driver.find_element(by=AppiumBy.ID, value="android:id/button1")
        el6.click()
        time.sleep(1)
    
    # if random.choice([False]):
    if False:
        print("doing sup visit")
        el7 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"Onsite Actions\")")
        el7.click()
        el8 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"BEGIN FORMS SUBMISSION\")")
        el8.click()
        time.sleep(30)  #sup visit duration
        driver.back()
        el9 = driver.find_element(by=AppiumBy.ANDROID_UIAUTOMATOR, value="new UiSelector().text(\"END FORMS SUBMISSION\")")
        el9.click()
    
    driver.back()

def do_sup():
    do_login(email="sup@hcms.com")
    time.sleep(5)
    # su_list= ["Dominic"]
    su_list = ['Abram', 'Alberto', 'Annnie', 'Blanka', 'Bradene', 'Cartey', 'Cash', 'Chrissie', 'Colette', 'Conny', 'Corolo', 'Dan', 'Debby', 'Elsa', 'Fourice', 'Francia', 'Franco', 'Frankowen', 'Hult', 'Huma', 'Isobel', 'Jacky', 'Joana', 'Ken', 'Kev', 'Leisa', 'Lucalo', 'Madeleine', 'Marija', 'Millie', 'Moana', 'Mossey', 'Nabila', 'Olub', 'Olumar', 'Quinn', 'Raylon', 'Rhonaldino', 'Smithowen', 'Sylvinne', 'Valera', 'Willow', 'Winnie']
    random.shuffle(su_list)
    for su_name in su_list:
        sup_do_su(su_name)
    # while True:
    #     sup_do_su(su_name=random.choice(su_list))
    #     time.sleep(3)
    
def do():
    # provider login
    do_provider_login()
    
    # if random.choice([True, False]):
    if True:
        # for cw test
        do_cw()
    else:    
        # for sup test
        do_sup()
    






# __name__ = "__interactive__"
# __name__ = "__create_emulator__"
__name__="__real__"

if __name__ == "__real__":
    name = 'Google pixel 6'
    print(f"Running in real device:{name}")
    adb_name = '192.168.137.170:5555'
    install_app(adb_name=adb_name, name=name)

    appium_service = AppiumService()
    appium_service.start(args=['--address', '127.0.0.1', '--port', f'{port}'])
    if appium_service.is_running:
        print("Appium server started successfully.")
    do_launch(adb_name=adb_name)
    time.sleep(50)

    
if __name__ == "__main__":
    name = 'cw1'
    avd_name, adb_name, uuid = create_emulator(name,recreate=True)
    print(avd_name, adb_name, uuid)
    exit()
    install_app(adb_name=adb_name, name=name)
    
    # adb_name = '192.168.137.170:5555'

    appium_service = AppiumService()
    appium_service.start(args=['--address', '127.0.0.1', '--port', f'{port}'])
    
    if appium_service.is_running:
        print("Appium server started successfully.")
    #### appium_service.stop()
    while True:
        try:
            do_launch(adb_name=adb_name)
            # do_provider_login()
            # exit()
            do()
        except Exception as e:
            print(e)
            driver.quit()


import concurrent.futures
def create_and_install(name):
    avd_name, adb_name, uuid = create_emulator(name, recreate=True, with_fingerprint=False)
    print(avd_name, adb_name, uuid)
    install_app(adb_name=adb_name, name=name)
if __name__ == "__create_emulator__":
    names = ['sup', 'cw1', 'cw2', 'su1']
    # names = ['cw1']
    # for name in names:
    #     avd_name, adb_name, uuid = create_emulator(name,recreate=True, with_fingerprint=False)
    #     print(avd_name, adb_name, uuid)
    #     install_app(adb_name=adb_name, name=name)   
    with concurrent.futures.ThreadPoolExecutor() as executor:
        futures = [executor.submit(create_and_install, name) for name in names]
        for future in concurrent.futures.as_completed(futures):
            try:
                future.result()
            except Exception as e:
                print(f"Error occurred: {e}") 


if __name__ == "__interactive__":
    name = 'cw2'
    email = name + '@hcms.com'
    avd_name, adb_name, uuid = create_emulator(name, recreate=False)
    print(avd_name, adb_name, uuid)
    install_app(adb_name=adb_name, name=name)
    print("port overwritten to 4723")
    port =4723
    appium_service = AppiumService()
    appium_service.start(args=['--address', '127.0.0.1', '--port', f'{port}'])
    do_launch(adb_name)
    # driver.start_recording_screen()
    try:
        do_provider_login()
        do_login(email=email)
        do_permission()
        # do_booking()
    except Exception as e:
        print(e)
    finally:
        pass
        # # video_base64 = driver.stop_recording_screen()
        # # file name contain date time upto minutes
        # file_name = datetime.now().strftime("%Y%m%d-%H%M")
        # file_path = f"C:\\Users\\Pradeep\\Project\\screenrecords\\{file_name}.mp4"
        # with open(file_path, "wb") as video_file:
        #     video_file.write(base64.b64decode(video_base64))
        # print(f"Video recording saved as {file_path}")
        # driver.quit()
        # appium_service.stop()