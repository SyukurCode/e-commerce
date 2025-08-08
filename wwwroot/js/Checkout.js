function calulateTotal(price) {
    const inputTotalToPay = document.getElementById("TotalToPay")
    const subtotalElement = document.getElementById("Subtotal")
    const deliveryCharge = document.getElementById("DeliveryCharge")
    const subtotal = parseFloat(subtotalElement.textContent.replace("RM", "").trim());
    const total = document.getElementById("Total")

    var newPrice = price + subtotal
    deliveryCharge.innerHTML = `RM${price.toFixed(2)}`;
    total.innerHTML = `RM${newPrice.toFixed(2)}`;
    inputTotalToPay.value = newPrice.toFixed(2);
};