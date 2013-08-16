exports.DiffInYearsAndDays = function (startDate, endDate) {

  // Copy and normalise dates
  var d0 = new Date(startDate);
  d0.setHours(12);
  var d1 = new Date(endDate);
  d1.setHours(12);

  // Make d0 earlier date
  // Can remember a sign here to make -ve if swapped
  if (d0 > d1) {
    var t = d0;
    d0 = d1;
    d1 = t;
  }

  // Initial estimate of years
  var dY = d1.getFullYear() - d0.getFullYear();

  // Modify start date
  d0.setYear(d0.getFullYear() + dY);

  // Adjust if required
  if (d0 > d1) {
    d0.setYear(d0.getFullYear() - 1);
    --dY;
  }

  // Get remaining difference in days
  var dD = (d1 - d0) / 8.64e7;
  dD = Math.round(dD) + 1;

  // If sign required, deal with it here
  return [dY, dD];
}

exports.DateFromDwellIdAndSeconds = function (dwellId, inputSeconds) {
  var retVal = new Date(dwellId);
  var hours = Math.floor(inputSeconds / 3600);
  retVal.setUTCHours(hours);
  var leftoverMinutesInSeconds = inputSeconds - (hours * 3600);
  var minutes = Math.floor(leftoverMinutesInSeconds / 60);
  retVal.setUTCMinutes(minutes);
  var seconds = Math.floor((inputSeconds - (hours * 3600)) - (minutes * 60));
  retVal.setUTCSeconds(seconds);
  var leftoverSeconds = ((inputSeconds - (hours * 3600)) - (minutes * 60)) - seconds;
  var milliSeconds = Math.floor(leftoverSeconds * 1000);
  retVal.setUTCMilliseconds(milliSeconds);
  return retVal;
}

exports.DateFromSensorNanoTime = function (sensorNanoTime) {
  /*  var retVal = new Date();
   retVal.setUTCFullYear(1950);
   retVal.setUTCMonth(0);
   retVal.setUTCDate(1);
   retVal.setUTCHours(0);
   retVal.setUTCMinutes(0);
   retVal.setUTCSeconds(0);
   retVal.setUTCMilliseconds(0);*/
  var millisFrom01Jan_1950 = -631152000000;

  var retVal = new Date(millisFrom01Jan_1950);

  var millis = sensorNanoTime / 1e3;

  return new Date(retVal.getTime() + millis);
}

