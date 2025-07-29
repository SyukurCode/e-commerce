function changeCartQuantity(amount, id) {
    const input = document.getElementById(id);
    const totalToPay = document.getElementById("totalToPay")
    var token = $('input[name="__RequestVerificationToken"]').val();
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
            console.log("Update berjaya!", response);
            totalToPay.innerHTML = response.toFixed(2)
            calculatePrice(current,id)
        },
        error: function (xhr, status, error) {
            console.log("Ada error:", error);
        }
    });
}

function calculatePrice(amount, id) {
    const priceEl = document.getElementById("price" + id);
    const totalEl = document.getElementById("totalPrice" + id);
    const totalToPay = document.getElementById("tottalTpPay");

    if (!priceEl || !totalEl) {
        console.error("Missing element for ID:", id);
        return;
    }

    const unitPrice = parseFloat(priceEl.textContent.trim().replace('RM', ''));
    

    const total = unitPrice * parseInt(amount);
    totalEl.textContent = total.toFixed(2);
}