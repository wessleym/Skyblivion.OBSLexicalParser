ScriptName TES4ObjectReferenceUtility

Bool Function ContainsItem(ObjectReference objectRef, Form soughtObject) Global
Int numItems = objectRef.GetNumItems()
Int i = 0
While i < numItems
If objectRef.GetNthForm(i) == soughtObject
Return True
EndIf
i += 1
EndWhile
Return False
EndFunction

; Checks if Legacy gamebryo animation is playing
Bool Function IsAnimPlaying(ObjectReference objectRef) Global Native

; Gets the "destroyed" Property
Int Function LegacyGetDestroyed(ObjectReference objectRef) Global Native

Float Function LegacySayTo(ObjectReference objectRef, Actor speakTo, Topic speakTopic, bool value) Global Native

Float Function LegacySay(ObjectReference objectRef, Topic speakTopic, bool value) Global Native