var map;
var _dataCache = {};
var _currentDocID = "";

// Overide Array 
Array.prototype.contains = function (obj) {
    var i = this.length;
    while (i--) {
        if (this[i] === obj) {
            return true;
        }
    }
    return false;
}
// Overide String
if (typeof String.prototype.startsWith != 'function') {
    String.prototype.startsWith = function (str) {
        return this.indexOf(str) == 0;
    };
}

/* Local config save in user cookie */
var config = {
    defaultKeyTable: null,
    defaultKeyMap: null,
    authorizedKeys: null,
    keysDisplayInTable: null,
    keysDisplayInMapPopup: null,
    syncTableCursorAndMap: false,
    zoomInClickTable: true,
    displayDriverBehavOnMap: true,
    show: function () {
        config.load();
        $('#modalConfig').modal('show');
    },
    isVisibleOnMap: function (key) {
        return this.keysDisplayInMapPopup.contains(key);
    },
    isVisibleInTable: function (key) {
        return this.keysDisplayInTable.contains(key);
    },
    hide: function () {
        $('#modalConfig').modal('hide');
    },
    reset: function () {
        config.keysDisplayInMapPopup = config.defaultKeyMap;
        config.keysDisplayInTable = config.defaultKeyTable;
        config.syncTableCursorAndMap = false;
        config.displayDriverBehavOnMap = true;
        config.zoomInClickTable = true;

        $.jStorage.set("cloudDataConf",
        {
            keysDisplayInMapPopup: config.keysDisplayInMapPopup,
            keysDisplayInTable: config.keysDisplayInTable,
            syncTableCursorAndMap: config.syncTableCursorAndMap,
            zoomInClickTable: config.zoomInClickTable,
            displayDriverBehavOnMap: config.displayDriverBehavOnMap
        });
        config.load();
    },
    load: function () {
        var data = $.jStorage.get("cloudDataConf");
        if (data) {
            this.keysDisplayInMapPopup = data.keysDisplayInMapPopup;
            this.keysDisplayInTable = data.keysDisplayInTable;
            if (!(data.syncTableCursorAndMap === 'undefined'))
                this.syncTableCursorAndMap = data.syncTableCursorAndMap;
            if (!(data.zoomInClickTable === 'undefined'))
                this.zoomInClickTable = data.zoomInClickTable;
            if (!(data.displayDriverBehavOnMap === 'undefined'))
                this.displayDriverBehavOnMap = data.displayDriverBehavOnMap
        } else {
            this.keysDisplayInMapPopup = this.defaultKeyMap;
            this.keysDisplayInTable = this.defaultKeyTable;
        }
        for (var i = 0; i < this.authorizedKeys.length; i++) {
            if (i == 0) {
                $("#fieldsForTable").html("<option " + (this.keysDisplayInTable.contains(this.authorizedKeys[i]) ? "selected='selected'" : "") + " value='" + this.authorizedKeys[i] + "'>" + this.authorizedKeys[i] + "</option>");
                $("#fieldsForMap").html("<option " + (this.keysDisplayInMapPopup.contains(this.authorizedKeys[i]) ? "selected='selected'" : "") + " value='" + this.authorizedKeys[i] + "'>" + this.authorizedKeys[i] + "</option>");
            }
            else {
                $("#fieldsForTable option:last").after("<option " + (this.keysDisplayInTable.contains(this.authorizedKeys[i]) ? "selected='selected'" : "") + " value='" + this.authorizedKeys[i] + "'>" + this.authorizedKeys[i] + "</option>");
                $("#fieldsForMap option:last").after("<option " + (this.keysDisplayInMapPopup.contains(this.authorizedKeys[i]) ? "selected='selected'" : "") + " value='" + this.authorizedKeys[i] + "'>" + this.authorizedKeys[i] + "</option>");
            }
        }
        if (this.syncTableCursorAndMap)
            $("#trackFeature").attr('checked', 'checked');
        else
            $("#trackFeature").removeAttr('checked');
        if (this.zoomInClickTable)
            $("#clickFeature").attr('checked', 'checked');
        else
            $("#clickFeature").removeAttr('checked');
        if (this.displayDriverBehavOnMap)
            $("#dBehavEvent").attr('checked', 'checked');
        else
            $("#dBehavEvent").removeAttr('checked');
    },
    save: function () {

        config.syncTableCursorAndMap = $("#trackFeature").is(':checked');
        config.zoomInClickTable = $("#clickFeature").is(':checked');
        config.displayDriverBehavOnMap = $("#dBehavEvent").is(':checked');

        config.keysDisplayInTable = new Array();
        $("#fieldsForTable option:selected").each(function () {
            config.keysDisplayInTable.push($(this).text());
        });
        config.keysDisplayInMapPopup = new Array();
        $("#fieldsForMap option:selected").each(function () {
            config.keysDisplayInMapPopup.push($(this).text());
        });

        $.jStorage.set("cloudDataConf",
        {
            keysDisplayInMapPopup: config.keysDisplayInMapPopup,
            keysDisplayInTable: config.keysDisplayInTable,
            syncTableCursorAndMap: config.syncTableCursorAndMap,
            zoomInClickTable: config.zoomInClickTable,
            displayDriverBehavOnMap: config.displayDriverBehavOnMap
        });
        $('#modalConfig').modal('hide');
    },
    init: function () {
        $("#bt_configure").bind("click", this.show);
        $("#btCloseConfig").bind("click", this.hide);
        $("#btSaveConfig").bind("click", this.save);
        $("#btResetConfig").bind("click", this.reset);
        $.ajax({
            url: '/tracking/AuthorizeFields',
            type: 'GET',
            success: function (res) {
                config.authorizedKeys = res.authorizedKeys;
                config.defaultKeyMap = res.defaultKeyMap;
                config.defaultKeyTable = res.defaultKeyTable;
                config.load();
            }
        });
    }
};
/* End Config */

