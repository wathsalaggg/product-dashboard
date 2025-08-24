// Shopping Cart Management
class CartManager {
    static cartCount = 0;

    static async addToCart(productId, quantity = 1) {
        try {
            const formData = new FormData();
            formData.append('productId', productId);
            formData.append('quantity', quantity);

            const response = await fetch('/Cart/AddToCart', {
                method: 'POST',
                body: formData,
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            const result = await response.json();

            if (result.success) {
                this.updateCartCount(result.cartCount);
                AjaxHelper.showSuccess(result.message);

                // If cart modal is open, refresh it
                if (ModalHelper.currentModal === 'cartModal') {
                    ModalHelper.showCart();
                }
            } else {
                AjaxHelper.showError(result.message);
            }
        } catch (error) {
            AjaxHelper.showError('Failed to add product to cart');
            console.error('Add to cart error:', error);
        }
    }

    static async removeFromCart(productId) {
        try {
            const formData = new FormData();
            formData.append('productId', productId);

            const response = await fetch('/Cart/RemoveFromCart', {
                method: 'POST',
                body: formData,
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            const result = await response.json();

            if (result.success) {
                this.updateCartCount(result.cartCount);
                AjaxHelper.showSuccess(result.message);

                // Refresh cart modal if open
                if (ModalHelper.currentModal === 'cartModal') {
                    ModalHelper.showCart();
                }
            } else {
                AjaxHelper.showError('Failed to remove product from cart');
            }
        } catch (error) {
            AjaxHelper.showError('Failed to remove product from cart');
            console.error('Remove from cart error:', error);
        }
    }

    static async getCart() {
        try {
            const response = await AjaxHelper.get('/Cart/GetCart');
            return response;
        } catch (error) {
            console.error('Get cart error:', error);
            throw error;
        }
    }

    static updateCartCount(count) {
        this.cartCount = count;
        $('#cartCount').text(count);

        if (count > 0) {
            $('#cartCount').removeClass('d-none');
        } else {
            $('#cartCount').addClass('d-none');
        }
    }

    static async loadCartCount() {
        try {
            const response = await this.getCart();
            if (response.success) {
                this.updateCartCount(response.cart.totalItems);
            }
        } catch (error) {
            console.error('Failed to load cart count:', error);
        }
    }
}