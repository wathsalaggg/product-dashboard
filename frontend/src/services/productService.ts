import axios from "axios";
import type { Product } from "../types/product.types"; // Make sure this exists

const API_URL = "https://localhost:44379/api/products"; // backend endpoint

export const getProducts = async (): Promise<Product[]> => {
  const response = await axios.get<Product[]>(API_URL);
  return response.data;
};
