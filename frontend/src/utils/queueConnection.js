import * as signalR from '@microsoft/signalr'

const apiUrl = import.meta.env.VITE_API_URL

export function createQueueConnection() {
  return new signalR.HubConnectionBuilder()
    .withUrl(`${apiUrl}/hubs/queue`, {
      accessTokenFactory: () => {
        return (sessionStorage.getItem('accessToken') ?? '')
      }
    })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Warning)
    .build()
}