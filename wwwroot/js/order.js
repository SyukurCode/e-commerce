const priceElement = document.getElementById("productPrice");
const basePrice = parseFloat(priceElement.textContent.replace("RM", "").trim());
function chooseOption() {
    const checkboxes = document.querySelectorAll('input[name="SelectedOptionIds"]:checked');
    const selectedIds = [];
    var token = $('input[name="__RequestVerificationToken"]').val();
    checkboxes.forEach(cb => {
        const id = parseInt(cb.value);
        if (!isNaN(id)) {
            selectedIds.push(id);
        }
    });
    $.ajax({
        type: "POST",
        url: "/Order/ChooseChecked", // endpoint dalam controller
        data: {
            __RequestVerificationToken: token,
            ids: selectedIds
        },
        success: function (response) {
            setProductPrice(response + basePrice)
        },
        error: function (xhr, status, error) {
            console.log("Ada error:", error);
        }
    });
}
function setProductPrice(newPrice) {
    const priceElement = document.getElementById("productPrice");
    priceElement.innerHTML = `<small class="me-1">RM</small>${newPrice.toFixed(2)}`;
}