/* MapObject is a global object use to manipulate marker & polylone on the map */
var mapObject = {
    polylineMap: null,
    markers: null,
    infos: null,
    markersDbehav: null,
    infosDbehav: null,
    bounds: null,
    polylines: null,
    currentPolyline: null,
    isfocusMap: true,
    currentInfo: null,
    colors: ["#E03C31", "#72A0C1", "#29AB87", "#9D81BA", "#FF8F00"],
    emptyArray: function (data) {
        if (data) {
            var tmp = null;
            while (data.length > 0) {
                tmp = data.pop();
                if (tmp.setMap)
                    tmp.setMap(null);
                tmp = null;
            }
        }
    },
    clear: function () {
        if (this.polylineMap) {
            for (var i = 0; i < this.polylineMap.length; i++) {
                if (this.polylineMap[i]) {
                    this.polylineMap[i].setMap(null);
                    this.polylineMap[i] = null;
                }
            }
            for (var i = 0; i < this.polylines.length; i++) {
                this.emptyArray(this.polylines[i]);
            }
        }
        this.emptyArray(this.markers);
        this.emptyArray(this.infos);
        this.emptyArray(this.markersDbehav);
        this.emptyArray(this.infosDbehav);
        this.bounds = new google.maps.LatLngBounds();
    },
    focusOn: function (id) {
        map.setZoom(17);
        var currentMarker = this.markers[id];
        map.panTo(currentMarker.getPosition());
        if (this.currentInfo)
            this.currentInfo.close();
        this.currentInfo = this.infos[id];
        this.currentInfo.open(map, currentMarker);
        this.isFocusMap = false;
    },
    hoverMap: function () {
        if (!this.isFocusMap) {
            this.update();
            if (this.currentInfo) {
                this.currentInfo.close();
                this.currentInfo = null;
            }
            this.isFocusMap = true;
        }
    },
    IncIdxColor: function () {
        if (this.currentPolyline.length > 0) {
            this.displayCurrentPolyline();
            if (this.idxColor < this.colors.length)
                this.idxColor++;
            else
                this.idxColor = 0;
        }
    },
    displayCurrentPolyline: function () {
        if (this.currentPolyline && this.currentPolyline.length > 0) {

            this.polylines.push(this.currentPolyline);

            var pMap = new google.maps.Polyline({
                path: this.currentPolyline,
                strokeColor: this.colors[this.idxColor],
                strokeOpacity: 1.0,
                strokeWeight: 2
            });

            this.polylineMap.push(pMap);
            pMap.setMap(map);
            this.currentPolyline = new Array();
        }
    },
    pushOnCurentPolyline: function (myLatlng) {
        this.currentPolyline.push(myLatlng);
        //this.polylines.push(myLatlng);
    },
    update: function () {
        this.displayCurrentPolyline();
        map.fitBounds(this.bounds);
    },
    init: function () {
        this.markers = new Array();
        this.markersDbehav = new Array();
        this.bounds = new google.maps.LatLngBounds();
        this.polylines = new Array();
        this.currentPolyline = new Array();
        this.polylines.push(this.currentPolyline);
        this.infos = new Array();
        this.infosDbehav = new Array();
        this.polylineMap = new Array();
        this.idxColor = 0;
    }
};
/* End MapObject */

