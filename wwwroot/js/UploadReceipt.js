function showLoadingModal() {
    var modal = new bootstrap.Modal(document.getElementById('loadingModal'));
    modal.show();
}

function hideLoadingModal() {
    var modalEl = document.getElementById('loadingModal');
    var modal = bootstrap.Modal.getInstance(modalEl);
    if (modal) modal.hide();
}