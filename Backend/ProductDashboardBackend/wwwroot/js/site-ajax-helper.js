// Reusable AJAX Helper Functions
class AjaxHelper {
    static async makeRequest(options) {
        const defaultOptions = {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest'
            },
            timeout: 30000
        };

        const config = { ...defaultOptions, ...options };

        try {
            const controller = new AbortController();
            const timeoutId = setTimeout(() => controller.abort(), config.timeout);

            const response = await fetch(config.url, {
                method: config.method,
                headers: config.headers,
                body: config.data ? JSON.stringify(config.data) : undefined,
                signal: controller.signal
            });

            clearTimeout(timeoutId);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const contentType = response.headers.get('content-type');
            if (contentType && contentType.includes('application/json')) {
                return await response.json();
            } else {
                return await response.text();
            }
        } catch (error) {
            console.error('AJAX Request failed:', error);
            throw error;
        }
    }

    static async get(url, params = {}) {
        const queryString = new URLSearchParams(params).toString();
        const fullUrl = queryString ? `${url}?${queryString}` : url;

        return await this.makeRequest({
            url: fullUrl,
            method: 'GET'
        });
    }

    static async post(url, data = {}) {
        // For form data, we need to handle it differently
        if (data instanceof FormData) {
            return await this.makeRequest({
                url: url,
                method: 'POST',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: data
            });
        }

        return await this.makeRequest({
            url: url,
            method: 'POST',
            data: data
        });
    }

    static async loadPartialView(url, params = {}) {
        return await this.get(url, params);
    }

    static showError(message = 'An error occurred') {
        this.showNotification(message, 'error');
    }

    static showSuccess(message) {
        this.showNotification(message, 'success');
    }

    static showNotification(message, type = 'info') {
        // Simple toast notification
        const toast = $(`
            <div class="toast align-items-center text-white bg-${type === 'error' ? 'danger' : type === 'success' ? 'success' : 'primary'} border-0" role="alert">
                <div class="d-flex">
                    <div class="toast-body">
                        ${message}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                </div>
            </div>
        `);

        // Create toast container if it doesn't exist
        if (!$('#toastContainer').length) {
            $('body').append('<div id="toastContainer" class="toast-container position-fixed bottom-0 end-0 p-3"></div>');
        }

        $('#toastContainer').append(toast);
        const bsToast = new bootstrap.Toast(toast[0]);
        bsToast.show();

        // Remove toast element after it's hidden
        toast.on('hidden.bs.toast', function () {
            $(this).remove();
        });
    }
}