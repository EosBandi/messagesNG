# messagesNG #
NextGen messages tab for Mission Planner

## Prelude ##
The last time I checked the Ardupilot source code, I found more than a thousand different messages that could be sent from the flight controller to the Ground Control Station (GCS). Some of these messages contain essential notifications and warnings related to flight safety. However, in Mission Planner, these messages are simply displayed in a text box, and some with high severity are shown in the Heads-Up Display (HUD).

The issue is that in the messages text box, it's challenging to differentiate an information message from a critical warning. And because Ardupilot can be quite chatty in some situations, high-severity messages in the HUD may only be displayed for one or two seconds.

To address these limitations, I have developed a replacement for the current messages tab. This aims to overcome these issues and provide a more valuable tool for Ground Control Station operators."

## Design and Target audience ##
`messagesNG` is primarily designed for large or multi-screen Ground Control Station (GCS) installations. While it can be utilized on a small or single screen, the limited screen real estate may impact its usability. Certain functions have been incorporated to assist in long-range automated flight operations. These are particularly beneficial in scenarios where the operator has the time and attention to focus on messages and take appropriate actions.

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

## Usage ##
The messagesNG window consists of two sections. The larger upper section displays the messages, while control buttons are located at the bottom.\
![image](https://github.com/EosBandi/messagesNG/assets/11500559/7160cf1b-0091-46da-b68e-72a15ec194e6)

The left group of buttons controls the severity level of the displayed messages. Additionally, the buttons indicate the number of actual messages in the given severity.
* All - All messages
* Err - Messages with Mavlink Severity 0,1,2,3
* E/W - Error and Warning messages with Mavlink severity 0,1,2,3,4,5
* Wrn - Warning messages with mavlink severity 4 and 5
* INFO - Info and Debug messages with mavlink severity 6 and 7

The trashcan button deletes all the displayed messages. 

You can change the font size (zoom level) by clicking on the messages display and using a CTRL+ScrollWheel.

Right-click on a message brings up the details window. \
![image](https://github.com/EosBandi/messagesNG/assets/11500559/bad2ebf4-c79d-4155-973f-156787896315)

Here, you can delete the single message (including all occurrences) or put this message on the ignore list. Putting a message on the ignore list means it will be suppressed (not shown) until the next restart or until you delete the ignore list from the settings (see below). Note: Error messages on the ignore list will be displayed in the HUD.

## Settings ##
Clicking on the gear icon at the lower left corner brings up the settings window. \
![image](https://github.com/EosBandi/messagesNG/assets/11500559/ea2c8058-5de2-47c5-95d0-aba8241ba45b) 

### Display Format ###
Sets how the messages are displayed
* 'Folded' - The same messages are displayed once, with a counter. Latest message at the top
* 'FoldedWithSeverity' - Same as folded, but messages are sorted by severity then by time
* 'Unfolded' - All messages are displayed in the order they were received, without grouping
### Maximum number of lines ###
Set the maximum number of lines kept in the message box; when it is reached, oldest message will be deleted. Decreasing it helps slow machines to cut update times
### Color Mode ###
Dark / Light display color for the messages box
### Show MAVLink Severity ###
Show mavlink severity in brackets after the time
### Allow Delete All ###
Enable the Delete button when ALL messages are displayed
### Clear ignore list ###
Clear the Ignored messages list; they will be displayed again
### Detach messages tab ###
The messages tab will be shown in a separate window. You can reattach the tab by unchecking this option or by closing the form.
### Messages zoom level ###
The zoom level of the textbox displaying the messages min 50% max 250%

## Installation ##
Go to releases and download the latest binary release pack. Unpack it to your Program Files(x86)\Mission Planner\plugins folder. The next start, you will have messagesNG as a new tab.

### Windows security issues. ###
Since a latest windows update, it seems that dll files downloaded from github or other sources are marked insecure by default. This prevents loading downloaded plugins.

To solve this issue you have to install plugin files in the following way.

- Download the files from github
- Extract the archive to a separated directory
- Right Click and select properties for every dll files.
- If you see the Security warning at the bottom of the window, select unblock and press OK.
- When done, copy all files to the Mission Planner/plugins folder.

![image](https://github.com/EosBandi/messagesNG/assets/11500559/cc9a806d-1806-487a-b460-1b304dfd9ad8)

## Build your own ##
Make sure that you have a working Mission Planner build environment.
Clone the repository into a directory that is the same level as your Mission Planner repository, not into your Mission Planner source folder.
Add the project from the MPMessagesControl to the Extlibs project folder in MP and MessagesNGplugin to the Plugins project folder.
Right-click on MPMessagesControl project and build, then do the same with MessagesNGplugin, THEN build and start the Mission Planner project.

 
