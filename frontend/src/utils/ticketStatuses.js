const ticketStatuses = {
  waiting: {
    label: 'Ожидает',
    className: 'status_waiting'
  },
  called: {
    label: 'Вызван',
    className: 'status_called'
  },
  serving: {
    label: 'Обслуживается',
    className: 'status_serving'
  },
  completed: {
    label: 'Завершён',
    className: 'status_completed'
  },
  skipped: {
    label: 'Пропущен',
    className: 'status_skipped'
  },
  cancelled: {
    label: 'Отменён',
    className: 'status_cancelled'
  }
}

export function getTicketStatus(statusName) {
  return (
    ticketStatuses[statusName] ?? {
      label: statusName || 'Неизвестно',
      className: 'status--unknown'
    }
  )
}