# get command line argument --skip-build
$skipBuild = $args -contains "--skip-build"
$addFingerprint = $args -contains "--fingerprint"
# if --release is passed mode is released unless default is Debug
$mode = "Debug"
$apk_location = ".\src\VisitTracker\bin\Debug\net9.0-android\com.artivis.vm-Signed.apk"
if ($args -contains "--release") {
    $mode = "Release"
    $apk_location = ".\src\VisitTracker\bin\Release\net9.0-android\com.artivis.vm-Signed.apk"
}
if ($skipBuild) {
    echo "Skipping build"
} else {
    echo "Building the app"
    dotnet restore src
    if ($LASTEXITCODE -ne 0) {
        Write-Error "dotnet restore failed"
        exit $LASTEXITCODE
    }
    dotnet build src -c $mode
    if ($LASTEXITCODE -ne 0) {
        Write-Error "dotnet build failed"
        exit $LASTEXITCODE
    }
}
# get all adb device names, loop through each device, and install the apk
$devices = adb devices | Select-String -Pattern "device" | Where-Object { $_ -notmatch "List of devices attached" } | ForEach-Object { $_ -replace "device", "" -replace "\s", "" }
foreach ($device in $devices) {
    echo "Installing app on $device"
    # set geolocation 
    # adb -s $device emu geo fix -0.0932975498808169 51.619814684820646 #docker UK
    # adb -s $device emu geo fix 103.692116708944 1.3468235406004814
    adb -s $device emu geo fix 103.6822123467668 1.3467180260140514 # cysren
    # delete app before installtion
    adb -s $device uninstall com.artivis.vm
    adb -s $device install -r $apk_location
    # grant permissions
    adb -s $device shell pm grant com.artivis.vm android.permission.ACCESS_COARSE_LOCATION
    adb -s $device shell pm grant com.artivis.vm android.permission.ACCESS_FINE_LOCATION
    adb -s $device shell pm grant com.artivis.vm android.permission.ACTIVITY_RECOGNITION
    adb -s $device shell pm grant com.artivis.vm android.permission.POST_NOTIFICATIONS
    adb -s $device shell pm grant com.artivis.vm android.permission.ACCESS_BACKGROUND_LOCATION
    adb -s $device shell pm grant com.artivis.vm android.permission.BATTERY_STATS
    adb -s $device shell pm grant com.artivis.vm android.permission.READ_EXTERNAL_STORAGE
    adb -s $device shell pm grant com.artivis.vm android.permission.WRITE_EXTERNAL_STORAGE
    adb -s $device shell pm grant com.artivis.vm android.permission.RECORD_AUDIO
    adb -s $device shell pm grant com.artivis.vm android.permission.CAMERA
    
    adb -s $device shell dumpsys deviceidle whitelist +com.artivis.vm

    adb -s $device shell am start -a "android.intent.action.MAIN" -c "android.intent.category.LAUNCHER" -n "com.artivis.vm/com.artivis.vm.activity"  
    
}


#################################### do_login.ps1 ####################################





$devices = adb devices | Select-String -Pattern "device" | Where-Object { $_ -notmatch "List of devices attached" } | ForEach-Object { $_ -replace "device", "" -replace "\s", "" }

# function which does tab given x,y floating point so do tab at x*$width, y*$height 
function doTab($x, $y) {
    $x = [math]::Round($x * $width)
    $y = [math]::Round($y * $height)
    adb -s $device shell input tap $x $y
}

# function to press back space
function doBack($time=6) {
    Start-Sleep -s 1
    adb -s $device shell input keyevent 123
    Start-Sleep -s 1
    for ($i = 0; $i -lt $time; $i++) {
        adb -s $device shell input keyevent 67
    }
}

function sendText($text) {
    adb -s $device shell input text $text
}

function minimizeKeyboard(){
    doTab 0.1 0.99
}

function doProviderLogin() {
    doTab 0.5 0.72
    doBack
    sendText "112033"
    minimizeKeyboard
    Start-Sleep -s 1
    doTab 0.5 0.82
    Start-Sleep -s 5

    # confirm provider login
    doTab 0.5 0.6
    Start-Sleep -s 5
}

function doUserLogin($email) {
    # user login
    doTab 0.5 0.525
    doBack(20)
    sendText $email
    minimizeKeyboard
    Start-Sleep -s 1
    doTab 0.5 0.625
    doBack(15)
    sendText "qwer\$#@!"
    minimizeKeyboard
    Start-Sleep -s 1
    doTab 0.5 0.725
    Start-Sleep -s 3
}




foreach ($device in $devices) {
    $name,$other = adb -s $device emu avd name
    $email = "$name@hcms.com"

    $windowSize = adb -s $device shell wm size
    # extract from the string "Physical size: 1080x2340" and get the width and height
    $width = $windowSize -replace "Physical size: (\d+)x(\d+)", '$1'
    $height = $windowSize -replace "Physical size: (\d+)x(\d+)", '$2'
    # echo "Width: $width, Height: $height"
    
    doProviderLogin
    doUserLogin $email
}

