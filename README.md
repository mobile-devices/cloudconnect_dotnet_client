The Coud Connect C# Wrapper
==========================

A C# wrapper for the Cloud Connect API

Installation
------------
#### With [Nuget Package](http://nuget.org/packages/MD.CloudConnect)
    
    Install-Package MD.CloudConnect

#### Manually
	
Dlls required for your project (available from library directory on github) : 
* MD.CloudConnect
* Newtonsoft.Json (dependencie)

Basic Notification Usage
-----

On your server create an HttpHandler.ashx to receive http notification coming from CloudConnect. Read the body message, and send the Json message to the function 'Notification Decode'. Make a loop on MDData response to retrieve information that you need.
```csharp
	List<MD.CloudConnect.MDData> decodedData = MD.CloudConnect.Notification.Instance.Decode(jsonData);
	foreach (MD.CloudConnect.MDData mdData in decodedData)
    {
        if (mdData.Meta.Event == "track")
        {
            ITracking tacking = mdData.Tracking;

            /** Use tracking.Longitude , tracking.Speed to read data **/
            mylongitude = tracking.Longitude;
            ...

            /** In case where you manage by yourself the data cache, you must
            ** check if field exist in MDData before to access it
            **/

             if(tracking.ContainsField(MD.CloudConnect.FieldDefinition.DIO_IGNITION.Key))   
                myIgnition = tracking.Ignition;
            ...
        }
    }
```

## Case where the library manage data cache for you (Beta feature)

The Cloud Connect notification system only sends updated fields (if a recorded field is identical to the previous one, it is not resend) thus previous state of each fields must be stored. This library helps you by  managing a part of this data cache. You only need to initialize the library with "Field" that you need and a special object MD.CloudConnect.IDataCache.

In the global.asax , Application_Start() :
```csharp
	protected void Application_Start()
    {
        InitializeCloudConnect();
    }

    private void InitializeCloudConnect()
    {
        //List the field that you want in the notification result
        string[] fieldsThatINeed = new string[]
        {
            MD.CloudConnect.FieldDefinition.GPRMC_VALID.Key,
            MD.CloudConnect.FieldDefinition.GPS_SPEED.Key,
            MD.CloudConnect.FieldDefinition.GPS_DIR.Key,
            MD.CloudConnect.FieldDefinition.ODO_FULL.Key,
            MD.CloudConnect.FieldDefinition.DIO_IGNITION.Key,
            MD.CloudConnect.EasyFleet.MDI_DRIVING_JOURNEY.Key,
            MD.CloudConnect.EasyFleet.MDI_IDLE_JOURNEY.Key,
            MD.CloudConnect.EasyFleet.MDI_JOURNEY_TIME.Key
        };

        //initialize field and object (IDataCache)
        MD.CloudConnect.Notification.Instance.Initialize(fieldsThatINeed, MD.CloudConnect.Example.Tools.MyDataCacheRepository.Instance, true);
    }
```
An example of object that you could  implement for IDataCache interface :

```csharp
	public class MyDataCacheRepository : MD.CloudConnect.IDataCache
    {
    	/* Singleton */
        protected static readonly MyDataCacheRepository _instance = new MyDataCacheRepository();
        public static MyDataCacheRepository Instance
        {
            get
            {
                lock (_instance)
                {
                    return _instance;
                }
            }
        }

        static MyDataCacheRepository()
        {

        }

        /* IDataCache */
        public DateTime getHistoryFor(string asset, ITracking data)
        {
            MyModelOfData lastTracking = MyPersistantRepo.GetLastTrackingDataFor(asset);
            
            if(lastTracking != null)
            {
            	data.Speed = lastTracking.Speed;
            	data.IsValid = lastTracking.IsValid;
            	data.Direction = lastTracking.Direction;
				
				/*
				* ...
				*/

            	return lastTracking.Date;
            } else
				return DateTime.MinValue;
        }
    }
```    

As the library does not manage persistance data, you need to fill the "GetHistoryFor" with the last information stored in your database.
The function "GetHistoryFor" will be call only one time per Device (Asset) upon the start after start server when the library
will received a new data for an asset not present in Data cache of the library.

List of fields manage by the library
------------
This first of the library does not implement all fields. here the list of field that we manage for you :
* GPRMC_VALID
* GPS_SPEED
* GPS_DIR
* DIO_IGNITION
* ODO_FULL
* DIO_ALARM
* DRIVER_ID
* DIO_IN_TOR

In the case where you need a field which is not present you can use special function to decode directly your field : 
```csharp
    
    // ID is an optional parameter
    public static FieldDetail MY_FIELD = new FieldDetail() { Key = "MY_FIELD", Id = 212 };
    
    ITracking tacking = mdData.Tracking;

    if(tracking.ContainsField(MY_FIELD.Key))
        mypersonalData = tracking.GetFieldAsInt(MY_FIELD.Key);
    // you have also a function for string and boolean
    // GetFieldAsString
    // GetFieldAsBool
``` 

Contributing
------------
In the spirit of [free software](http://www.fsf.org/licensing/essays/free-sw.html), **everyone** is encouraged to help improve this project.

Here are some ways *you* can contribute:

* by using alpha, beta, and prerelease versions
* by reporting bugs
* by suggesting new features
* by writing or editing documentation
* by writing specifications
* by writing code (**no patch is too small**: fix typos, add comments, clean up inconsistent whitespace)
* by refactoring code
* by closing [issues](http://github.com/mobile-devices/cloudconnect_dotnet_client/issues)
* by reviewing patches

All contributors will be added to the [HISTORY](https://github.com/mobile-devices/cloud_connect_dotnet_client/blob/master/HISTORY.md)
file and will receive the respect and gratitude of the community.

Submitting an Issue
-------------------
We use the [GitHub issue tracker](http://github.com/mobile-devices/cloudconnect_dotnet_client/issues) to track bugs and
features. Before submitting a bug report or feature request, check to make sure it hasn't already
been submitted. You can indicate support for an existing issuse by voting it up. When submitting a
bug report, please include a [Gist](http://gist.github.com/) that includes a stack trace and any
details that may be necessary to reproduce the bug, including your .net Version and
operating system. Ideally, a bug report should include a pull request with failing specs.