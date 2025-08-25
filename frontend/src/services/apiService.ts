import { apiClient } from "../api/client";

export const fetchProducts = async (
  searchTerm?: string,
  categoryId?: number,
  page?: number
) => {
  const params: Record<string, unknown> = {};
  if (searchTerm) params.searchTerm = searchTerm;
  if (categoryId) params.categoryId = categoryId;
  if (page !== undefined) params.page = page;

  const response = await apiClient.get("/Products", { params });
  return response.data.products;
};

export const fetchCategories = async () => {
  const response = await apiClient.get("/Products/categories");
  return response.data;
};