import subprocess
import time


def run_ps(ps_command):
    result = subprocess.run(["powershell", "-NoProfile", "-Command", ps_command], capture_output=True, text=True)
    return result.stdout, result.stderr

def run_ps_detached(ps_command):
    # Use Popen to run the command in detached mode
    process = subprocess.Popen(
        ["powershell", "-NoProfile", "-Command", ps_command],
        creationflags=subprocess.CREATE_NEW_CONSOLE,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        text=True
    )
    return process

def get_avd_device_list():
    # get emulator list and check if device exisit
    # list_command = 'avdmanager list avd'
    list_command = '''
    $avds = & avdmanager list avd | Select-String -Pattern "Name: (.*)" | ForEach-Object { $_.Matches[0].Groups[1].Value }
    $avds
    '''
    list_output,_ = run_ps(list_command)
    device_list  = list_output.strip().split('\n')
    return device_list

def get_adb_device_list():
    ps_command = '''
    adb devices | Select-String -Pattern "device$" | ForEach-Object { $_.Line.Split("`t")[0] }
    '''
    result = subprocess.run(["powershell", "-NoProfile", "-Command", ps_command], capture_output=True, text=True)
    if result.returncode == 0:
        device_names = result.stdout.strip().split('\n')
        return device_names
    else:
        print("Error:", result.stderr)
        return []

def get_adb_device(avd_name):
    # List connected devices
    devices = get_adb_device_list()
    for device in devices:
        output,_ = run_ps(f"adb -s {device} emu avd name")
        # get first line of output
        output = output.split('\n')[0]
        if output == avd_name:
            print(f"Found {avd_name} device: {device}")
            return device
    return None

def enable_dev(adb_name):
    output,_ = run_ps(f'adb -s {adb_name} shell settings put global development_settings_enabled 1')
    print(f'Developer options enabled for {adb_name}')
    

def send_fingerprint(adb_name):
    run_ps(f'adb -s {adb_name} emu finger touch 2')

def add_fingerprint(adb_name):
    command_list = [ f'adb -s {adb_name} shell locksettings set-pin 1234',
        f'adb -s {adb_name} shell am start -a android.settings.FINGERPRINT_ENROLL',
        f'adb -s {adb_name} shell input text 1234',
        f'adb -s {adb_name} shell input keyevent 66',
        f'adb -s {adb_name} shell input swipe 500 1000 500 500',
        f'adb -s {adb_name} shell input swipe 500 1000 500 500',
        f'adb -s {adb_name} shell input swipe 500 1000 500 500',
        f'adb -s {adb_name} shell input tap 900 2200',
        f'adb -s {adb_name} emu finger touch 1',
        f'adb -s {adb_name} emu finger touch 1',
        f'adb -s {adb_name} emu finger touch 1',
        f'adb -s {adb_name} emu finger touch 1',
        f'adb -s {adb_name} emu finger touch 1',
        f'adb -s {adb_name} emu finger touch 1',
        f'adb -s {adb_name} shell input tap 900 2200'
        ]
    for command in command_list:
        run_ps(command)
        print(f'Executed: {command}')
        time.sleep(1)
    print(f'Fingerprint added to {adb_name}')

def get_uuid(adb_name=None, avd_name=None):
    if avd_name:
        adb_name = get_adb_device(avd_name)
    ps_command = f'''
    adb -s {adb_name} shell settings get secure android_id
    '''
    result = subprocess.run(["powershell", "-NoProfile", "-Command", ps_command], capture_output=True, text=True)
    if result.returncode == 0:
        return result.stdout.strip()
    else:
        print("Error:", result.stderr)
        return None

