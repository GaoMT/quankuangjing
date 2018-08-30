/**
 * Created by YD on 5/7/15.
 * Base on Jquery
 */
var ele;

$.fn.extend({
    rotate: function () {
        ele = this;
        setInterval('singleRotate()', 20);
    }
});

var degree = 0;

function singleRotate() {
    degree = degree + 50 * Math.PI / 180;
    ele.css("transform", "rotate(" + degree + "deg)");
}