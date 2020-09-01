ScriptName TES4Timer extends ObjectReference Conditional
Float Property StartTime_p Auto Conditional
Float Property CurrentTime_p Auto Conditional
Float Property StartDay_p Auto Conditional
Float Property CurrentDay_p Auto Conditional
Float Property CurrentTime2_p Auto Conditional
Int Property Doonce_p Auto Conditional
Int Property Doonce1_p Auto Conditional
GlobalVariable Property GameHour_p Auto Conditional
Quest Property MS11_p Auto Conditional
GlobalVariable Property GameDaysPassed_p Auto Conditional
State ActiveState
Event OnBeginState()
self.OnUpdate()
EndEvent
Event OnUpdate()
If((GameHour_p.GetValueInt() <= 12))
CurrentTime_p = GameHour_p.GetValueInt() as Float
Else
CurrentTime_p = (GameHour_p.GetValueInt() - 12) as Float
EndIf
If((((MS11_p.GetStage() >= 90) && (MS11_p.GetStage() < 100)) && (Doonce_p == 0)))
If((GameHour_p.GetValueInt() <= 12))
StartTime_p = (GameHour_p.GetValueInt() + 1) as Float
Doonce_p = 1
Else
StartTime_p = (GameHour_p.GetValueInt() - 11) as Float
Doonce_p = 1
EndIf
EndIf
If((CurrentTime_p >= StartTime_p))
MS11_p.SetStage(150)
EndIf
If((((MS11_p.GetStage() >= 90) && (MS11_p.GetStage() < 100)) && (CurrentDay_p == 0)))
CurrentDay_p = GameDaysPassed_p.GetValueInt() as Float
CurrentTime2_p = GameHour_p.GetValueInt() as Float
EndIf
If(((MS11_p.GetStage() >= 90) && (MS11_p.GetStage() < 100)))
If((GameDaysPassed_p.GetValueInt() >= (CurrentDay_p + 1)))
If((GameHour_p.GetValueInt() >= CurrentTime2_p))
MS11_p.SetStage(150)
EndIf
EndIf
EndIf
self.RegisterForSingleUpdate(0.4)
EndEvent
EndState
Auto State InactiveState
EndState
Event OnCellAttach()
self.GotoState("ActiveState")
EndEvent
Event OnCellDetach()
self.GotoState("InactiveState")
EndEvent