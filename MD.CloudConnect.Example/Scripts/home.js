﻿var map;

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
            this.keysDisplayInMapPopup = this.authorizedKeys;
            this.keysDisplayInTable = this.authorizedKeys;
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
        $("#trackFeature").attr('checked', this.syncTableCursorAndMap);
        $("#clickFeature").attr('checked', this.zoomInClickTable);
        $("#dBehavEvent").attr('checked', this.displayDriverBehavOnMap);
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

        $.ajax({
            url: '/tracking/AuthorizeFields',
            type: 'GET',
            success: function (res) {
                config.authorizedKeys = res;
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
    isfocusMap: true,
    currentInfo: null,
    clear: function () {
        if (this.polylineMap) {
            this.polylineMap.setMap(null);
            this.polylineMap = null;
            var poly;
            while (this.polylines.length > 0) {
                poly = this.polylines.pop();
                poly = null;
            }
            this.polylines = new Array();
        }
        var marker = null;
        while (this.markers.length > 0) {
            marker = this.markers.pop();
            marker.setMap(null);
            marker = null;
        }
        this.markers = new Array();
        while (this.markersDbehav.length > 0) {
            markersDbehav = this.markersDbehav.pop();
            markersDbehav.setMap(null);
            markersDbehav = null;
        }
        this.markersDbehav = new Array();
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
    update: function () {

        if (this.polylines && this.polylines.length > 0) {
            this.polylineMap = new google.maps.Polyline({
                path: this.polylines,
                strokeColor: '#FF0000',
                strokeOpacity: 1.0,
                strokeWeight: 2
            });
            this.polylineMap.setMap(map);
        }

        map.fitBounds(this.bounds);
    },
    init: function () {
        this.markers = new Array();
        this.markersDbehav = new Array();
        this.bounds = new google.maps.LatLngBounds();
        this.polylines = new Array();
        this.infos = new Array();
        this.infosDbehav = new Array();
    }
};
/* End MapObject */

/* Class use for Driver Behavior Event */
function DataDriverBehavior() {
    this.IdEvent = 0;
    this.Date = "";
    this.Time = "";
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

    this.getDriverBehaviorEventLabel = function () {
        var result = "-";
        switch (this.IdEvent) {
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
        return "<b>Event : </b>" + this.getDriverBehaviorEventLabel() + "<br />"
        + "<b>Date : </b>" + this.Date + "<br />"
        + "<b>Time : </b>" + this.Time + "<br />"
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

    this.update = function (data) {

        if (data.Key.startsWith("BEHAVE_") && data.Value != " - ") {
            switch (data.Key) {
                case "BEHAVE_ID": this.IdEvent = data.Value; break;
                case "BEHAVE_LONG": this.Longitude = data.Value / 100000.0; break;
                case "BEHAVE_LAT": this.Latitude = data.Value / 100000.0; break;
                case "BEHAVE_DAY_OF_YEAR": this.Date = data.Value; break;
                case "BEHAVE_TIME_OF_DAY": this.Time = data.Value; break;
                case "BEHAVE_GPS_SPEED_BEGIN": this.SpeedBegin = Math.round(data.Value * 1.852 / 10000); break;
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

function initialize() {
    var mapOptions = {
        zoom: 6,
        center: new google.maps.LatLng(48.81499, 2.4237),
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    map = new google.maps.Map(document.getElementById('map_canvas'), mapOptions);
}

function AddHeaderTable(data) {
    var line = "<tr><th> # </th><th>Time [Record] (Utc)</th>";
    for (var i = 0; i < data.Fields.length; i++) {
        if (config.isVisibleInTable(data.Fields[i].Key))
            line += "<th>" + data.Fields[i].DisplayName + "</th>";
    }
    line += "</tr>";
    $('#headTable').html(line);
}


function AddLineInTable(data, idx) {
    var line = "<tr line=" + data.Id + "><td>" + data.Id + "</td><td>" + data.Recorded_at + "</td>";
    var dBeahvData = new DataDriverBehavior();

    for (var i = 0; i < data.Fields.length; i++) {
        dBeahvData.update(data.Fields[i]);
        if (config.isVisibleInTable(data.Fields[i].Key)) {
            line += "<td>" + data.Fields[i].Value + "</td>";
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

function AddPointOnMap(data, idx) {

    if (data.Latitude != 0 && data.Latitude != -0.00001 && data.Latitude != 0.00001) {
        var myLatlng = new google.maps.LatLng(data.Latitude, data.Longitude);

        mapObject.bounds.extend(myLatlng);
        mapObject.polylines.push(myLatlng);

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

function AddDriverBehavPointOnMap(data, idx) {
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

function loadTrackingData() {
    var datevalue = $("#date").datepicker('getDate');

    $.ajax({
        url: '/tracking/loadData',
        type: 'GET',
        data: "asset=" + $("#asset").val() + "&year=" + datevalue.getFullYear() + "&month=" + (datevalue.getMonth() + 1).toString() + "&day=" + datevalue.getDate(),
        success: function (res) {
            if (res) {
                $("#bodyTable").html("");
                $("#headTable").html("");
                mapObject.clear();

                var driverBehavData = null;
                for (var i = 0; i < res.length; i++) {
                    if (i == 0)
                        AddHeaderTable(res[i]);

                    driverBehavData = AddLineInTable(res[i], i);
                    AddPointOnMap(res[i], i);
                    if (config.displayDriverBehavOnMap && driverBehavData)
                        AddDriverBehavPointOnMap(driverBehavData, i);
                }
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
        },
        error: function (res) {
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

$(document).ready(function () {
    mapObject.init();
    config.init();

    $("#date").datepicker({ dateFormat: 'dd/mm/yy' });
    google.maps.event.addDomListener(window, 'load', initialize);
    resize();

    $("#bt_rawdata").bind("click", function () {
        var datevalue = $("#date").datepicker('getDate');
        if ($("#asset").val())
            window.open('/tracking/' + $("#asset").val() + "/" + datevalue.getFullYear() + "/" + (datevalue.getMonth() + 1).toString() + "/" + datevalue.getDate());
    });

    $("#bt_execute").bind("click", loadTrackingData);
});