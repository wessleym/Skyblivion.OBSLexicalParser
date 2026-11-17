ScriptName SKYBDispositionUtil

; =============================================================================
; GetDisposition
;
; Returns an NPC's current disposition with the player.
;
; Parameter npc:
;   The Actor whose disposition should be retrieved.
; =============================================================================

Int Function GetDisposition(Actor npc) Global
	Return npc.GetBaseActorValue("Variable01") as Int
EndFunction

; =============================================================================
; ModDisposition
;
; Changes an NPC's disposition with the player by the specified amount and
; returns the new value of the disposition after the modification.
;
; Parameter npc:
;   The Actor whose disposition should be set.
;
; Parameter amount:
;   The amount to add to the disposition. If you want to decrease the
;   disposition, pass a negative value. If the resulting disposition is less
;   than 0, it is set to 0. If it is greater than 100, it is set to 100.
; =============================================================================

Function ModDisposition(Actor npc, Int amount) Global
	npc.ModActorValue("Variable01", amount)
EndFunction

; =============================================================================
; SetDisposition
;
; Sets an NPC's disposition with the player to a specific value and returns the
; new disposition value.
;
; Parameter npc:
;   The Actor whose disposition should be set.
;
; Parameter value:
;   The new disposition value. If this is less than 0, it is set to 0. If it is
;   greater than 100, it is set to 100.
; =============================================================================

Int Function SetDisposition(Actor npc, Int value) Global
	If value < 0
		value = 0
	ElseIf value > 100
		value = 100
	EndIf

	npc.SetActorValue("variable01", value)

	Int newValue = GetDisposition(npc)

	If newValue != value
		Debug.Trace("TES4 WARNING: Disposition not what expected. Expected: " + value + ", Actual: " + newValue)
	EndIf

	Return newValue
EndFunction

; =============================================================================
; SetDispositionNoLessThan
;
; If an NPC's disposition is less than the specified value, sets it to that
; value; otherwise, does nothing.
;
; Parameter npc:
;   The Actor whose disposition should be set.
;
; Parameter value:
;   The minimum disposition value. If the NPC's disposition is less than this,
;   it will be forced to this. Otherwise, it will be untouched. This value will
;   be forced into the inclusive range of 0 to 100.
; =============================================================================

Function SetDispositionNoLessThan(Actor npc, Int value) Global
	If value < 0
		value = 0
	EndIf

	If value > 100
		value = 100
	EndIf

	If GetDisposition(npc) < value
		SetDisposition(npc, value)
	Endif
EndFunction