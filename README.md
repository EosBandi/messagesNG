# messagesNG #
NextGen messages tab for Mission Planner

## Prelude ##
The last time I checked the Ardupilot source code, there were more than a thousand different messages that could be sent from the flight controller to the GCS. Some of these messages contain essential
notifications and warnings regarding flight safety. However, in Mission Planner, these messages are poured into a simple text box, and some of them with high severity are displayed in the HUD.
The issue is that in the messages textbox, you cannot differentiate an information message from a Critical warning, and since, in some situations, Ardupilot can be very chatty, high-severity messages in the HUD can be displayed 
only for one or two seconds. I wrote a replacement for the current messages tab to overcome these limitations and provide a valuable tool for GCS operators.

## Design and Target audience ##
`messagesNG` is primarily targeted for large or multi-screen GCS installations. It can be used on a small/single screen, but the limited screen real estate will impact its usability. Some of the functions were added
to help long-range automated flight operations, where the operator has time and attention to the messages and can act on them.

## Functions ##
* Incoming messages are colored according to the message severity
* GCS-generated messages are added to the message box for retention
* Long messages transferred in multiple mavlink packets are reassembled and handled as a single message
* All messages are actionable (currently, the operator can delete a message or put it to an ignore list to hide upcoming occurrences, future updates to call up descriptions, help, or checklist for a given message)
* Messages display can be filtered for severity or severity range
* Repeated messages can be "folded" to show the number of occurrences and the last occurrence time.
* Displayed messages can be sorted by time or severity and time
* Bulk delete of messages of a given severity
* Messages display can detached from the main GCS window and moved/resized
* Text size can be changed on the fly (CTRL+MouseScrollWheel)
* Implemented fully in a plugin; no need to change Mission Planner code
 
