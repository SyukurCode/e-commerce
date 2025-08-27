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


function confirmCancelled() {
    return swal({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        buttons: {
            cancel: {
                text: "No",
                visible: true,
                className: "btn btn-danger"
            },
            confirm: {
                text: "Yes",
                className: "btn btn-success"
            }
        }
    }).then((willDelete) => {
        if (willDelete) {
            swal("Cancelled!", "Item has been canceled.", {
                icon: "success",
                buttons: {
                    confirm: {
                        className: "btn btn-success"
                    }
                }
            });
            return true;
        } else {
            swal("Ok!", "Your item is safe.", {
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
function submitCancelled(btn) {
    confirmCancelled().then(function (confirmed) {
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

// text,date,icon,mode,duration
function addTimelineStatus(model)
{
    if (model.length > 0)
    {

        //document.getElementById("details").innerHTML = m.detail;

        document.getElementById("order-timeline").innerHTML = "";

        model.forEach(m => {
            // Create new <li>
            const li = document.createElement("li");
            if (m.mode != "") {
                li.classList.add(m.mode);
            }

            // Add inner HTML (icon + message)
            li.innerHTML = `
            <div class="timeline-badge ${m.state}">
                <i class="${m.icon}"></i>
            </div>
            <div class="timeline-panel">
                <div class="timeline-heading">
                    <h4 class="timeline-title">${m.date}</h4>
                    <p>
                        <small class="text-muted">
                            ${m.duration}
                        </small>
                    </p>
                </div>
                <div class="timeline-body">
                    <p>
                        ${m.text}
                    </p>
                </div>
            </div>`;

            if (m.detail != "") {
                document.getElementById("details").innerHTML = m.detail;
            }
            // Append ke timeline
            document.getElementById("order-timeline").appendChild(li);
        });
    }
}

document.addEventListener("click", function (e) {
    if (e.target.id === "btnRemoveAll") {
        e.preventDefault();
        return fetch('/Notification/RemoveAll')
            .then(res => res.text())
            .then(html => {
                document.getElementById("navbarContainer").innerHTML = html;
            });
    }
    if (e.target.id === "btnMarkAll") {
        e.preventDefault();
        return fetch('/Notification/MarkAllRead')
            .then(res => res.text())
            .then(html => {
                document.getElementById("navbarContainer").innerHTML = html;
            });
    }

});

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

connection.on("Noti-Receive", function (count) {
    fetch('/PartialView/NavbarLoad')
        .then(res => res.text())
        .then(html => {
            document.getElementById("navbarContainer").innerHTML = html;
        });
});
//connection.on("Order-Receive", function (orderNo, totalPrice) {
//    appendTableOrder(orderNo, totalPrice)
//});

connection.on("Order-Tracking", function (no, model) {
    let orderNo = document.getElementById("orderNoInput")?.value;
    if (orderNo) {
        if (orderNo == no) {
            
            addTimelineStatus(model);
        }
    }
});

connection.start().then(function () {
    console.log("Connected to NotificationHub");
}).catch(err => console.error("❌ Connection failed: ", err));