export function renderAlert(parentSelector: string, message: string) {
    const alertHtml = `<div  class="col-10 alert alert-dismissible alert-danger text-center">
                    <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                    <span></span>
                </div>`;
    const feedback = document.querySelector(parentSelector);
    feedback.innerHTML = alertHtml;
    feedback.querySelector('span').textContent = message;
    return feedback;
}