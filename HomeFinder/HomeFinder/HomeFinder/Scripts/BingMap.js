var BING_MAPS_KEY = "AjO3RbVtzGrL52_nhfqrgsTwlB8olumKAIccHDq7CNdz0T-ryjFtFzPKFTGB22fY";
function nullToString(value) { if (value == null) { return ''; } else { return value; } }
function CreateDefaultMap(mapDiv, zoomLevel, userActive, isFixed) {
    var lat = 47.643798; var lon = -122.133124; //OTC 
    var map = new Microsoft.Maps.Map(mapDiv, { 
        credentials: BING_MAPS_KEY, 
        center: new Microsoft.Maps.Location(lat, lon), 
        mapTypeId: Microsoft.Maps.MapTypeId.road, 
        zoom: zoomLevel, 
        // showMapTypeSelector: userActive, 
        enableSearchLogo: false, 
        // fixedMapPosition: isFixed, 
        // enableClickableLogo: false, 
        // showCopyright: false, 
        // showDashboard: userActive, 
        // showScalebar: true 
    });
    drawCircle(map, 5280/4, 47.614092, -122.32567); // Cap Hill Connector
    // drawCircle(map, 50, 47.6097, -122.3331);
    return map;
}
function drawCircle(map, radius, latitude, longitude) {
    var circleFillColor = new Microsoft.Maps.Color(105, 0, 153, 0); var circleOutlineColor = new Microsoft.Maps.Color(255, 204, 255, 153);
    var RadPerDeg = Math.PI / 180;var earthRadius = 20903520;var lat = latitude * RadPerDeg;var lon = longitude * RadPerDeg;var AngDist = parseFloat(radius) / earthRadius;
    //used for zooming to extent and circle coords
    var locs = new Array();
    for (x = 0; x <= 360; x++) { //making a 360-sided polygon
        var pLatitude, pLongitude;brng = x * RadPerDeg;
        pLatitude = Math.asin(Math.sin(lat) * Math.cos(AngDist) + Math.cos(lat) * Math.sin(AngDist) * Math.cos(brng)); //still in radians
        pLongitude = lon + Math.atan2(Math.sin(brng) * Math.sin(AngDist) * Math.cos(lat), Math.cos(AngDist) - Math.sin(lat) * Math.sin(pLatitude));pLatitude = pLatitude / RadPerDeg;pLongitude = pLongitude / RadPerDeg;
        var loc = new Microsoft.Maps.Location(pLatitude, pLongitude);locs.push(loc);
    };
    circle = new Microsoft.Maps.Polygon(locs, { visible: true, strokeThickness: 2, strokeDashArray: "1", strokeColor: circleOutlineColor, fillColor: circleFillColor });map.entities.push(circle);
    if (locs.length > 0) { map.setView({ bounds: new Microsoft.Maps.LocationRect.fromLocations(locs) }); }
}//end draw circle

var object_g, infobox_g;
function displayInfoBox(object,infobox) { //todo:allow the display of more than one infobox, possibly create on the fly.
    if (object.targetType == 'pushpin') {
        infobox.setLocation(object.target.getLocation());
        infobox.setOptions({ visible: true, title: object.target.Title, description: object.target.description.toString() });
        object_g = object; infobox_g = infobox;
    }
}
function hideInfoBox() {
    var len = map.entities.getLength();
    var entity;
    for (var i = 0; i < len; i++) {
        entity = map.entities.get(i);
        console.log(entity.targetType);
    }


    if (object_g.targetType == 'pushpin') {
        infobox_g.setOptions({ visible: false});
    }
}
