(function () {
    var elm = document.getElementById("time1");

    setInterval(updateTime, 1000);

    function updateTime() {

        var hours = Number($('#hourLabel').text());
        var minutes = Number($('#minuteLabel').text());
        var seconds = Number($('#secondLabel').text()) + 1;

        if (seconds > 59) {
            seconds = 0;
            minutes = minutes + 1;
            if (minutes > 59) {
                minutes = 0;
                hours = hours + 1;
                if (hours > 23) {
                    hours = 0;
                }
            }
        }

        var age = $('#ageLabel').text();

        document.getElementById("secondLabel").textContent = seconds;
        document.getElementById("minuteLabel").textContent = minutes;
        document.getElementById("hourLabel").textContent = hours;
        elm.innerHTML = age.substr(0, age.indexOf('D') + 7) + hours + " Hour(s) " + minutes + " Minute(s) " + seconds + " Second(s)";
    }
})();

function modalPopUp() {
    var url = $('#addImageModal').data('url');
    $.get(url,
        function (data) {
            $('#addImageModal').html(data);
            $('#addImageModal').modal('show');
        });
};