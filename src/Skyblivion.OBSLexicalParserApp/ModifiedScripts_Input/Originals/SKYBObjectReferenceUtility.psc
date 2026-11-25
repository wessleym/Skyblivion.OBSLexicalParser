ScriptName SKYBObjectReferenceUtility

Import Math

Bool Function ContainsItem(ObjectReference objectRef, Form soughtObject) Global Native

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