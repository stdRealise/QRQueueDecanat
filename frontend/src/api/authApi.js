import instance from './axios'

export async function loginOperator(username, password) {
  const response = await instance.post(
    '/api/auth/login', 
    { 
      username, 
      password 
    })
  return response.data
}