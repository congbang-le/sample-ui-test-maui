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



