function onClickRow() {
    var asset = $(this).attr('id');
    $("#track_table").html("");
    $.ajax({
        url: '/home/getLastracks',
        type: 'GET',
        data: "account=" + $("#account").val() + "&environment=" + $("#environment").val() + "&token=" + $("#token").val() + "&asset=" + asset,
        success: function (res) {
            if (res) {
                var line = "";
                for (var i = 0; i < res.length; i++) {
                    line = "<tr><td>" + i.toString() + "</td><td>" + res[i].Recorded_at + "/" + res[i].Received_at + "</td><td>" + res[i].Longitude + "/" + res[i].Latitude + "</td><td>" + res[i].Speed + "</td><td>" + res[i].IsValid + "</td></tr>";
                    if (i == 0) {
                        $('#track_table').html(line);
                    } else
                        $('#track_table tr:last').after(line);
                }
            }
        },
        error: function (res) {
            alert("Error : " + res);
        }
    });
}

$(document).ready(function () {
    $('#btRun').bind('click', function () {
        $.ajax({
            url: '/home/getAssets',
            type: 'GET',
            data: "account=" + $("#account").val() + "&environment=" + $("#environment").val() + "&token=" + $("#token").val(),
            success: function (res) {
                if (res) {
                    $("#asset_table").html("");
                    var line = "";
                    for (var i = 0; i < res.length; i++) {
                        line = "<tr id='" + res[i].Imei + "'><td>" + i.toString() + "</td><td>" + res[i].Imei + "</td></tr>";
                        if (i == 0) {
                            $('#asset_table').html(line);
                        } else
                            $('#asset_table tr:last').after(line);
                    }

                    $("tr").bind("click", onClickRow);
                }
            },
            error: function (res) {
                alert("Error : " + res);
            }
        });
    });
});