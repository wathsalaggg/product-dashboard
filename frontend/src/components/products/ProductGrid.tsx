import { useEffect, useState } from 'react';
import { getProducts } from '../../services/productService';
import type { Product } from '../../types/product.types';

export default function ProductGrid() {
  const [products, setProducts] = useState<Product[]>([]);

  useEffect(() => {
    getProducts()
      .then(setProducts)
      .catch(err => console.error(err));
  }, []);

  return (
    <div>
      {products.length === 0 ? (
        <p>Loading products...</p>
      ) : (
        <div className="grid grid-cols-3 gap-4">
          {products.map(product => (
            <div key={product.id} className="border p-4">
              <h3>{product.name}</h3>
              <p>{product.description}</p>
             <p className="font-bold">${product.price}</p>

            </div>
          ))}
        </div>
      )}
    </div>
  );
}
