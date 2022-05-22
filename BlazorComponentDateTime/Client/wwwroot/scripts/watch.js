function addZero(i) {
    if (i < 10) {
        i = "0" + i;
    }
    return i;
}
var SystemWatchCaller = SystemWatchCaller || {};
SystemWatchCaller.NewWatch = function (dotNetObject) {
    setInterval(function watch() {
        var d = new Date();
        var date = d.getDate();
        var hour = addZero(d.getHours());
        var min = addZero(d.getMinutes());
        var sec = addZero(d.getSeconds());
        var systemWatch = hour + ":" + min + ":" + sec;
        //console.log("JS: Generated " + systemWatch.toString());
        dotNetObject.invokeMethodAsync('UpdateWatch', systemWatch.toString());
    }, 1000);
};