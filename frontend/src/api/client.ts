import axios from "axios";

export const apiClient = axios.create({
  withCredentials: false,
    baseURL: import.meta.env.VITE_API_URL || "http://localhost:49783/api"
  });