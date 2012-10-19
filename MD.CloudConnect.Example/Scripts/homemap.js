function initialize() {
    var mapOptions = {
        zoom: 8,
        center: new google.maps.LatLng(48.78403,2.36691),
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    map = new google.maps.Map(document.getElementById('map_canvas'), mapOptions);
}

$(document).ready(function () {
    initialize();
});