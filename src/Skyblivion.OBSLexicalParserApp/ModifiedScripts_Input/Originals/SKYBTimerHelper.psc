;/ Decompiled by Champollion V1.0.1
Source   : TES4TimerHelper.psc
Modified : 2017-02-18 19:42:56
Compiled : 2017-07-13 01:20:58
User     : jenkins
Computer : vps409760
/;
scriptName SKYBTimerHelper extends Quest

;-- Properties --------------------------------------
Float property timer = 0.0 auto
bool property started = false auto

;-- Variables ---------------------------------------

;-- Functions ---------------------------------------

Int function GetPassedGameDays() global
    Float GameTime = utility.GetCurrentGameTime()
    Float GameDaysPassed = math.Floor(GameTime) as Float
    return GameDaysPassed as Int
endFunction

Int function GetDayOfWeek() global
    Int GameDaysPassed = SKYBTimerHelper.GetPassedGameDays()
    return GameDaysPassed % 7
endFunction
