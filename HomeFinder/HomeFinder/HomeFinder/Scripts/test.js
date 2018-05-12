var map;

var busstops = [
	{"Route":545,"Name":"Sr 520 Ramp & Montlake Blvd E","Latitude":47.6441468,"Longitude":-122.3040449},
	{"Route":545,"Name":"Bellevue Ave & E Olive St","Latitude":47.6164754,"Longitude":-122.3266711},
	{"Route":545,"Name":"Olive Way & Terry Ave","Latitude":47.6146958,"Longitude":-122.3321959},
	{"Route":545,"Name":"Olive Way & 8th Ave","Latitude":47.6141625,"Longitude":-122.3340773},
	{"Route":545,"Name":"4th Ave & Pike St","Latitude":47.6106128,"Longitude":-122.3370879},
	{"Route":545,"Name":"4th Ave & University St","Latitude":47.608468,"Longitude":-122.3350628},
	{"Route":545,"Name":"4th Ave & Madison St","Latitude":47.6064629,"Longitude":-122.3331885},
	{"Route":545,"Name":"4th Ave & James St","Latitude":47.603444,"Longitude":-122.3305747},
	{"Route":545,"Name":"4th Ave S & S Jackson St","Latitude":47.5991411,"Longitude":-122.3287725},
	{"Route":545,"Name":"6th Ave S & S Atlantic St","Latitude":47.5910547,"Longitude":-122.3264128},
	{"Route":542,"Name":"NE 65th St & Oswego Pl NE","Latitude":47.6756148,"Longitude":-122.3204485},
	{"Route":542,"Name":"NE 50th St & University Way NE","Latitude":47.6653214,"Longitude":-122.3128932},
	{"Route":542,"Name":"15th Ave NE & NE 45th St","Latitude":47.6615121,"Longitude":-122.3124762},
	{"Route":542,"Name":"15th Ave NE & NE 43rd St","Latitude":47.6588437,"Longitude":-122.3121368},
	{"Route":542,"Name":"15th Ave NE & NE Campus Pkwy","Latitude":47.6561811,"Longitude":-122.3124441},
	{"Route":542,"Name":"15th Ave NE & NE 40th St","Latitude":47.6551679,"Longitude":-122.3124682},
	{"Route":542,"Name":"NE Pacific St & 15th Ave NE","Latitude":47.6522684,"Longitude":-122.3117792},
	{"Route":542,"Name":"NE Pacific St & Montlake Blvd NE - Bay 1","Latitude":47.6494588,"Longitude":-122.3066586},
	{"Route":"Microsoft","Name":"Microsoft Westlake-Terry Office","Latitude":47.6212764,"Longitude":-122.3380618,"Description":"Seattle Route to OTC"},
];

var aptLayer = new Microsoft.Maps.EntityCollection();
var connectorLayer = new Microsoft.Maps.EntityCollection();
var sTransitLayer = new Microsoft.Maps.EntityCollection();
var gymLayer = new Microsoft.Maps.EntityCollection();
var restaurantLayer = new Microsoft.Maps.EntityCollection();

function UpdateMapWithFilteredApartments(filteredApartments) {
    AddAptPins(map, filteredApartments, true);
}

function AddAptPins(map, apts, clearFirst) {
	if (clearFirst == true) {
		aptLayer.clear();
	}
	var starray = new Array();var loc;var ppin;

	for (i = 0; i < apts.length; i++) {
	    loc = new Microsoft.Maps.Location(parseFloat(apts[i].Longitude), parseFloat(apts[i].Latitude));
	    ppin = new Microsoft.Maps.Pushpin(loc, { icon: 'Content/house.png', anchor: new Microsoft.Maps.Point(12, 12) });
	    ppin.Title = "test";
	    ppin.Description = "Description";
	    //ppin.Title = apts[i].Name.substr(0, 30);
	    starray.push(loc);
		ppin.description = '<table style=width:95%;font-size:0.8em;>' 
			+ '<tr><td>price: ' + nullToString(apts[i].price) + '; ' 
			+ 'area: ' + nullToString(apts[i].area) + '</td></tr>'
			+ '<a href="' + nullToString(apts[i].url) + '">' + 'link' + '</a>' + '; ' 
			+ 'bedrooms: ' + nullToString(apts[i].bedrooms) + '</td></tr>'
			+ '<tr><td>where: ' + nullToString(apts[i].where) + '</td></tr>' 
			+ '</table>';
		Microsoft.Maps.Events.addHandler(ppin, 'click', function (event) { displayInfoBox(event, infobox); });
		var infobox = new Microsoft.Maps.Infobox(new Microsoft.Maps.Location(0, 0), { visible: false }); 
		// map.entities.push(ppin); map.entities.push(infobox); 
		aptLayer.push(ppin); aptLayer.push(infobox); map.entities.push(aptLayer);
	}
}

function AddBusStopPins(map, stops, clearFirst) {
    if (clearFirst == true) {
        connectorLayer.clear();
    }
    var starray = new Array();var loc;var ppin;    
    for (i = 0; i < stops.length; i++) {
        loc = new Microsoft.Maps.Location(stops[i].Longitude, stops[i].Latitude);
        if (stops[i].IsHub == true) {
            ppin = new Microsoft.Maps.Pushpin(loc, { icon: 'Content/hubstop.png', anchor: new Microsoft.Maps.Point(12, 12) });
            ppin.Title = stops[i].Name + ' <div style=\'font-style:italic;font-weight:normal\'>[HUB]</div>';
        } else {
            ppin = new Microsoft.Maps.Pushpin(loc, { icon: 'Content/fixedstop.png', anchor: new Microsoft.Maps.Point(12, 12) });
            ppin.Title = stops[i].Name;
        }
        starray.push(loc);
        ppin.description = '<table style=width:95%;font-size:0.8em;><tr><td>' + nullToString(stops[i].Address) + '</td></tr><tr><td>' + nullToString(stops[i].City) + '</td></tr></table>';
        Microsoft.Maps.Events.addHandler(ppin, 'click', function (event) { displayInfoBox(event, infobox); });
        var infobox = new Microsoft.Maps.Infobox(new Microsoft.Maps.Location(0, 0), { visible: false }); 
       	// map.entities.push(ppin); map.entities.push(infobox);
       	connectorLayer.push(ppin); connectorLayer.push(infobox); map.entities.push(connectorLayer);
    }
}

