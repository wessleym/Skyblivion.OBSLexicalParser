ScriptName SKYBObjectReferenceUtility

Import Math

Bool Function ContainsItem1(ObjectReference objectRef, Form soughtObject) Global
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

Bool Function ContainsItem2(ObjectReference objectRef, Form soughtObject) Global Native

;The below function temporarily compares the output of the old and new ContainsItem functions.
;Eventually, I will need to update the SKSE plugin (renaming ContainsItem2 to ContainsItem) and use this line here:
;Bool Function ContainsItem(ObjectReference objectRef, Form soughtObject) Global Native
Bool Function ContainsItem(ObjectReference objectRef, Form soughtObject) Global
    Float time1 = Utility.GetCurrentRealTime()
    Bool contains1 = ContainsItem1(objectRef, soughtObject)
    Float time2 = Utility.GetCurrentRealTime()
    Bool contains2 = ContainsItem2(objectRef, soughtObject)
    Float time3 = Utility.GetCurrentRealTime()
    String msg = "ContainsItem 1 (" + contains1 + ", " + (time2 - time1) + " secs) and 2 (" + contains2 + ", " + (time3 - time2) + " secs) "
    If (contains1 == contains2)
        msg += "matched"
    Else
        msg += "did not match"
    EndIf
    msg += "."
    Debug.Notification(msg)
    return contains1
EndFunction

Function LegacyRotate(ObjectReference objectRef, Float x, Float y, Float z) Global
    Float angleX = x * math.Cos(z) - y * math.Sin(z)
    Float angleY = x * math.Cos(z) + y * math.Sin(z)
    objectRef.SetAngle(angleX, angleY, z)
EndFunction

; Checks if Legacy gamebryo animation is playing
Bool Function IsAnimPlaying(ObjectReference objectRef) Global Native

; Gets the "destroyed" Property
Int Function LegacyGetDestroyed(ObjectReference objectRef) Global Native

Float Function LegacySayTo(ObjectReference objectRef, Actor speakTo, Topic speakTopic, bool value) Global Native

Float Function LegacySay(ObjectReference objectRef, Topic speakTopic, bool value) Global Native