/* Class use for Driver Behavior Event */
function DataDriverBehavior() {
    this.IdEvent = 0;
    this.Date = "";
    this.Time = "";
    this.Timestamp = "";
    this.Longitude = 0.0;
    this.Latitude = 0.0;
    this.IsDriverEvent = false;
    this.SpeedBegin = 0;
    this.SpeedPeak = 0;
    this.SpeedEnd = 0;
    this.HeadingBegin = 0;
    this.HeadingPeak = 0;
    this.HeadingEnd = 0;
    this.AccXBegin = 0;
    this.AccXPeak = 0;
    this.AccXEnd = 0;
    this.AccYBegin = 0;
    this.AccYPeak = 0;
    this.AccYEnd = 0;
    this.AccZBegin = 0;
    this.AccZPeak = 0;
    this.AccZEnd = 0;
    this.Elapsed = "";
    this.Recorded_at = "";
    this.hack_3_1_28 = false;

    this.getDriverBehaviorEventLabel = function () {
        var result = "-";
        switch (this.IdEvent) {
            case "2": result = "HARSH BRAKING"; break;
            case "10": result = "DECELERATION"; break;
            case "11": result = "ACCELERATION"; break;
            case "12": result = "TURN LEFT"; break;
            case "13": result = "TURN RIGHT"; break;
            case "14": result = "CORNERING LEFT"; break;
            case "15": result = "CORNERING RIGHT"; break;
            default: result = "ID" + this.IdEvent.toString(); break;
        }
        return result;
    };

    this.getContent = function () {
        if (this.hack_3_1_28)
            return "<b>Event : </b>" + this.getDriverBehaviorEventLabel() + "<br />"
            + "<b>Record time : </b>" + this.Recorded_at + "<br />"
            + "<b>Long/Lat : </b>" + this.Longitude + " / " + this.Latitude + "<br />"
            + "<b>Speed Begin/Peak/End : </b>" + this.SpeedBegin + " / " + this.SpeedPeak + " / " + this.SpeedEnd + "<br />"
            + "<b>Heading Begin/Peak/End : </b>" + this.HeadingBegin + " / " + this.HeadingPeak + " / " + this.HeadingEnd + "<br />"
            + "<b>AccX Begin/Peak/End : </b>" + this.AccXBegin + " / " + this.AccXPeak + " / " + this.AccXEnd + "<br />"
            + "<b>AccY Begin/Peak/End : </b>" + this.AccYBegin + " / " + this.AccYPeak + " / " + this.AccYEnd + "<br />"
            + "<b>AccZ Begin/Peak/End : </b>" + this.AccZBegin + " / " + this.AccZPeak + " / " + this.AccZEnd + "<br />"
            + "<b>Elasped : </b>" + this.Elapsed + "<br />";
        else
            return "<b>Event : </b>" + this.getDriverBehaviorEventLabel() + "<br />"
            + "<b>Record time : </b>" + this.Recorded_at + "<br />"
            + "<b>Event Date : </b>" + this.Date + "<br />"
            + "<b>Event Time : </b>" + this.Time + "<br />"
            + "<b>Long/Lat : </b>" + this.Longitude + " / " + this.Latitude + "<br />"
            + "<b>Speed Begin/Peak/End : </b>" + this.SpeedBegin + " / " + this.SpeedPeak + " / " + this.SpeedEnd + "<br />"
            + "<b>Heading Begin/Peak/End : </b>" + this.HeadingBegin + " / " + this.HeadingPeak + " / " + this.HeadingEnd + "<br />"
            + "<b>AccX Begin/Peak/End : </b>" + this.AccXBegin + " / " + this.AccXPeak + " / " + this.AccXEnd + "<br />"
            + "<b>AccY Begin/Peak/End : </b>" + this.AccYBegin + " / " + this.AccYPeak + " / " + this.AccYEnd + "<br />"
            + "<b>AccZ Begin/Peak/End : </b>" + this.AccZBegin + " / " + this.AccZPeak + " / " + this.AccZEnd + "<br />"
            + "<b>Elasped : </b>" + this.Elapsed + "<br />";
    };

    this.getUrlMarker = function () {
        var result = "-";
        switch (this.IdEvent) {
            case "2": result = "http://webdemo.integration.cloudconnect.io/content/stop.png"; break;
            case "10": result = "http://webdemo.integration.cloudconnect.io/content/stop.png"; break;
            case "11": result = "http://webdemo.integration.cloudconnect.io/content/caution.png"; break;
            case "12": result = "http://webdemo.integration.cloudconnect.io/content/direction_left.png"; break;
            case "13": result = "http://webdemo.integration.cloudconnect.io/content/direction_right.png"; break;
            case "14": result = "http://webdemo.integration.cloudconnect.io/content/direction_left.png"; break;
            case "15": result = "http://webdemo.integration.cloudconnect.io/content/direction_right.png"; break;
            default: result = "http://labs.google.com/ridefinder/images/mm_20_red.png"; break;
        }
        return result;
    };

    this.checkDateAndTime = function (data) {
        if (this.IsDriverEvent && this.Date != " - " && this.Time != " - ") {
            this.Longitude = data.Longitude;
            this.Latitude = data.Latitude;
            this.hack_3_1_28 = true;
            return true;
        }
        return false;
    };

    this.update = function (data) {

        if (data.Key.startsWith("BEHAVE_") && data.Value != " - ") {
            switch (data.Key) {
                case "BEHAVE_ID": this.IdEvent = data.Value; break;
                case "BEHAVE_LONG": this.Longitude = data.Value / 100000.0; break;
                case "BEHAVE_LAT": this.Latitude = data.Value / 100000.0; break;
                case "BEHAVE_DAY_OF_YEAR": this.Date = data.Value; break;
                case "BEHAVE_TIME_OF_DAY": this.Time = data.Value; break;
                case "BEHAVE_TIMESTAMP": this.Timestamp = data.Value; break;
                case "BEHAVE_GPS_SPEED_BEGIN": this.SpeedBegin = Math.round(data.Value * 1.852 / 1000); break;
                case "BEHAVE_GPS_SPEED_PEAK": this.SpeedPeak = Math.round(data.Value * 1.852 / 1000); break;
                case "BEHAVE_GPS_SPEED_END": this.SpeedEnd = Math.round(data.Value * 1.852 / 1000); break;
                case "BEHAVE_GPS_HEADING_BEGIN": this.HeadingBegin = data.Value / 1000; break;
                case "BEHAVE_GPS_HEADING_PEAK": this.HeadingPeak = data.Value / 1000; break;
                case "BEHAVE_GPS_HEADING_END": this.HeadingEnd = data.Value / 1000; break;
                case "BEHAVE_ACC_X_BEGIN": this.AccXBegin = data.Value; break;
                case "BEHAVE_ACC_X_PEAK": this.AccXPeak = data.Value; break;
                case "BEHAVE_ACC_X_END": this.AccXEnd = data.Value; break;
                case "BEHAVE_ACC_Y_BEGIN": this.AccYBegin = data.Value; break;
                case "BEHAVE_ACC_Y_PEAK": this.AccYPeak = data.Value; break;
                case "BEHAVE_ACC_Y_END": this.AccYEnd = data.Value; break;
                case "BEHAVE_ACC_Z_BEGIN": this.AccZBegin = data.Value; break;
                case "BEHAVE_ACC_Z_PEAK": this.AccZPeak = data.Value; break;
                case "BEHAVE_ACC_Z_END": this.AccZEnd = data.Value; break;
                case "BEHAVE_ELAPSED": this.Elapsed = data.Value; break;
            }
            this.IsDriverEvent = true;
        }
    };
}

