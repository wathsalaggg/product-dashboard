// Reusable Modal Helper Functions
class ModalHelper {
    static currentModal = null;

    static async showProductDetails(productId) {
        try {
            const response = await AjaxHelper.get('/Product/Details', { id: productId });

            if (response.success) {
                const product = response.product;
                const modalHtml = this.generateProductDetailsModal(product);
                this.showModal(modalHtml, 'productDetailsModal');
            } else {
                AjaxHelper.showError('Failed to load product details');
            }
        } catch (error) {
            AjaxHelper.showError('Error loading product details');
        }
    }

    static async showCart() {
        try {
            const response = await CartManager.getCart();

            if (response.success) {
                const modalHtml = this.generateCartModal(response.cart);
                this.showModal(modalHtml, 'cartModal');
            } else {
                AjaxHelper.showError('Failed to load cart');
            }
        } catch (error) {
            AjaxHelper.showError('Error loading cart');
        }
    }

    static generateProductDetailsModal(product) {
        return `
            <div class="modal fade" id="productDetailsModal" tabindex="-1">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">
                                <i class="fas fa-box-open me-2"></i>
                                ${product.name}
                            </h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <div class="row">
                                <div class="col-md-6">
                                    <img src="${product.imageUrl || '/images/placeholder.jpg'}" 
                                         class="product-detail-image mb-3" alt="${product.name}">
                                    <div class="d-grid gap-2">
                                        <a href="${product.imageUrl || '/images/placeholder.jpg'}" 
                                           class="btn btn-outline-secondary btn-sm" 
                                           download="${product.name.replace(/\s+/g, '_')}_image.jpg">
                                            <i class="fas fa-download me-1"></i>Download Image
                                        </a>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="product-details">
                                        <p class="text-muted mb-2">
                                            <strong>Category:</strong> ${product.category || 'N/A'}
                                        </p>
                                        <p class="mb-3">${product.description}</p>
                                        <div class="price-section mb-3">
                                            <h3 class="text-primary mb-0">${product.price.toFixed(2)}</h3>
                                        </div>
                                        <div class="stock-status mb-3">
                                            ${product.inStock ?
                '<span class="badge bg-success"><i class="fas fa-check me-1"></i>In Stock</span>' :
                '<span class="badge bg-danger"><i class="fas fa-times me-1"></i>Out of Stock</span>'
            }
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                                <i class="fas fa-times me-1"></i>Close
                            </button>
                            ${product.inStock ?
                `<button type="button" class="btn btn-primary add-to-cart-btn" data-product-id="${product.id}">
                                    <i class="fas fa-cart-plus me-1"></i>Add to Cart
                                </button>` : ''
            }
                        </div>
                    </div>
                </div>
            </div>
        `;
    }

    static generateCartModal(cart) {
        const cartItemsHtml = cart.items.map(item => `
            <div class="cart-item" data-product-id="${item.productId}">
                <div class="row align-items-center">
                    <div class="col-2">
                        <img src="${item.imageUrl || '/images/placeholder.jpg'}" 
                             class="cart-item-image" alt="${item.name}">
                    </div>
                    <div class="col-5">
                        <h6 class="mb-0">${item.name}</h6>
                        <small class="text-muted">${item.price.toFixed(2)} each</small>
                    </div>
                    <div class="col-2">
                        <span class="fw-bold">Qty: ${item.quantity}</span>
                    </div>
                    <div class="col-2">
                        <span class="fw-bold text-primary">${item.totalPrice.toFixed(2)}</span>
                    </div>
                    <div class="col-1">
                        <button class="btn btn-sm btn-outline-danger remove-from-cart-btn" 
                                data-product-id="${item.productId}" title="Remove from cart">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </div>
            </div>
        `).join('');

        return `
            <div class="modal fade" id="cartModal" tabindex="-1">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">
                                <i class="fas fa-shopping-cart me-2"></i>
                                Shopping Cart (${cart.totalItems} items)
                            </h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            ${cart.items.length > 0 ? `
                                <div class="cart-items">
                                    ${cartItemsHtml}
                                </div>
                                <div class="cart-total">
                                    <div class="row">
                                        <div class="col-6">
                                            <h5 class="mb-0">Total Amount:</h5>
                                        </div>
                                        <div class="col-6 text-end">
                                            <h4 class="text-primary mb-0">${cart.totalAmount.toFixed(2)}</h4>
                                        </div>
                                    </div>
                                </div>
                            ` : `
                                <div class="text-center py-4">
                                    <i class="fas fa-shopping-cart fa-3x text-muted mb-3"></i>
                                    <h5 class="text-muted">Your cart is empty</h5>
                                    <p class="text-muted">Add some products to get started!</p>
                                </div>
                            `}
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                                <i class="fas fa-times me-1"></i>Continue Shopping
                            </button>
                            ${cart.items.length > 0 ? `
                                <button type="button" class="btn btn-primary">
                                    <i class="fas fa-credit-card me-1"></i>Proceed to Checkout
                                </button>
                            ` : ''}
                        </div>
                    </div>
                </div>
            </div>
        `;
    }

    static showModal(modalHtml, modalId) {
        // Remove existing modal if present
        if (this.currentModal) {
            $(`#${this.currentModal}`).modal('hide');
            $(`#${this.currentModal}`).remove();
        }

        // Add new modal to container
        $('#modalContainer').html(modalHtml);

        // Show modal
        const modal = $(`#${modalId}`);
        modal.modal('show');
        this.currentModal = modalId;

        // Clean up when modal is hidden
        modal.on('hidden.bs.modal', () => {
            modal.remove();
            this.currentModal = null;
        });

        // Bind events for this modal
        this.bindModalEvents(modalId);
    }

    static bindModalEvents(modalId) {
        const modal = $(`#${modalId}`);

        // Bind add to cart events
        modal.find('.add-to-cart-btn').on('click', function (e) {
            e.preventDefault();
            const productId = $(this).data('product-id');
            CartManager.addToCart(productId);
        });

        // Bind remove from cart events
        modal.find('.remove-from-cart-btn').on('click', function (e) {
            e.preventDefault();
            const productId = $(this).data('product-id');
            CartManager.removeFromCart(productId);
        });
    }

    static closeModal() {
        if (this.currentModal) {
            $(`#${this.currentModal}`).modal('hide');
        }
    }
}