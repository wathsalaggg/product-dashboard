import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:49782', //backend HTTPS URL
});

export default api;
