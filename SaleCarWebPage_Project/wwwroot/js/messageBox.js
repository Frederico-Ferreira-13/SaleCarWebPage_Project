function prepareReply(parentId, subject) {
    const field = document.getElementById('parentMessageIdField');
    const subjectDisplay = document.getElementById('reply-subject');
    const indicator = document.getElementById('reply-indicator');
    const input = document.getElementById('txtMessage');

    if (field && subjectDisplay && indicator) {
        field.value = parentId;
        subjectDisplay.innerText = subject;
        indicator.style.display = 'block';
        if (input) input.focus();
    }
}

function cancelReply() {
    const field = document.getElementById('parentMessageIdField');
    const indicator = document.getElementById('reply-indicator');

    if (field) field.value = '';
    if (indicator) indicator.style.display = 'none';
}