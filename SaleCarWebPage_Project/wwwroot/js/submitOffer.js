async function submitOffer() {
    const valueEl = document.getElementById("offerValue");
    const contactEl = document.getElementById("offerContact");
    const nifEl = document.getElementById("offerNif");

    const carId = parseInt(document.getElementById("hiddenCarId").value);

    if (isNaN(carId) || carId <= 0) {
        toastr.error("Erro: Identificador do veículo não encontrado.");
        return;
    }

    if (!valueEl.value || !contactEl.value) {
        toastr.warning("Preencha o valor e o contacto.");
        return;
    }

    const btn = document.querySelector('.btn-luxury-submit');
    const originalText = btn.innerHTML;
    btn.innerHTML = '<i class="fa-solid fa-circle-notch fa-spin"></i> A ENVIAR...';
    btn.disabled = true;

    const url = `?handler=SubmitProposal&carId=${carId}&offerValue=${valueEl.value}&contact=${contactEl.value}`;

    try {
        const response = await fetch(`?handler=SubmitProposal`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: new URLSearchParams({
                'carId': carId,
                'offerValue': valueEl.value,
                'contact': contactEl.value,
                'nif': nifEl.value
            })
        });

        const result = await response.json();

        if (result.success) {
            toastr.success(result.message);
            bootstrap.Modal.getInstance(document.getElementById('offerModal')).hide();
            setTimeout(() => location.reload(), 1500);
        } else {
            toastr.error(result.message || "Erro ao enviar proposta.");
            btn.innerHTML = originalText;
            btn.disabled = false;
        }
    } catch (error) {
        console.error("Erro:", error);
        toastr.error("Erro de ligação ao servidor.");
        btn.innerHTML = originalText;
        btn.disabled = false;
    }
}