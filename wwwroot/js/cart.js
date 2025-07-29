function changeCartQuantity(amount, id) {
    const input = document.getElementById(id);
    let current = parseInt(input.value) || 1;
    current += amount;
    if (current < 1) current = 1;
    input.value = current;
    //calculatePrice(current, id);
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