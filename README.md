The Coud Connect C# Wrapper
==========================

A C# wrapper for the Cloud Connect API

Installation
------------
#### With [Nuget Package](http://nuget.org/packages/MD.CloudConnect)
    
    Install-Package MD.CloudConnect

#### Manualy
	
	Reference in your project Dlls : 
		* MD.CloudConnect
		* Newtonsoft.Json (dependencie)

Notification Usage
-----

On your website create HttpHandler.ashx to receive http notification. Send the Json message to the function 'Notification Decode' and make loop on MDData response to retrieve information that you need.

	List<MD.CloudConnect.MDData> decodedData = MD.CloudConnect.Notification.Instance.Decode(data);
	foreach (MD.CloudConnect.MDData mdData in decodedData)
    {
        if (mdData.Meta.Event == "track")
        {
            ITracking tacking = mdData.Tracking;

            /** Use tracking.Longitude , tracking.Speed to read data **/
        }
    }


## Case where the library manage data cache (Beta)

The Cloud Connect notification will send to you only field updated. You must keep in memory the previous state of each fields that you use. In this case, the Cloud Connect library can manage for you a part of this data cache.
You only need to initialize the library with "Field" that you need and on object which is implemented the interface MD.CloudConnect.IDataCache.

In the global.asax , Application_Start() :

	protected void Application_Start()
    {
        InitializeCloudConnect();
    }

    private void InitializeCloudConnect()
    {
        //List the field that you want in the notification result
        string[] fieldsThatIWould = new string[]
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
        MD.CloudConnect.Notification.Instance.Initialize(fieldsThatIWould, MD.CloudConnect.Example.Tools.MyDataCacheRepository.Instance, true);
    }

An example of object that you could  implement for IDataCache interface :

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

The library does not manage directly persistance data, so it's for we need "GetHisoryFor" where you give us last information that you store in your database.
The function "GetHistoryFor" will be call only one time per Device (Asset) after start website when the library
will received a new data for an asset not present in Data cache of the library.

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
* by closing [issues](http://github.com/mobile-devices/cloud_connect_dotnet_client/issues)
* by reviewing patches

All contributors will be added to the [HISTORY](https://github.com/mobile-devices/cloud_connect_dotnet_client/blob/master/HISTORY.md)
file and will receive the respect and gratitude of the community.

Submitting an Issue
-------------------
We use the [GitHub issue tracker](http://github.com/mobile-devices/cloud_connect_dotnet_client/issues) to track bugs and
features. Before submitting a bug report or feature request, check to make sure it hasn't already
been submitted. You can indicate support for an existing issuse by voting it up. When submitting a
bug report, please include a [Gist](http://gist.github.com/) that includes a stack trace and any
details that may be necessary to reproduce the bug, including your gem version, Ruby version, and
operating system. Ideally, a bug report should include a pull request with failing specs.

Submitting a Pull Request
-------------------------
1. Fork the project.
2. Create a topic branch.
3. Implement your feature or bug fix.
4. Add documentation for your feature or bug fix.
5. Run <tt>bundle exec rake doc:yard</tt>. If your changes are not 100% documented, go back to step 4.
6. Add specs for your feature or bug fix.
7. Run <tt>bundle exec rake spec</tt>. If your changes are not 100% covered, go back to step 6.
8. Commit and push your changes.
9. Submit a pull request. Please do not include changes to the gemspec, version, or history file. (If you want to create your own version for some reason, please do so in a separate commit.)