function AddBusStopPinsST(map, stops, clearFirst) {
    if (clearFirst == true) {
        sTransitLayer.clear();
    }
    var starray = new Array();var loc;var ppin;    
    for (i = 0; i < stops.length; i++) {
        loc = new Microsoft.Maps.Location(stops[i].Latitude, stops[i].Longitude);       
        if (stops[i].Route === "Microsoft") {
            ppin = new Microsoft.Maps.Pushpin(loc, { icon: 'Content/hubstop.png', anchor: new Microsoft.Maps.Point(12, 12) });
        	ppin.description = '<table style=width:95%;font-size:0.8em;><tr><td>' + nullToString(stops[i].Description) + '</td></tr></table>';
        } else {
            ppin = new Microsoft.Maps.Pushpin(loc, { icon: 'Content/busstop.png', anchor: new Microsoft.Maps.Point(12, 12) });
        	ppin.description = '<table style=width:95%;font-size:0.8em;><tr><td>' + nullToString(stops[i].Route) + '</td></tr></table>';
        }
        ppin.Title = stops[i].Name;
        starray.push(loc);
        Microsoft.Maps.Events.addHandler(ppin, 'click', function (event) { displayInfoBox(event, infobox); });
        var infobox = new Microsoft.Maps.Infobox(new Microsoft.Maps.Location(0, 0), { visible: false }); 
        // map.entities.push(ppin); map.entities.push(infobox);
        sTransitLayer.push(ppin); sTransitLayer.push(infobox); map.entities.push(sTransitLayer);
    }
    if (starray.length > 0) {  map.setView({ bounds: new Microsoft.Maps.LocationRect.fromLocations(starray) }); }
}

function AddGymPins(map, gyms, clearFirst) {
    if (clearFirst == true) {
        gymLayer.clear();
    }
    var starray = new Array(); var loc; var ppin;

    for (i = 0; i < gyms.length; i++) {
        loc = new Microsoft.Maps.Location(parseFloat(gyms[i].Longitude), parseFloat(gyms[i].Latitude));
        ppin = new Microsoft.Maps.Pushpin(loc, { icon: 'Content/gym.png', anchor: new Microsoft.Maps.Point(12, 12) });
        starray.push(loc);
        Microsoft.Maps.Events.addHandler(ppin, 'click', function (event) { displayInfoBox(event, infobox); });
        var infobox = new Microsoft.Maps.Infobox(new Microsoft.Maps.Location(0, 0), { visible: false });
        gymLayer.push(ppin); gymLayer.push(infobox); map.entities.push(gymLayer);
    }
}

function AddRestaurantPins(map, restaurants, clearFirst) {
    if (clearFirst == true) {
        restaurantLayer.clear();
    }
    var starray = new Array(); var loc; var ppin;

    for (i = 0; i < restaurants.length; i++) {
        loc = new Microsoft.Maps.Location(parseFloat(restaurants[i].Latitude), parseFloat(restaurants[i].Longitude));
        ppin = new Microsoft.Maps.Pushpin(loc, { icon: 'Content/restaurant.png', anchor: new Microsoft.Maps.Point(12, 12) });
        starray.push(loc);
        Microsoft.Maps.Events.addHandler(ppin, 'click', function (event) { displayInfoBox(event, infobox); });
        var infobox = new Microsoft.Maps.Infobox(new Microsoft.Maps.Location(0, 0), { visible: false });
        restaurantLayer.push(ppin); restaurantLayer.push(infobox); map.entities.push(restaurantLayer);
    }
}

$(document).ready(function() {
    var div = document.getElementById("divMap"); map = CreateDefaultMap(div, 10, true, true);

    AddBusStopPins(map, getLocationsByType("connector"), false);
    AddBusStopPinsST(map, busstops, false);
    AddAptPins(map, getLocationsByType("apartment"), false);
    AddGymPins(map, getLocationsByType("gym"), false);
    AddRestaurantPins(map, getLocationsByType("restaurant"), false);
});

function getLocationsByType(type) {
    var locations = [];

    var xmlHttp = new XMLHttpRequest();
    var url = "home/querylocations?type=" + type
    xmlHttp.open("GET", url, false); // false for synchronous request
    xmlHttp.send(null);
    return JSON.parse(xmlHttp.responseText);
}

function toggleAptLayer() {
    aptLayer.setOptions({ visible: !aptLayer.getVisible() });
}
function toggleConLayer() {
    connectorLayer.setOptions({ visible: !connectorLayer.getVisible() });
}
function toggleSTLayer() {
    sTransitLayer.setOptions({ visible: !sTransitLayer.getVisible() });
}
function toggleGymLayer() {
    gymLayer.setOptions({ visible: !gymLayer.getVisible() });
}
function toggleRestaurantLayer() {
    restaurantLayer.setOptions({ visible: !restaurantLayer.getVisible() });
}