def create_emulator(name, recreate=False, with_fingerprint=False):
    device_list = get_avd_device_list()
    if name in device_list:
        print(f'{name} already exists')
        if not recreate:
            print(f'Using existing {name}')
            start_command = f'emulator -avd {name}'
            run_ps_detached(start_command)
            time.sleep(40)
            return name, get_adb_device(name), get_uuid(avd_name=name)
        adb_name= get_adb_device(name)
        if adb_name:
            print(f'Stopping {name}')
            stop_command = f'adb -s {adb_name} emu kill'
            run_ps(stop_command)
        print(f'Deleting and recreating {name}')
        delete_command = f'avdmanager delete avd -n {name}'
        run_ps(delete_command)
        print(f'{name} deleted')
    # to be already installed: # sdkmanager "platform-tools" "emulator" "system-images;android-34;google_apis;x86_64"
    create_command = f'avdmanager create avd -n {name} -k "system-images;android-34;google_apis;x86_64" -d pixel_5 --force'
    _,error = run_ps(create_command)
    if error:
        print(f'Error creating {name} {error}')
    print(f'{name} created successfully')
    # start_command = f'emulator -avd {name} -timezone "Asia/Singapore" -skin 780x1800 '
    start_command = f'emulator -avd {name} -timezone "Asia/Singapore"'
    run_ps_detached(start_command)
    time.sleep(40)
    adb_name = get_adb_device(name)
    if with_fingerprint:
        add_fingerprint(adb_name)
    return name, get_adb_device(name), get_uuid(avd_name=name)

def _add_permission(adb_name):
    permission_list = [
        "android.permission.ACCESS_COARSE_LOCATION",
        "android.permission.ACCESS_FINE_LOCATION",
        "android.permission.ACCESS_MOCK_LOCATION",
        "android.permission.ACCESS_NETWORK_STATE",
        "android.permission.ACTIVITY_RECOGNITION",
        "android.permission.POST_NOTIFICATIONS",
        "android.permission.NETWORK_SETTINGS",
        "android.permission.ACCESS_BACKGROUND_LOCATION",
        "android.permission.FOREGROUND_SERVICE",
        "android.permission.FOREGROUND_SERVICE_LOCATION",
        "android.permission.INTERNET",
        "android.permission.BATTERY_STATS",
        "android.permission.RECEIVE_BOOT_COMPLETED",
        "android.permission.CHANGE_NETWORK_STATE",
        "android.permission.MODIFY_AUDIO_SETTINGS",
        "android.permission.READ_EXTERNAL_STORAGE",
        "android.permission.WRITE_EXTERNAL_STORAGE",
        "android.permission.MANAGE_EXTERNAL_STORAGE",
        "android.permission.RECORD_AUDIO",
        "android.permission.CAMERA",
        "android.permission.USE_FINGERPRINT",
        "android.permission.USE_BIOMETRIC",
    ]
    for permission in permission_list:
        command = f'adb -s {adb_name} shell pm grant com.artivis.vm {permission}'
        run_ps(command)
    command = f'adb -s {adb_name} shell dumpsys deviceidle whitelist +com.artivis.vm'
    run_ps(command)
    print(f'Permission {permission} added to {adb_name}')

def install_app(adb_name, name):
    if not adb_name:
        print(f"Emulator device for {name} not found.")
        return
    echo_command = f'echo "Installing app on {adb_name} || {name}"'
    run_ps(echo_command)
    # set geolocation
    # geo_command = f'adb -s {adb_name} emu geo fix 103.692116708944 1.3468235406004814' # 851 home
    geo_command = f'adb -s {adb_name} emu geo fix 103.6822123467668 1.3467180260140514' # cysren
    run_ps(geo_command)
    # delete app before installtion
    uninstall_command = f'adb -s {adb_name} uninstall com.artivis.vm'
    run_ps(uninstall_command)
    # install app
    install_command = f'adb -s {adb_name} install -r .\\src\\VisitTracker\\bin\\Debug\\net8.0-android\\com.artivis.vm-Signed.apk'
    out,error = run_ps(install_command)
    print(error)
    ### start app
    # start_command = f'adb -s {name} shell am start -a "android.intent.action.MAIN" -c "android.intent.category.LAUNCHER" -n "com.artivis.vm/com.artivis.vm.activity"'
    # run_ps(start_command)
    # print(f'App installed on {name}')
    _add_permission(adb_name)
    
if __name__ == '__main__':
    pass
    # add_fingerprint('emulator-5556')
    # create_emulator('cw1')
    # install_app('cw1')
    