/* end Driver Behavior */

function isValid(data) {
    return data.Longitude && data.Longitude != 0.0 && data.Latitude && data.Latitude != 0.0 && data.Latitude != 0 && data.Latitude != -0.00001 && data.Latitude != 0.00001
}

function ignitionOn(data) {
    for (var i = 0; i < data.Fields.length; i++) {
        if (data.Fields[i].Key == "DIO_IGNITION") {
            if (data.Fields[i].Value == "True")
                return true;
            else
                return false;
        }
    }
    //if field does not exist with return true
    return true;
}


function initialize() {
    var mapOptions = {
        zoom: 6,
        center: new google.maps.LatLng(48.81499, 2.4237),
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        panControl: true,
        scaleControl: true
    };
    map = new google.maps.Map(document.getElementById('map_canvas'), mapOptions);
}

function AddHeaderTable(data) {
    var line = "<tr><th style='text-align:center'><button class='btn btn-mini btn-success' id='btToggleTable'>+</button></th><th>Recorded Time (Utc)</th>";
    for (var i = 0; i < data.Fields.length; i++) {
        if (config.isVisibleInTable(data.Fields[i].Key)) {
            line += "<th style='text-align:center'>" + data.Fields[i].DisplayName + "</th>";
        }
    }
    line += "</tr>";
    $('#headTable').html(line);

    $("#btToggleTable").bind("click", toggleTableauSize);
}


