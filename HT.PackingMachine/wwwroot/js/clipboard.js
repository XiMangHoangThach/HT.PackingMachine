window.clipboardCopy = {
    copyText: function (text) {
        navigator.clipboard.writeText(text).then(function () {
            alert("Đã sao chép!");
        })
            .catch(function (error) {
                alert(error);
            });
    }
};