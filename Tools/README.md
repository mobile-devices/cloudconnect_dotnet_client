The Cloud Connect Developer Tools
==========================

Notification Sender
------------
## Require

.Net framework 4.0 or greater 

## Description
    
This tools simulate notification that you can receive on a specific url. Data format are Json. There are some limitation with this tools. You can only simulate notification for one device on a specific day and only for tracking data.

## Configure

Edit the configuration file name "MD.DevTools.NotificationSender.exe.config" with text editor. You must change:

* YOUR_IMEI value by the imei of you device
* enter the date that you want format like that "yyyy-MM-dd"
* choose the number of item that you want per page : "ItemsPerPage"
* "PauseBetweenRequest" is a boolean "True" or "False", this parameter is use to make a "sleep" between each request; you need to press enter to continue between each request
* YOUR_URL is the url where you want to receive notification