function AddLineInTable(data, idx) {
    var line = "<tr line=" + data.Id + "><td>" + data.Id + "</td><td>" + data.Recorded_at + "</td>";
    var dBeahvData = new DataDriverBehavior();
    dBeahvData.Recorded_at = data.Recorded_at;

    for (var i = 0; i < data.Fields.length; i++) {
        dBeahvData.update(data.Fields[i]);
        if (config.isVisibleInTable(data.Fields[i].Key)) {

            if (_dataCache.hasOwnProperty(data.Fields[i].Key) && _dataCache[data.Fields[i].Key] == data.Fields[i].Value)
                line += "<td>" + data.Fields[i].Value + "</td>";
            else {
                line += "<td><strong>" + data.Fields[i].Value + "</strong></td>";
            }
            _dataCache[data.Fields[i].Key] = data.Fields[i].Value;
        }
    }

    line += "</tr>";

    if (idx == 0) {
        $('#bodyTable').html(line);
    } else
        $('#bodyTable tr:last').after(line);
    return dBeahvData;
}

function CreateContent(data) {
    var content = "<b>Recorded At : </b>" + data.Recorded_at + "<br /><b>Long / Lat : </b>" + data.Longitude + "/" + data.Latitude + "<br />";
    for (var i = 0; i < data.Fields.length; i++) {
        if (data.Fields[i].Value != " - " && config.isVisibleOnMap(data.Fields[i].Key))
            content += "<b>" + data.Fields[i].DisplayName + " : </b>" + data.Fields[i].Value + "<br />";
    }
    return content;
}

function clickevent(marker, content) {
    var infowindow = new google.maps.InfoWindow({
        content: content
    });

    google.maps.event.addListener(marker, 'click', function () {
        infowindow.open(marker.get('map'), marker);
    });
    return infowindow;
}

function AddPointOnMap(data, idx, format) {

    if (isValid(data)) {
        var myLatlng = new google.maps.LatLng(data.Latitude, data.Longitude);

        mapObject.bounds.extend(myLatlng);
        if (format == "0")
            mapObject.pushOnCurentPolyline(myLatlng);
        else {
            if (ignitionOn(data)) {
                mapObject.pushOnCurentPolyline(myLatlng);
            } else {
                mapObject.IncIdxColor();
            }
        }
        var marker = new google.maps.Marker(
        {
            position: myLatlng,
            map: map,
            title: data.Recorded_at,
            icon: 'http://labs.google.com/ridefinder/images/mm_20_blue.png'
        });

        var content = CreateContent(data, idx);
        var info = clickevent(marker, content);
        mapObject.markers.push(marker);
        mapObject.infos.push(info);
    }
}

