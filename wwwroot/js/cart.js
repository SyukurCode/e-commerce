function changeCartQuantity(amount, id, owner) {
    const input = document.getElementById(id);
    const totalToPay = document.getElementById("totalToPay" + owner)
    var token = $('input[name="__RequestVerificationToken"]').val();
    console.log("id", owner);
    let current = parseInt(input.value) || 1;
    current += amount;
    if (current < 1) current = 1;
    input.value = current;
    $.ajax({
        type: "POST",
        url: "/Cart/UpdateCartItem", // endpoint dalam controller
        data: {
            __RequestVerificationToken: token,
            id: id,
            quantity: current
        },
        success: function (response) {
            totalToPay.innerHTML = response.toFixed(2)
            calculatePrice(current,id)
        },
        error: function (xhr, status, error) {
            console.log("error:", error);
        }
    });
}

function calculatePrice(amount, id) {
    const priceEl = document.getElementById("price" + id);
    const totalEl = document.getElementById("totalPrice" + id);
    const totalToPay = document.getElementById("totalToPay" + owner);

    if (!priceEl || !totalEl) {
        console.error("Missing element for ID:", id);
        return;
    }

    const unitPrice = parseFloat(priceEl.value);
    

    const total = unitPrice * parseInt(amount);
    totalEl.textContent = total.toFixed(2);
}