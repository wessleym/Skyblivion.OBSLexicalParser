ScriptName SKYBDispositionUtil

; =============================================================================
; GetDisposition
;
; Returns an NPC's current disposition with the player.
;
; Parameter npc:
;   The Actor whose disposition should be retrieved.
; =============================================================================

int function GetDisposition(Actor npc) global
  return npc.GetActorValue("variable01") as int
endFunction

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

int function SetDisposition(Actor npc, int value) global
  if value < 0
    value = 0
  elseif value > 100
    value = 100
  endif

Debug.Trace("SetDisposition is about to set variable01 to " + value)

  npc.SetActorValue("variable01", value)

  int newValue = GetDisposition(npc)

Debug.Trace("GetDisposition returned " + newValue)

  if newValue != value
    Debug.Trace("TES4 WARNING: Disposition not what expected. Expected: " + value + ", Actual: " + newValue)
  endif

  return newValue
endFunction

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

function SetDispositionNoLessThan(Actor npc, int value) global
  if value < 0
    value = 0
  endif

  if value > 100
    value = 100
  endif

  if GetDisposition(npc) < value
    SetDisposition(npc, value)
  endif
endFunction

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

int function ModDisposition(Actor npc, int amount) global
  int currentDisposition = GetDisposition(npc)
  int newDisposition = currentDisposition + amount
  Debug.Trace("Current Disposition: "+currentDisposition)
  Debug.Trace("Calling SetDisposition("+newDisposition+")")
  return SetDisposition(npc, newDisposition)
endFunction