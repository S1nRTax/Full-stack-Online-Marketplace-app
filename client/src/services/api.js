import axios from 'axios';

const apiClient = axios.create({
  baseURL: '/api', // Proxy handles the rest
});

export const getTestMessage = async () => {
  const response = await apiClient.get('/sample/test');
  return response.data;
};
