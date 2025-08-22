$device = "emulator-5558"
adb -s $device shell locksettings set-pin 1234
adb -s $device shell locksettings set-pin --old 1234 1234
adb -s $device shell am start -a android.settings.FINGERPRINT_ENROLL
adb -s $device shell input text 1234
# sleep 1 second
Start-Sleep -s 1
adb -s $device shell input keyevent 66
Start-Sleep -s 1
adb -s $device shell input swipe 500 1000 500 500
Start-Sleep -s 1
adb -s $device shell input swipe 500 1000 500 500
Start-Sleep -s 1
adb -s $device shell input swipe 500 1000 500 500
Start-Sleep -s 1
adb -s $device shell input tap 900 2200
Start-Sleep -s 1
adb -s $device emu finger touch 1
Start-Sleep -s 1
adb -s $device emu finger touch 1
Start-Sleep -s 1
adb -s $device emu finger touch 1
Start-Sleep -s 1
adb -s $device emu finger touch 1
Start-Sleep -s 1
adb -s $device shell input tap 900 2200