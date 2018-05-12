var submitFilterForm = function () {
    var maxWalkingDistanceToConnector = document.getElementById("distance-connectors").value;
    var maxWalkingDistanceToGym = document.getElementById("distance-gyms").value;

    var filterResult = {};
    $('input').each(function (index) {
        filterResult[this.name] = this.type == "checkbox" ? this.checked : this.value;
    })
    console.log(filterResult);
    // callback
    var testString = "";
    var filteredApartments = getFilteredApartments(maxWalkingDistanceToConnector, maxWalkingDistanceToGym);
    testString += locationsToString(filteredApartments);
    document.getElementById("result-count").innerHTML = filteredApartments.length + " result(s) found";
    document.getElementById("result").innerHTML = testString;
    UpdateMapWithFilteredApartments(filteredApartments);

    return false;
}

function locationsToString(locations) {
    var result = "";
    for (var i = 0; i < locations.length; i++) {
        /*
        result += locations[i].Address + " "
            + locations[i].Longitude + " "
            + locations[i].Latitude + " "
            + locations[i].Name
            + "</br>";
            */
        result += locations[i].Name + "</br></br>";
    }

    return result;
}

function getFilteredApartments(maxWalkingDistanceToConnector, maxWalkingDistanceToGym) {
    var locations = [];

    var xmlHttp = new XMLHttpRequest();
    var url = "home/queryapartments";
    if (maxWalkingDistanceToConnector != -1 && maxWalkingDistanceToGym != -1) {
        url += "?maxWalkingDistanceToConnector=" + maxWalkingDistanceToConnector + "&maxWalkingDistanceToGym=" + maxWalkingDistanceToGym;
    } else if (maxWalkingDistanceToConnector != -1) {
        url += "?maxWalkingDistanceToConnector=" + maxWalkingDistanceToConnector; 
    } else if (maxWalkingDistanceToGym != -1) {
        url += "?maxWalkingDistanceToGym=" + maxWalkingDistanceToGym;
    }
    xmlHttp.open("GET", url, false); // false for synchronous request
    xmlHttp.send(null);
    return JSON.parse(xmlHttp.responseText);
}
