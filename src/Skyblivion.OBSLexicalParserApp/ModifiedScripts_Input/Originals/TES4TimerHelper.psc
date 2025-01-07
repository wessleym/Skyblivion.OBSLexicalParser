;/ Decompiled by Champollion V1.0.1
Source   : TES4TimerHelper.psc
Modified : 2017-02-18 19:42:56
Compiled : 2017-07-13 01:20:58
User     : jenkins
Computer : vps409760
/;
scriptName TES4TimerHelper extends Quest

import Math
import StringUtil
import Debug

string function d2h( int d, bool bWith0x = true ) global
  string digits = "0123456789ABCDEF"
  string hex
  int shifted = 0
  while shifted < 32
    hex = GetNthChar( digits, LogicalAnd( 0xF, d ) ) + hex
    d = RightShift( d, 4 )
    shifted += 4
  endwhile
  if bWith0x
    hex = "0x" + hex
  endif
  return hex
endfunction

;-- Properties --------------------------------------
faction[] property CrimeFactions auto
Float property timer = 0.0 auto
bool property started = false auto

;-- Variables ---------------------------------------

;-- Functions ---------------------------------------

Int function GetPassedGameDays() global

    Float GameTime = utility.GetCurrentGameTime()
    Float GameDaysPassed = math.Floor(GameTime) as Float
    return GameDaysPassed as Int
endFunction

Int function GetDayOfWeek()

    Int GameDaysPassed = TES4TimerHelper.GetPassedGameDays()
    return GameDaysPassed % 7
endFunction

function Rotate(ObjectReference MyObject, Float LocalX, Float LocalY, Float LocalZ)

    Float AngleX = LocalX * math.Cos(LocalZ) + LocalY * math.Sin(LocalZ)
    Float AngleY = LocalY * math.Cos(LocalZ) - LocalX * math.Sin(LocalZ)
    MyObject.SetAngle(AngleX, AngleY, LocalZ)
endFunction

Int function getCrimeGold()

    Int index = 0
    Int crimeGold = 0
    while index < CrimeFactions.length
        crimeGold += CrimeFactions[index].getCrimeGold()
    endWhile
    return crimeGold
endFunction

Float function legacySay(ObjectReference speaker, Topic speakingTopic, Actor speakAs, Bool speakInHead)
    string speakingTopic_s = d2h(speakingTopic.GetFormID())
    if (speakAs == None)
        Debug.Trace(speaker + " Say " + speakingTopic_s)
        speaker.Say(speakingTopic, None, speakInHead)
    else
        ConsoleUtil.SetSelectedReference(speaker)
        string speakAs_s = d2h(speakAs.GetFormID())
        Debug.Trace(speaker + " SayTo " + speakAs_s + " " + speakingTopic_s)
        ConsoleUtil.ExecuteCommand("SayTo " + speakAs_s + " " + speakingTopic_s)
    endif
    return 15.0
endFunction
