// Copyright (c) Microsoft Corporation.  All rights reserved.

using System.Collections.Generic;
using ColorSyntax.Common;

namespace ColorSyntax.Compilation.Languages
{
    public class Arduino : ILanguage
    {
        public string Id
        {
            get { return LanguageId.Arduino; }
        }

        public string Name
        {
            get { return "Arduino"; }
        }

        public string CssClassName
        {
            get { return "arduino"; }
        }

        public string FirstLinePattern
        {
            get
            {
                return null;
            }
        }

        public IList<LanguageRule> Rules
        {
            get
            {
                return new List<LanguageRule>
                {
                    new LanguageRule(
                        @"/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/",
                        new Dictionary<int, string>
                        {
                            {0, ScopeName.Comment},
                        }),
                    new LanguageRule(
                        @"(//.*?)\r?$",
                        new Dictionary<int, string>
                        {
                            {1, ScopeName.Comment}
                        }),
                    new LanguageRule(
                        @"(?s)(""[^\n]*?(?<!\\)"")",
                        new Dictionary<int, string>
                        {
                            {0, ScopeName.String}
                        }),
                    new LanguageRule(
                        @"\b(abstract|array|auto|bool|byte|boolean|break|case|catch|char|ref class|class|const|const_cast|continue|default|delegate|delete|deprecated|dllexport|dllimport|do|double|dynamic_cast|each|else|enum|event|explicit|export|extern|false|float|for|friend|friend_as|gcnew|generic|goto|if|in|initonly|inline|int|interface|literal|long|mutable|naked|namespace|new|noinline|noreturn|nothrow|novtable|nullptr|operator|private|property|protected|public|register|reinterpret_cast|return|safecast|sealed|selectany|short|signed|sizeof|static|static_cast|ref struct|struct|switch|template|this|thread|throw|true|try|typedef|typeid|typename|union|unsigned|using|uuid|value|virtual|void|volatile|wchar_t|while|word|string|String|array)\b",
                        new Dictionary<int, string>
                        {
                            {0, ScopeName.Keyword},
                        }),
                    new LanguageRule(
                        @"\b((0b[01\']+)|(0(x|X)[0123456789aAbBcCdDeEfF]+)|([\d']*\.[\d'])|([\d'])+)(u|U|l|L|ul|UL|f|F|b|B|ll|LL|e)?\b",
                        new Dictionary<int, string>
                        {
                            { 0, ScopeName.Number }
                        }),
                    new LanguageRule(
                        @"^\s*(\#if|\#else|\#elif|\#endif|\#define|\#undef|\#warning|\#error|\#line|\#pragme|\#ifdef|\#ifnder|\#include|\#region|\#endregion).*?$",
                        new Dictionary<int, string>
                        {
                            { 1, ScopeName.PreprocessorKeyword }
                        }),
                    new LanguageRule(
                        @"\b(setup|loop|while|catch|for|if|do|goto|try|switch|case|else|default|break|continue|return|KeyboardController|MouseController|SoftwareSerial|EthernetServer|EthernetClient|LiquidCrystal|RobotControl|GSMVoiceCall|EthernetUDP|EsploraTFT|HttpClient|RobotMotor|WiFiClient|GSMScanner|FileSystem|Scheduler|GSMServer|YunClient|YunServer|IPAddress|GSMClient|GSMModem|Keyboard|Ethernet|Console|GSMBand|Esplora|Stepper|Process|WiFiUDP|GSM_SMS|Mailbox|USBHost|Firmata|PImage|Client|Server|GSMPIN|FileIO|Bridge|Serial|EEPROM|Stream|Mouse|Audio|Servo|File|Task|GPRS|WiFi|Wire|TFT|GSM|SPI|SD|runShellCommandAsynchronously|analogWriteResolution|retrieveCallingNumber|printFirmwareVersion|analogReadResolution|sendDigitalPortPair|noListenOnLocalhost|readJoystickButton|setFirmwareVersion|readJoystickSwitch|scrollDisplayRight|getVoiceCallStatus|scrollDisplayLeft|writeMicroseconds|delayMicroseconds|beginTransmission|getSignalStrength|runAsynchronously|getAsynchronously|listenOnLocalhost|getCurrentCarrier|readAccelerometer|messageAvailable|sendDigitalPorts|lineFollowConfig|countryNameWrite|runShellCommand|readStringUntil|rewindDirectory|readTemperature|setClockDivider|readLightSensor|endTransmission|analogReference|detachInterrupt|countryNameRead|attachInterrupt|encryptionType|readBytesUntil|robotNameWrite|readMicrophone|robotNameRead|cityNameWrite|userNameWrite|readJoystickY|readJoystickX|mouseReleased|openNextFile|scanNetworks|noInterrupts|digitalWrite|beginSpeaker|mousePressed|isActionDone|mouseDragged|displayLogos|noAutoscroll|addParameter|remoteNumber|getModifiers|keyboardRead|userNameRead|waitContinue|processInput|parseCommand|printVersion|readNetworks|writeMessage|blinkVersion|cityNameRead|readMessage|setDataMode|parsePacket|isListening|setBitOrder|beginPacket|isDirectory|motorsWrite|drawCompass|digitalRead|clearScreen|serialEvent|rightToLeft|setTextSize|leftToRight|requestFrom|keyReleased|compassRead|analogWrite|interrupts|WiFiServer|disconnect|playMelody|parseFloat|autoscroll|getPINUsed|setPINUsed|setTimeout|sendAnalog|readSlider|analogRead|beginWrite|createChar|motorsStop|keyPressed|tempoWrite|readButton|subnetMask|debugPrint|macAddress|writeGreen|randomSeed|attachGPRS|readString|sendString|remotePort|releaseAll|mouseMoved|background|getXChange|getYChange|answerCall|getResult|voiceCall|endPacket|constrain|getSocket|writeJSON|getButton|available|connected|findUntil|readBytes|exitValue|readGreen|writeBlue|startLoop|IPAddress|isPressed|sendSysex|pauseMode|gatewayIP|setCursor|getOemKey|tuneWrite|noDisplay|loadImage|switchPIN|onRequest|onReceive|changePIN|playFile|noBuffer|parseInt|overflow|checkPIN|knobRead|beginTFT|bitClear|updateIR|bitWrite|position|writeRGB|highByte|writeRed|setSpeed|readBlue|noStroke|remoteIP|transfer|shutdown|hangCall|beginSMS|endWrite|attached|maintain|noCursor|checkReg|checkPUK|shiftOut|isValid|shiftIn|pulseIn|connect|println|localIP|pinMode|getIMEI|display|noBlink|process|getBand|running|beginSD|drawBMP|lowByte|setBand|release|bitRead|prepare|pointTo|readRed|setMode|noFill|remove|listen|stroke|detach|attach|noTone|exists|buffer|height|bitSet|circle|config|cursor|random|IRread|setDNS|endSMS|getKey|micros|millis|begin|print|write|ready|flush|width|isPIN|blink|clear|press|mkdir|rmdir|close|point|yield|image|BSSID|click|delay|read|text|move|peek|beep|rect|line|open|seek|fill|size|turn|stop|home|find|step|tone|sqrt|RSSI|SSID|end|bit|tan|cos|sin|pow|map|abs|max|min|get|run|put)\b",
                        new Dictionary<int, string>
                        {
                            {0, ScopeName.BuiltinFunction},
                        }),
                    new LanguageRule(
                        @"\b(DIGITAL_MESSAGE|FIRMATA_STRING|ANALOG_MESSAGE|REPORT_DIGITAL|REPORT_ANALOG|INPUT_PULLUP|INPUT_PULLDOWN16|SET_PIN_MODE|INTERNAL2V56|SYSTEM_RESET|LED_BUILTIN|INTERNAL1V1|SYSEX_START|INTERNAL|EXTERNAL|DEFAULT|OUTPUT|INPUT|HIGH|LOW|OUTPUT_OPENDRAIN|WAKEUP_PULLUP|WAKEUP_PULLDOWN|SPECIAL|FUNCTION_[0-4]|PI|HALF_PI|TWO_PI|DEG_TO_RAD|RAD_TO_DEG|EULER|SERIAL|DISPLAY|LSBFIRST|MSBFIRST|RISING|FALLING|CHANGE|ONLOW|ONHIGH|ONLOW_WE|ONHIGH_WE|TIM_EDGE|TIM_LEVEL|TIM_SINGLE|TIM_LOOP)\b",
                        new Dictionary<int, string>
                        {
                            {0, ScopeName.ControlKeyword},
                        })
                };
            }
        }

        public bool HasAlias(string lang)
        {
            switch (lang.ToLower())
            {
                case "arduino":
                    return true;

                default:
                    return false;
            }
        }
        string[] ILanguage.Aliases => new string[] { "arduino" };
        public override string ToString()
        {
            return Name;
        }
    }
}
