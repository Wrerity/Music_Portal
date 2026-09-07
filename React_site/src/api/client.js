import axios from 'axios'

const api = axios.create({
  baseURL: 'http://localhost:5090',
  timeout: 10000,
  headers: { 'Content-Type': 'application/json' },
})

// Bearer JWT из localStorage
api.interceptors.request.use(cfg => {
  const t = localStorage.getItem('token')
  if (t) cfg.headers.Authorization = `Bearer ${t}`
  return cfg
})

api.interceptors.response.use(
  r => r,
  err => {
    const msg = err.response?.data?.detail || err.response?.data?.title || err.message
    // 401 -> сброс токена
    if (err.response?.status === 401) {
      localStorage.removeItem('token')
    }
    return Promise.reject({ status: err.response?.status, data: err.response?.data, message: msg })
  }
)

export default api
export const setToken = t => t ? localStorage.setItem('token', t) : localStorage.removeItem('token')
export const getToken = () => localStorage.getItem('token')
