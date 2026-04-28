async function submitOffer() {
    const valueEl = document.getElementById("offerValue");
    const contactEl = document.getElementById("offerContact");
    const nifEl = document.getElementById("offerNif");
    const carIdEl = document.getElementById("hiddenCarId");
    const modal = document.getElementById('offerModal');

    const carId = parseInt(carIdEl.value);

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

    try {
        // Removemos os parâmetros da URL para evitar o erro de binding duplicado
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

        if (!response.ok) throw new Error("Falha na comunicação com o servidor.");

        const result = await response.json();

        if (result.success) {
            toastr.success(result.message);

            // FECHAR MODAL (PURO CSS/JS)
            if (modal) {
                modal.style.display = 'none'; // Esconde o modal
                modal.classList.remove('show'); // Remove classe se usares para animação
                document.body.classList.remove('modal-open'); // Liberta o scroll da página

                // Remove o "backdrop" escuro se o criares manualmente
                const backdrop = document.querySelector('.modal-backdrop');
                if (backdrop) backdrop.remove();
            }

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