function AddDriverBehavPointOnMap(data, idx, replace_loc_map) {
    if (data.Latitude != 0) {
        var myLatlng = new google.maps.LatLng(data.Latitude, data.Longitude);

        var marker = new google.maps.Marker(
        {
            position: myLatlng,
            map: map,
            title: data.getDriverBehaviorEventLabel(),
            icon: data.getUrlMarker()
        });

        var content = data.getContent();
        var info = clickevent(marker, content);
        mapObject.markersDbehav.push(marker);
        mapObject.infosDbehav.push(info);
    }
}


function trackingDataAnswer(res, format, erase) {
    var docID = "";
    if (res) {
        if (erase) {
            $("#bodyTable").html("");
            $("#headTable").html("");
            mapObject.clear();
            _dataCache = {};
        }
        var driverBehavData = null;
        for (var i = 0; i < res.length; i++) {
            if (i == 0 && erase)
                AddHeaderTable(res[i]);

            driverBehavData = AddLineInTable(res[i], i);
            AddPointOnMap(res[i], i, format);
            if (config.displayDriverBehavOnMap && driverBehavData) {

                //hack 3.1.27
                if (driverBehavData.checkDateAndTime(res[i])) {
                    AddDriverBehavPointOnMap(driverBehavData, i, true);
                }
                else {
                    AddPointOnMap(res[i], i, format);
                    AddDriverBehavPointOnMap(driverBehavData, i, false);
                }
            }
            docID = res[i].DocId;
        }

        // make the header fixed on scroll
        $('.table-fixed-header').fixedHeader();

        if (config.syncTableCursorAndMap) {
            $("#bodyTable tr").hover(function () {
                var id = $(this).attr("line");
                mapObject.focusOn(id);
            });
            $("#map_canvas").hover(function () {
                mapObject.hoverMap();
            });
        }
        if (config.zoomInClickTable) {
            $("#bodyTable tr").click(function () {
                var id = $(this).attr("line");
                mapObject.focusOn(id);
            });
        }
        mapObject.update();
    }
    return docID;
}

function loadTrackingData(erase) {
    var datevalue = $("#date").datepicker('getDate');
    var format = $("#format_view option:selected").val();
    $('#indicator').show();
    $.ajax({
        url: '/tracking/loadData',
        type: 'GET',
        data: "asset=" + $("#asset").val() + "&year=" + datevalue.getFullYear() + "&month=" + (datevalue.getMonth() + 1).toString() + "&day=" + datevalue.getDate() + "&limit=10&nextDocID=" + _currentDocID,
        success: function (res) {
            var docID = trackingDataAnswer(res, format, true);
            //if (res && res.length >0 && docID != _currentDocID) {
            //    _currentDocID = docID;
            //    loadTrackingData(false);
            //}
            $('#indicator').hide();
            if (res && res.length == 0) {
                alert("no data for this date");
            }
        },
        error: function (res) {
            $('#indicator').hide();
            alert("Error : " + res);
        }
    });
}

function resize() {
    var authorizeHeight = $(window).height() - 45;
    var width = $("#mainContainer").width();
    $("#map_canvas").css('width', width);
    $("#map_canvas").css('height', authorizeHeight * (2 / 3));
    $("#table").css('height', authorizeHeight * (1 / 3));
}

function toggleTableauSize() {
    if ($(this).html() == "+") {
        $("#map_canvas").slideUp(400, function () {
            var authorizeHeight = $(window).height() - 45;
            $("#table").css('height', authorizeHeight);
        });
        $(this).html("-");
    }
    else {
        $("#map_canvas").slideDown(400, function () {
            var authorizeHeight = $(window).height() - 45;
            $("#table").css('height', authorizeHeight * (1 / 3));
        });

        $(this).html("+");
    }
}

$(document).ready(function () {
    mapObject.init();
    config.init();

    $("#date").datepicker({ dateFormat: 'dd/mm/yy' });
    google.maps.event.addDomListener(window, 'load', initialize);
    resize();

    if ($("#autoload_asset").val() != "") {
        $("#autoload_asset").val("");
        loadTrackingData();
    }
    $("#bt_rawdata").bind("click", function () {
        var datevalue = $("#date").datepicker('getDate');
        if ($("#asset").val())
            window.open('/tracking/index/' + $("#asset").val() + "/" + datevalue.getFullYear() + "/" + (datevalue.getMonth() + 1).toString() + "/" + datevalue.getDate());
    });

    $("#bt_execute").bind("click", loadTrackingData);
});