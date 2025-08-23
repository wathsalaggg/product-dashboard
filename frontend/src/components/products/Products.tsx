import React, { useEffect, useState } from "react";
import type { Product } from "../../types/product.types";
import { getProducts } from "../../services/productService";

const Products: React.FC = () => {
  const [products, setProducts] = useState<Product[]>([]);

  useEffect(() => {
    getProducts().then((data) => setProducts(data));
  }, []);

  if (!products.length) return <p>Loading products...</p>;

  return (
    <div className="grid grid-cols-4 gap-4 p-4">
      {products.map((product) => (
        <div key={product.id} className="border p-2 rounded shadow">
          <img src={product.imageUrl} alt={product.name} className="w-full h-40 object-cover" />
          <h2 className="font-bold">{product.name}</h2>
          <p>{product.description}</p>        
          <p className="font-bold">${product.price.toFixed(2)}</p>
        </div>
      ))}
    </div>
  );
};

export default Products;
