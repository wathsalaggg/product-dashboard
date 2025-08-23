export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  stockQuantity: number;
  brand: string;
  rating: number;
  categoryId: number;
  category: Category;
}

export interface Category {
  id: number;
  name: string;
  description: string;
  iconName: string;
  products: Product[] | null[];
}