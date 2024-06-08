; Checks if Legacy gamebryo animation is playing
bool Function IsAnimPlaying() native

Float Function LegacySayTo(Actor speakTo, Topic speakTopic, bool value) native

Float Function LegacySay(Topic speakTopic, bool value) native

Function LegacyStartConversation(Actor speakTo, Topic speakTopic = None) native

; Gets the "destroyed" Property
int Function LegacyGetDestroyed() native

Bool Function ContainsItem(Form soughtObject)
Int numItems = GetNumItems()
Int i = 0
While i < numItems
If GetNthForm(i) == soughtObject
Return True
EndIf
i += 1
EndWhile
Return False
EndFunction