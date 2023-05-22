var map;
var markers = [];

function initialize(jsonResult) {
    var mapOptions = {
        mapTypeControl: true,
        mapTypeControlOptions: {
            style: google.maps.MapTypeControlStyle.DROPDOWN_MENU
        },
        zoomControl: true,
        zoomControlOptions: {
            style: google.maps.ZoomControlStyle.LARGE
            //,
            //position: google.maps.ControlPosition.RIGHT_TOP
        },
        mapTypeControlOptions: {
            style: google.maps.MapTypeControlStyle.HORIZONTAL_BAR,
            position: google.maps.ControlPosition.TOP_RIGHT
        },
        panControl: false,
        streetViewControl: false,
        mapTypeId: google.maps.MapTypeId.HYBRID,
        zoom: 4,
        minZoom: 2
    };
    map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
    google.maps.event.addListener(map, "click", function () {
        infoWindow.close();
    });

    // Try HTML5 geolocation
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var pos = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
            map.setCenter(pos);
        }, function () {
            handleNoGeolocation(true);
        });
    } else {
        // Browser doesn't support Geolocation
        handleNoGeolocation(false);
    }
    getActivities(jsonResult);
}

function handleNoGeolocation(errorFlag) {
    if (errorFlag) {
        var content = 'Error: The Geolocation service failed.';
    } else {
        var content = 'Error: Your browser doesn\'t support geolocation.';
    }

    var options = {
        map: map,
        position: new google.maps.LatLng(60, 105),
        content: content
    };
    var infowindow = new google.maps.InfoWindow(options);
    map.setCenter(options.position);
}


function ReInitialize(lat, lon, Location) {
    var myLatlng = new google.maps.LatLng(lat, lon);
    var mapOptions = {
        zoom: 8,
        center: myLatlng,
        streetViewControl: false,
        panControl: false,
        //mapTypeControlOptions: { style: google.maps.MapTypeControlStyle.DROPDOWN_MENU },
        navigationControl: true,
        navigationControlOptions: { style: google.maps.NavigationControlStyle.SMALL },
        zoomControlOptions: {
            style: google.maps.ZoomControlStyle.LARGE//,
            //position: google.maps.ControlPosition.RIGHT_TOP
        },
        mapTypeControlOptions: {
            style: google.maps.MapTypeControlStyle.HORIZONTAL_BAR,
            position: google.maps.ControlPosition.TOP_RIGHT
        },
        mapTypeControlOptions: {
            style: google.maps.MapTypeControlStyle.HORIZONTAL_BAR,
            position: google.maps.ControlPosition.TOP_RIGHT
        },
        minZoom: 1,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    map = new google.maps.Map(document.getElementById('vMap-canvas'),
        mapOptions);

    var contentString =
        '<div id="content" style="padding:10px; width:200px;">' +
        Location
    '</div>';

    var infowindow = new google.maps.InfoWindow({
        content: contentString
    });

    var marker = new google.maps.Marker({
        position: myLatlng,
        map: map,
        title: ''
    });
    markers.push(marker);
    google.maps.event.addListener(marker, 'click', function () {
        infowindow.open(map, marker);
    });
}
var infoWindow = new google.maps.InfoWindow();
var markerBounds = new google.maps.LatLngBounds();
var markerArray = [];

function makeMarker(options) {
    var pushPin = new google.maps.Marker({ map: map });
    pushPin.setOptions(options);
    google.maps.event.addListener(pushPin, "click", function () {
        infoWindow.setOptions(options);
        infoWindow.open(map, pushPin);
        //$('.gm-style-iw').parent().css('width', '');
        //$('.gm-style-iw').parent().css('background-color', '#FF0000');
        $('.gm-style-iw').prev().children().children().css('width', '605px!important');
        $('.gm-style-iw').next().remove();       
    });
    markerBounds.extend(options.position);
    markerArray.push(pushPin);
    return pushPin;
}

function toggleSlide(div) {
    if ($('#main').hasClass('expanded')) {
        $('#main').animate({ height: 35 }, 500, function () {
            $(this).removeClass('expanded');
            $('#mainsub').hide();
            $(div).toggleClass('whiteUp');
        });
    } else {
        $('#main').animate({ height: '90%' }, 500).addClass('expanded');
        $('#mainsub').show();
        $(div).toggleClass('whiteUp');
    }
    
}

function getRandom(min, max) {
    return min + Math.floor(Math.random() * (max - min + 1));
}

function DeleteMarkers() {
    //Loop through all the markers and remove
    for (var i = 0; i < markerArray.length; i++) {
        markerArray[i].setMap(null);
    }
    markerArray = [];
};

function getActivities(jsonResult) {
    DeleteMarkers();
    //$(btn).siblings('img.uiLoading').addClass('loading');
  
    var _PhotoOnly = "false";
    if ($('#chkPhotos').prop('checked')) {
        _PhotoOnly = "true";
    } else {
        _PhotoOnly = "false";
    }

    var icons = 0;
  
            var locations = [
                ["Syria", { lat: 34.802074, lng: 34.802074 }]
            ];
            var data = jsonResult;
            //deleteAll();
            var obj = data;//jQuery.parseJSON(data);
            //var iconBase = '/ptd/google/icons/'; 
            //-----------------------------------loop through JSON result!
            for (var i = 0; i < obj.length; i++) {
                var contentString = '<iframe src="/MRS/NoteVerbalesMapIndex/' + obj[i].LocationGUID + '" width="585px" height="700px" frameborder="0px" scrolling="auto">';
                var lat = obj[i].Latitude - (getRandom(1, 100) / 15000);
                var lon = obj[i].Longitude-(getRandom(1, 100) / 15000);
                icons++;
                makeMarker({
                    position: new google.maps.LatLng(lat, lon),
                    title: "Click",
                    closeBoxURL: "",
                    content: contentString,
                    //position: {
                    //    lat: parseFloat(34.802074),
                    //    lng: parseFloat(34.802074)
                    //},
                    //url: new google.maps.LatLng(34.802074, 34.802074)
                    //icon: iconBase + obj[i].IconType + '.png'
                });
               
                
            }
            //-----------------------------------loop end
           
           // new MarkerClusterer(map, markerArray, { minimumClusterSize: 10, imagePath: '../Assets/Images/m' });
            map.fitBounds(markerBounds);
           //
           
            //$(btn).attr('disabled', false);
            //$('#sTotal').text('Result: ' + icons + ' activities found');
            //$('#btnShare').show();
            //if (icons == 0) {
            //    alert('No activity found. please change your filter criteria');
            //    $('#btnShare').hide();
            //}

}

function Submit(btn) {
    $(btn).attr('disabled', 'diabled');
    $(btn).siblings('img.uiLoading').addClass('loading');
    $('.ErrorMessageBox').hide();
}

function closePopup(model) {
    //model is the cancel button to grap the parent model!
    $(model).parents().eq(2).fadeOut();
    $(".backgroundPopup").fadeOut();
}

