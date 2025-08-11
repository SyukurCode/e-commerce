function confirmDeleted() {
    return swal({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        buttons: {
            cancel: {
                text: "No, cancel!",
                visible: true,
                className: "btn btn-danger"
            },
            confirm: {
                text: "Yes, delete it!",
                className: "btn btn-success"
            }
        }
    }).then((willDelete) => {
        if (willDelete) {
            swal("Deleted!", "Your item has been deleted.", {
                icon: "success",
                buttons: {
                    confirm: {
                        className: "btn btn-success"
                    }
                }
            });
            return true;
        } else {
            swal("Cancelled", "Your item is safe.", {
                icon: "info",
                buttons: {
                    confirm: {
                        className: "btn btn-primary"
                    }
                }
            });
            return false;
        }
    });
}

function confirmDisabled() {
    return swal({
        title: "Are you sure?",
        text: "You able to revert this if you need!",
        icon: "warning",
        buttons: {
            cancel: {
                text: "No, cancel!",
                visible: true,
                className: "btn btn-danger"
            },
            confirm: {
                text: "Yes, diactivate it!",
                className: "btn btn-success"
            }
        }
    }).then((willDelete) => {
        if (willDelete) {
            swal("Diactivate!", "Your item has been diactivated.", {
                icon: "success",
                buttons: {
                    confirm: {
                        className: "btn btn-success"
                    }
                }
            });
            return true;
        } else {
            swal("Cancelled", "Your item is safe.", {
                icon: "info",
                buttons: {
                    confirm: {
                        className: "btn btn-primary"
                    }
                }
            });
            return false;
        }
    });
}
function submitDelete(btn) {
    confirmDeleted().then(function (confirmed) {
        if (confirmed) {
            document.getElementById(btn).submit();
        }
    });
}
function submitDisable(btn) {
    confirmDisabled().then(function (confirmed) {
        if (confirmed) {
            document.getElementById(btn).submit();
        }
    });
}
function confirmDisable(id, name) {
    if (confirm("Are you sure to disable this user?")) {
        return true;
    } else {
        return false;
    }
};

function confirmDelete(id, name) {
    if (confirm("Are you sure to delete?")) {
        return true;
    } else {
        return false;
    }
};

function togglePassword() {
    const passwordInput = document.getElementById("passwordInput");
    const eyeIcon = document.getElementById("eyeIcon");

    if (passwordInput.type === "password") {
        passwordInput.type = "text";
        eyeIcon.classList.remove("fa-eye-slash");
        eyeIcon.classList.add("fa-eye");
    } else {
        passwordInput.type = "password";
        eyeIcon.classList.remove("fa-eye");
        eyeIcon.classList.add("fa-eye-slash");
    }
}

function togglePassword1() {
    const passwordInput = document.getElementById("passwordInput1");
    const eyeIcon = document.getElementById("eyeIcon1");

    if (passwordInput.type === "password") {
        passwordInput.type = "text";
        eyeIcon.classList.remove("fa-eye-slash");
        eyeIcon.classList.add("fa-eye");
    } else {
        passwordInput.type = "password";
        eyeIcon.classList.remove("fa-eye");
        eyeIcon.classList.add("fa-eye-slash");
    }
}

function showNotification(title, msg, state, icon) {
    var content = {}
    content.message = msg;
    content.title = title;
    content.icon = icon;

    $.notify(content, {
        type: state, // success || danger || info || warning || default || primary || secondary
        placement: {
            from: 'top',  //top || bottom
            align: 'right', //left,center,right
        },
        time: 1000,
        delay: 3000,
    });
}

function showDialog(title, msg, icon, btn) {
    swal(title, msg, {
        icon: icon, //warning || error || success || info
        buttons: {
            confirm: {
                className: "btn btn-" + btn,
            },
        },
    });
}
function changeQuantity(amount) {
    const input = document.getElementById('quantity');
    let current = parseInt(input.value) || 1;
    current += amount;
    if (current < 1) current = 1;
    input.value = current;
}

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

connection.on("NotiReceive", function (count) {
    document.getElementById("noti-count").innerText = count;
});